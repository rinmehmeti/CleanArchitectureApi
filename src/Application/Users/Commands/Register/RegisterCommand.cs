using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public record RegisterCommand : IRequest<string>
{
    [Required]
    public string Email { get; init; }

    [Required]
    public string Password { get; init; }
}

public class RegisterCommandCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CreateUserAsync(request.Email, request.Password);
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IIdentityService _identityService;
    public RegisterCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmail)
                .WithMessage("There is an existing account with same email.");

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(6);
    }

    public async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _identityService.ExistsAsync(email);
    }
}
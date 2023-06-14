using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands.AuthenticateUser;

public record LoginCommand : IRequest<LoginResponse>
{
    [Required]
    public string Email { get; init; }

    [Required]
    public string Password { get; init; }
}

public class LoginCommandCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = await _identityService.GenerateTokenAsync(request.Email);

        var userId = await _identityService.GetUserIdAsync(request.Email);

        var response = new LoginResponse
        {
            Id = userId,
            Email = request.Email,
            Token = token
        };

        return response;
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly IIdentityService _identityService;

    public LoginCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(v => v.Password)
            .NotEmpty();

        RuleFor(m => m)
            .MustAsync(PasswordIsCorrect)
            .WithName("Authentication")
            .WithMessage("Email or password is incorrect.");
    }

    public async Task<bool> PasswordIsCorrect(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CheckPasswordAsync(request.Email, request.Password);
    }
}

public class LoginResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}

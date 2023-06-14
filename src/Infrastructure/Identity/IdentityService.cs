using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly IDateTime _dateTime;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        IDateTime dateTime)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _dateTime = dateTime;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), userId);
        }

        return user.UserName;
    }

    public async Task<string> GetUserIdAsync(string email)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Email == email);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), email);
        }

        return user.Id;
    }

    public async Task<string> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return user.Id;
    }

    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<string> GenerateTokenAsync(string email)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), email);
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        return GenerateJwtToken(user.Id, user.UserName, userRoles.ToList());
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), userId);
        }

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), userId);
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException(nameof(user), userId);
        }

        return await DeleteUserAsync(user);
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);

        return user != null;
    }

    private string GenerateJwtToken(string userId, string email, List<string> roles)
    {
        var jwtKey = _configuration.GetValue<string>("JwtKey");
        var jwtIssuer = _configuration.GetValue<string>("JwtIssuer");

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtExpiration = _configuration.GetValue<int>("JwtExpirationHours");

        var tokenConfig = new JwtSecurityToken(
            jwtIssuer,
            jwtIssuer,
            claims,
            expires: _dateTime.Now.AddDays(jwtExpiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenConfig);
    }

    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException(nameof(email), email);
        }

        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }
}

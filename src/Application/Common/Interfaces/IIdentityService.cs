using Application.Common.Models;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);

    Task<string> GetUserIdAsync(string email);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> ExistsAsync(string email);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<string> CreateUserAsync(string userName, string password);

    Task<bool> CheckPasswordAsync(string email, string password);

    Task<string> GenerateTokenAsync(string email);

    Task<Result> DeleteUserAsync(string userId);
}

namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    public string UserEmail { get; }
    public IEnumerable<string> UserRoles { get; }
}

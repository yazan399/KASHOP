namespace KASHOP.DAL.Dto;

public sealed class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}

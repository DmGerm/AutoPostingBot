namespace AutoPost_Bot.Models;

public class UserModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public RoleId RoleId { get; set; }
    public virtual RoleModel? Role { get; set; }
}
using AutoPost_Bot.Models;

public class UserEditModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public byte[] PasswordSalt { get; set; } = [];
    public RoleId RoleId { get; set; }
    public string EmailError { get; set; } = string.Empty;
    public string PasswordError { get; set; } = string.Empty;
}
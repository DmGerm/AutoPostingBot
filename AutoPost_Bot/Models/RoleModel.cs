namespace AutoPost_Bot.Models;

public class RoleModel
{
    public RoleId RoleId { get; set; }
    public virtual IEnumerable<UserModel>? Users { get; set; }
}
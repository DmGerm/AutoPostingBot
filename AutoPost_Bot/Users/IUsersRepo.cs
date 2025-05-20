using AutoPost_Bot.Models;

namespace AutoPost_Bot.UsersRepo;

public interface IUsersRepo
{
    public Task<UserModel> CreateUserAsync(UserModel user);
    public Task<UserModel> CreateUserAsync(long userId, string password);
    public Task<UserModel> UpdateUserAsync(UserModel user);
    public Task<UserModel> DeleteUserAsync(UserModel user);
    public Task<UserModel> DeleteUserAsync(long userId);
    public Task<UserModel> FindUserAsync(long userId);
    public Task<bool> LoginUserAsync(UserModel user);
    public Task<bool> LoginUserAsync(long userId, string password);
}
using System.Text.RegularExpressions;
using AutoPost_Bot.Models;

namespace AutoPost_Bot.UsersRepo;

public partial class UsersRepo : IUsersRepo
{
    public Task<UserModel> CreateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> CreateUserAsync(long userId, string password)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> UpdateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> DeleteUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> DeleteUserAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserModel> FindUserAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LoginUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LoginUserAsync(long userId, string password)
    {
        throw new NotImplementedException();
    }

    [GeneratedRegex(@"^[\w.+-]+@[\w.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}
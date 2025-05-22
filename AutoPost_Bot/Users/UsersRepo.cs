using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using AutoPost_Bot.UsersRepo;

namespace AutoPost_Bot.Users;

public partial class UsersRepo(IServiceProvider serviceProvider) : IUsersRepo
{
    private readonly UserContext? _userContext = serviceProvider.GetService<UserContext>();

    public Task<UserModel> CreateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel> CreateUserAsync(string email, string password)
    {
        if (await FindUserAsync(email).ConfigureAwait(false) is not null)
            throw new Exception("User already exists");

        var hash = GeneratePasswordHash(password, out byte[] salt);

        var newUser = new UserModel
        {
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = RoleId.User
        };

        try
        {
            if (_userContext == null)
                throw new Exception("UserContext can't be null.");

            _userContext.Users.Add(newUser);
            await _userContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return newUser;
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

    public async Task<UserModel?> FindUserAsync(string email)
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

    private byte[] GeneratePasswordHash(string password, out byte[] salt)
    {
        salt = new byte[16];
        new Random().NextBytes(salt);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        return SHA256.HashData(passwordBytes.Concat(salt).ToArray());
    }

    private bool VerifyPasswordHash(string password, byte[] salt, byte[] passwordHash)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        return Equals(SHA256.HashData(passwordBytes.Concat(salt).ToArray()), passwordHash);
    }
}
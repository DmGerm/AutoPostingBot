using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.Users;

public partial class UsersRepo(UserContext userContext) : IUsersRepo
{
    private readonly UserContext _userContext = userContext 
                                                ?? throw new InvalidOperationException("User context is null!");

    public Task<UserModel> CreateUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel> CreateUserAsync(string email, string password, RoleId roleId)
    {
        if (_userContext == null)
            throw new NullReferenceException("UserContext is null");
        
        if (await _userContext.Users.AnyAsync(u => 
                u.Email.ToLower() == email.ToLower()))
            throw new Exception("User already exists");

        var hash = GeneratePasswordHash(password, out var salt);
        Console.WriteLine(email);
        var newUser = new UserModel
        {
            UserId = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = roleId
        };
        Console.WriteLine(newUser.Email);
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

    public Task<UserModel> DeleteUserAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<UserModel?> FindUserAsync(string email)
    {
        if (_userContext is null)
            throw new InvalidOperationException("UserContext не инициализирован");
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email не может быть пустым", nameof(email));

        return await _userContext.Users
            .FirstOrDefaultAsync(user => 
                user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            .ConfigureAwait(false);
    }

    public Task<bool> LoginUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IfAnyUsersAsync() => await _userContext.Users.AnyAsync();

    public async Task<bool> LoginUserAsync(string email, string password)
    {
        var user = await FindUserAsync(email).ConfigureAwait(false) ??
            throw new InvalidOperationException("Пользователь не найден");
        
        return VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash);
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
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        var newUser = new UserModel
        {
            UserId = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = roleId
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

    public async Task<UserModel> UpdateUserAsync(UserModel user)
    {
        try
        {
            var existingUser = await _userContext.Users.FirstOrDefaultAsync(bUser => bUser.UserId == user.UserId)
                               ?? throw new Exception("User can't be found");

            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.PasswordSalt = user.PasswordSalt;
            existingUser.RoleId = user.RoleId;

            await _userContext.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
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
                user.Email.ToLower() == email.ToLower())
            .ConfigureAwait(false);
    }

    public Task<bool> LoginUserAsync(UserModel user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IfAnyUsersAsync()
    {
        return await _userContext.Users.AnyAsync();
    }

    public async Task<UserModel?> LoginUserAsync(string email, string password)
    {
        var user = await FindUserAsync(email).ConfigureAwait(false) ??
                   throw new InvalidOperationException("Пользователь не найден");

        if (!VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash))
            throw new ArgumentException("Пароль неправильный");

        return user;

    }

    [GeneratedRegex(@"^[\w.+-]+@[\w.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    public byte[] GeneratePasswordHash(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(16);
        using var sha256 = SHA256.Create();

        var combined = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();

        return sha256.ComputeHash(combined);
    }

    private bool VerifyPasswordHash(string password, byte[] salt, byte[] passwordHash)
    {

        var combined = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        var computedHash = SHA256.HashData(combined);

        return computedHash.SequenceEqual(passwordHash);
    }

    public async Task<List<UserModel>> GetAllUsersAsync() => await userContext.Users.ToListAsync()
                                                                                .ConfigureAwait(false);

    public async Task UpdateUsersAsync(List<UserModel> incomingUsers)
    {
        if (incomingUsers == null)
            throw new ArgumentNullException(nameof(incomingUsers));

        var existingUsers = await _userContext.Users.ToListAsync();

        var incomingIds = incomingUsers
            .Where(u => u.UserId != Guid.Empty)
            .Select(u => u.UserId)
            .ToHashSet();

        var usersToRemove = existingUsers
            .Where(u => !incomingIds.Contains(u.UserId))
            .ToList();

        if (usersToRemove.Count > 0)
            _userContext.Users.RemoveRange(usersToRemove);

        foreach (var incomingUser in incomingUsers)
        {
            if (incomingUser.UserId == Guid.Empty)
            {
                incomingUser.UserId = Guid.NewGuid();
                _userContext.Users.Add(incomingUser);
                continue;
            }

            var existingUser = existingUsers
                .FirstOrDefault(u => u.UserId == incomingUser.UserId);

            if (existingUser != null)
            {
                existingUser.Email = incomingUser.Email;
                existingUser.PasswordHash = incomingUser.PasswordHash;
                existingUser.PasswordSalt = incomingUser.PasswordSalt;
                existingUser.RoleId = incomingUser.RoleId;
            }
            else
            {
                incomingUser.UserId = Guid.NewGuid();
                _userContext.Users.Add(incomingUser);
            }
        }

        await _userContext.SaveChangesAsync();
    }
}
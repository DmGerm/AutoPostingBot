﻿using AutoPost_Bot.Models;

namespace AutoPost_Bot.Users;

public interface IUsersRepo
{
    public Task<UserModel> CreateUserAsync(UserModel user);
    public Task<UserModel> CreateUserAsync(string email, string password, RoleId roleId);
    public Task<List<UserModel>> GetAllUsersAsync();
    public Task<UserModel> UpdateUserAsync(UserModel user);
    public Task UpdateUsersAsync(List<UserModel> users);
    public Task<UserModel> DeleteUserAsync(UserModel user);
    public Task<UserModel> DeleteUserAsync(string email);
    public Task<UserModel?> FindUserAsync(string email);
    public Task<bool> LoginUserAsync(UserModel user);
    public Task<bool> IfAnyUsersAsync();
    public Task<UserModel?> LoginUserAsync(string email, string password);
    public byte[] GeneratePasswordHash(string password, out byte[] salt);
}
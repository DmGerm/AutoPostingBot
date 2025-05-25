using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.TelegramGroupsRepo;

public class GroupRepo(PostsContext dbContext) : IGroupRepo
{
    private readonly PostsContext _dbContext = dbContext;
    public event Action? StateChanged;

    public async Task AddGroup(long groupId, string groupName)
    {
        if (await _dbContext.Groups.FirstOrDefaultAsync(group => group.GroupId == groupId) != null)
            throw new Exception("Группа с таким ID уже добавлена.");


        try
        {
            await _dbContext.Groups.AddAsync(new GroupModel { GroupId = groupId, Name = groupName });
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Problem with group saving, {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
        }

        OnStateChanged();
    }

    public async Task<string> ChangeGroupAsync(long groupId)
    {
        var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId)
                    ?? throw new Exception("Группа с указанным id не найдена в базе данных.");

        try
        {
            await _dbContext.SaveChangesAsync();
            OnStateChanged();
            return group.Name;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Проблема при обновлении группы: {ex.Message}");
            Console.WriteLine($"Внутреннее исключение: {ex.InnerException?.Message}");
            throw;
        }
    }


    public async Task<Dictionary<long, string>> GetAllGroupsAsync()
    {
        try
        {
            return await _dbContext.Groups
                .ToDictionaryAsync(group => group.GroupId, group => group.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<long> RemoveGroupAsync(long groupId)
    {
        var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId)
                    ?? throw new Exception("Group with this id not found in db.");

        _dbContext.Groups.Remove(group);
        await _dbContext.SaveChangesAsync();

        return group.GroupId;
    }

    private void OnStateChanged()
    {
        StateChanged?.Invoke();
    }
}
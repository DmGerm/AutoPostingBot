using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.TelegramGroupsRepo;

public class GroupRepo(PostsContext dbContext) : IGroupRepo
{
    private readonly PostsContext _dbContext = dbContext;
    public event Action? StateChanged;

    public async Task AddGroup(long groupId, string groupName, string botToken)
    {
        try
        {
            if (await _dbContext.Groups.FirstOrDefaultAsync(group => group.GroupId == groupId) != null)
                throw new Exception("Группа с таким ID уже добавлена.");

            var group = new GroupModel
            {
                GroupId = groupId,
                Name = groupName,
                Bots = new List<BotModel>
                {
                    await _dbContext.Bots.FirstAsync(bot => bot.Token == botToken)
                }
            };
            await _dbContext.Groups.AddAsync(group);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Problem with group saving, {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
        }

        OnStateChanged();
    }

    public async Task<long> RemoveGroupAsync(long groupId, string botToken)
    {
        try
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId)
                        ?? throw new Exception("Group with this id not found in db.");

            if (group.Bots.All(bot => bot.Token != botToken))
                throw new Exception("This bot is not associated with the group.");
            else
            {
                var bot = await _dbContext.Bots.FirstAsync(b => b.Token == botToken);
                group.Bots.Remove(bot);
                await _dbContext.SaveChangesAsync();
                OnStateChanged();
                if (group.Bots.Count > 0)
                    return group.GroupId;
                else
                {
                    Console.WriteLine("No bots left in group, deleting group...");
                    _dbContext.Groups.Remove(group);
                    await _dbContext.SaveChangesAsync();
                    return group.GroupId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Problem with group removing, {ex.Message}");
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
            throw;
        }
    }

    private void OnStateChanged()
    {
        StateChanged?.Invoke();
    }
}
namespace AutoPost_Bot.TelegramGroupsRepo
{
    public interface IGroupRepo
    {
        event Action StateChanged;
        public Task AddGroup(long groupId, string groupName, string botToken);
        public Task<long> RemoveGroupAsync(long groupId, string botToken);
    }
}

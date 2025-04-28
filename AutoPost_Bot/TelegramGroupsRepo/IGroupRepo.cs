namespace AutoPost_Bot.TelegramGroupsRepo
{
    public interface IGroupRepo
    {
        event Action StateChanged;
        public void AddGroup(long groupId, string groupName);
        public Task<string> RemoveGroupAsync(string groupName);
        public Task<string> ChangeGroupAsync(long groupId);
        public Task<Dictionary<long, string>> GetAllGroupsAsync();
    }
}

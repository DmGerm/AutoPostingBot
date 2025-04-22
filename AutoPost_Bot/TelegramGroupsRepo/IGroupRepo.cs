namespace AutoPost_Bot.TelegramGroupsRepo
{
    public interface IGroupRepo
    {
        public void AddGroup(string groupName, long groupId);
        public Task<string> RemoveGroupAsync(string groupName);
        public Task<string> ChangeGroupAsync(long groupId);
        public Task<Dictionary<long, string>> GetAllGroupsAsync();
    }
}

namespace AutoPost_Bot.TelegramGroupsRepo
{
    public class GroupRepo : IGroupRepo
    {
        private readonly Dictionary<long, string> groups = [];
        public event Action? StateChanged;

        public void AddGroup(long groupId, string groupName)
        {
            if (groups.ContainsKey(groupId))
            {
                throw new Exception("Group with same id already added, please check.");
            }

            groups.Add(groupId, groupName);

            OnStateChanged();
        }

        public Task<string> ChangeGroupAsync(long groupId)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<long, string>> GetAllGroupsAsync() => Task.FromResult(groups);

        public async Task<long> RemoveGroupAsync(long groupId)
        {
            if (!groups.Remove(groupId))
                throw new Exception("Group with this id, not found in db.");

            return await Task.FromResult(groupId);
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
        }

    }
}

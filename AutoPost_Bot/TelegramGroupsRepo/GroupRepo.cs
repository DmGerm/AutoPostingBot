namespace AutoPost_Bot.TelegramGroupsRepo
{
    public class GroupRepo : IGroupRepo
    {
        private Dictionary<long, string> groups = [];
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

        public Task<string> RemoveGroupAsync(string groupName)
        {
            throw new NotImplementedException();
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke(); 
        }

    }
}

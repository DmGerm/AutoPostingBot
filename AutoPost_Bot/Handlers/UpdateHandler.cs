using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AutoPost_Bot.Handlers
{
    public class UpdateHandler(IGroupRepo groupRepo)
    {
        private readonly IGroupRepo _groupRepo = groupRepo;
        public async Task OnUpdate(Update update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            if (update.Type == UpdateType.MyChatMember && update.MyChatMember != null)
            {
                var newStatus = update.MyChatMember.NewChatMember.Status;

                if (newStatus == ChatMemberStatus.Member || newStatus == ChatMemberStatus.Administrator)
                {
                    if (update.MyChatMember.Chat.Type is ChatType.Private)
                        return;

                    if (update.MyChatMember.Chat.Title is null)
                        throw new Exception("chat title can't be null.");

                    try
                    {
                        _groupRepo.AddGroup(update.MyChatMember.Chat.Id, update.MyChatMember.Chat.Title);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine($"Бот был добавлен в чат: {update.MyChatMember.Chat.Title}");

                }
                else if (newStatus == ChatMemberStatus.Kicked || newStatus == ChatMemberStatus.Left)
                {
                    if (update.MyChatMember.Chat.Type is ChatType.Private)
                        return;

                    Console.WriteLine($"Бот был удалён из чата: {update.MyChatMember.Chat.Title}");
                }
            }
        }
    }
}
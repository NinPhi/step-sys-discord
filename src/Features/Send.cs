namespace StepSys.Features;

public class MessageModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send", "Переотправляет любое сообщение.")]
    public async Task Send(
        [Summary(nameof(text), "Текст сообщения.")] string text)
    {
        await RespondAsync(text: "Сообщение отправлено!", ephemeral: true);
        await ReplyAsync(text: text);
    }
}

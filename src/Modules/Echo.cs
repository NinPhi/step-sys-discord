namespace StepSys.Modules;

public class EchoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("echo", "Переотправляет любое сообщение.")]
    public async Task Echo(
        [Summary(nameof(text), "Текст сообщения.")] string text,
        [Summary(nameof(seconds), "Задержка в секундах.")] int seconds = 0)
    {
        string response = $"Сообщение отправлено!";

        if (seconds > 0)
        {
            int remainder = seconds % 10;
            response = $"Сообщение будет отправлено через {seconds} ";

            response += remainder switch
            {
                1 => "секунду!",
                > 1 and < 5 => "секунды!",
                _ => "секунд!",
            };
        }

        await RespondAsync(text: response, ephemeral: true);

        if (seconds > 0)
            await Task.Delay(seconds * 1000);

        await ReplyAsync(text: text);
    }
}

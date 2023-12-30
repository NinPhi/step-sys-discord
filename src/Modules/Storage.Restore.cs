using StepSys.Infrastructure.Data;

namespace StepSys.Modules;

public partial class StorageModule
{
    public partial async Task Restore(string alias)
    {
        var request = new RestoreMessage.Query
        {
            UserId = Context.User.Id,
            Alias = alias,
        };

        var response = await _mediator.Send(request);
        if (response is null)
        {
            await RespondAsync(text: "Ключевое слово не найдено. Проверьте на опечатки или сохраните сообщение с помощью команды в меню.", ephemeral: true);
            return;
        }

        var channel = await Context.Client.GetChannelAsync(response.ChannelId) as ISocketMessageChannel;
        if (channel is null)
        {
            await RespondAsync(text: "Канал не найден. Возможно, доступ боту запрещен или канал больше не существует.", ephemeral: true);
            return;
        }
        
        var message = await channel.GetMessageAsync(response.MessageId);
        if (message is null)
        {
            await RespondAsync(text: "Сообщение не найдено. Возможно, оно больше не существует.", ephemeral: true);
            return;
        }

        var builder = new EmbedBuilder();
        var embeds = message.Attachments
            .Select(a => builder
                .WithImageUrl(a.Url)
                .Build())
            .ToArray();

        await RespondAsync(
            text: $"Сохраненное сообщение по ключевому слову '{alias}' успешно отправлено.",
            ephemeral: true);

        await ReplyAsync(
            text: message.Content,
            embeds: embeds);
    }
}

internal static class RestoreMessage
{
    internal record Query : IRequest<Response?>
    {
        public required ulong UserId { get; init; }
        public required string Alias { get; init; }
    }

    internal record Response
    {
        public required string Alias { get; init; }
        public required ulong ChannelId { get; init; }
        public required ulong MessageId { get; init; }
    }

    internal class Handler : IRequestHandler<Query, Response?>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response?> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var loweredAlias = request.Alias.ToLowerInvariant();

            var response = await _context.StoredMessages
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.Alias == loweredAlias)
                .Select(a => new Response()
                {
                    Alias = a.Alias,
                    ChannelId = a.ChannelId,
                    MessageId = a.MessageId
                })
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }
    }
}
using StepSys.Infrastructure.Data;

namespace StepSys.Modules;

public partial class StorageModule
{
    public partial async Task StoreModal(IMessage message)
    {
        string modalId = $"store_message:{message.Channel.Id},{message.Id}";
        await RespondWithModalAsync<Modal>(customId: modalId);
    }

    public partial async Task StoreModalResponse(
        ulong channelId, ulong messageId, Modal modal) // TODO
    {
        var request = new StoreMessage.Command()
        {
            UserId = Context.User.Id,
            Alias = modal.Alias,
            ChannelId = channelId,
            MessageId = messageId,
        };

        await _mediator.Send(request);

        await RespondAsync(
            text: $"Сообщение было сохранено под ключевым словом '{modal.Alias}'.",
            ephemeral: true);

        var channel = await Context.Client.GetChannelAsync(channelId) as ISocketMessageChannel;
        if (channel is null)
        {
            await RespondAsync(
                text: "Канал не найден. Возможно, доступ боту запрещен или канал больше не существует.",
                ephemeral: true);
            return;
        }

        var message = await channel.GetMessageAsync(messageId);
        if (message is null)
        {
            await RespondAsync(
                text: "Сообщение не найдено. Возможно, оно больше не существует.",
                ephemeral: true);
            return;
        }

        await message.AddReactionAsync(new CheckEmote());
    }

    public record Modal : IModal
    {
        public string Title => "Store message";

        [RequiredInput(true)]
        [InputLabel("Alias")]
        [ModalTextInput("alias")]
        public required string Alias { get; init; }
    }

    public record CheckEmote : IEmote
    {
        public string Name => "✅";
    }
}

internal static class StoreMessage
{
    internal record Command : IRequest
    {
        public required ulong UserId { get; init; }
        public required string Alias { get; init; }
        public required ulong ChannelId { get; init; }
        public required ulong MessageId { get; init; }
    }

    internal class Handler : IRequestHandler<Command>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(
            Command request, CancellationToken cancellationToken)
        {
            var loweredAlias = request.Alias.ToLowerInvariant();

            var storedMessage = await _context.StoredMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.UserId == request.UserId &&
                    a.Alias == loweredAlias,
                    cancellationToken);

            if (storedMessage is not null)
            {
                storedMessage.ChannelId = request.ChannelId;
                storedMessage.MessageId = request.MessageId;
                _context.StoredMessages.Update(storedMessage);
            }
            else
            {
                storedMessage = new StoredMessage
                {
                    UserId = request.UserId,
                    Alias = loweredAlias,
                    ChannelId = request.ChannelId,
                    MessageId = request.MessageId,
                };
                _context.StoredMessages.Add(storedMessage);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
namespace StepSys.Modules;

public partial class StorageModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public StorageModule(IMediator mediator)
    {
        _mediator = mediator;
    }

    [MessageCommand("Store message")]
    public partial Task StoreModal(IMessage message);

    [ModalInteraction("store_message:*,*")]
    public partial Task StoreModalResponse(
        ulong channelId, ulong messageId, Modal modal);

    [SlashCommand("restore", "Отправляет ранее сохраненное сообщение.")]
    public partial Task Restore(
        [Summary(nameof(alias), "Ключевое слово изображения.")] string alias);
}

namespace StepSys.Features;

public partial class PictureModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public PictureModule(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("save", "Запоминает изображение под указанным словом.")]
    public partial Task Save(
        [Summary(nameof(alias), "Ключевое слово.")] string alias,
        [Summary(nameof(image), "Изображение.")] IAttachment image);

    [SlashCommand("show", "Присылает изображение, ранее добавленное с помощью /save.")]
    public partial Task Show(
        [Summary(nameof(alias), "Ключевое слово изображения.")] string alias);
}


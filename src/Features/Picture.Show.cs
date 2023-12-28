using StepSys.Infrastructure.Data;

namespace StepSys.Features;

public partial class PictureModule
{
    public partial async Task Show(string alias)
    {
        var request = new ShowPicture.Query
        {
            UserId = Context.User.Id,
            Alias = alias,
        };

        var response = await _mediator.Send(request);

        if (response is not null)
        {         
            await RespondAsync(
                text: "Изображение отправлено!",
                ephemeral: true);

            var embed = new EmbedBuilder()
                .WithImageUrl(response.Url)
                .Build();

            await ReplyAsync(
                embed: embed);
        }
        else
        {
            await RespondAsync(
                text: "Ключевое слово не найдено. Воспользуйтесь /save, чтобы добавить изображение.",
                ephemeral: true);
        }
    }
}

internal static class ShowPicture
{
    internal record Query : IRequest<Response?>
    {
        public required ulong UserId { get; init; }
        public required string Alias { get; init; }
    }

    internal record Response
    {
        public required string Url { get; init; }
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

            var response = await _context.Pictures
                .AsNoTracking()
                .Where(a =>
                    a.UserId == request.UserId &&
                    a.Alias == loweredAlias)
                .Select(a => new Response() { Url = a.Url })
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }
    }
}
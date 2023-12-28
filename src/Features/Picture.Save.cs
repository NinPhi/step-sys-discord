using StepSys.Infrastructure.Data;

namespace StepSys.Features;

public partial class PictureModule
{
    public partial async Task Save(
        string alias, IAttachment image)
    {
        var request = new SavePicture.Command
        {
            UserId = Context.User.Id,
            Alias = alias,
            Url = image.Url,
        };

        await _mediator.Send(request);

        var embed = new EmbedBuilder().WithImageUrl(image.Url).Build();

        await RespondAsync(
            text: $"Ключевое слово: {alias}",
            embed: embed);
    }
}

internal static class SavePicture
{
    internal record Command : IRequest
    {
        public required ulong UserId { get; init; }
        public required string Alias { get; init; }
        public required string Url { get; init; }
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

            var picture = await _context.Pictures
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.UserId == request.UserId &&
                    a.Alias == loweredAlias,
                    cancellationToken);

            if (picture is not null)
            {
                picture.Url = request.Url;
                _context.Pictures.Update(picture);
            }
            else
            {
                picture = new Picture
                {
                    UserId = request.UserId,
                    Alias = loweredAlias,
                    Url = request.Url
                };
                _context.Pictures.Add(picture);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
namespace StepSys.Infrastructure.Data;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigurePictures();
        modelBuilder.ConfigureStoredMessages();
    }

    public DbSet<Picture> Pictures => Set<Picture>();
    public DbSet<StoredMessage> StoredMessages => Set<StoredMessage>();
}

public static class ModelBuilderExtensions
{
    public static void ConfigurePictures(this ModelBuilder modelBuilder)
    {
        var picture = modelBuilder.Entity<Picture>();
        picture.HasKey(x => new { x.UserId, x.Alias });
        picture.Property(x => x.UserId).ValueGeneratedNever();
        picture.Property(x => x.Alias).ValueGeneratedNever();
    }

    public static void ConfigureStoredMessages(this ModelBuilder modelBuilder)
    {
        var storedMessage = modelBuilder.Entity<StoredMessage>();
        storedMessage.HasKey(x => new { x.UserId, x.Alias });
        storedMessage.Property(x => x.UserId).ValueGeneratedNever();
        storedMessage.Property(x => x.Alias).ValueGeneratedNever();
    }
}
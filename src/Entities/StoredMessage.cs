namespace StepSys.Entities;

internal class StoredMessage
{
    public required ulong UserId { get; init; }
    public required string Alias { get; init; }
    public required ulong ChannelId { get; set; }
    public required ulong MessageId { get; set; }
}

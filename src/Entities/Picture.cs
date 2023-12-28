namespace StepSys.Entities;

internal class Picture
{
    public required ulong UserId { get; init; }
    public required string Alias { get; init; }
    public required string Url { get; set; }
}

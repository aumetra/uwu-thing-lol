namespace FixelPlut.Shared.Models;

public sealed class WorkItem
{
    public long Id { get; set; }
    public long TimeStamp { get; set; }
    public string Cmd { get; set; }
}

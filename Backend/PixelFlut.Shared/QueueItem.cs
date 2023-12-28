namespace PixelFlut.Shared;

public class QueueItem
{
    public Guid Id { get; set; }
    public long Timestamp { get; set; }
    public string[] Cmd { get; set; }
}

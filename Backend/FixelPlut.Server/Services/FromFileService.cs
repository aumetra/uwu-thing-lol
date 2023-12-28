using ImageMagick;
using System.Text.RegularExpressions;

namespace FixelPlut.Server.Services;

public partial class FromFileService : ILoaderService
{
    private readonly QueueService queueService;

    public FromFileService(IQueueService queueService)
    {
        this.queueService = (QueueService)queueService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var ms = new MemoryStream();
            await using var writer = new StreamWriter(ms);
            using var img = new MagickImage("37c3-cover-image.gif", MagickFormat.Gif);

            img.Resize(400, 600);

            const int offsetX = 1100; // 960;
            const int offsetY = 400; // 540;

            var width = img.Width;
            var height = img.Height;

            await img.WriteAsync(ms, MagickFormat.Txt, cancellationToken);
            using var reader = new StreamReader(ms);
            reader.BaseStream.Position = 0;

            var regex = PositionRegex();

            while (!reader.EndOfStream)
            {
                var row = await reader.ReadLineAsync(cancellationToken);
                if (row == null)
                    continue;

                var result = regex.Match(row);
                if (!result.Success || result.Groups.Count != 4)
                    continue;

                if (result.Groups[3].Value == "000000")
                    continue;

                queueService.Add(
                    "PX " +
                    Parse(result.Groups[1].Value, offsetX) +
                    " " +
                    Parse(result.Groups[2].Value, offsetY) +
                    " " +
                    result.Groups[3].Value);
            }
            queueService.Shuffle();
            return;

        }
        catch (Exception ex)
        {

            throw;
        }
        /*
PX 1300 16 000000
PX 1300 17 000000
PX 1300 18 000000
PX 1300 19 000000

128,231: (0,0,0)  #000000  black
129,231: (0,0,0)  #000000  black
130,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
131,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
132,231: (0,0,0)  #000000  black
133,231: (96,96,96)  #606060  srgb(96,96,96)
134,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
135,231: (96,96,96)  #606060  srgb(96,96,96)
136,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
137,231: (96,96,96)  #606060  srgb(96,96,96)
138,231: (0,0,0)  #000000  black
139,231: (96,96,96)  #606060  srgb(96,96,96)
140,231: (255,255,255)  #FFFFFF  white
141,231: (96,96,96)  #606060  srgb(96,96,96)
142,231: (96,96,96)  #606060  srgb(96,96,96)
143,231: (0,0,0)  #000000  black
144,231: (255,255,255)  #FFFFFF  white
145,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
146,231: (159,159,159)  #9F9F9F  srgb(159,159,159)
147,231: (96,96,96)  #606060  srgb(96,96,96)
148,231: (96,96,96)  #606060  srgb(96,96,96)
        */
    }

    private static int Parse(string x, int offset)
        => int.Parse(x) + offset;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    [GeneratedRegex("(\\d*),(\\d*).*?#(\\w{6})")]
    private static partial Regex PositionRegex();
}

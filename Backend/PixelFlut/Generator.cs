using System.Drawing;

namespace PixelFlut;

internal static class Generator
{
    internal static string[] GetCommands(string[,] pixels, int offsetX, int offsetY)
    {
        var cmds = new string[pixels.GetLength(0) * pixels.GetLength(1)];
        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                //if (pixels[x, y] == "000000")
                //    continue;

                cmds[(x * pixels.GetLength(1)) + y] = "PX " + (x + offsetX) + " " + (y + offsetY) + " " + pixels[x, y] + "\n";
            }
        }
        return cmds;
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PixelFlut
{
    internal static class Program
    {
        const int s_maxClients = 3;

        static async Task Main(string[] args)
        {
            // w: 1920
            // h: 1080

            try
            {
                using (Image gifImage = Image.FromFile("C:\\Users\\danielh\\Downloads\\37c3-cover-image.gif"))
                {
                    // Loop through each frame in the GIF
                    FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
                    int frameCount = gifImage.GetFrameCount(dimension);

                    var cmds = new string[frameCount][];
                    for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                    {
                        gifImage.SelectActiveFrame(dimension, frameIndex);
                        Bitmap frameBitmap = new Bitmap(gifImage);
                        var pixels = LoadImage(frameBitmap);
                        cmds[frameIndex] = Generator.GetCommands(pixels, 1300, 0); //, (1920 ) - (pixels.GetLength(0) ), (1080 ) - (pixels.GetLength(1) )-200);
                    }
                    Console.WriteLine("READY");

                    var a = JsonSerializer.Serialize(cmds);
                    var path = "..\\..\\..\\..\\PixelFlut.Server\\Commands\\37C3\\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    for (int i = 0; i < cmds.Length; i++)
                    {
                        var sb = new StringBuilder();
                        for (int x = 0; x < cmds[i].Length; x++)
                        {
                            sb.Append(cmds[i][x]);
                        }
                        File.WriteAllText(path + i + ".txt", sb.ToString());
                    }
                    return;
                    Run(cmds);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void Run(string[][] cmds)
        {
            var tasks = new List<Task>();
            var part = cmds[0].Length / s_maxClients;

            do
            {
                readyState = new bool[s_maxClients];
                for (int i = 0; i < s_maxClients; i++)
                {
                    var tmpCmds = new string[cmds.Length][];
                    for (int x = 0; x < cmds.Length; x++)
                    {
                        tmpCmds[x] = cmds[x].Skip(i * part).Take(part).ToArray();
                    }
                    var index = i;
                    Console.WriteLine(i.ToString());
                    tasks.Add(Task.Run(async () => await Update(tmpCmds, index)));
                }
                Task.WaitAll([.. tasks]);
            } while (true);
        }
        static bool[] readyState;
        private static async Task Update(string[][] frameCmds, int index)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync("151.217.15.79", 1337);
                await using var writer = new StreamWriter(client.GetStream());

                while (true)
                {
                    for (int frame = 0; frame < frameCmds.Length; frame++)
                    {
                        for (int sleep = 0; sleep < 1; sleep++)
                        {
                            var cmds = frameCmds[frame];
                            for (int i = 0; i < cmds.Length; i++)
                            {
                                if (cmds[i] != null)
                                {
                                    await writer.WriteAsync(cmds[i]);
                                }
                            }
                            await writer.FlushAsync();
                            //Console.WriteLine("SLEEP");
                        }

                        //if (index == 0)
                        //{
                        //    while (true)
                        //    {
                        //        bool any = false;
                        //        for (int i = 1; i < s_maxClients; i++)
                        //        {
                        //            if (!readyState[i])
                        //            {
                        //                any = true;
                        //                break;
                        //            }
                        //        }
                        //        if (!any)
                        //            break;
                        //        await Task.Delay(50);
                        //    }
                        //    for (int i = 1; i < s_maxClients; i++)
                        //    {
                        //        readyState[i] = false;
                        //    }
                        //}
                        //else
                        //{
                        //    if (readyState[index])
                        //        await Task.Delay(50);
                        //}
                    }

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.FFF"));
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private static string[,] LoadImage(Bitmap img)
        {
            var pixels = new string[img.Width, img.Height];
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    var p = img.GetPixel(x, y);
                    pixels[x, y] = $"{p.R:X2}{p.G:X2}{p.B:X2}";
                }
            }
            return pixels;
        }
    }
}

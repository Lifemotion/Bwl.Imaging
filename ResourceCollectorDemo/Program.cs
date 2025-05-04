using System.Diagnostics;
using System.Drawing;
using Bwl.Imaging;

namespace ResourceCollectorDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WriteLine("ResourceCollectorDemo");
            Console.WriteLine();

            // Params
            int channelCount = 16;
            var frameSize = new Size(1920, 1080);
            double fpsTotal = 10 * channelCount;
            double framePeriod = 1000.0 / fpsTotal;
            
            // JPEG
            byte[] jpeg;
            using (var bitmapInfo = new BitmapInfo(new Bitmap(frameSize.Width, frameSize.Height)))
            {
                bitmapInfo.Compress();
                jpeg = bitmapInfo.GetJpg();
            }

            // Frames
            var sw = new Stopwatch();
            var rl = new RunLimiter();
            var thrFrames = new Thread(() =>
            {
                while (true)
                {
                    sw.Restart();
                    rl.Run(() => Console.WriteLine($"ResourceCollector.Count={ResourceCollector<BitmapInfo>.Count}; Proc={ResourceCollector<BitmapInfo>.ProcTimeMs} ms; Period={ResourceCollector<BitmapInfo>.PeriodMs} ms; channelCount={channelCount}; fpsTotal={fpsTotal};"));
                    var bitmapInfo = new BitmapInfo(jpeg);
                    var bmp = bitmapInfo.GetClonedBmp();
                    sw.Stop(); Thread.Sleep(TimeSpan.FromMilliseconds(Math.Max(framePeriod - sw.Elapsed.TotalMilliseconds, 0)));
                }
            }) { IsBackground = true };
            thrFrames.Start();

            // GC
            var thrGC = new Thread(() =>
            {
                while (true)
                {
                    GC.Collect();
                    Thread.Sleep(1000);
                }
            }) { IsBackground = true };
            thrGC.Start();

            Console.ReadKey();
        }
    }
}

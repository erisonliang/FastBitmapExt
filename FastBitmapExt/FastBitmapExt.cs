using System;
using System.Threading.Tasks;

namespace Hazdryx.Drawing.Extension
{
    /// <summary>
    ///     A class which contains various extensions for FastBitmap.
    /// </summary>
    public static class FastBitmapExt
    {
        /// <summary>
        ///     Streams each Y value to the callback.
        /// </summary>
        /// <param name="callback">The callback delgate</param>
        /// <param name="threads">The number of threads to use.</param>
        public static void StreamByLine(this FastBitmap bitmap, StreamByLineCallback callback, int threads)
        {
            Task[] tasks = new Task[threads];
            for (int i = 0; i < threads; i++)
            {
                tasks[i] = new Task(new ScanlineStreamInfo(bitmap, callback, i, threads).GetAction());
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        ///     Streams each X and Y value to the callback.
        /// </summary>
        /// <param name="callback">The callback delegate</param>
        /// <param name="threads">The number threads to use.</param>
        public static void StreamByPixel(this FastBitmap bitmap, StreamByPixelCallback callback, int threads)
        {
            bitmap.StreamByLine(new StreamByLineCallback((bmp, y) =>
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    callback(bmp, x, y);
                }
            }), threads);
        }
    }

    /// <summary>
    ///     A delegate for handling StreamByLine calls.
    /// </summary>
    /// <param name="bmp">The bitmap being used for streaming.</param>
    /// <param name="y">The current line on the y axis.</param>
    public delegate void StreamByLineCallback(FastBitmap bmp, int y);
    /// <summary>
    ///     A delegate for handling StreamByPixel calls.
    /// </summary>
    /// <param name="bmp">The bitmap being used for streaming.</param>
    /// <param name="x">The x component of the current pixel.</param>
    /// <param name="y">The y component of the current pixel.</param>
    public delegate void StreamByPixelCallback(FastBitmap bmp, int x, int y);
}

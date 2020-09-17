/*
    MIT License

    Copyright(c) 2020 Christopher Bishop

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System.Threading.Tasks;

namespace Hazdryx.Drawing.Extension
{
    /// <summary>
    ///     A class which contains various extensions for FastBitmap.
    /// </summary>
    public static class FastBitmapExt
    {
        /// <summary>
        ///     Gets a color at the index as a ColorF.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ColorF GetF(this FastBitmap self, int index)
        {
            return new ColorF(self.Data[index]);
        }
        /// <summary>
        ///     Gets the color at a coordinate as a ColorF.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static ColorF GetF(this FastBitmap self, int x, int y)
        {
            return new ColorF(self.Data[x + y * self.Width]);
        }
        /// <summary>
        ///     Sets the color at an index using a ColorF.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="color"></param>
        /// <param name="index"></param>
        public static void SetF(this FastBitmap self, ColorF color, int index)
        {
            self.Data[index] = color.ToArgb();
        }
        /// <summary>
        ///     Sets the color at a coordinate using a ColorF.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="color"></param>
        /// <param name="index"></param>
        public static void SetF(this FastBitmap self, ColorF color, int x, int y)
        {
            self.Data[x + y * self.Width] = color.ToArgb();
        }

        /// <summary>
        ///     Streams each Y value to the callback.
        /// </summary>
        /// <param name="callback">The callback delgate</param>
        /// <param name="threads">The number of threads to use.</param>
        public static void StreamByLine(this FastBitmap self, StreamByLineCallback callback, int threads)
        {
            Task[] tasks = new Task[threads];
            for (int i = 0; i < threads; i++)
            {
                tasks[i] = new Task(new ScanlineStreamInfo(self, callback, i, threads).GetAction());
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }
        /// <summary>
        ///     Streams each Y value to the callback on a single thread.
        /// </summary>
        /// <param name="callback">The callback delgate</param>
        public static void StreamByLine(this FastBitmap self, StreamByLineCallback callback)
        {
            self.StreamByLine(callback, 1);
        }

        /// <summary>
        ///     Streams each X and Y value to the callback.
        /// </summary>
        /// <param name="callback">The callback delegate</param>
        /// <param name="threads">The number threads to use.</param>
        public static void StreamByPixel(this FastBitmap self, StreamByPixelCallback callback, int threads)
        {
            self.StreamByLine(new StreamByLineCallback((bmp, y) =>
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    callback(bmp, x, y);
                }
            }), threads);
        }
        /// <summary>
        ///     Streams each X and Y value to the callback on a
        ///     single thread.
        /// </summary>
        /// <param name="callback">The callback delegate</param>
        public static void StreamByPixel(this FastBitmap self, StreamByPixelCallback callback)
        {
            self.StreamByPixel(callback, 1);
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

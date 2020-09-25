using System;
using System.Drawing;

namespace Hazdryx.Drawing.Processing
{
    /// <summary>
    ///     Filters images using a weighted matrix kernel.
    /// </summary>
    public static class KernelFilter
    {
        public static void ApplyKernel(FastBitmap bitmap, KernelMatrix matrix, int threads)
        {
            FastBitmap src = (FastBitmap) bitmap.Clone();
            int halfSize = (matrix.Size - 1) / 2;

            src.StreamByLine((bmp, y) =>
            {
                int[] line = new int[bitmap.Width];
                float a, r, g, b;
                float colorSum;

                // Go through each horizontal pixel.
                for (int x = 0; x < bmp.Width; x++)
                {
                    // Reset vars.
                    a = 0;
                    r = 0;
                    g = 0;
                    b = 0;
                    colorSum = matrix.Sum;

                    // Go through matrix.
                    for (int mx = 0; mx < matrix.Size; mx++)
                    {
                        for (int my = 0; my < matrix.Size; my++)
                        {
                            float weight = matrix[mx, my];
                            bmp.TryGet(x + mx - halfSize, y + my - halfSize, out Color c);

                            if (c.A == 0)
                            {
                                colorSum -= weight;
                            }
                            else
                            {
                                a += c.A * weight;
                                r += c.R * weight;
                                g += c.G * weight;
                                b += c.B * weight;
                            }
                        }
                    }

                    // Set pixel.
                    if (colorSum > 0)
                    {
                        line[x] = (byte) (a / matrix.Sum) << 24 |
                            (byte) (r / colorSum) << 16 |
                            (byte) (g / colorSum) << 8 |
                            (byte) (b / colorSum);
                    }
                }

                // Write line to image
                Buffer.BlockCopy(line, 0, bitmap.Data, y * bitmap.Width * 4, line.Length * 4);
            }, threads);
        }
    }

    /// <summary>
    ///     A matrix specifically used for the KernelFilter.
    /// </summary>
    public struct KernelMatrix
    {
        private float[] Matrix { get; }

        public int Length => Matrix.Length;
        /// <summary>
        ///     Gets the size of the matrix.
        /// </summary>
        public int Size { get; }
        /// <summary>
        ///     Gets the sum of the matrix.
        /// </summary>
        public float Sum { get; private set; }

        public KernelMatrix(int size)
        {
            if (size <= 1) throw new ArgumentException("Size must be greater than 1.");
            if (size % 2 != 1) throw new ArgumentException("Size must be an odd number.");

            Matrix = new float[size * size];
            Size = size;
            Sum = 0;
        }
        public KernelMatrix(float[] matrix) : this((int) Math.Sqrt(matrix.Length))
        {
            try
            {
                SetAll(matrix);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Mismatched matrix length (not a perfect square).", e);
            }
        }

        /// <summary>
        ///     Gets or sets the matrix based on an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get => Matrix[index];
            set
            {
                Sum += value - Matrix[index];
                Matrix[index] = value;
            }
        }
        /// <summary>
        ///     Gets or sets the value at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float this[int x, int y]
        {
            get => this[x + y * Size];
            set => this[x + y * Size] = value;
        }
        /// <summary>
        ///     Sets the entire matrix.
        /// </summary>
        /// <param name="matrix"></param>
        public void SetAll(float[] matrix)
        {
            if (matrix.Length != Matrix.Length) throw new ArgumentException("KernelMatrix and matrix must be the same size");

            Sum = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                Matrix[i] = matrix[i];
                Sum += matrix[i];
            }
        }

        /// <summary>
        ///     Creates a box blur kernel with
        ///     a given size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static KernelMatrix BoxBlur(int size)
        {
            KernelMatrix matrix = new KernelMatrix(size);
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = 1;
            }
            return matrix;
        }
    }
}

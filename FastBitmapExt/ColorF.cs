﻿/*
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
using System;
using System.Drawing;

namespace Hazdryx.Drawing
{
    /// <summary>
    ///     A color strcutures which represents colors as normalized floats
    ///     for easier color manipulation.
    /// </summary>
    public struct ColorF
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        /// <summary>
        ///     Instantiates a color which is a copy of the
        ///     color given.
        /// </summary>
        /// <param name="color"></param>
        public ColorF(ColorF color)
        {
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
            this.A = color.A;
        }
        /// <summary>
        ///     Instantiates a color which is a copy of the
        ///     System.Drawing.Color given.
        /// </summary>
        /// <param name="color"></param>
        public ColorF(Color color) : this(color.R, color.G, color.B, color.A)
        {
        }
        /// <summary>
        ///     Instantiates color with the specified values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public ColorF(float r, float g, float b, float a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        /// <summary>
        ///     Instantiates color with the specified values and
        ///     an alpha of 1.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public ColorF(float r, float g, float b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 1;
        }
        /// <summary>
        ///     Instantiates a color with the specified values
        ///     after they are normalized.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public ColorF(int r, int g, int b, int a)
        {
            this.R = r / 255f;
            this.G = g / 255f;
            this.B = b / 255f;
            this.A = a / 255f;
        }
        /// <summary>
        ///     Instantiates a color with the specified values
        ///     after they are normalized with an alpha of 255.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public ColorF(int r, int g, int b)
        {
            this.R = r / 255f;
            this.G = g / 255f;
            this.B = b / 255f;
            this.A = 1;
        }
        /// <summary>
        ///     Instatiates a color from a ARGB32 int value.
        /// </summary>
        /// <param name="argb"></param>
        public ColorF(int argb)
        {
            this.B = ((argb >> 0) & 0xFF) / 255f;
            this.G = ((argb >> 8) & 0xFF) / 255f;
            this.R = ((argb >> 16) & 0xFF) / 255f;
            this.A = ((argb >> 24) & 0xFF) / 255f;
        }

        /// <summary>
        ///     Gets the squared magnitude of the color.
        /// </summary>
        public float MagnitudeSq => R * R + G * G + B * B;
        /// <summary>
        ///     Gets the magnitude of the color.
        /// </summary>
        public float Magnitude => (float) Math.Sqrt(Magnitude);
        /// <summary>
        ///     Gets a premultiplied version of this color.
        /// </summary>
        public ColorF PreMultiplied
        {
            get
            {
                return new ColorF
                {
                    R = R * A,
                    G = G * A,
                    B = B * A,
                    A = A
                };
            }
        }

        /// <summary>
        ///     Clamps the color components to be between 0 and 1.
        /// </summary>
        public void Clamp()
        {
            R = Clamp(R);
            G = Clamp(G);
            B = Clamp(B);
            A = Clamp(A);
        }
        private float Clamp(float value)
        {
            return (value >= 1) ? 1 : (value <= 0) ? 0 : value;
        }
        /// <summary>
        ///     Normalizes the RGB values as if it were a
        ///     3D vector and clamp the alpha value.
        /// </summary>
        public void Normalize()
        {
            A = Clamp(A);

            float magSq = MagnitudeSq;
            if (magSq != 0 && magSq != 1)
            {
                float k = 1 / (float) Math.Sqrt(magSq);
                R *= k;
                G *= k;
                B *= k;
            }
        }

        /// <summary>
        ///     Converts this color into an ARGB32 int.
        /// </summary>
        /// <returns></returns>
        public int ToArgb()
        {
            ColorF c = new ColorF(this);
            c.Clamp();

            byte r = (byte) Math.Round(c.R * 255);
            byte g = (byte) Math.Round(c.G * 255);
            byte b = (byte) Math.Round(c.B * 255);
            byte a = (byte) Math.Round(c.A * 255);

            return a << 24 | r << 16 | g << 8 | b;
        }

        #region Operators
        /// <summary>
        ///     Blends two colors together based on their alphas.
        /// </summary>
        /// <param name="cb">Back color</param>
        /// <param name="cf">Fore color</param>
        /// <returns></returns>
        public static ColorF operator +(ColorF cb, ColorF cf)
        {
            float af = cf.A;
            float ab = af * (1 - cb.A);
            float ao = af + ab;

            return new ColorF
            {
                R = (cf.R * af + cb.R * ab) / ao,
                G = (cf.G * af + cb.G * ab) / ao,
                B = (cf.B * af + cb.B * ab) / ao,
                A = ao
            };
        }

        /// <summary>
        ///     Adds two colors together regardless of alpha values.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static ColorF operator |(ColorF c1, ColorF c2)
        {
            return new ColorF
            {
                R = c1.R + c2.R,
                G = c1.G + c2.G,
                B = c1.B + c2.B,
                A = c1.A + c2.A
            };
        }
        /// <summary>
        ///     Subtracts two colors regardless of alpha values.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static ColorF operator &(ColorF c1, ColorF c2)
        {
            return new ColorF
            {
                R = c1.R - c2.R,
                G = c1.G - c2.G,
                B = c1.B - c2.B,
                A = c1.A - c2.A
            };
        }

        /// <summary>
        ///     Multiplies two colors' RGB values, which the second
        ///     color is premultiplied. Alpha of the first color is
        ///     maintained.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static ColorF operator *(ColorF c1, ColorF c2)
        {
            ColorF pm = c2.PreMultiplied;
            return new ColorF
            {
                R = c1.R * pm.R,
                G = c1.G * pm.G,
                B = c1.B * pm.B,
                A = c1.A
            };
        }
        /// <summary>
        ///     Divides two colors' RGB values, which the second
        ///     color is premultiplied. Alpha of the first color is
        ///     maintained.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static ColorF operator /(ColorF c1, ColorF c2)
        {
            ColorF pm = c2.PreMultiplied;
            return new ColorF
            {
                R = c1.R / pm.R,
                G = c1.G / pm.G,
                B = c1.B / pm.B,
                A = c1.A
            };
        }

        /// <summary>
        ///     Multiplies the color components by a scalar.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static ColorF operator *(ColorF c, float scalar)
        {
            return new ColorF
            {
                R = c.R * scalar,
                G = c.G * scalar,
                B = c.B * scalar,
                A = c.A * scalar
            };
        }
        /// <summary>
        ///     Multiplies the RGB components by a scalar.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static ColorF operator *(float scalar, ColorF c) => c * scalar;
        /// <summary>
        ///     Divides the RGB components by a scalar (Multiplies by 1 / scalar).
        /// </summary>
        /// <param name="c"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static ColorF operator /(ColorF c, float scalar) => c * (1 / scalar);
        #endregion
    }
}

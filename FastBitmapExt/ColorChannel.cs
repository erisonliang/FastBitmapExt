using System;
using System.Collections.Generic;
using System.Text;

namespace Hazdryx.Drawing.Processing
{
    /// <summary>
    ///     Defines one or many color channels.
    /// </summary>
    public enum ColorChannel
    {
        Alpha = 1,
        Red = 2,
        Green = 4,
        Blue = 8,
        RGB = Red | Green | Blue,
        All = Alpha | Red | Green | Blue,
    }
}

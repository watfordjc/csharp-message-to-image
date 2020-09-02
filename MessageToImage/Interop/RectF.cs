using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImage.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RectF
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImage.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PointF
    {
        public float X;
        public float Y;
    }
}

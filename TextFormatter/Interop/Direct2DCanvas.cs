using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TextFormatter.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Direct2DCanvas
    {
        public IntPtr Bitmap;
        public IntPtr Canvas;
        public Direct2DPointers Direct2DPointers;
    }
}

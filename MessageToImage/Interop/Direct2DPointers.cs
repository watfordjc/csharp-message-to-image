using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TextFormatter.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Direct2DPointers
    {
        public IntPtr Direct2DFactory;
        public IntPtr DirectWriteFactory;
        public IntPtr WICImagingFactory;
    }
}

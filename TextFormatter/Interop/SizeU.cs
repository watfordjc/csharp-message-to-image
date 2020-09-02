using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TextFormatter.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SizeU
    {
        public UInt32 Width;
        public UInt32 Height;
    }
}

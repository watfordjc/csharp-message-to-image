using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImage.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SizeU
    {
        public UInt32 Width;
        public UInt32 Height;
    }
}

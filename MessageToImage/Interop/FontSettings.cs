using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImage.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontSettings
    {
        public float FontSize;
        public int FontWeight;
        public bool JustifyCentered;
        public String FontName;
        public String LocaleName;
    }
}

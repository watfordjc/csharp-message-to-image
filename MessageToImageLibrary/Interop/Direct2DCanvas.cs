using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImageLibrary.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Direct2DCanvas
    {
        public IntPtr DXGISwapChain;
        public IntPtr Surface;
        public IntPtr Bitmap;
        public IntPtr RenderTarget;
        public IntPtr SharedTexture;
        public IntPtr SharedHandle;
        public IntPtr SharedTextureMutex;
        public Direct2DPointers Direct2DPointers;
    }
}

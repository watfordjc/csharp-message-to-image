using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImageLibrary.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Direct2DPointers
    {
		// Pointers for Direct2D
		public IntPtr Direct2DFactory;
		public IntPtr Direct2DDevice;
		public IntPtr Direct2DDeviceContext;
        public IntPtr DirectWriteFactory;

		// Pointers for Direct3D
		public IntPtr Direct3DDevice;
		public IntPtr Direct3DFeatureLevel;
		public IntPtr Direct3DDeviceContext;

		// Pointers for DXGI
		public IntPtr DXGIDevice;
		public IntPtr DXGIFactory;
	}
}

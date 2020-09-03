using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImageLibrary.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextLayoutResult
    {
		public IntPtr pDWriteTextLayout;
		public int lineCount; // Lines of text
		public int top; // Top edge of text block from edge of canvas
		public int left; // Left edge of text block from edge of canvas
		public double height; // Height of text block
		public double width; // Width of text block
		public float lineSpacing; // Line-spacing for font used
		public float baseline; // Baseline for font used
		public double lineHeight;
		public double lineHeightEm;
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImage
{
    public class VerticalMessagePanel : MessagePanel
    {
        public VerticalMessagePanel(ref Interop.Direct2DPointers direct2DPointers, Interop.SizeU canvasSize, UInt32 backgroundcolor) : base(ref direct2DPointers, canvasSize, backgroundcolor)
        {

        }
    }
}

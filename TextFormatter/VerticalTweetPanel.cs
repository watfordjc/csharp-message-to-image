using System;
using System.Collections.Generic;
using System.Text;

namespace TextFormatter
{
    public class VerticalTweetPanel : TweetPanel
    {
        public VerticalTweetPanel(ref Interop.Direct2DPointers direct2DPointers, Interop.SizeU canvasSize, UInt32 backgroundcolor) : base(ref direct2DPointers, canvasSize, backgroundcolor)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImageLibrary.Interop
{
    internal static class Import
    {
        public const string lib = @"lib\Direct2DWrapper.dll";
    }

    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// Add an integer and double together
        /// </summary>
        /// <param name="a">An integer</param>
        /// <param name="b">A double</param>
        /// <returns>A double equal to a + b</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Add(int a, double b);

        /// <summary>
        /// Create an ID2D1Factory
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal static extern int CreateD2D1Factory(ref Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Free an ID2D1Factory
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseD2D1Factory(Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Create an IWICImagingFactory
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal static extern int CreateImagingFactory(ref Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Free an IWICImagingFactory
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseImagingFactory(Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Create an IDWriteFactory7
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal static extern int CreateDWriteFactory(ref Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Free an IDWriteFactory7
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDWriteFactory(Direct2DPointers pDirect2DPointers);

        /// <summary>
        /// Create an IWICBitmap
        /// </summary>
        /// <param name="pDirect2DPointers">An instantiated instance of Direct2DPointers</param>
        /// <param name="width">Desired bitmap width in pixels</param>
        /// <param name="height">Desired bitmap height in pixels</param>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal static extern int CreateWICBitmap(ref Direct2DPointers pDirect2DPointers, uint width, uint height, ref Direct2DCanvas pCanvas);

        /// <summary>
        /// Free an IWICBitmap
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseWICBitmap(Direct2DCanvas pCanvas);

        /// <summary>
        /// Create an ID2D1RenderTarget
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int CreateRenderTarget(ref Direct2DCanvas pCanvas);

        /// <summary>
        /// Free an ID2D1RenderTarget
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseRenderTarget(Direct2DCanvas pCanvas);

        /// <summary>
        /// Call Direct2D BeginDraw()
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void BeginDraw(Direct2DCanvas pCanvas);

        /// <summary>
        /// Call Direct2D EndDraw()
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int EndDraw(Direct2DCanvas pCanvas);

        /// <summary>
        /// Draw an image
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="clearColor">A 32-bit unsigned integer containing the 8-bit values for ARGB - 0xAARRGGBB</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawImage(Direct2DCanvas pCanvas, UInt32 clearColor);

        /// <summary>
        /// Create an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="fillColor">A 32-bit unsigned integer containing the 8-bit values for ARGB - 0xAARRGGBB</param>
        /// <returns>A pointer to an ID2D1SolidColorBrush</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr CreateSolidColorBrush(Direct2DCanvas pCanvas, UInt32 fillColor);

        /// <summary>
        /// Free an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pD2D1SolidColorBrush">A pointer to the ID2D1SolidColorBrush to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseSolidColorBrush(IntPtr pD2D1SolidColorBrush);

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="lineColor">A pointer to an ID2D1SolidColorBrush for the line colour</param>
        /// <param name="headingSeparatorPoint1">The coordinates of the start point</param>
        /// <param name="headingSeparatorPoint2">The coordinates of the end point</param>
        /// <param name="lineThickness">The brush thickness (line width) of the line drawn</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawLine(Direct2DCanvas pCanvas, IntPtr lineColor, PointF headingSeparatorPoint1, PointF headingSeparatorPoint2, float lineThickness);

        /// <summary>
        /// Draw a rectangle border
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the border colour</param>
        /// <param name="startX">The x coordinate of the top-left pixel</param>
        /// <param name="startY">The y coordinate of the top-left pixel</param>
        /// <param name="lengthX">The length of the rectangle in pixels</param>
        /// <param name="lengthY">The height of the rectangle in pixels</param>
        /// <param name="lineWidth">The brush thickness (line width) of the lines drawn</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawRectangleBorder(Direct2DCanvas pCanvas, IntPtr pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth);

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the background/fill colour</param>
        /// <param name="bounds">The left, top, right, and bottom of the rectangle</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawRectangle(Direct2DCanvas pCanvas, IntPtr pD2D1SolidColorBrush, RectF bounds);

        /// <summary>
        /// Call Direct2D BeginDraw() followed by a PushLayer() of an ellipses
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the background/fill colour</param>
        /// <param name="centerX">The x coordinate of the center of the ellipsis</param>
        /// <param name="centerY">The y coordinate of the center of the ellipsis</param>
        /// <param name="radiusX">The x radius of the ellipsis</param>
        /// <param name="radiusY">The y radius of the ellipsis</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int PushEllipseLayer(Direct2DCanvas pCanvas, IntPtr pD2D1SolidColorBrush, float centerX, float centerY, float radiusX, float radiusY);

        /// <summary>
        /// Call Direct2D PopLayer() followed by EndDraw()
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int PopLayer(Direct2DCanvas pCanvas);

        /// <summary>
        /// Draw an image from a filename
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="filename">The path to the file of an existing image</param>
        /// <param name="originPoint">The coordinates of the top-left pixel</param>
        /// <param name="bounds">The left, top, right, and bottom of the rectangle</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int DrawImageFromFilename(Direct2DCanvas pCanvas, String filename, PointF originPoint, RectF bounds);

        /// <summary>
        /// Create an IDWriteTextLayout
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="text">A string containing the text to draw</param>
        /// <param name="bounds">The left, top, right, and bottom of the rectangle</param>
        /// <param name="fontSettings">The FontSettings for this text</param>
        /// <param name="textLayoutResult">A TextLayoutResult struct</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int CreateTextLayoutFromString(Direct2DCanvas pCanvas, String text, RectF bounds, FontSettings fontSettings, out TextLayoutResult textLayoutResult);

        /// <summary>
        /// Draw an IDWriteTextLayout
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="textLayoutResult">A struct TextLayoutResult containing a pointer to an IDWriteTextLayout</param>
        /// <param name="originPoint">The coordinates of the top-left pixel of the text block</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the text colour</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static void DrawTextLayout(Direct2DCanvas pCanvas, TextLayoutResult textLayoutResult, PointF originPoint, IntPtr pD2D1SolidColorBrush);

        /// <summary>
        /// Free an IDWriteTextLayout
        /// </summary>
        /// <param name="textLayoutResult">A pointer to the IDWriteTextLayout to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static void ReleaseTextLayout(TextLayoutResult textLayoutResult);

        /// <summary>
        /// Save a drawn image
        /// </summary>
        /// <param name="pCanvas">An instantiated instance of Direct2DCanvas</param>
        /// <param name="filename">The path to the file where the image is to be saved</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int SaveImage(Direct2DCanvas pCanvas, String filename);
    }
}

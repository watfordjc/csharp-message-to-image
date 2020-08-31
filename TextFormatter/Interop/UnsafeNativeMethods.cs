using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TextFormatter.Interop
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
        /// <returns>A pointer to an ID2D1Factory</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateD2D1Factory();

        /// <summary>
        /// Free an ID2D1Factory
        /// </summary>
        /// <param name="pD2DFactory">A pointer to the ID2D1Factory to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseD2D1Factory(IntPtr pD2DFactory);

        /// <summary>
        /// Create an IWICImagingFactory
        /// </summary>
        /// <returns>A pointer to an IWICImagingFactory</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateImagingFactory();

        /// <summary>
        /// Free an IWICImagingFactory
        /// </summary>
        /// <param name="pWICImagingFactory">A pointer to the IWICImagingFactory to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseImagingFactory(IntPtr pWICImagingFactory);

        /// <summary>
        /// Create an IDWriteFactory7
        /// </summary>
        /// <returns>A pointer to an IDWriteFactory7</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateDWriteFactory();

        /// <summary>
        /// Free an IDWriteFactory7
        /// </summary>
        /// <param name="pDWriteFactory">A pointer to the IDWriteFactory7 to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDWriteFactory(IntPtr pDWriteFactory);

        /// <summary>
        /// Create an IWICBitmap
        /// </summary>
        /// <param name="pWICImagingFactory">A pointer to an IWICImagingFactory</param>
        /// <param name="width">Desired bitmap width in pixels</param>
        /// <param name="height">Desired bitmap height in pixels</param>
        /// <returns>A pointer to an IWICBitmap</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateWICBitmap(IntPtr pWICImagingFactory, uint width, uint height);

        /// <summary>
        /// Free an IWICBitmap
        /// </summary>
        /// <param name="pWICBitmap">A pointer to the IWICBitmap to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseWICBitmap(IntPtr pWICBitmap);

        /// <summary>
        /// Create an ID2D1RenderTarget
        /// </summary>
        /// <param name="pD2D1Factory">A pointer to an ID2D1Factory</param>
        /// <param name="pWICBitmap">A pointer to an IWICBitmap</param>
        /// <returns>A pointer to an ID2D1RenderTarget</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr CreateRenderTarget(IntPtr pD2D1Factory, IntPtr pWICBitmap);

        /// <summary>
        /// Free an ID2D1RenderTarget
        /// </summary>
        /// <param name="factory">A pointer to the ID2D1RenderTarget to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseRenderTarget(IntPtr pD2D1RenderTarget);

        /// <summary>
        /// Call Direct2D BeginDraw()
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void BeginDraw(IntPtr pD2D1RenderTarget);

        /// <summary>
        /// Call Direct2D EndDraw()
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int EndDraw(IntPtr pD2D1RenderTarget);

        /// <summary>
        /// Draw an image
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="clearColor">A 32-bit unsigned integer containing the 8-bit values for ARGB - 0xAARRGGBB</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawImage(IntPtr pD2D1RenderTarget, UInt32 clearColor);

        /// <summary>
        /// Create an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="fillColor">A 32-bit unsigned integer containing the 8-bit values for ARGB - 0xAARRGGBB</param>
        /// <returns>A pointer to an ID2D1SolidColorBrush</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr CreateSolidColorBrush(IntPtr pD2D1RenderTarget, UInt32 fillColor);

        /// <summary>
        /// Free an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pD2D1SolidColorBrush">A pointer to the ID2D1SolidColorBrush to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseSolidColorBrush(IntPtr pD2D1SolidColorBrush);

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the stroke colour</param>
        /// <param name="startX">The x coordinate of the first pixel</param>
        /// <param name="startY">The y coordinate of the first pixel</param>
        /// <param name="lengthX">The x coordinate of the second pixel</param>
        /// <param name="lengthY">The y coordinate of the second pixel</param>
        /// <param name="lineWidth">The brush thickness (line width) of the line drawn</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawLine(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, int startX, int startY, int stopX, int stopY, float lineWidth);

        /// <summary>
        /// Draw a rectangle border
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the border colour</param>
        /// <param name="startX">The x coordinate of the top-left pixel</param>
        /// <param name="startY">The y coordinate of the top-left pixel</param>
        /// <param name="lengthX">The length of the rectangle in pixels</param>
        /// <param name="lengthY">The height of the rectangle in pixels</param>
        /// <param name="lineWidth">The brush thickness (line width) of the lines drawn</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawRectangleBorder(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth);

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the background/fill colour</param>
        /// <param name="startX">The x coordinate of the top-left pixel</param>
        /// <param name="startY">The y coordinate of the top-left pixel</param>
        /// <param name="lengthX">The length of the rectangle in pixels</param>
        /// <param name="lengthY">The height of the rectangle in pixels</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void DrawRectangle(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY);

        /// <summary>
        /// Call Direct2D BeginDraw() followed by a PushLayer() of an ellipses
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the background/fill colour</param>
        /// <param name="centerX">The x coordinate of the center of the ellipsis</param>
        /// <param name="centerY">The y coordinate of the center of the ellipsis</param>
        /// <param name="radiusX">The x radius of the ellipsis</param>
        /// <param name="radiusY">The y radius of the ellipsis</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int PushEllipseLayer(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, float centerX, float centerY, float radiusX, float radiusY);

        /// <summary>
        /// Call Direct2D PopLayer() followed by EndDraw()
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, PreserveSig = false)]
        internal extern static int PopLayer(IntPtr pD2D1RenderTarget);

        /// <summary>
        /// Draw an image from a filename
        /// </summary>
        /// <param name="pWICImagingFactory">A pointer to an IWICImagingFactory</param>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="filename">The path to the file of an existing image</param>
        /// <param name="startX">The x coordinate of the top-left pixel</param>
        /// <param name="startY">The y coordinate of the top-left pixel</param>
        /// <param name="width">The width of the image to be drawn after resizing</param>
        /// <param name="height">The height of the image to be drawn after resizing</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int DrawImageFromFilename(IntPtr pWICImagingFactory, IntPtr pD2D1RenderTarget, String filename, int startX, int startY, int width, int height);

        /// <summary>
        /// Create an IDWriteTextLayout
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="text">A string containing the text to draw</param>
        /// <param name="startX">The x coordinate of the top-left pixel of the text block</param>
        /// <param name="startY">The y coordinate of the top-left pixel of the text block</param>
        /// <param name="width">The width of the text block</param>
        /// <param name="height">The height of the text block</param>
        /// <param name="justifyCentered">True if text should be centered, False if it should be left-aligned</param>
        /// <param name="fontName">A string containing the name of the font</param>
        /// <param name="fontSize">Desire text size in DIPs</param>
        /// <param name="fontWeight">Desired font weight (e.g. 400 for normal)</param>
        /// <param name="localeName">A string containing the locale name, such as "en-GB"</param>
        /// <param name="textLayoutResult">A TextLayoutResult struct</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int CreateTextLayoutFromString(IntPtr pDWriteFactory, IntPtr pD2D1RenderTarget, String text, int startX, int startY, int width, int height, bool justifyCentered, String fontName, float fontSize, int fontWeight, String localeName, out TextLayoutResult textLayoutResult);

        /// <summary>
        /// Draw an IDWriteTextLayout
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="textLayoutResult">A struct TextLayoutResult containing a pointer to an IDWriteTextLayout</param>
        /// <param name="startX">The x coordinate of the top-left pixel of the text block</param>
        /// <param name="startY">The y coordinate of the top-left pixel of the text block</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the text colour</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static void DrawTextLayout(IntPtr pD2D1RenderTarget, TextLayoutResult textLayoutResult, int startX, int startY, IntPtr pD2D1SolidColorBrush);

        /// <summary>
        /// Free an IDWriteTextLayout
        /// </summary>
        /// <param name="textLayoutResult">A pointer to the IDWriteTextLayout to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static void ReleaseTextLayout(TextLayoutResult textLayoutResult);

        /// <summary>
        /// Save a drawn image
        /// </summary>
        /// <param name="pWICImagingFactory">A pointer to an IWICImagingFactory</param>
        /// <param name="pWICBitmap">A pointer to an IWICBitmap</param>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="filename">The path to the file where the image is to be saved</param>
        /// <returns>0 if successful, otherwise throws an exception</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int SaveImage(IntPtr pWICImagingFactory, IntPtr pWICBitmap, IntPtr pD2D1RenderTarget, String filename);
    }
}

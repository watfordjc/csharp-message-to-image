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
        /// Draw an image
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <returns>True if image drawn successfully, False if ID2D1RenderTarget needs recreating</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool DrawImage(IntPtr pD2D1RenderTarget);

        /// <summary>
        /// Create an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="rgb">A 32-bit unsigned integer containing the 8-bit values for RGB, such as 0x0000FF for Blue</param>
        /// <param name="alpha">A float for the alpha value</param>
        /// <returns>A pointer to an ID2D1SolidColorBrush</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr CreateSolidColorBrush(IntPtr pD2D1RenderTarget, UInt32 argb);

        /// <summary>
        /// Free an ID2D1SolidColorBrush
        /// </summary>
        /// <param name="pD2D1SolidColorBrush">A pointer to the ID2D1SolidColorBrush to be freed</param>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void ReleaseSolidColorBrush(IntPtr pD2D1SolidColorBrush);

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
        /// <returns>True if rectangle border drawn successfully, False if ID2D1RenderTarget needs recreating</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool DrawRectangleBorder(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth);

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="pD2D1SolidColorBrush">A pointer to an ID2D1SolidColorBrush for the background/fill colour</param>
        /// <param name="startX">The x coordinate of the top-left pixel</param>
        /// <param name="startY">The y coordinate of the top-left pixel</param>
        /// <param name="lengthX">The length of the rectangle in pixels</param>
        /// <param name="lengthY">The height of the rectangle in pixels</param>
        /// <returns>True if rectangle drawn successfully, False if ID2D1RenderTarget needs recreating</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal extern static bool DrawRectangle(IntPtr pD2D1RenderTarget, IntPtr pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY);

        /// <summary>
        /// Save a drawn image
        /// </summary>
        /// <param name="pWICImagingFactory">A pointer to an IWICImagingFactory</param>
        /// <param name="pWICBitmap">A pointer to an IWICBitmap</param>
        /// <param name="pD2D1RenderTarget">A pointer to an ID2D1RenderTarget</param>
        /// <param name="filename">The path to the file where the image is to be saved</param>
        /// <returns>True if the image was saved successfully, otherwise False</returns>
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal extern static int SaveImage(IntPtr pWICImagingFactory, IntPtr pWICBitmap, IntPtr pD2D1RenderTarget, String filename);
    }
}

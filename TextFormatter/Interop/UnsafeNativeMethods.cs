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
        [DllImport(Import.lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Add(int a, double b);
    }
}

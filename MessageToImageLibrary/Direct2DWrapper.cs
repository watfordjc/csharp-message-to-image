using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace uk.JohnCook.dotnet.MessageToImageLibrary
{
    public class Direct2DWrapper : IDisposable
    {
        /// <summary>
        /// Instances of ID2D1Factory, IDWriteFactory, and IWICImagingFactory should be reused for the life of an application
        /// </summary>
        private Interop.Direct2DPointers direct2DPointers = new Interop.Direct2DPointers();
        private bool disposedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public Direct2DWrapper()
        {
            CreateDirect2DPointers();

            // Original DLL existence test - README needs modifying if changed
            Trace.WriteLine(Interop.UnsafeNativeMethods.Add(3, 6));
        }

        /// <summary>
        /// Create pointers for ID2D1Factory, IDWriteFactory, and IWICImagingFactory
        /// </summary>
        private void CreateDirect2DPointers()
        {
            try
            {
                Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.CreateD2D1Factory(ref direct2DPointers));
                Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.CreateDWriteFactory(ref direct2DPointers));
                Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.CreateImagingFactory(ref direct2DPointers));
            }
            catch (COMException ce)
            {
                Interop.UnsafeNativeMethods.ReleaseDWriteFactory(ref direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseImagingFactory(ref direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseD2D1Factory(ref direct2DPointers);
                Trace.WriteLine($"COMException in {nameof(CreateDirect2DPointers)} - {ce.Message} - {ce.InnerException?.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a MessagePanel instance
        /// </summary>
        /// <param name="canvasSize">Size of the canvas</param>
        /// <param name="backgroundColor">Background color of the canvas</param>
        /// <returns></returns>
        public MessagePanel CreateMessagePanel(Interop.SizeU canvasSize, UInt32 backgroundColor)
        {
            return new MessagePanel(
                direct2DPointers: ref direct2DPointers,
                canvasSize: canvasSize,
                backgroundColor: backgroundColor
                );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                Interop.UnsafeNativeMethods.ReleaseDWriteFactory(ref direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseImagingFactory(ref direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseD2D1Factory(ref direct2DPointers);
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Direct2DWrapper()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

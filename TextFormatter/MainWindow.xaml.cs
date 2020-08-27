using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextFormatter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly IntPtr direct2DFactory = Interop.UnsafeNativeMethods.CreateD2D1Factory();
        private readonly IntPtr imagingFactory = Interop.UnsafeNativeMethods.CreateImagingFactory();
        private IntPtr bitmap;
        private IntPtr canvas;
        private bool disposedValue;

        public MainWindow()
        {
            InitializeComponent();
            Trace.WriteLine(Interop.UnsafeNativeMethods.Add(3, 6));

            bitmap = Interop.UnsafeNativeMethods.CreateWICBitmap(imagingFactory, 1200, 3096);
            canvas = Interop.UnsafeNativeMethods.CreateRenderTarget(direct2DFactory, bitmap);
            if (DrawAndSaveImage() == ReturnCode.LOST_D2D1_RENDER_TARGET)
            {
                canvas = Interop.UnsafeNativeMethods.CreateRenderTarget(direct2DFactory, bitmap);
            }
            Interop.UnsafeNativeMethods.ReleaseRenderTarget(canvas);
            Interop.UnsafeNativeMethods.ReleaseWICBitmap(bitmap);
        }

        private enum ReturnCode
        {
            LOST_D2D1_RENDER_TARGET = -1,
            SUCCESS = 0,
            SAVE_ERROR = 1
        }

        private ReturnCode DrawAndSaveImage()
        {
            #region Draw Image
            // DrawImage() returns false if the ID2D1RenderTarget instance needs recreating. Attempt to recreate once.
            if (!Interop.UnsafeNativeMethods.DrawImage(canvas))
            {
                canvas = Interop.UnsafeNativeMethods.CreateRenderTarget(direct2DFactory, bitmap);
                if (!Interop.UnsafeNativeMethods.DrawImage(canvas))
                {
                    Trace.WriteLine("Failed to draw image! Lost ID2D1RenderTarget.");
                    return ReturnCode.LOST_D2D1_RENDER_TARGET;
                }
            }
            IntPtr greenBrush = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, (uint)System.Drawing.Color.Green.ToArgb());
            IntPtr blueBrush = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, (uint)System.Drawing.Color.CadetBlue.ToArgb());
            Interop.UnsafeNativeMethods.DrawRectangleBorder(canvas, greenBrush, 0, 0, 512, 512, 4);
            Interop.UnsafeNativeMethods.DrawRectangle(canvas, blueBrush, 4, 4, 512, 512);
            Interop.UnsafeNativeMethods.ReleaseSolidColorBrush(blueBrush);
            Interop.UnsafeNativeMethods.ReleaseSolidColorBrush(greenBrush);
            Trace.WriteLine("Image drawing successful!");
            #endregion
            #region Save Image
            // Date/Time format to append to file name. Custom format because filenames cannot contain colons.
            DateTimeFormat dateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHHmmss.fffffffZ");
            // Set result for file saving to SAVE_ERROR so it is only SUCCESS on success.
            ReturnCode result = ReturnCode.SAVE_ERROR;
            // Set file name to a deterministic but unlikely to be duplicated name using executable name and datetime, e.g. TextFormatter_2020-08-27T181638.7742753Z.PNG
            string fileName = Assembly.GetEntryAssembly().GetName().Name + "_" + DateTime.UtcNow.ToString(dateTimeFormat.FormatString) + ".PNG";
            // Set save location to %TEMP%
            string saveLocation = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
            try
            {
                Interop.UnsafeNativeMethods.SaveImage(imagingFactory, bitmap, canvas, saveLocation);
                result = ReturnCode.SUCCESS;
                Trace.WriteLine($"Image successfully saved to {saveLocation}");
                // Open file explorer with the saved file selected
                string selectFileArgument = $"/select, \"{saveLocation}\"";
                Process.Start("explorer.exe", selectFileArgument);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error saving to file {saveLocation}: {e.Message} - {e.InnerException?.Message}");
            }
            return result;
            #endregion
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
                Interop.UnsafeNativeMethods.ReleaseImagingFactory(imagingFactory);
                Interop.UnsafeNativeMethods.ReleaseD2D1Factory(direct2DFactory);
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainWindow()
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

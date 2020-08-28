using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        private System.Drawing.Size canvasDimensions = new System.Drawing.Size(1280, 3408);
        private IntPtr bitmap;
        private IntPtr canvas;
        private readonly Dictionary<string, IntPtr> brushes = new Dictionary<string, IntPtr>();
        private bool disposedValue;

        public MainWindow()
        {
            InitializeComponent();
            Trace.WriteLine(Interop.UnsafeNativeMethods.Add(3, 6));

            bitmap = Interop.UnsafeNativeMethods.CreateWICBitmap(imagingFactory, (uint)canvasDimensions.Width, (uint)canvasDimensions.Height);
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

        private void CreateBrushes()
        {
            brushes["borderBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, (uint)System.Drawing.Color.DarkGray.ToArgb());
            brushes["backgroundBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, (uint)System.Drawing.Color.Orange.ToArgb());
            brushes["disablePixelBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, 0xFF00FFFF);
            brushes["enablePixelBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, 0xFF00FF00);
        }

        private void ReleaseBrushes()
        {
            foreach(KeyValuePair<string, IntPtr> entry in brushes)
            {
                Interop.UnsafeNativeMethods.ReleaseSolidColorBrush(entry.Value);
            }
            brushes.Clear();
        }

        private ReturnCode DrawAndSaveImage()
        {
            Exception ex1;

            #region Draw Image
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            Interop.UnsafeNativeMethods.DrawImage(canvas, (uint)System.Drawing.Color.Black.ToArgb());
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Create Brushes
            CreateBrushes();
            #endregion

            #region Draw rectangular border
            System.Drawing.Size rectangleDimensions = new System.Drawing.Size(512, 512);
            int borderWidth = 4;
            System.Drawing.Point centredTopLeft = new System.Drawing.Point()
            {
                X = (canvasDimensions.Width - rectangleDimensions.Width) / 2 - borderWidth,
                Y = (canvasDimensions.Height - rectangleDimensions.Height) / 2 - borderWidth
            };
            if ((canvasDimensions.Width - rectangleDimensions.Width) % 2 == 0 && borderWidth % 2 == 0)
            {
                Trace.WriteLine("Rectangle cannot be centred horizontally: off by 0.5 pixels.");
            }
            if ((canvasDimensions.Height - rectangleDimensions.Height) % 2 == 0 && borderWidth % 2 == 0)
            {
                Trace.WriteLine("Rectangle cannot be centred vertically: off by 0.5 pixels.");
            }
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            Interop.UnsafeNativeMethods.DrawRectangleBorder(canvas, brushes["borderBrush"], centredTopLeft.X, centredTopLeft.Y, rectangleDimensions.Width, rectangleDimensions.Height, borderWidth);
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
                ReleaseBrushes();
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Draw a filled rectangle
            //Interop.UnsafeNativeMethods.BeginDraw(canvas);
            //Interop.UnsafeNativeMethods.DrawRectangle(canvas, brushes["disablePixelBrush"], centredTopLeft.X + borderWidth, centredTopLeft.Y + borderWidth, rectangleDimensions.Width, rectangleDimensions.Height);
            //ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            //if (ex1 != null)
            //{
            //    ReleaseBrushes();
            //    return ReturnCode.LOST_D2D1_RENDER_TARGET;
            //}
            #endregion

            #region Push a circle layer that will mask a profile image
            System.Drawing.Point centerPointForEllipse = new System.Drawing.Point()
            {
                X = (centredTopLeft.X + borderWidth) + (rectangleDimensions.Width / 2),
                Y = (centredTopLeft.Y + borderWidth) + (rectangleDimensions.Height / 2)
            };
            Interop.UnsafeNativeMethods.PushEllipseLayer(canvas, IntPtr.Zero, centerPointForEllipse.X, centerPointForEllipse.Y, rectangleDimensions.Width / 2, rectangleDimensions.Height / 2);
            #endregion
            #region Draw a profile image
            string profileImageFilename = @"C:\JohnDocs\tmp2\Computing\Web Sites\image manipulation\shaving_250px_square.png";
            try
            {
                Interop.UnsafeNativeMethods.DrawImageFromFilename(imagingFactory, canvas, profileImageFilename, centredTopLeft.X + borderWidth, centredTopLeft.Y + borderWidth, rectangleDimensions.Width, rectangleDimensions.Height);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error reading file {profileImageFilename}: {e.Message} - {e.InnerException?.Message}");
            }
            #endregion
            #region Draw an emoji instead of a profile image
            //Canvas is in pixels, fonts are in DIPs
            double pixelMultiplier = 96.0 / 72.0;
            //Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "🪒", centredTopLeft.X, centredTopLeft.Y, rectangleDimensions.Width, rectangleDimensions.Height, true, "Segoe UI Emoji", (float)(512 / pixelMultiplier), "en-GB", brushes["borderBrush"]);
            #endregion
            #region Pop the circle layer mask
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.PopLayer(canvas));
            if (ex1 != null)
            {
                ReleaseBrushes();
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Free the brushes
            ReleaseBrushes();
            #endregion

            Trace.WriteLine("Image drawing successful!");

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

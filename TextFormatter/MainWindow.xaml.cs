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
            brushes["textBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(canvas, 0xFFFFFFFF);
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

            #region Draw heading, subheading, and separator
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            //Canvas is in pixels, fonts are in DIPs
            double pixelMultiplier = 96.0 / 72.0;
            double startY = 40;
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "UK Tweets", 40, (int)Math.Round(startY), canvasDimensions.Width - 80, canvasDimensions.Height - (int)Math.Round(startY), true, "Noto Sans", 104.0f, 700, "en-GB", brushes["textBrush"]);
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "Tweets and Retweets from UK Resiliency Twitter Accounts", 40, (int)(Math.Round(startY)), canvasDimensions.Width - 80, canvasDimensions.Height - (int)Math.Round(startY), true, "Noto Sans", 74.0f, 500, "en-GB", brushes["textBrush"]);
            startY += 60;
            Interop.UnsafeNativeMethods.DrawLine(canvas, brushes["textBrush"], 40, (int)(Math.Round(startY)), canvasDimensions.Width - 40, (int)(Math.Round(startY)), 8.0f);
            startY += 90;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Draw rectangular border
            System.Drawing.Size rectangleDimensions = new System.Drawing.Size(240, 240);
            int borderWidth = 0;
            System.Drawing.Point centredTopLeft = new System.Drawing.Point()
            {
                X = 60 - borderWidth,
                Y = (int)(Math.Round(startY)) - borderWidth
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

            #region Push a circle layer that will mask a profile image
            System.Drawing.Point centerPointForEllipse = new System.Drawing.Point()
            {
                X = (centredTopLeft.X + borderWidth) + (rectangleDimensions.Width / 2),
                Y = (centredTopLeft.Y + borderWidth) + (rectangleDimensions.Height / 2)
            };
            Interop.UnsafeNativeMethods.PushEllipseLayer(canvas, IntPtr.Zero, centerPointForEllipse.X, centerPointForEllipse.Y, rectangleDimensions.Width / 2, rectangleDimensions.Height / 2);
            #endregion
            #region Draw a profile image
            //string profileImageFilename = @"C:\JohnDocs\tmp2\Computing\Web Sites\image manipulation\shaving_250px_square.png";
            string profileImageFilename = @"G:\Program Files (x86)\mIRC\twimg\HertsFRSControl.jpg";
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

            #region Draw display name and username
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            //Canvas is in pixels, fonts are in DIPs
            startY = centredTopLeft.Y - 6.0f;
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "Herts Fire Control", (centredTopLeft.X * 2) + rectangleDimensions.Width, (int)Math.Round(startY), canvasDimensions.Width - (centredTopLeft.X * 2) - rectangleDimensions.Width, canvasDimensions.Height - (int)Math.Round(startY), false, "Noto Sans", 90.0f, 700, "en-GB", brushes["textBrush"]);
            startY -= 6.0f;
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "@HertsFRSControl", (centredTopLeft.X * 2) + rectangleDimensions.Width, (int)Math.Round(startY), canvasDimensions.Width - (centredTopLeft.X * 2) - rectangleDimensions.Width, canvasDimensions.Height - (int)Math.Round(startY), false, "Noto Sans", 90.0f, 500, "en-GB", brushes["textBrush"]);
            startY += 90;
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "Blue watch are getting ready to handover to green watch for the night. We will be back at 20:00 tomorrow night. Stay safe 🚒😀🚒", centredTopLeft.X, (int)Math.Round(startY), canvasDimensions.Width - (centredTopLeft.X * 2), canvasDimensions.Height - (int)Math.Round(startY), false, "Segoe UI Emoji", 90.0f, 700, "en-GB", brushes["textBrush"]);
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Draw Twitter logo and timestamp
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            startY += 60;
            string twitterLogoFilename = @"C:/Users/John/Pictures/Twitch/Twitter_Logo_Blue.png";
            try
            {
                Interop.UnsafeNativeMethods.DrawImageFromFilename(imagingFactory, canvas, twitterLogoFilename, centredTopLeft.X + borderWidth, (int)Math.Round(startY), rectangleDimensions.Width, rectangleDimensions.Height);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error reading file {twitterLogoFilename}: {e.Message} - {e.InnerException?.Message}");
            }
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "Today, 19:33 UTC+1", centredTopLeft.X + rectangleDimensions.Width, (int)Math.Round(startY) + 60, canvasDimensions.Width - (centredTopLeft.X * 2), canvasDimensions.Height - (int)Math.Round(startY), false, "Noto Sans", (float)(rectangleDimensions.Height / pixelMultiplier / 2.5), 700, "en-GB", brushes["textBrush"]);
            Interop.UnsafeNativeMethods.EndDraw(canvas);
            #endregion

            #region Draw Retweet logo and Retweeter
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            startY += 60;
            string twitterRetweetLogoFilename = @"C:/Users/John/Pictures/Twitch/Twitter_Retweet.png";
            try
            {
                Interop.UnsafeNativeMethods.DrawImageFromFilename(imagingFactory, canvas, twitterRetweetLogoFilename, centredTopLeft.X + borderWidth + 50, (int)Math.Round(startY) + 50, rectangleDimensions.Width - 100, rectangleDimensions.Height - 100);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error reading file {twitterRetweetLogoFilename}: {e.Message} - {e.InnerException?.Message}");
            }
            startY = Interop.UnsafeNativeMethods.DrawTextFromString(canvas, "Nobody", centredTopLeft.X + rectangleDimensions.Width, (int)Math.Round(startY) + 60, canvasDimensions.Width - (centredTopLeft.X * 2), canvasDimensions.Height - (int)Math.Round(startY), false, "Noto Sans", (float)(rectangleDimensions.Height / pixelMultiplier / 2.5), 700, "en-GB", brushes["textBrush"]);
            Interop.UnsafeNativeMethods.EndDraw(canvas);
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

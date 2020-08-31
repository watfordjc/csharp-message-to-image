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
        private IntPtr direct2DFactory;
        private IntPtr directWriteFactory;
        private IntPtr imagingFactory;
        private System.Drawing.Size canvasDimensions;
        private IntPtr bitmap;
        private IntPtr canvas;
        private readonly Dictionary<string, IntPtr> brushes = new Dictionary<string, IntPtr>();
        private bool disposedValue;

        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += MainWindow_ContentRendered;
            Trace.WriteLine(Interop.UnsafeNativeMethods.Add(3, 6));
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                canvasDimensions = new System.Drawing.Size(1280, 3408);
                direct2DFactory = Interop.UnsafeNativeMethods.CreateD2D1Factory();
                directWriteFactory = Interop.UnsafeNativeMethods.CreateDWriteFactory();
                imagingFactory = Interop.UnsafeNativeMethods.CreateImagingFactory();
                bitmap = Interop.UnsafeNativeMethods.CreateWICBitmap(imagingFactory, (uint)canvasDimensions.Width, (uint)canvasDimensions.Height);
                canvas = Interop.UnsafeNativeMethods.CreateRenderTarget(direct2DFactory, bitmap);
                if (DrawAndSaveImage() == ReturnCode.LOST_D2D1_RENDER_TARGET)
                {
                    canvas = Interop.UnsafeNativeMethods.CreateRenderTarget(direct2DFactory, bitmap);
                }
                Interop.UnsafeNativeMethods.ReleaseRenderTarget(canvas);
                Interop.UnsafeNativeMethods.ReleaseWICBitmap(bitmap);
            }
            catch (COMException ce)
            {
                Exception ex = Marshal.GetExceptionForHR(ce.HResult);
                Trace.WriteLine($"COMException: {ex.Message} - {ex.InnerException?.Message}");
            }
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
            foreach (KeyValuePair<string, IntPtr> entry in brushes)
            {
                Interop.UnsafeNativeMethods.ReleaseSolidColorBrush(entry.Value);
            }
            brushes.Clear();
        }

        private void DrawText(Interop.TextLayoutResult textLayout, int startX, int startY, IntPtr colorBrush, bool releaseLayout)
        {
            Interop.UnsafeNativeMethods.DrawTextLayout(canvas, textLayout, startX, startY, colorBrush);
            // Memory leak: following code commented due to COMException, need to investigate
            //if (releaseLayout)
            //{
            //    Interop.UnsafeNativeMethods.ReleaseTextLayout(textLayout);
            //}
        }

        private ReturnCode DrawAndSaveImage()
        {
            Exception ex1, ex2;

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
            double startY = 40;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "UK Tweets", 40, (int)Math.Round(startY), canvasDimensions.Width - 80, canvasDimensions.Height - (int)Math.Round(startY), true, "Noto Sans", 104.0f, 700, "en-GB", out Interop.TextLayoutResult currentTextResult));
            if (ex1 == null)
            {
                DrawText(currentTextResult, 40, (int)Math.Round(startY), brushes["textBrush"], true);
                startY += currentTextResult.height;
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "Tweets and Retweets from UK Resiliency Twitter Accounts", 40, (int)(Math.Round(startY)), canvasDimensions.Width - 80, canvasDimensions.Height - (int)Math.Round(startY), true, "Noto Sans", 74.0f, 500, "en-GB", out currentTextResult));
            if (ex1 == null)
            {
                DrawText(currentTextResult, 40, (int)Math.Round(startY), brushes["textBrush"], true);
                startY += currentTextResult.height;
            }
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
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.PushEllipseLayer(canvas, IntPtr.Zero, centerPointForEllipse.X, centerPointForEllipse.Y, rectangleDimensions.Width / 2, rectangleDimensions.Height / 2));
            if (ex1 != null)
            {
                ReleaseBrushes();
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion
            #region Draw a profile image
            string profileImageFilename = @"C:\JohnDocs\tmp2\Computing\Web Sites\image manipulation\shaving_250px_square.png";
            //string profileImageFilename = @"G:\Program Files (x86)\mIRC\twimg\DerbyshireFRS.jpg";
            try
            {
                Interop.UnsafeNativeMethods.DrawImageFromFilename(imagingFactory, canvas, profileImageFilename, centredTopLeft.X + borderWidth, centredTopLeft.Y + borderWidth, rectangleDimensions.Width, rectangleDimensions.Height);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error reading file {profileImageFilename}: {e.Message} - {e.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"And error occurred reading file {profileImageFilename}: {ex.Message} - {ex.InnerException?.Message}");
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
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "John Cook", (centredTopLeft.X * 2) + rectangleDimensions.Width, centredTopLeft.Y + borderWidth, canvasDimensions.Width - (centredTopLeft.X * 2) - rectangleDimensions.Width, (rectangleDimensions.Height - borderWidth) / 2, false, "Noto Sans", 88.0f, 700, "en-GB", out Interop.TextLayoutResult displayNameTextLayout));
            ex2 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "@WatfordJC", (centredTopLeft.X * 2) + rectangleDimensions.Width, centredTopLeft.Y + borderWidth, canvasDimensions.Width - (centredTopLeft.X * 2) - rectangleDimensions.Width, (rectangleDimensions.Height - borderWidth) / 2, false, "Noto Sans", 88.0f, 500, "en-GB", out Interop.TextLayoutResult usernameTextLayout));
            if (ex1 == null && ex2 == null)
            {
                if (rectangleDimensions.Height / 2 >= displayNameTextLayout.height)
                {
                    usernameTextLayout.top = centredTopLeft.Y + borderWidth + (rectangleDimensions.Height / 2);
                }
                else
                {
                    usernameTextLayout.top += (int)displayNameTextLayout.height;
                }
                DrawText(displayNameTextLayout, (centredTopLeft.X * 2) + rectangleDimensions.Width, displayNameTextLayout.top, brushes["textBrush"], true);
                DrawText(usernameTextLayout, (centredTopLeft.X * 2) + rectangleDimensions.Width, usernameTextLayout.top, brushes["textBrush"], true);
                startY = Math.Max(centredTopLeft.Y + borderWidth + rectangleDimensions.Height, centredTopLeft.Y + borderWidth + (int)Math.Round(displayNameTextLayout.height + usernameTextLayout.height));
            }
            startY += 90;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "File -> New -> 1280x3600 -> Save As -> Something.PNG. You'd think creating a blank PNG in Direct2D wouldn't involve that much learning, but it is day 3 and I think I'm now drawing a blank canvas with a white background off screen. A rectangle in 6 steps sounded way too easy.", centredTopLeft.X, (int)Math.Round(startY), canvasDimensions.Width - (centredTopLeft.X * 2), canvasDimensions.Height - (int)Math.Round(startY), false, "Segoe UI Emoji", 90.0f, 700, "en-GB", out currentTextResult));
            if (ex1 == null)
            {
                DrawText(currentTextResult, centredTopLeft.X, (int)Math.Round(startY), brushes["textBrush"], true);
                startY += currentTextResult.height;
            }
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
            catch (Exception ex)
            {
                Trace.WriteLine($"An error occurred reading file {profileImageFilename}: {ex.Message} - {ex.InnerException?.Message}");
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "August 27th 2020", centredTopLeft.X + rectangleDimensions.Width, (int)Math.Round(startY), canvasDimensions.Width - (centredTopLeft.X * 2), rectangleDimensions.Height, false, "Noto Sans", (float)(rectangleDimensions.Height / 3), 700, "en-GB", out currentTextResult));
            if (ex1 == null)
            {
                if (currentTextResult.height < rectangleDimensions.Height)
                {
                    currentTextResult.top = (int)(startY + ((rectangleDimensions.Height - currentTextResult.height) / 2));
                }
                DrawText(currentTextResult, centredTopLeft.X + rectangleDimensions.Width, currentTextResult.top, brushes["textBrush"], true);
                startY += Math.Max(currentTextResult.height, rectangleDimensions.Height);
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
                return ReturnCode.LOST_D2D1_RENDER_TARGET;
            }
            #endregion

            #region Draw Retweet logo and Retweeter
            Interop.UnsafeNativeMethods.BeginDraw(canvas);
            string twitterRetweetLogoFilename = @"C:/Users/John/Pictures/Twitch/Twitter_Retweet.png";
            startY += 50;
            try
            {
                Interop.UnsafeNativeMethods.DrawImageFromFilename(imagingFactory, canvas, twitterRetweetLogoFilename, centredTopLeft.X + borderWidth + 50, (int)Math.Round(startY), rectangleDimensions.Width - 100, rectangleDimensions.Height - 100);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error reading file {twitterRetweetLogoFilename}: {e.Message} - {e.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"An error occurred reading file {profileImageFilename}: {ex.Message} - {ex.InnerException?.Message}");
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(directWriteFactory, canvas, "Nobody", centredTopLeft.X + rectangleDimensions.Width, (int)Math.Round(startY) + 60, canvasDimensions.Width - (centredTopLeft.X * 2), canvasDimensions.Height - (int)Math.Round(startY), false, "Noto Sans", (float)(rectangleDimensions.Height / 3), 700, "en-GB", out currentTextResult));
            if (ex1 == null)
            {
                if (currentTextResult.height < rectangleDimensions.Height - 100)
                {
                    currentTextResult.top = (int)(startY + ((rectangleDimensions.Height - 100 - currentTextResult.height) / 2));
                }
                DrawText(currentTextResult, centredTopLeft.X + rectangleDimensions.Width, currentTextResult.top, brushes["textBrush"], true);
                startY += Math.Max(currentTextResult.height, rectangleDimensions.Height);
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(canvas));
            if (ex1 != null)
            {
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
                Interop.UnsafeNativeMethods.ReleaseDWriteFactory(directWriteFactory);
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

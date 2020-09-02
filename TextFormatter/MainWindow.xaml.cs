﻿using System;
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
        /// <summary>
        /// Instances of ID2D1Factory, IDWriteFactory, and IWICImagingFactory should be reused for the life of an application
        /// </summary>
        private Interop.Direct2DPointers direct2DPointers = new Interop.Direct2DPointers();
        /// <summary>
        /// Temporary way of storing pointers for ID2D1SolidColorBrush - haven't worked out a suitable COM Interop way yet
        /// </summary>
        private readonly Dictionary<string, IntPtr> brushes = new Dictionary<string, IntPtr>();
        private bool disposedValue;

        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += MainWindow_ContentRendered;
            // Original DLL existence test - README needs modifying if changed
            Trace.WriteLine(Interop.UnsafeNativeMethods.Add(3, 6));
        }

        /// <summary>
        /// Create pointers for ID2D1Factory, IDWriteFactory, and IWICImagingFactory
        /// </summary>
        private void CreateDirect2DPointers()
        {
            Exception ex1;
            bool noErrors = true;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateD2D1Factory(ref direct2DPointers));
            noErrors = noErrors && ex1 == null;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateDWriteFactory(ref direct2DPointers));
            noErrors = noErrors && ex1 == null;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateImagingFactory(ref direct2DPointers));
            noErrors = noErrors && ex1 == null;
            if (!noErrors)
            {
                Interop.UnsafeNativeMethods.ReleaseDWriteFactory(direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseImagingFactory(direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseD2D1Factory(direct2DPointers);
                throw new Exception($"Error during {nameof(CreateDirect2DPointers)}.");
            }
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                // Create the Direct2D factories
                CreateDirect2DPointers();
                // Create the Direct2D bitmap and render target
                Models.TweetPanel verticalTweetPanel = CreateVerticalTweetPanel();
                DrawVerticalTweet(verticalTweetPanel);
                // We're not reusing the render target
                //Interop.UnsafeNativeMethods.ReleaseRenderTarget(verticalTweetPanel.Direct2DCanvas);
                // We're not reusing the bitmap
                //Interop.UnsafeNativeMethods.ReleaseWICBitmap(verticalTweetPanel.Direct2DCanvas);
                // Direct2D pointers are cleaned up in finalizer*/
            }
            catch (COMException ce)
            {
                Exception ex = Marshal.GetExceptionForHR(ce.HResult);
                Trace.WriteLine($"COMException: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// Return codes for DrawAndSaveImage() method
        /// </summary>
        private enum ReturnCode
        {
            LOST_D2D1_RENDER_TARGET = -1,
            SUCCESS = 0,
            SAVE_ERROR = 1
        }

        /// <summary>
        /// Create pointers for each ID2D1SolidColorBrush and store in dictionary
        /// </summary>
        private void CreateBrushes(Interop.Direct2DCanvas direct2DCanvas)
        {
            brushes["borderBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(direct2DCanvas, (uint)System.Drawing.Color.DarkGray.ToArgb());
            brushes["backgroundBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(direct2DCanvas, (uint)System.Drawing.Color.Black.ToArgb());
            brushes["disablePixelBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(direct2DCanvas, 0xFF00FFFF);
            brushes["enablePixelBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(direct2DCanvas, 0xFF00FF00);
            brushes["textBrush"] = Interop.UnsafeNativeMethods.CreateSolidColorBrush(direct2DCanvas, 0xFFFFFFFF);
            foreach (KeyValuePair<string, IntPtr> entry in brushes)
            {
                if (entry.Value == IntPtr.Zero)
                {
                    Debugger.Break();
                }
            }
        }

        /// <summary>
        /// Release pointers for all ID2D1SolidColorBrush stored in dictionary
        /// </summary>
        private void ReleaseBrushes()
        {
            foreach (KeyValuePair<string, IntPtr> entry in brushes)
            {
                Interop.UnsafeNativeMethods.ReleaseSolidColorBrush(entry.Value);
            }
            brushes.Clear();
        }

        private Models.TweetPanel CreateVerticalTweetPanel()
        {
            Exception ex1;

            VerticalTweetPanel verticalTweetPanel = new VerticalTweetPanel(
                direct2DPointers: ref direct2DPointers,
                canvasSize: new Models.SizeU() { Width = 1280, Height = 3408 },
                backgroundcolor: (uint)System.Drawing.Color.Black.ToArgb()
                );

            #region Set strings for header
            verticalTweetPanel.Header = "UK Tweets";
            verticalTweetPanel.SubHeader = "Tweets and Retweets from UK Resiliency Twitter Accounts";
            #endregion

            #region Set filenames and image sizes for resources
            verticalTweetPanel.SetImage(
                Models.CanvasElement.TWITTER_LOGO,
                @"C:/Users/John/Pictures/Twitch/Twitter_Logo_Blue.png",
                240.0f,
                240.0f
                );
            verticalTweetPanel.SetImage(
                Models.CanvasElement.RETWEET_LOGO,
                @"C:/Users/John/Pictures/Twitch/Twitter_Retweet.png",
                240.0f - 100.0f,
                240.0f - 100.0f
                );
            #endregion

            #region Set Fonts
            verticalTweetPanel.SetFont(Models.CanvasElement.HEADER, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 104.0f,
                FontWeight = 700,
                LocaleName = "en-GB",
                JustifyCentered = true
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.SUBHEADER, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 74.0f,
                FontWeight = 500,
                LocaleName = "en-GB",
                JustifyCentered = true
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.DISPLAY_NAME, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 88.0f,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.USERNAME, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 88.0f,
                FontWeight = 500,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.TEXT, new Models.FontSettings()
            {
                FontName = "Segoe UI Emoji",
                FontSize = 90.0f,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.TIME, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.TwitterLogoRectangle.Bottom / 3,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.RETWEETER_DISPLAY_NAME, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.TwitterLogoRectangle.Bottom / 3,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(Models.CanvasElement.RETWEETER_USERNAME, new Models.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.TwitterLogoRectangle.Bottom / 3,
                FontWeight = 500,
                LocaleName = "en-GB"
            });
            #endregion

            #region Draw canvas with header

            #region Start drawing to canvas
            Interop.UnsafeNativeMethods.BeginDraw(verticalTweetPanel.Direct2DCanvas);
            #endregion

            #region Create brushes
            CreateBrushes(verticalTweetPanel.Direct2DCanvas);
            #endregion

            #region Draw Heading
            verticalTweetPanel.HeaderOriginPoint = new Models.PointF() {
                X = 40.0f,
                Y = 40.0f
            };
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.HEADER, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.HeaderOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - (verticalTweetPanel.HeaderOriginPoint.Y * 2)
            });
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.HEADER, brushes["textBrush"]);
            #endregion

            #region Draw SubHeading
            verticalTweetPanel.SubHeaderOriginPoint = new Models.PointF() {
                X = 40.0f,
                Y = verticalTweetPanel.HeaderOriginPoint.Y + verticalTweetPanel.HeaderRectangle.Bottom
            };
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.SUBHEADER, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.SubHeaderOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - (verticalTweetPanel.SubHeaderOriginPoint.Y * 2)
            });
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.SUBHEADER, brushes["textBrush"]);
            #endregion

            #region Draw Heading Separator
            verticalTweetPanel.HeadingSeparatorPoint1 = new Models.PointF() {
                X = 40.0f,
                Y = verticalTweetPanel.SubHeaderOriginPoint.Y + verticalTweetPanel.SubHeaderRectangle.Bottom + 60.0f
            };
            verticalTweetPanel.HeadingSeparatorPoint2 = new Models.PointF()
            {
                X = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.HeadingSeparatorPoint1.X * 2),
                Y = verticalTweetPanel.HeadingSeparatorPoint1.Y
            };
            verticalTweetPanel.DrawHeadingSeparator(brushes["textBrush"], 8.0f);
            #endregion

            #region Finish drawing to canvas
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(verticalTweetPanel.Direct2DCanvas));
            if (ex1 != null)
            {
                Debugger.Break();
                throw ex1;
            }
            #endregion

            #region Set Tweet area relative to canvas
            verticalTweetPanel.TweetOriginPoint = new Models.PointF()
            {
                X = 40.0f,
                Y = verticalTweetPanel.HeadingSeparatorRectangle.Bottom + 90.0f
            };
            verticalTweetPanel.TweetRectangle = new Models.RectF()
            {
                Left = 0,
                Top = 0,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.TweetOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - verticalTweetPanel.TweetOriginPoint.Y
            };
            #endregion

            #region Set Tweet profile image area relative to canvas
            int borderWidth = 0;
            verticalTweetPanel.ProfileImageOriginPoint = new Models.PointF()
            {
                X = 60 - borderWidth,
                Y = verticalTweetPanel.TweetOriginPoint.Y - borderWidth
            };
            verticalTweetPanel.ProfileImageRectangle = new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = 240.0f,
                Bottom = 240.0f
            };
            #endregion

            #region Set display name origin point relative to canvas
            verticalTweetPanel.DisplayNameOriginPoint = new Models.PointF()
            {
                X = (verticalTweetPanel.ProfileImageOriginPoint.X * 2) + verticalTweetPanel.ProfileImageRectangle.Right,
                Y = verticalTweetPanel.ProfileImageOriginPoint.Y
            };
            #endregion

            #endregion

            return verticalTweetPanel;
        }

        private void DrawVerticalTweet(Models.TweetPanel verticalTweetPanel)
        {
            verticalTweetPanel.ClearArea(verticalTweetPanel.TweetOriginPoint, verticalTweetPanel.TweetRectangle, brushes["backgroundBrush"], true, true);

            #region Profile image
            // TODO: Set ProfileImageFilename
            verticalTweetPanel.SetImage(
                Models.CanvasElement.PROFILE_IMAGE,
                @"C:\JohnDocs\tmp2\Computing\Web Sites\image manipulation\shaving_250px_square.png",
                240.0f,
                240.0f
                );

            // TODO: Create and draw round profile image
            verticalTweetPanel.PushCircleLayer(Models.CanvasElement.PROFILE_IMAGE, brushes["enablePixelBrush"]);
            verticalTweetPanel.DrawImage(Models.CanvasElement.PROFILE_IMAGE);
            verticalTweetPanel.PopLayer();
            #endregion

            #region Start drawing to canvas
            Interop.UnsafeNativeMethods.BeginDraw(verticalTweetPanel.Direct2DCanvas);
            #endregion

            #region Display name and username
            // TODO: Get DisplayName and set DisplayNameRectangle
            verticalTweetPanel.DisplayName = "John Cook";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.DISPLAY_NAME, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right - verticalTweetPanel.ProfileImageRectangle.Right,
                Bottom = verticalTweetPanel.ProfileImageRectangle.Bottom / 2
            });
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.SUBHEADER, brushes["textBrush"]);
            
            // TODO: Get Username and set UsernameRectangle
            verticalTweetPanel.Username = "@WatfordJC";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.USERNAME, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right - verticalTweetPanel.ProfileImageRectangle.Right,
                Bottom = verticalTweetPanel.ProfileImageRectangle.Bottom / 2
            });

            // TODO: Calculate and set DisplayNameOriginPoint and UsernameOriginPoint
            if (verticalTweetPanel.ProfileImageRectangle.Bottom / 2 >= verticalTweetPanel.DisplayNameRectangle.Bottom)
            {
                verticalTweetPanel.UsernameOriginPoint = new Models.PointF()
                {
                    X = verticalTweetPanel.DisplayNameOriginPoint.X,
                    Y = verticalTweetPanel.TweetOriginPoint.Y + (verticalTweetPanel.ProfileImageRectangle.Bottom / 2)
                };
            }
            else
            {
                verticalTweetPanel.UsernameOriginPoint = new Models.PointF()
                {
                    X = verticalTweetPanel.DisplayNameOriginPoint.X,
                    Y = verticalTweetPanel.UsernameOriginPoint.Y + verticalTweetPanel.DisplayNameRectangle.Bottom
                };
            }
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.DISPLAY_NAME, brushes["textBrush"]);
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.USERNAME, brushes["textBrush"]);
            #endregion

            #region Tweet text
            // TODO: Get TweetText and set TweetTextRectangle
            verticalTweetPanel.TweetText = "File -> New -> 1280x3600 -> Save As -> Something.PNG. You'd think creating a blank PNG in Direct2D wouldn't involve that much learning, but it is day 3 and I think I'm now drawing a blank canvas with a white background off screen. A rectangle in 6 steps sounded way too easy.";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.TEXT, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right,
                Bottom = verticalTweetPanel.TweetRectangle.Bottom - verticalTweetPanel.UsernameRectangle.Bottom
            });
            // TODO: Calculate and set TweetTextOriginPoint
            verticalTweetPanel.TweetTextOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.TweetOriginPoint.X,
                Y = verticalTweetPanel.ProfileImageOriginPoint.Y + Math.Max(verticalTweetPanel.ProfileImageRectangle.Bottom, verticalTweetPanel.UsernameRectangle.Bottom) + 90.0f
            };
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.TEXT, brushes["textBrush"]);
            #endregion

            #region Twitter logo and time
            // TODO: Calculate and set TwitterLogoOriginPoint
            verticalTweetPanel.TwitterLogoOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.TweetOriginPoint.X,
                Y = verticalTweetPanel.TweetTextOriginPoint.Y + verticalTweetPanel.TweetTextRectangle.Bottom + 60.0f
            };
            verticalTweetPanel.DrawImage(Models.CanvasElement.TWITTER_LOGO);

            // TODO: Get TweetTime and set TweetTimeRectangle
            verticalTweetPanel.TweetTime = "August 27th 2020";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.TIME, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right - verticalTweetPanel.TwitterLogoRectangle.Right,
                Bottom = verticalTweetPanel.TwitterLogoRectangle.Bottom
            });

            // TODO: Calculate and set TweetTimeOriginPoint
            verticalTweetPanel.TweetTimeOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.TweetOriginPoint.X + verticalTweetPanel.TwitterLogoRectangle.Right,
                Y = verticalTweetPanel.TweetTimeRectangle.Bottom < verticalTweetPanel.TwitterLogoRectangle.Bottom
                    ? verticalTweetPanel.TwitterLogoOriginPoint.Y + ((verticalTweetPanel.TwitterLogoRectangle.Bottom - verticalTweetPanel.TweetTimeRectangle.Bottom) / 2)
                    : verticalTweetPanel.TwitterLogoOriginPoint.Y
            };
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.TIME, brushes["textBrush"]);
            #endregion

            #region Retweet logo and Retweeter display name & username
            // TODO: Calculate and set RetweetLogoOriginPoint
            verticalTweetPanel.RetweetLogoOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.TweetOriginPoint.X + 50.0f,
                Y = verticalTweetPanel.TwitterLogoOriginPoint.Y + Math.Max(verticalTweetPanel.TwitterLogoRectangle.Bottom, verticalTweetPanel.TweetTimeRectangle.Bottom) + 50.0f
            };
            verticalTweetPanel.DrawImage(Models.CanvasElement.RETWEET_LOGO);

            // TODO: Get RetweeterDisplayName and set RetweeterDisplayNameRectangle
            verticalTweetPanel.RetweeterDisplayName = "Nobody";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.RETWEETER_DISPLAY_NAME, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right - verticalTweetPanel.RetweetLogoRectangle.Right - 100.0f,
                Bottom = verticalTweetPanel.RetweetLogoRectangle.Bottom
            });
            // TODO: Calculate and set RetweeterDisplayNameOriginPoint
            verticalTweetPanel.RetweeterDisplayNameOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.TweetOriginPoint.X + verticalTweetPanel.RetweetLogoRectangle.Right + 100.0f,
                Y = verticalTweetPanel.RetweeterDisplayNameRectangle.Bottom < verticalTweetPanel.RetweetLogoRectangle.Bottom
                    ? verticalTweetPanel.RetweetLogoOriginPoint.Y + ((verticalTweetPanel.RetweetLogoRectangle.Bottom - verticalTweetPanel.RetweeterDisplayNameRectangle.Bottom) / 2)
                    : verticalTweetPanel.RetweetLogoOriginPoint.Y
            };

            // TODO: Get RetweeterUsername and set RetweeterUsernameRectangle
            verticalTweetPanel.RetweeterUsername = "(@search)";
            verticalTweetPanel.CreateTextLayout(Models.CanvasElement.RETWEETER_USERNAME, new Models.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.TweetRectangle.Right - verticalTweetPanel.RetweetLogoRectangle.Right,
                Bottom = verticalTweetPanel.RetweetLogoRectangle.Bottom
            });
            // TODO: Calculate and set RetweeterUsernameOriginPoint
            verticalTweetPanel.RetweeterUsernameOriginPoint = new Models.PointF()
            {
                X = verticalTweetPanel.RetweeterDisplayNameOriginPoint.X,
                Y = verticalTweetPanel.RetweetLogoOriginPoint.Y + Math.Max(verticalTweetPanel.RetweetLogoRectangle.Bottom, verticalTweetPanel.RetweeterDisplayNameRectangle.Bottom)
            };
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.RETWEETER_DISPLAY_NAME, brushes["textBrush"]);
            verticalTweetPanel.DrawTextLayout(Models.CanvasElement.RETWEETER_USERNAME, brushes["textBrush"]);
            #endregion

            #region Finish drawing to canvas
            Exception ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(verticalTweetPanel.Direct2DCanvas));
            if (ex1 != null)
            {
                Debugger.Break();
                throw ex1;
            }
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
                Interop.UnsafeNativeMethods.SaveImage(verticalTweetPanel.Direct2DCanvas, saveLocation);
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
                Interop.UnsafeNativeMethods.ReleaseDWriteFactory(direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseImagingFactory(direct2DPointers);
                Interop.UnsafeNativeMethods.ReleaseD2D1Factory(direct2DPointers);
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

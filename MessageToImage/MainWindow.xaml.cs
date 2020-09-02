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

namespace uk.JohnCook.dotnet.MessageToImage
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
                MessagePanel verticalTweetPanel = CreateVerticalTweetPanel();
                DrawVerticalTweet(verticalTweetPanel, @"G:\Program Files (x86)\mIRC\twimg\tmp\GMMH_NHS.jpg", "Greater Manchester Mental Health", "@GMMH_NHS", "A huge thank you to the wonderful team at HMP Hindley. You are all #GMMHSuperstars! 🌟🌟 #TogetherGMMH 💙", "Today, 13:42 UTC+1");
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

        private MessagePanel CreateVerticalTweetPanel()
        {
            Exception ex1;

            VerticalMessagePanel verticalTweetPanel = new VerticalMessagePanel(
                direct2DPointers: ref direct2DPointers,
                canvasSize: new Interop.SizeU() { Width = 1280, Height = 3408 },
                backgroundcolor: (uint)System.Drawing.Color.Black.ToArgb()
                );

            #region Set strings for header
            verticalTweetPanel.Header = "UK Tweets";
            verticalTweetPanel.SubHeader = "Tweets and Retweets from UK Resiliency Twitter Accounts";
            #endregion

            #region Set filenames and image sizes for resources
            verticalTweetPanel.SetImage(
                CanvasElement.NETWORK_LOGO,
                @"C:/Users/John/Pictures/Twitch/Twitter_Logo_Blue.png",
                240.0f,
                240.0f
                );
            verticalTweetPanel.SetImage(
                CanvasElement.SHARE_LOGO,
                @"C:/Users/John/Pictures/Twitch/Twitter_Retweet.png",
                240.0f - 100.0f,
                240.0f - 100.0f
                );
            #endregion

            #region Set Fonts
            verticalTweetPanel.SetFont(CanvasElement.HEADER, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 104.0f,
                FontWeight = 700,
                LocaleName = "en-GB",
                JustifyCentered = true
            });
            verticalTweetPanel.SetFont(CanvasElement.SUBHEADER, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 74.0f,
                FontWeight = 500,
                LocaleName = "en-GB",
                JustifyCentered = true
            });
            verticalTweetPanel.SetFont(CanvasElement.DISPLAY_NAME, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 82.0f,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(CanvasElement.USERNAME, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = 82.0f,
                FontWeight = 500,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(CanvasElement.TEXT, new Interop.FontSettings()
            {
                FontName = "Segoe UI Emoji",
                FontSize = 90.0f,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(CanvasElement.TIME, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.NetworkLogoRectangle.Bottom / 3,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(CanvasElement.SHARER_DISPLAY_NAME, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.NetworkLogoRectangle.Bottom / 3,
                FontWeight = 700,
                LocaleName = "en-GB"
            });
            verticalTweetPanel.SetFont(CanvasElement.SHARER_USERNAME, new Interop.FontSettings()
            {
                FontName = "Noto Sans",
                FontSize = verticalTweetPanel.NetworkLogoRectangle.Bottom / 3,
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
            verticalTweetPanel.HeaderOriginPoint = new Interop.PointF() {
                X = 40.0f,
                Y = 40.0f
            };
            verticalTweetPanel.CreateTextLayout(CanvasElement.HEADER, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.HeaderOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - (verticalTweetPanel.HeaderOriginPoint.Y * 2)
            });
            verticalTweetPanel.DrawTextLayout(CanvasElement.HEADER, brushes["textBrush"]);
            #endregion

            #region Draw SubHeading
            verticalTweetPanel.SubHeaderOriginPoint = new Interop.PointF() {
                X = 40.0f,
                Y = verticalTweetPanel.HeaderOriginPoint.Y + verticalTweetPanel.HeaderRectangle.Bottom
            };
            verticalTweetPanel.CreateTextLayout(CanvasElement.SUBHEADER, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.SubHeaderOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - (verticalTweetPanel.SubHeaderOriginPoint.Y * 2)
            });
            verticalTweetPanel.DrawTextLayout(CanvasElement.SUBHEADER, brushes["textBrush"]);
            #endregion

            #region Draw Heading Separator
            verticalTweetPanel.HeadingSeparatorPoint1 = new Interop.PointF() {
                X = 40.0f,
                Y = verticalTweetPanel.SubHeaderOriginPoint.Y + verticalTweetPanel.SubHeaderRectangle.Bottom + 60.0f
            };
            verticalTweetPanel.HeadingSeparatorPoint2 = new Interop.PointF()
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
            verticalTweetPanel.MessageOriginPoint = new Interop.PointF()
            {
                X = 60.0f,
                Y = verticalTweetPanel.HeadingSeparatorRectangle.Bottom + 90.0f
            };
            verticalTweetPanel.MessageRectangle = new Interop.RectF()
            {
                Left = 0,
                Top = 0,
                Right = verticalTweetPanel.PanelRectangle.Width - (verticalTweetPanel.MessageOriginPoint.X * 2),
                Bottom = verticalTweetPanel.PanelRectangle.Height - verticalTweetPanel.MessageOriginPoint.Y - 60.0f
            };
            #endregion

            #region Set Tweet profile image area relative to canvas
            verticalTweetPanel.ProfileImageOriginPoint = new Interop.PointF()
            {
                X = verticalTweetPanel.MessageOriginPoint.X,
                Y = verticalTweetPanel.MessageOriginPoint.Y
            };
            verticalTweetPanel.ProfileImageRectangle = new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = 240.0f,
                Bottom = 240.0f
            };
            #endregion

            #region Set display name origin point relative to canvas
            verticalTweetPanel.DisplayNameOriginPoint = new Interop.PointF()
            {
                X = (verticalTweetPanel.ProfileImageOriginPoint.X * 2) + verticalTweetPanel.ProfileImageRectangle.Right,
                Y = verticalTweetPanel.ProfileImageOriginPoint.Y
            };
            #endregion

            #endregion

            return verticalTweetPanel;
        }

        private string DrawVerticalTweet(MessagePanel verticalTweetPanel, string profileImageFilename, string displayName, string username, string text, string time, string retweeterDisplayName = null, string retweeterUsername = null)
        {
            verticalTweetPanel.ClearArea(verticalTweetPanel.MessageOriginPoint, verticalTweetPanel.MessageRectangle, brushes["backgroundBrush"], true, true);

            #region Profile image
            // TODO: Set ProfileImageFilename
            verticalTweetPanel.SetImage(
                CanvasElement.PROFILE_IMAGE,
                profileImageFilename,
                240.0f,
                240.0f
                );

            // TODO: Create and draw round profile image
            verticalTweetPanel.PushCircleLayer(CanvasElement.PROFILE_IMAGE, brushes["enablePixelBrush"]);
            verticalTweetPanel.DrawImage(CanvasElement.PROFILE_IMAGE);
            verticalTweetPanel.PopLayer();
            #endregion

            #region Start drawing to canvas
            Interop.UnsafeNativeMethods.BeginDraw(verticalTweetPanel.Direct2DCanvas);
            #endregion

            #region Display name and username
            // TODO: Get DisplayName and set DisplayNameRectangle
            verticalTweetPanel.DisplayName = displayName;
            verticalTweetPanel.CreateTextLayout(CanvasElement.DISPLAY_NAME, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.MessageRectangle.Right - verticalTweetPanel.ProfileImageRectangle.Right - verticalTweetPanel.ProfileImageOriginPoint.X,
                Bottom = verticalTweetPanel.ProfileImageRectangle.Bottom / 2
            });
            verticalTweetPanel.DrawTextLayout(CanvasElement.SUBHEADER, brushes["textBrush"]);

            // TODO: Get Username and set UsernameRectangle
            verticalTweetPanel.Username = username;
            verticalTweetPanel.CreateTextLayout(CanvasElement.USERNAME, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.MessageRectangle.Right - verticalTweetPanel.ProfileImageRectangle.Right - verticalTweetPanel.ProfileImageOriginPoint.X,
                Bottom = verticalTweetPanel.ProfileImageRectangle.Bottom / 2
            });

            // TODO: Calculate and set DisplayNameOriginPoint and UsernameOriginPoint
            if (verticalTweetPanel.ProfileImageRectangle.Bottom / 2 >= verticalTweetPanel.DisplayNameRectangle.Bottom)
            {
                verticalTweetPanel.UsernameOriginPoint = new Interop.PointF()
                {
                    X = verticalTweetPanel.DisplayNameOriginPoint.X,
                    Y = verticalTweetPanel.MessageOriginPoint.Y + (verticalTweetPanel.ProfileImageRectangle.Bottom / 2)
                };
            }
            else
            {
                verticalTweetPanel.UsernameOriginPoint = new Interop.PointF()
                {
                    X = verticalTweetPanel.DisplayNameOriginPoint.X,
                    Y = verticalTweetPanel.DisplayNameOriginPoint.Y + verticalTweetPanel.DisplayNameRectangle.Bottom
                };
            }
            verticalTweetPanel.DrawTextLayout(CanvasElement.DISPLAY_NAME, brushes["textBrush"]);
            verticalTweetPanel.DrawTextLayout(CanvasElement.USERNAME, brushes["textBrush"]);
            #endregion

            #region Tweet text
            // TODO: Get TweetText and set TweetTextRectangle
            verticalTweetPanel.MessageText = text;
            verticalTweetPanel.CreateTextLayout(CanvasElement.TEXT, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.MessageRectangle.Right,
                Bottom = verticalTweetPanel.MessageRectangle.Bottom - verticalTweetPanel.UsernameRectangle.Bottom
            });
            // TODO: Calculate and set TweetTextOriginPoint
            verticalTweetPanel.MessageTextOriginPoint = new Interop.PointF()
            {
                X = verticalTweetPanel.MessageOriginPoint.X,
                Y = verticalTweetPanel.ProfileImageOriginPoint.Y + Math.Max(verticalTweetPanel.ProfileImageRectangle.Bottom, verticalTweetPanel.DisplayNameRectangle.Bottom + verticalTweetPanel.UsernameRectangle.Bottom) + 90.0f
            };
            verticalTweetPanel.DrawTextLayout(CanvasElement.TEXT, brushes["textBrush"]);
            #endregion

            #region Twitter logo and time
            // TODO: Calculate and set TwitterLogoOriginPoint
            verticalTweetPanel.NetworkLogoOriginPoint = new Interop.PointF()
            {
                X = verticalTweetPanel.MessageOriginPoint.X,
                Y = verticalTweetPanel.MessageTextOriginPoint.Y + verticalTweetPanel.MessageTextRectangle.Bottom + 60.0f
            };
            verticalTweetPanel.DrawImage(CanvasElement.NETWORK_LOGO);

            // TODO: Get TweetTime and set TweetTimeRectangle
            verticalTweetPanel.Time = time;
            verticalTweetPanel.CreateTextLayout(CanvasElement.TIME, new Interop.RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = verticalTweetPanel.MessageRectangle.Right - verticalTweetPanel.NetworkLogoRectangle.Right,
                Bottom = verticalTweetPanel.NetworkLogoRectangle.Bottom
            });

            // TODO: Calculate and set TweetTimeOriginPoint
            verticalTweetPanel.TimeOriginPoint = new Interop.PointF()
            {
                X = verticalTweetPanel.MessageOriginPoint.X + verticalTweetPanel.NetworkLogoRectangle.Right,
                Y = verticalTweetPanel.TimeRectangle.Bottom < verticalTweetPanel.NetworkLogoRectangle.Bottom
                    ? verticalTweetPanel.NetworkLogoOriginPoint.Y + ((verticalTweetPanel.NetworkLogoRectangle.Bottom - verticalTweetPanel.TimeRectangle.Bottom) / 2)
                    : verticalTweetPanel.NetworkLogoOriginPoint.Y
            };
            verticalTweetPanel.DrawTextLayout(CanvasElement.TIME, brushes["textBrush"]);
            #endregion

            #region Retweet logo and Retweeter display name & username
            if (retweeterDisplayName != null && retweeterUsername != null)
            {
                // TODO: Calculate and set RetweetLogoOriginPoint
                verticalTweetPanel.ShareLogoOriginPoint = new Interop.PointF()
                {
                    X = verticalTweetPanel.MessageOriginPoint.X + 50.0f,
                    Y = verticalTweetPanel.NetworkLogoOriginPoint.Y + Math.Max(verticalTweetPanel.NetworkLogoRectangle.Bottom, verticalTweetPanel.TimeRectangle.Bottom) + 50.0f
                };
                verticalTweetPanel.DrawImage(CanvasElement.SHARE_LOGO);

                // TODO: Get RetweeterDisplayName and set RetweeterDisplayNameRectangle
                verticalTweetPanel.SharerDisplayName = retweeterDisplayName;
                verticalTweetPanel.CreateTextLayout(CanvasElement.SHARER_DISPLAY_NAME, new Interop.RectF()
                {
                    Left = 0.0f,
                    Top = 0.0f,
                    Right = verticalTweetPanel.MessageRectangle.Right - verticalTweetPanel.ShareLogoRectangle.Right - 100.0f,
                    Bottom = verticalTweetPanel.ShareLogoRectangle.Bottom
                });
                // TODO: Calculate and set RetweeterDisplayNameOriginPoint
                verticalTweetPanel.SharerDisplayNameOriginPoint = new Interop.PointF()
                {
                    X = verticalTweetPanel.MessageOriginPoint.X + verticalTweetPanel.ShareLogoRectangle.Right + 100.0f,
                    Y = verticalTweetPanel.SharerDisplayNameRectangle.Bottom < verticalTweetPanel.ShareLogoRectangle.Bottom
                    ? verticalTweetPanel.ShareLogoOriginPoint.Y + ((verticalTweetPanel.ShareLogoRectangle.Bottom - verticalTweetPanel.SharerDisplayNameRectangle.Bottom) / 2)
                    : verticalTweetPanel.ShareLogoOriginPoint.Y
                };

                // TODO: Get RetweeterUsername and set RetweeterUsernameRectangle
                verticalTweetPanel.SharerUsername = $"({retweeterUsername})";
                verticalTweetPanel.CreateTextLayout(CanvasElement.SHARER_USERNAME, new Interop.RectF()
                {
                    Left = 0.0f,
                    Top = 0.0f,
                    Right = verticalTweetPanel.MessageRectangle.Right - verticalTweetPanel.ShareLogoRectangle.Right,
                    Bottom = verticalTweetPanel.ShareLogoRectangle.Bottom
                });
                // TODO: Calculate and set RetweeterUsernameOriginPoint
                verticalTweetPanel.SharerUsernameOriginPoint = new Interop.PointF()
                {
                    X = verticalTweetPanel.SharerDisplayNameOriginPoint.X,
                    Y = verticalTweetPanel.ShareLogoOriginPoint.Y + Math.Max(verticalTweetPanel.ShareLogoRectangle.Bottom, verticalTweetPanel.SharerDisplayNameRectangle.Bottom)
                };
                verticalTweetPanel.DrawTextLayout(CanvasElement.SHARER_DISPLAY_NAME, brushes["textBrush"]);
                verticalTweetPanel.DrawTextLayout(CanvasElement.SHARER_USERNAME, brushes["textBrush"]);
            }
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
                return saveLocation;
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine($"Error saving to file {saveLocation}: {e.Message} - {e.InnerException?.Message}");
                return null;
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

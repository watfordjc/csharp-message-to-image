using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using TextFormatter.Interop;

namespace TextFormatter.Models
{

    [StructLayout(LayoutKind.Sequential)]
    public struct SizeU
    {
        public UInt32 Width;
        public UInt32 Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RectF
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointF
    {
        public float X;
        public float Y;
    }

    public enum CanvasElement
    {
        CANVAS = 0,
        HEADER = 1,
        SUBHEADER = 2,
        HEADING_SEPARATOR = 3,
        TWEET = 4,
        PROFILE_IMAGE = 5,
        DISPLAY_NAME = 6,
        USERNAME = 7,
        TEXT = 8,
        TWITTER_LOGO = 9,
        TIME = 10,
        RETWEET_LOGO = 11,
        RETWEETER_DISPLAY_NAME = 12,
        RETWEETER_USERNAME = 13
    };

    public class TweetPanel
    {
        #region Variables
        public readonly SizeU PanelRectangle;
        public Interop.Direct2DCanvas Direct2DCanvas = new Interop.Direct2DCanvas();
        public UInt32 BackgroundColor;
        private TextLayoutResult headerTextLayout;
        private TextLayoutResult subHeaderTextLayout;
        private TextLayoutResult displayNameTextLayout;
        private TextLayoutResult usernameTextLayout;
        private TextLayoutResult tweetTextTextLayout;
        private TextLayoutResult timeTextLayout;
        private TextLayoutResult retweeterDisplayNameTextLayout;
        private TextLayoutResult retweeterUsernameTextLayout;
        #endregion

        #region Properties
        public PointF HeaderOriginPoint { get; set; }
        public RectF HeaderRectangle { get; set; }
        public FontSettings HeaderFont { get; set; }
        public string Header { get; set; } = String.Empty;

        public PointF SubHeaderOriginPoint { get; set; }
        public RectF SubHeaderRectangle { get; set; }
        public FontSettings SubHeaderFont { get; set; }
        public string SubHeader { get; set; } = String.Empty;

        public PointF HeadingSeparatorPoint1 { get; set; }
        public PointF HeadingSeparatorPoint2 { get; set; }
        public RectF HeadingSeparatorRectangle
        {
            get
            {
                return new RectF()
                {
                    Top = Math.Min(HeadingSeparatorPoint1.Y, HeadingSeparatorPoint2.Y),
                    Left = Math.Min(HeadingSeparatorPoint1.X, HeadingSeparatorPoint2.X),
                    Bottom = Math.Max(HeadingSeparatorPoint2.Y, HeadingSeparatorPoint1.Y),
                    Right = Math.Max(HeadingSeparatorPoint2.X, HeadingSeparatorPoint1.X)
                };
            }
        }

        public PointF TweetOriginPoint { get; set; }
        public RectF TweetRectangle { get; set; }

        public PointF ProfileImageOriginPoint { get; set; }
        public RectF ProfileImageRectangle { get; set; }
        public string ProfileImageFilename { get; set; } = String.Empty;

        public PointF DisplayNameOriginPoint { get; set; }
        public RectF DisplayNameRectangle { get; set; }
        public FontSettings DisplayNameFont { get; set; }
        public string DisplayName { get; set; } = String.Empty;

        public PointF UsernameOriginPoint { get; set; }
        public RectF UsernameRectangle { get; set; }
        public FontSettings UsernameFont { get; set; }
        public string Username { get; set; } = String.Empty;

        public PointF TweetTextOriginPoint { get; set; }
        public RectF TweetTextRectangle { get; set; }
        public FontSettings TweetTextFont { get; set; }
        public string TweetText { get; set; } = String.Empty;

        public PointF TwitterLogoOriginPoint { get; set; }
        public RectF TwitterLogoRectangle { get; set; }
        public string TwitterLogoFilename { get; set; } = String.Empty;

        public PointF TweetTimeOriginPoint { get; set; }
        public RectF TweetTimeRectangle { get; set; }
        public FontSettings TweetTimeFont { get; set; }
        public string TweetTime { get; set; } = String.Empty;

        public PointF RetweetLogoOriginPoint { get; set; }
        public RectF RetweetLogoRectangle { get; set; }
        public string RetweetLogoFilename { get; set; } = String.Empty;

        public PointF RetweeterDisplayNameOriginPoint { get; set; }
        public RectF RetweeterDisplayNameRectangle { get; set; }
        public FontSettings RetweeterDisplayNameFont { get; set; }
        public string RetweeterDisplayName { get; set; } = String.Empty;

        public PointF RetweeterUsernameOriginPoint { get; set; }
        public RectF RetweeterUsernameRectangle { get; set; }
        public FontSettings RetweeterUsernameFont { get; set; }
        public string RetweeterUsername { get; set; } = String.Empty;
        #endregion

        public TweetPanel(ref Interop.Direct2DPointers direct2DPointers, Models.SizeU canvasSize, UInt32 backgroundColor)
        {
            PanelRectangle = canvasSize;
            BackgroundColor = backgroundColor;
            CreateDirect2DCanvas(PanelRectangle, ref direct2DPointers);
            WipeCanvas();
        }

        /// <summary>
        /// Create pointers for IWICBitmap and ID2D1RenderTarget, and set the canvas size
        /// </summary>
        /// <param name="width">Desired bitmap width in pixels</param>
        /// <param name="height">Desired bitmap height in pixels</param>
        public void CreateDirect2DCanvas(Models.SizeU canvasSize, ref Interop.Direct2DPointers direct2DPointers)
        {
            Exception ex1;
            bool noErrors = true;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateWICBitmap(ref direct2DPointers, canvasSize.Width, canvasSize.Height, ref Direct2DCanvas));
            noErrors = noErrors && ex1 == null;
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateRenderTarget(ref Direct2DCanvas));
            noErrors = noErrors && ex1 == null;
            if (!noErrors)
            {
                throw new Exception($"Error during {nameof(CreateDirect2DCanvas)}.");
            }
        }

        public void SetFont(CanvasElement canvasElement, FontSettings fontSettings)
        {
            switch (canvasElement)
            {
                case CanvasElement.HEADER:
                    HeaderFont = fontSettings;
                    break;
                case CanvasElement.SUBHEADER:
                    SubHeaderFont = fontSettings;
                    break;
                case CanvasElement.DISPLAY_NAME:
                    DisplayNameFont = fontSettings;
                    break;
                case CanvasElement.USERNAME:
                    UsernameFont = fontSettings;
                    break;
                case CanvasElement.TEXT:
                    TweetTextFont = fontSettings;
                    break;
                case CanvasElement.TIME:
                    TweetTimeFont = fontSettings;
                    break;
                case CanvasElement.RETWEETER_DISPLAY_NAME:
                    RetweeterDisplayNameFont = fontSettings;
                    break;
                case CanvasElement.RETWEETER_USERNAME:
                    RetweeterUsernameFont = fontSettings;
                    break;
                default:
                    throw new Exception($"{nameof(canvasElement)} does not have settable font settings.");
            }
        }

        public void SetImage(CanvasElement canvasElement, string filename, float width, float height)
        {
            switch (canvasElement)
            {
                case CanvasElement.TWITTER_LOGO:
                    TwitterLogoFilename = filename;
                    TwitterLogoRectangle = new RectF() { Left = 0, Top = 0, Right = width, Bottom = height };
                    break;
                case CanvasElement.RETWEET_LOGO:
                    RetweetLogoFilename = filename;
                    RetweetLogoRectangle = new RectF() { Left = 0, Top = 0, Right = width, Bottom = height };
                    break;
                case CanvasElement.PROFILE_IMAGE:
                    ProfileImageFilename = filename;
                    ProfileImageRectangle = new RectF() { Left = 0, Top = 0, Right = width, Bottom = height };
                    break;
                default:
                    throw new Exception($"{nameof(canvasElement)} is not an image element.");
            }
        }

        public FontSettings GetFont(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.HEADER => HeaderFont,
                CanvasElement.SUBHEADER => SubHeaderFont,
                CanvasElement.DISPLAY_NAME => DisplayNameFont,
                CanvasElement.USERNAME => UsernameFont,
                CanvasElement.TEXT => TweetTextFont,
                CanvasElement.TIME => TweetTimeFont,
                CanvasElement.RETWEETER_DISPLAY_NAME => RetweeterDisplayNameFont,
                CanvasElement.RETWEETER_USERNAME => RetweeterUsernameFont,
                _ => throw new Exception($"{nameof(canvasElement)} does not have settable font settings."),
            };
        }

        public string GetImageFilename(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.TWITTER_LOGO => TwitterLogoFilename,
                CanvasElement.RETWEET_LOGO => RetweetLogoFilename,
                CanvasElement.PROFILE_IMAGE => ProfileImageFilename,
                _ => throw new Exception($"{nameof(canvasElement)} is not an image element."),
            };
        }

        public string GetText(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.HEADER => Header,
                CanvasElement.SUBHEADER => SubHeader,
                CanvasElement.DISPLAY_NAME => DisplayName,
                CanvasElement.USERNAME => Username,
                CanvasElement.TEXT => TweetText,
                CanvasElement.TIME => TweetTime,
                CanvasElement.RETWEETER_DISPLAY_NAME => RetweeterDisplayName,
                CanvasElement.RETWEETER_USERNAME => RetweeterUsername,
                _ => throw new Exception($"{nameof(canvasElement)} is not a text element."),
            };
        }

        private void SetTextLayout(CanvasElement canvasElement, TextLayoutResult textLayoutResult)
        {
            RectF textLayoutRectangle = new RectF()
            {
                Left = 0.0f,
                Top = 0.0f,
                Right = (float)textLayoutResult.width,
                Bottom = (float)textLayoutResult.height
            };

            switch (canvasElement)
            {
                case CanvasElement.HEADER:
                    headerTextLayout = textLayoutResult;
                    HeaderRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.SUBHEADER:
                    subHeaderTextLayout = textLayoutResult;
                    SubHeaderRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.DISPLAY_NAME:
                    displayNameTextLayout = textLayoutResult;
                    DisplayNameRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.USERNAME:
                    usernameTextLayout = textLayoutResult;
                    UsernameRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.TEXT:
                    tweetTextTextLayout = textLayoutResult;
                    TweetTextRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.TIME:
                    timeTextLayout = textLayoutResult;
                    TweetTimeRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.RETWEETER_DISPLAY_NAME:
                    retweeterDisplayNameTextLayout = textLayoutResult;
                    RetweeterDisplayNameRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.RETWEETER_USERNAME:
                    retweeterUsernameTextLayout = textLayoutResult;
                    RetweeterUsernameRectangle = textLayoutRectangle;
                    break;
                default:
                    throw new Exception($"{nameof(canvasElement)} is not a text element.");
            }
        }

        private TextLayoutResult GetTextLayout(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.HEADER => headerTextLayout,
                CanvasElement.SUBHEADER => subHeaderTextLayout,
                CanvasElement.DISPLAY_NAME => displayNameTextLayout,
                CanvasElement.USERNAME => usernameTextLayout,
                CanvasElement.TEXT => tweetTextTextLayout,
                CanvasElement.TIME => timeTextLayout,
                CanvasElement.RETWEETER_DISPLAY_NAME => retweeterDisplayNameTextLayout,
                CanvasElement.RETWEETER_USERNAME => retweeterUsernameTextLayout,
                _ => throw new Exception($"{nameof(canvasElement)} is not a text element."),
            };
        }

        private PointF GetOriginPoint(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.CANVAS => new PointF() { X = 0, Y = 0 },
                CanvasElement.HEADER => HeaderOriginPoint,
                CanvasElement.SUBHEADER => SubHeaderOriginPoint,
                CanvasElement.HEADING_SEPARATOR => new PointF() { X = HeadingSeparatorRectangle.Left, Y = HeadingSeparatorRectangle.Top },
                CanvasElement.TWEET => TweetOriginPoint,
                CanvasElement.PROFILE_IMAGE => ProfileImageOriginPoint,
                CanvasElement.DISPLAY_NAME => DisplayNameOriginPoint,
                CanvasElement.USERNAME => UsernameOriginPoint,
                CanvasElement.TEXT => TweetTextOriginPoint,
                CanvasElement.TWITTER_LOGO => TwitterLogoOriginPoint,
                CanvasElement.TIME => TweetTimeOriginPoint,
                CanvasElement.RETWEET_LOGO => RetweetLogoOriginPoint,
                CanvasElement.RETWEETER_DISPLAY_NAME => RetweeterDisplayNameOriginPoint,
                CanvasElement.RETWEETER_USERNAME => RetweeterUsernameOriginPoint,
                _ => throw new NotImplementedException()
            };
        }

        private RectF GetRectangle(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.CANVAS => new RectF() { Left = 0, Top = 0, Right = PanelRectangle.Width, Bottom = PanelRectangle.Height },
                CanvasElement.HEADER => HeaderRectangle,
                CanvasElement.SUBHEADER => SubHeaderRectangle,
                CanvasElement.HEADING_SEPARATOR => HeadingSeparatorRectangle,
                CanvasElement.TWEET => TweetRectangle,
                CanvasElement.PROFILE_IMAGE => ProfileImageRectangle,
                CanvasElement.DISPLAY_NAME => DisplayNameRectangle,
                CanvasElement.USERNAME => UsernameRectangle,
                CanvasElement.TEXT => TweetTextRectangle,
                CanvasElement.TWITTER_LOGO => TwitterLogoRectangle,
                CanvasElement.TIME => TweetTimeRectangle,
                CanvasElement.RETWEET_LOGO => RetweetLogoRectangle,
                CanvasElement.RETWEETER_DISPLAY_NAME => RetweeterDisplayNameRectangle,
                CanvasElement.RETWEETER_USERNAME => RetweeterUsernameRectangle,
                _ => throw new NotImplementedException()
            };
        }

        public void CreateTextLayout(CanvasElement canvasElement, RectF bounds)
        {
            Exception ex1;
            TextLayoutResult previousLayoutResult = GetTextLayout(canvasElement);
            if (previousLayoutResult.pDWriteTextLayout != IntPtr.Zero)
            {
                UnsafeNativeMethods.ReleaseTextLayout(previousLayoutResult);
            }
            ex1 = Marshal.GetExceptionForHR(Interop.UnsafeNativeMethods.CreateTextLayoutFromString(Direct2DCanvas, GetText(canvasElement), bounds, GetFont(canvasElement), out TextLayoutResult textLayoutResult));
            if (ex1 == null)
            {
                SetTextLayout(canvasElement, textLayoutResult);
            }
        }

        public void DrawTextLayout(CanvasElement canvasElement, IntPtr colorBrush)
        {
            UnsafeNativeMethods.DrawTextLayout(Direct2DCanvas, GetTextLayout(canvasElement), GetOriginPoint(canvasElement), colorBrush);
        }

        public void DrawHeadingSeparator(IntPtr lineColor, float lineThickness)
        {
            Interop.UnsafeNativeMethods.DrawLine(Direct2DCanvas, lineColor, HeadingSeparatorPoint1, HeadingSeparatorPoint2, lineThickness);
        }

        public void PushCircleLayer(CanvasElement canvasElement, IntPtr colorBrush)
        {
            PointF originPoint = GetOriginPoint(canvasElement);
            RectF dimensions = GetRectangle(canvasElement);

            #region Push a circle layer
            System.Drawing.Point centerPointForEllipse = new System.Drawing.Point()
            {
                X = (int)(originPoint.X + (dimensions.Right / 2)),
                Y = (int)(originPoint.Y + (dimensions.Bottom / 2))
            };
            Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.PushEllipseLayer(Direct2DCanvas, colorBrush, centerPointForEllipse.X, centerPointForEllipse.Y, dimensions.Right / 2, dimensions.Bottom / 2));
            #endregion
        }

        public void PopLayer()
        {
            Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.PopLayer(Direct2DCanvas));
        }

        public void DrawImage(CanvasElement canvasElement)
        {
            Interop.UnsafeNativeMethods.DrawImageFromFilename(Direct2DCanvas, GetImageFilename(canvasElement), GetOriginPoint(canvasElement), GetRectangle(canvasElement));
        }

        public void WipeCanvas(bool beginDraw = true, bool endDraw = true)
        {
            if (beginDraw)
            {
                Interop.UnsafeNativeMethods.BeginDraw(Direct2DCanvas);
            }
            Interop.UnsafeNativeMethods.DrawImage(Direct2DCanvas, BackgroundColor);
            if (endDraw)
            {
                Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(Direct2DCanvas));
            }
        }

        public void ClearArea(PointF originPoint, RectF areaOfCanvas, IntPtr backgroundBrush, bool beginDraw = true, bool endDraw = true)
        {
            RectF clearArea = new RectF()
            {
                Left = originPoint.X,
                Top = originPoint.Y,
                Right = originPoint.X + areaOfCanvas.Right,
                Bottom = originPoint.Y + areaOfCanvas.Bottom
            };
            if (beginDraw)
            {
                Interop.UnsafeNativeMethods.BeginDraw(Direct2DCanvas);
            }
            Interop.UnsafeNativeMethods.DrawRectangle(Direct2DCanvas, backgroundBrush, clearArea);
            if (endDraw)
            {
                Marshal.ThrowExceptionForHR(Interop.UnsafeNativeMethods.EndDraw(Direct2DCanvas));
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using TextFormatter.Interop;

namespace TextFormatter
{
    public enum CanvasElement
    {
        CANVAS = 0,
        HEADER = 1,
        SUBHEADER = 2,
        HEADING_SEPARATOR = 3,
        MESSAGE = 4,
        PROFILE_IMAGE = 5,
        DISPLAY_NAME = 6,
        USERNAME = 7,
        TEXT = 8,
        NETWORK_LOGO = 9,
        TIME = 10,
        SHARE_LOGO = 11,
        SHARER_DISPLAY_NAME = 12,
        SHARER_USERNAME = 13
    };

    public class MessagePanel
    {
        #region Variables
        public readonly SizeU PanelRectangle;
        public Direct2DCanvas Direct2DCanvas = new Direct2DCanvas();
        public UInt32 BackgroundColor;
        private TextLayoutResult headerTextLayout;
        private TextLayoutResult subHeaderTextLayout;
        private TextLayoutResult displayNameTextLayout;
        private TextLayoutResult usernameTextLayout;
        private TextLayoutResult messageTextTextLayout;
        private TextLayoutResult timeTextLayout;
        private TextLayoutResult sharerDisplayNameTextLayout;
        private TextLayoutResult sharerUsernameTextLayout;
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

        public PointF MessageOriginPoint { get; set; }
        public RectF MessageRectangle { get; set; }

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

        public PointF MessageTextOriginPoint { get; set; }
        public RectF MessageTextRectangle { get; set; }
        public FontSettings MessageTextFont { get; set; }
        public string MessageText { get; set; } = String.Empty;

        public PointF NetworkLogoOriginPoint { get; set; }
        public RectF NetworkLogoRectangle { get; set; }
        public string NetworkLogoFilename { get; set; } = String.Empty;

        public PointF TimeOriginPoint { get; set; }
        public RectF TimeRectangle { get; set; }
        public FontSettings TimeFont { get; set; }
        public string Time { get; set; } = String.Empty;

        public PointF ShareLogoOriginPoint { get; set; }
        public RectF ShareLogoRectangle { get; set; }
        public string ShareLogoFilename { get; set; } = String.Empty;

        public PointF SharerDisplayNameOriginPoint { get; set; }
        public RectF SharerDisplayNameRectangle { get; set; }
        public FontSettings SharerDisplayNameFont { get; set; }
        public string SharerDisplayName { get; set; } = String.Empty;

        public PointF SharerUsernameOriginPoint { get; set; }
        public RectF SharerUsernameRectangle { get; set; }
        public FontSettings SharerUsernameFont { get; set; }
        public string SharerUsername { get; set; } = String.Empty;
        #endregion

        public MessagePanel(ref Direct2DPointers direct2DPointers, SizeU canvasSize, UInt32 backgroundColor)
        {
            PanelRectangle = canvasSize;
            BackgroundColor = backgroundColor;
            CreateDirect2DCanvas(PanelRectangle, ref direct2DPointers);
            WipeCanvas();
        }

        /// <summary>
        /// Create pointers for IWICBitmap and ID2D1RenderTarget, and set the canvas size
        /// </summary>
        /// <param name="canvasSize">Desired bitmap width and height</param>
        /// <param name="direct2DPointers">An instantiated instance of Direct2DPointers</param>
        public void CreateDirect2DCanvas(SizeU canvasSize, ref Direct2DPointers direct2DPointers)
        {
            Exception ex1;
            bool noErrors = true;
            ex1 = Marshal.GetExceptionForHR(UnsafeNativeMethods.CreateWICBitmap(ref direct2DPointers, canvasSize.Width, canvasSize.Height, ref Direct2DCanvas));
            noErrors = noErrors && ex1 == null;
            ex1 = Marshal.GetExceptionForHR(UnsafeNativeMethods.CreateRenderTarget(ref Direct2DCanvas));
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
                    MessageTextFont = fontSettings;
                    break;
                case CanvasElement.TIME:
                    TimeFont = fontSettings;
                    break;
                case CanvasElement.SHARER_DISPLAY_NAME:
                    SharerDisplayNameFont = fontSettings;
                    break;
                case CanvasElement.SHARER_USERNAME:
                    SharerUsernameFont = fontSettings;
                    break;
                default:
                    throw new Exception($"{nameof(canvasElement)} does not have settable font settings.");
            }
        }

        public void SetImage(CanvasElement canvasElement, string filename, float width, float height)
        {
            switch (canvasElement)
            {
                case CanvasElement.NETWORK_LOGO:
                    NetworkLogoFilename = filename;
                    NetworkLogoRectangle = new RectF() { Left = 0, Top = 0, Right = width, Bottom = height };
                    break;
                case CanvasElement.SHARE_LOGO:
                    ShareLogoFilename = filename;
                    ShareLogoRectangle = new RectF() { Left = 0, Top = 0, Right = width, Bottom = height };
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
                CanvasElement.TEXT => MessageTextFont,
                CanvasElement.TIME => TimeFont,
                CanvasElement.SHARER_DISPLAY_NAME => SharerDisplayNameFont,
                CanvasElement.SHARER_USERNAME => SharerUsernameFont,
                _ => throw new Exception($"{nameof(canvasElement)} does not have settable font settings."),
            };
        }

        public string GetImageFilename(CanvasElement canvasElement)
        {
            return canvasElement switch
            {
                CanvasElement.NETWORK_LOGO => NetworkLogoFilename,
                CanvasElement.SHARE_LOGO => ShareLogoFilename,
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
                CanvasElement.TEXT => MessageText,
                CanvasElement.TIME => Time,
                CanvasElement.SHARER_DISPLAY_NAME => SharerDisplayName,
                CanvasElement.SHARER_USERNAME => SharerUsername,
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
                    messageTextTextLayout = textLayoutResult;
                    MessageTextRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.TIME:
                    timeTextLayout = textLayoutResult;
                    TimeRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.SHARER_DISPLAY_NAME:
                    sharerDisplayNameTextLayout = textLayoutResult;
                    SharerDisplayNameRectangle = textLayoutRectangle;
                    break;
                case CanvasElement.SHARER_USERNAME:
                    sharerUsernameTextLayout = textLayoutResult;
                    SharerUsernameRectangle = textLayoutRectangle;
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
                CanvasElement.TEXT => messageTextTextLayout,
                CanvasElement.TIME => timeTextLayout,
                CanvasElement.SHARER_DISPLAY_NAME => sharerDisplayNameTextLayout,
                CanvasElement.SHARER_USERNAME => sharerUsernameTextLayout,
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
                CanvasElement.MESSAGE => MessageOriginPoint,
                CanvasElement.PROFILE_IMAGE => ProfileImageOriginPoint,
                CanvasElement.DISPLAY_NAME => DisplayNameOriginPoint,
                CanvasElement.USERNAME => UsernameOriginPoint,
                CanvasElement.TEXT => MessageTextOriginPoint,
                CanvasElement.NETWORK_LOGO => NetworkLogoOriginPoint,
                CanvasElement.TIME => TimeOriginPoint,
                CanvasElement.SHARE_LOGO => ShareLogoOriginPoint,
                CanvasElement.SHARER_DISPLAY_NAME => SharerDisplayNameOriginPoint,
                CanvasElement.SHARER_USERNAME => SharerUsernameOriginPoint,
                _ => throw new NotImplementedException($"{nameof(canvasElement)} is not a known element.")
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
                CanvasElement.MESSAGE => MessageRectangle,
                CanvasElement.PROFILE_IMAGE => ProfileImageRectangle,
                CanvasElement.DISPLAY_NAME => DisplayNameRectangle,
                CanvasElement.USERNAME => UsernameRectangle,
                CanvasElement.TEXT => MessageTextRectangle,
                CanvasElement.NETWORK_LOGO => NetworkLogoRectangle,
                CanvasElement.TIME => TimeRectangle,
                CanvasElement.SHARE_LOGO => ShareLogoRectangle,
                CanvasElement.SHARER_DISPLAY_NAME => SharerDisplayNameRectangle,
                CanvasElement.SHARER_USERNAME => SharerUsernameRectangle,
                _ => throw new NotImplementedException($"{nameof(canvasElement)} is not a known element.")
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
            ex1 = Marshal.GetExceptionForHR(UnsafeNativeMethods.CreateTextLayoutFromString(Direct2DCanvas, GetText(canvasElement), bounds, GetFont(canvasElement), out TextLayoutResult textLayoutResult));
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
            UnsafeNativeMethods.DrawLine(Direct2DCanvas, lineColor, HeadingSeparatorPoint1, HeadingSeparatorPoint2, lineThickness);
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
            Marshal.ThrowExceptionForHR(UnsafeNativeMethods.PushEllipseLayer(Direct2DCanvas, colorBrush, centerPointForEllipse.X, centerPointForEllipse.Y, dimensions.Right / 2, dimensions.Bottom / 2));
            #endregion
        }

        public void PopLayer()
        {
            Marshal.ThrowExceptionForHR(UnsafeNativeMethods.PopLayer(Direct2DCanvas));
        }

        public void DrawImage(CanvasElement canvasElement)
        {
            UnsafeNativeMethods.DrawImageFromFilename(Direct2DCanvas, GetImageFilename(canvasElement), GetOriginPoint(canvasElement), GetRectangle(canvasElement));
        }

        public void WipeCanvas(bool beginDraw = true, bool endDraw = true)
        {
            if (beginDraw)
            {
                UnsafeNativeMethods.BeginDraw(Direct2DCanvas);
            }
            UnsafeNativeMethods.DrawImage(Direct2DCanvas, BackgroundColor);
            if (endDraw)
            {
                Marshal.ThrowExceptionForHR(UnsafeNativeMethods.EndDraw(Direct2DCanvas));
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
                UnsafeNativeMethods.BeginDraw(Direct2DCanvas);
            }
            UnsafeNativeMethods.DrawRectangle(Direct2DCanvas, backgroundBrush, clearArea);
            if (endDraw)
            {
                Marshal.ThrowExceptionForHR(UnsafeNativeMethods.EndDraw(Direct2DCanvas));
            }
        }
    }
}
#pragma once

#ifdef DIRECT2DWRAPPER_EXPORTS
#define DIRECT2DWRAPPER_CPP_CLASS __declspec(dllexport)
#define DIRECT2DWRAPPER_CPP_FUNCTION __declspec(dllexport)
#define DIRECT2DWRAPPER_C_FUNCTION extern "C" __declspec(dllexport)
#else
#define DIRECT2DWRAPPER_CPP_CLASS __declspec(dllimport)
#define DIRECT2DWRAPPER_CPP_FUNCTION __declspec(dllimport)
#define DIRECT2DWRAPPER_C_FUNCTION extern "C" __declspec(dllimport)
#endif

namespace Direct2DWrapper
{
	DIRECT2DWRAPPER_C_FUNCTION
		double Add(int a, double b);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateD2D1Factory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD2D1Factory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateD3D11Device(struct Direct2DPointers* pDirect2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD3D11Device(struct Direct2DPointers* pDirect2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDXGIDevice(struct Direct2DPointers* pDirect2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDXGIDevice(struct Direct2DPointers* pDirect2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDXGISwapChain(struct Direct2DPointers* pDirect2DPointers, UINT width, UINT height, struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDXGISwapChain(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDWriteFactory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDWriteFactory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateRenderTarget(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseRenderTarget(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		void BeginDraw(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT EndDraw(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1SolidColorBrush* CreateSolidColorBrush(struct Direct2DCanvas* pCanvas, UINT32 argb);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseSolidColorBrush(ID2D1SolidColorBrush* pD2D1SolidColorBrush);

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawRectangleBorder(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth);

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawLine(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, D2D1_POINT_2F HeadingSeparatorPoint1, D2D1_POINT_2F HeadingSeparatorPoint2, float lineThickness);

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawRectangle(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, D2D1_RECT_F bounds);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT PushEllipseLayer(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, float centerX, float centerY, float radiusX, float radiusY);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT PopLayer(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateTextLayoutFromString(struct Direct2DCanvas* pCanvas, PCWSTR text, D2D1_RECT_F bounds, struct FontSettings* fontSettings, struct TextLayoutResult* textLayoutResult);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawTextLayout(struct Direct2DCanvas* pCanvas, struct TextLayoutResult* textLayoutResult, D2D1_POINT_2F originPoint, ID2D1SolidColorBrush* pD2D1SolidColorBrush);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseTextLayout(struct TextLayoutResult* textLayoutResult);

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawImage(struct Direct2DCanvas* pCanvas, UINT32 argb);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateImagingFactory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseImagingFactory(struct Direct2DPointers* direct2DPointers);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawImageFromFilename(struct Direct2DCanvas* pCanvas, PCWSTR filename, D2D1_POINT_2F originPoint, D2D1_RECT_F bounds);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT UpdateHandle(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT UpdateImage(struct Direct2DCanvas* pCanvas);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT SaveImage(struct Direct2DCanvas* pCanvas, PCWSTR filename);

} // namespace Direct2DWrapper

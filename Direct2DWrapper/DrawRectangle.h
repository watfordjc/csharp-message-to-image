#pragma once

// Windows Header Files:
#include <windows.h>

// C RunTime Header Files:
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <wchar.h>
#include <math.h>

#include <d2d1.h>
#pragma comment(lib, "d2d1.lib")
#include <d2d1helper.h>
#include <d2d1_1.h>
#include <d2d1_1helper.h>
#include <d2d1effects.h>
#include <d2d1effecthelpers.h>
#pragma comment(lib, "dxguid.lib")
#include <dwrite.h>
#include <wincodec.h>

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
		ID2D1Factory* CreateD2D1Factory();

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD2D1Factory(ID2D1Factory* pD2D1Factory);

	DIRECT2DWRAPPER_C_FUNCTION
		IWICImagingFactory* CreateImagingFactory();

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseImagingFactory(IWICImagingFactory* pWICImagingFactory);

	DIRECT2DWRAPPER_C_FUNCTION
		IWICBitmap* CreateWICBitmap(IWICImagingFactory* pWICImagingFactory, UINT width, UINT height);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseWICBitmap(IWICBitmap* pWICBitmap);

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1RenderTarget* CreateRenderTarget(ID2D1Factory* pD2D1Factory, IWICBitmap* pWICBitmap);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseRenderTarget(ID2D1RenderTarget* pD2D1RenderTarget);

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1SolidColorBrush* CreateSolidColorBrush(ID2D1RenderTarget* pD2D1RenderTarget, UINT32 argb);

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseSolidColorBrush(ID2D1SolidColorBrush* pD2D1SolidColorBrush);

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawRectangleBorder(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth);

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawRectangle(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawImageFromFilename(IWICImagingFactory* pWICImagingFactory, ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR filename, int startX, int startY, int width, int height);

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawImage(ID2D1RenderTarget* pD2D1RenderTarget);

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT SaveImage(IWICImagingFactory* pWICImagingFactory, IWICBitmap* pWICBitmap, ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR filename);
}


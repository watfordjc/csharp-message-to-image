#include "pch.h"
#include "DrawRectangle.h"

namespace Direct2DWrapper
{

	template <class T> void SafeRelease(T** ppT)
	{
		if (*ppT)
		{
			(*ppT)->Release();
			*ppT = NULL;
		}
	}

	DIRECT2DWRAPPER_C_FUNCTION
		double Add(int a, double b)
	{
		return a + b;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1Factory* CreateD2D1Factory()
	{
		ID2D1Factory* pD2D1Factory = NULL;
		HRESULT hr = D2D1CreateFactory(
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			&pD2D1Factory
		);
		return pD2D1Factory;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD2D1Factory(ID2D1Factory* pD2D1Factory)
	{
		SafeRelease(&pD2D1Factory);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		IWICImagingFactory* CreateImagingFactory()
	{
		IWICImagingFactory* pWICImagingFactory = NULL;

		HRESULT hr = CoCreateInstance(
			CLSID_WICImagingFactory,
			nullptr,
			CLSCTX_INPROC_SERVER,
			__uuidof(IWICImagingFactory),
			reinterpret_cast<void**>(&pWICImagingFactory)
		);
		return pWICImagingFactory;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseImagingFactory(IWICImagingFactory* pWICImagingFactory)
	{
		SafeRelease(&pWICImagingFactory);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		IWICBitmap* CreateWICBitmap(IWICImagingFactory* pWICImagingFactory, UINT width, UINT height)
	{
		IWICBitmap* pWICBitmap = NULL;
		HRESULT hr = pWICImagingFactory->CreateBitmap(
			width,
			height,
			GUID_WICPixelFormat32bppPBGRA,
			WICBitmapCacheOnLoad,
			&pWICBitmap
		);
		return pWICBitmap;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseWICBitmap(IWICBitmap* pWICBitmap)
	{
		SafeRelease(&pWICBitmap);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1RenderTarget* CreateRenderTarget(ID2D1Factory* pD2D1Factory, IWICBitmap* pWICBitmap)
	{
		D2D1_RENDER_TARGET_PROPERTIES targetProperties = D2D1::RenderTargetProperties();
		D2D1_PIXEL_FORMAT desiredFormat = D2D1::PixelFormat(
			DXGI_FORMAT_B8G8R8A8_UNORM,
			D2D1_ALPHA_MODE_PREMULTIPLIED
		);
		targetProperties.pixelFormat = desiredFormat;

		ID2D1RenderTarget* pD2D1RenderTarget = NULL;
		HRESULT hr = pD2D1Factory->CreateWicBitmapRenderTarget(
			pWICBitmap,
			targetProperties,
			&pD2D1RenderTarget
		);
		return pD2D1RenderTarget;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseRenderTarget(ID2D1RenderTarget* pD2D1RenderTarget)
	{
		SafeRelease(&pD2D1RenderTarget);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawImage(ID2D1RenderTarget* pD2D1RenderTarget)
	{
		pD2D1RenderTarget->BeginDraw();
		pD2D1RenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::White));

		HRESULT hr = pD2D1RenderTarget->EndDraw();
		if (FAILED(hr) || hr == D2DERR_RECREATE_TARGET)
		{
			SafeRelease(&pD2D1RenderTarget);
			return false;
		}
		return true;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1SolidColorBrush* CreateSolidColorBrush(ID2D1RenderTarget* pD2D1RenderTarget, UINT32 argb)
	{
		ID2D1SolidColorBrush* pD2D1SolidColorBrush = NULL;
		HRESULT hr = pD2D1RenderTarget->CreateSolidColorBrush(
			D2D1::ColorF(D2D1::ColorF( // using overload ColorF(UINT32 rgb, float a)
				argb & 0xFFFFFF, // Lower 24 bits are 0xRRGGBB
				((float)(argb >> 24 & 0xFF)) / 0xFF // Upper 8 bits are 0xAA - bit shift, cast to float, then divide by 255 (0xFF)
			)),
			&pD2D1SolidColorBrush
		);
		return pD2D1SolidColorBrush;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseSolidColorBrush(ID2D1SolidColorBrush* pD2D1SolidColorBrush)
	{
		SafeRelease(&pD2D1SolidColorBrush);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawRectangleBorder(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth)
	{
		pD2D1RenderTarget->BeginDraw();

		pD2D1RenderTarget->DrawRectangle(
			D2D1::RectF(
				startX + (lineWidth / 2),
				startY + (lineWidth / 2),
				startX + lengthX + (lineWidth * 1.5),
				startY + lengthY + (lineWidth * 1.5)
			),
			pD2D1SolidColorBrush,
			lineWidth
		);

		HRESULT hr = pD2D1RenderTarget->EndDraw();
		if (FAILED(hr) || hr == D2DERR_RECREATE_TARGET)
		{
			SafeRelease(&pD2D1RenderTarget);
			return false;
		}
		return true;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		bool DrawRectangle(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY)
	{
		pD2D1RenderTarget->BeginDraw();

		pD2D1RenderTarget->FillRectangle(
			D2D1::RectF(
				startX,
				startY,
				startX + lengthX,
				startY + lengthY
			),
			pD2D1SolidColorBrush
		);

		HRESULT hr = pD2D1RenderTarget->EndDraw();
		if (FAILED(hr) || hr == D2DERR_RECREATE_TARGET)
		{
			SafeRelease(&pD2D1RenderTarget);
			return false;
		}
		return true;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawImageFromFilename(IWICImagingFactory* pWICImagingFactory, ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR filename, int startX, int startY, int width, int height)
	{
		IWICBitmapDecoder* pWICBitmapDecoder = NULL;
		IWICBitmapFrameDecode* pWICBitmapFrameDecode = NULL;
		IWICFormatConverter* pWICFormatConverter = NULL;
		ID2D1Bitmap* pD2D1Bitmap = NULL;

		HRESULT hr = pWICImagingFactory->CreateDecoderFromFilename(
			filename,
			NULL,
			GENERIC_READ,
			WICDecodeMetadataCacheOnLoad,
			&pWICBitmapDecoder
		);
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapDecoder->GetFrame(0, &pWICBitmapFrameDecode);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICImagingFactory->CreateFormatConverter(&pWICFormatConverter);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICFormatConverter->Initialize(
				pWICBitmapFrameDecode,
				GUID_WICPixelFormat32bppPBGRA,
				WICBitmapDitherTypeNone,
				NULL,
				0.f,
				WICBitmapPaletteTypeMedianCut
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pD2D1RenderTarget->CreateBitmapFromWicBitmap(
				pWICFormatConverter,
				NULL,
				&pD2D1Bitmap
			);
		}
		if (SUCCEEDED(hr))
		{
			pD2D1RenderTarget->BeginDraw();
			pD2D1RenderTarget->DrawBitmap(
				pD2D1Bitmap,
				D2D1::RectF(
					startX,
					startY,
					startX + width,
					startY + height
				),
				1.0f,
				D2D1_BITMAP_INTERPOLATION_MODE_LINEAR
			);
			hr = pD2D1RenderTarget->EndDraw();
		}
		SafeRelease(&pD2D1Bitmap);
		SafeRelease(&pWICFormatConverter);
		SafeRelease(&pWICBitmapFrameDecode);
		SafeRelease(&pWICBitmapDecoder);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT SaveImage(IWICImagingFactory* pWICImagingFactory, IWICBitmap* pWICBitmap, ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR filename)
	{

		IWICStream* pWICStream = NULL;
		IWICBitmapEncoder* pWICBitmapEncoder = NULL;
		IWICBitmapFrameEncode* pWICBitmapFrameEncode = NULL;
		WICPixelFormatGUID pixelFormat = GUID_WICPixelFormatDontCare;

		HRESULT hr = pWICImagingFactory->CreateStream(&pWICStream);

		if (SUCCEEDED(hr))
		{
			hr = pWICStream->InitializeFromFilename(
				filename,
				GENERIC_WRITE
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICImagingFactory->CreateEncoder(
				GUID_ContainerFormatPng,
				NULL,
				&pWICBitmapEncoder
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapEncoder->Initialize(
				pWICStream,
				WICBitmapEncoderNoCache
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapEncoder->CreateNewFrame(
				&pWICBitmapFrameEncode,
				NULL
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapFrameEncode->Initialize(NULL);
		}
		if (SUCCEEDED(hr))
		{
			D2D1_SIZE_U renderTargetSize = pD2D1RenderTarget->GetPixelSize();
			hr = pWICBitmapFrameEncode->SetSize(
				renderTargetSize.width,
				renderTargetSize.height
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapFrameEncode->SetPixelFormat(&pixelFormat);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapFrameEncode->WriteSource(pWICBitmap, NULL);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapFrameEncode->Commit();
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapEncoder->Commit();
		}

		SafeRelease(&pWICBitmapFrameEncode);
		SafeRelease(&pWICBitmapEncoder);
		SafeRelease(&pWICStream);

		return hr;
	}

}

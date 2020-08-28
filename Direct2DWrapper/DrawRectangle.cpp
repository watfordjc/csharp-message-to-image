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
		D2D1_FACTORY_OPTIONS factoryOptions;
		factoryOptions.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

		ID2D1Factory* pD2D1Factory = NULL;

		HRESULT hr = D2D1CreateFactory(
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			factoryOptions,
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
		void BeginDraw(ID2D1RenderTarget* pD2D1RenderTarget)
	{
		pD2D1RenderTarget->BeginDraw();
	}

	DIRECT2DWRAPPER_C_FUNCTION
	HRESULT EndDraw(ID2D1RenderTarget* pD2D1RenderTarget)
	{
		HRESULT hr = pD2D1RenderTarget->EndDraw();
		if (FAILED(hr) || hr == D2DERR_RECREATE_TARGET)
		{
			SafeRelease(&pD2D1RenderTarget);
		}
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawImage(ID2D1RenderTarget* pD2D1RenderTarget, UINT32 argb)
	{
		pD2D1RenderTarget->Clear(
			D2D1::ColorF( // using overload ColorF(UINT32 rgb, float a)
				argb & 0xFFFFFF, // Lower 24 bits are 0xRRGGBB
				((float)(argb >> 24 & 0xFF)) / 0xFF // Upper 8 bits are 0xAA - bit shift, cast to float, then divide by 255 (0xFF)
			));
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
		void DrawRectangleBorder(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth)
	{
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
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawRectangle(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY)
	{
		pD2D1RenderTarget->FillRectangle(
			D2D1::RectF(
				startX,
				startY,
				startX + lengthX,
				startY + lengthY
			),
			pD2D1SolidColorBrush
		);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT PushEllipseLayer(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, float centerX, float centerY, float radiusX, float radiusY)
	{
		ID2D1Factory* pD2D1Factory = NULL;
		ID2D1DeviceContext* pD2D1DeviceContext = NULL;
		ID2D1EllipseGeometry* ellipseMask = NULL;

		pD2D1RenderTarget->GetFactory(&pD2D1Factory);
		pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);

		D2D1_POINT_2F centerPoint = D2D1::Point2F(
			centerX,
			centerY
		);

		HRESULT hr = pD2D1Factory->CreateEllipseGeometry(
			D2D1::Ellipse(
				centerPoint,
				radiusX,
				radiusY
			),
			&ellipseMask
		);

		if (SUCCEEDED(hr))
		{
			D2D1_LAYER_PARAMETERS1 layerParams = D2D1::LayerParameters1(
				D2D1::RectF(
					centerX - radiusX,
					centerY - radiusY,
					centerX + radiusX,
					centerY + radiusY
				),
				ellipseMask,
				D2D1_ANTIALIAS_MODE_PER_PRIMITIVE,
				D2D1::IdentityMatrix(),
				1.0f,
				pD2D1SolidColorBrush,
				D2D1_LAYER_OPTIONS1_NONE
			);
			pD2D1RenderTarget->BeginDraw();
			pD2D1DeviceContext->PushLayer(layerParams, NULL);
		}

		SafeRelease(&ellipseMask);
		pD2D1DeviceContext->Release();
		pD2D1Factory->Release();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
	HRESULT PopLayer(ID2D1RenderTarget* pD2D1RenderTarget)
	{
		pD2D1RenderTarget->PopLayer();
		HRESULT hr = pD2D1RenderTarget->EndDraw();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawImageFromFilename(IWICImagingFactory* pWICImagingFactory, ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR filename, int startX, int startY, int width, int height)
	{
		IWICBitmapDecoder* pWICBitmapDecoder = NULL;
		IWICBitmapFrameDecode* pWICBitmapFrameDecode = NULL;
		IWICFormatConverter* pWICFormatConverter = NULL;
		ID2D1Bitmap* pD2D1Bitmap = NULL;
		ID2D1Effect* pBitmapSourceEffect = NULL;
		ID2D1DeviceContext* pD2D1DeviceContext = NULL;

		pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);

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
			hr = pD2D1DeviceContext->CreateBitmapFromWicBitmap(
				pWICFormatConverter,
				NULL,
				&pD2D1Bitmap
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pD2D1DeviceContext->CreateEffect(
				CLSID_D2D1BitmapSource,
				&pBitmapSourceEffect
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pBitmapSourceEffect->SetValue(
				D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
				pWICFormatConverter
			);
		}
		if (SUCCEEDED(hr))
		{
			D2D1_SIZE_F originalSize = pD2D1Bitmap->GetSize();
			D2D1_VECTOR_2F resizeVector = D2D1::Vector2F(
				width / originalSize.width,
				height / originalSize.height
			);
			hr = pBitmapSourceEffect->SetValue(
				D2D1_BITMAPSOURCE_PROP_SCALE,
				resizeVector
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pBitmapSourceEffect->SetValue(
				D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
				D2D1_BITMAPSOURCE_INTERPOLATION_MODE_CUBIC
			);
		}
		if (SUCCEEDED(hr))
		{
			pD2D1DeviceContext->DrawImage(
				pBitmapSourceEffect,
				D2D1::Point2F(startX, startY),
				D2D1::RectF(
					0,
					0,
					width,
					height
				),
				D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC,
				D2D1_COMPOSITE_MODE_SOURCE_OVER
			);
		}
		SafeRelease(&pBitmapSourceEffect);
		SafeRelease(&pD2D1Bitmap);
		SafeRelease(&pWICFormatConverter);
		SafeRelease(&pWICBitmapFrameDecode);
		SafeRelease(&pWICBitmapDecoder);
		pD2D1DeviceContext->Release();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawTextFromString(ID2D1RenderTarget* pD2D1RenderTarget, PCWSTR text, int startX, int startY, int width, int height, bool justifyCentered, PCWSTR fontName, float fontSize, PCWSTR localeName, ID2D1SolidColorBrush* pD2D1SolidColorBrush)
	{
		ID2D1DeviceContext4* pD2D1DeviceContext = NULL;
		IDWriteFactory2* pDWriteFactory = NULL;
		IDWriteTextFormat2* pDWriteTextFormat2 = NULL;

		pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);

		HRESULT hr = DWriteCreateFactory(
			DWRITE_FACTORY_TYPE_SHARED,
			__uuidof(IDWriteFactory),
			reinterpret_cast<IUnknown**>(&pDWriteFactory)
		);
		if (SUCCEEDED(hr))
		{
			IDWriteTextFormat* pDWriteTextFormat = NULL;
			hr = pDWriteFactory->CreateTextFormat(
				fontName,
				NULL,
				DWRITE_FONT_WEIGHT_NORMAL,
				DWRITE_FONT_STYLE_NORMAL,
				DWRITE_FONT_STRETCH_NORMAL,
				fontSize,
				localeName,
				&pDWriteTextFormat
			);
			pDWriteTextFormat2 = (IDWriteTextFormat2*)pDWriteTextFormat;
		}
		if (SUCCEEDED(hr) && justifyCentered)
		{
			hr = pDWriteTextFormat2->SetTextAlignment(
				DWRITE_TEXT_ALIGNMENT_CENTER
			);
		}
		if (SUCCEEDED(hr))
		{
			D2D1_RECT_F layoutRectangle = D2D1::RectF(
				startX,
				startY,
				startX + width,
				startY + height
			);
			UINT32 len = wcslen(text);
			pD2D1DeviceContext->DrawTextW(
				text,
				len,
				pDWriteTextFormat2,
				&layoutRectangle,
				pD2D1SolidColorBrush,
				D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT
			);
		}
		SafeRelease(&pDWriteTextFormat2);
		SafeRelease(&pDWriteFactory);
		pD2D1DeviceContext->Release();
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

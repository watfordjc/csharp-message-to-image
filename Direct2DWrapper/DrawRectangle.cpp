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

	struct Direct2DPointers
	{
		ID2D1Factory* Direct2DFactory;
		IDWriteFactory7* DirectWriteFactory;
		IWICImagingFactory* WICImagingFactory;
	};

	struct Direct2DCanvas
	{
		IWICBitmap* Bitmap;
		ID2D1RenderTarget* RenderTarget;
		struct Direct2DPointers Direct2DPointers;
	};

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateD2D1Factory(struct Direct2DPointers* pDirect2DPointers)
	{
		D2D1_FACTORY_OPTIONS factoryOptions;
		//factoryOptions.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

		HRESULT hr = D2D1CreateFactory(
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			//factoryOptions,
			&pDirect2DPointers->Direct2DFactory
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD2D1Factory(struct Direct2DPointers* direct2DPointers)
	{
		SafeRelease(&direct2DPointers->Direct2DFactory);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateImagingFactory(struct Direct2DPointers* pDirect2DPointers)
	{
		HRESULT hr = CoCreateInstance(
			CLSID_WICImagingFactory,
			nullptr,
			CLSCTX_INPROC_SERVER,
			__uuidof(IWICImagingFactory),
			reinterpret_cast<void**>(&pDirect2DPointers->WICImagingFactory)
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseImagingFactory(struct Direct2DPointers* direct2DPointers)
	{
		SafeRelease(&direct2DPointers->WICImagingFactory);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDWriteFactory(struct Direct2DPointers* pDirect2DPointers)
	{
		HRESULT hr = DWriteCreateFactory(
			DWRITE_FACTORY_TYPE_SHARED,
			__uuidof(IDWriteFactory7),
			reinterpret_cast<IUnknown**>(&pDirect2DPointers->DirectWriteFactory)
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDWriteFactory(struct Direct2DPointers* direct2DPointers)
	{
		SafeRelease(&direct2DPointers->DirectWriteFactory);
	}


	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateWICBitmap(struct Direct2DPointers* pDirect2DPointers, UINT width, UINT height, struct Direct2DCanvas* pCanvas)
	{
		pCanvas->Direct2DPointers = *pDirect2DPointers;
		HRESULT hr = pDirect2DPointers->WICImagingFactory->CreateBitmap(
			width,
			height,
			GUID_WICPixelFormat32bppPBGRA,
			WICBitmapCacheOnLoad,
			&pCanvas->Bitmap
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseWICBitmap(struct Direct2DCanvas* pCanvas)
	{
		SafeRelease(&pCanvas->Bitmap);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateRenderTarget(struct Direct2DCanvas* pCanvas)
	{
		D2D1_RENDER_TARGET_PROPERTIES targetProperties = D2D1::RenderTargetProperties();
		D2D1_PIXEL_FORMAT desiredFormat = D2D1::PixelFormat(
			DXGI_FORMAT_B8G8R8A8_UNORM,
			D2D1_ALPHA_MODE_PREMULTIPLIED
		);
		targetProperties.pixelFormat = desiredFormat;

		HRESULT hr = pCanvas->Direct2DPointers.Direct2DFactory->CreateWicBitmapRenderTarget(
			pCanvas->Bitmap,
			targetProperties,
			&pCanvas->RenderTarget
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseRenderTarget(struct Direct2DCanvas* pCanvas)
	{
		SafeRelease(&pCanvas->RenderTarget);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void BeginDraw(struct Direct2DCanvas* pCanvas)
	{
		pCanvas->RenderTarget->BeginDraw();
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT EndDraw(struct Direct2DCanvas* pCanvas)
	{
		HRESULT hr = pCanvas->RenderTarget->EndDraw();
		if (FAILED(hr) || hr == D2DERR_RECREATE_TARGET)
		{
			SafeRelease(&pCanvas->RenderTarget);
		}
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawImage(struct Direct2DCanvas* pCanvas, UINT32 argb)
	{
		pCanvas->RenderTarget->Clear(
			D2D1::ColorF( // using overload ColorF(UINT32 rgb, float a)
				argb & 0xFFFFFF, // Lower 24 bits are 0xRRGGBB
				((float)(argb >> 24 & 0xFF)) / 0xFF // Upper 8 bits are 0xAA - bit shift, cast to float, then divide by 255 (0xFF)
			));
	}

	DIRECT2DWRAPPER_C_FUNCTION
		ID2D1SolidColorBrush* CreateSolidColorBrush(struct Direct2DCanvas* pCanvas, UINT32 argb)
	{
		ID2D1SolidColorBrush* pD2D1SolidColorBrush = NULL;
		HRESULT hr = pCanvas->RenderTarget->CreateSolidColorBrush(
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
		void DrawRectangleBorder(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY, float lineWidth)
	{
		pCanvas->RenderTarget->DrawRectangle(
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
		void DrawLine(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int stopX, int stopY, float lineWidth)
	{
		pCanvas->RenderTarget->DrawLine(
			D2D1::Point2F(startX, startY),
			D2D1::Point2F(stopX, stopY),
			pD2D1SolidColorBrush,
			lineWidth
		);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawRectangle(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int lengthX, int lengthY)
	{
		pCanvas->RenderTarget->FillRectangle(
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
		HRESULT PushEllipseLayer(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, float centerX, float centerY, float radiusX, float radiusY)
	{
		ID2D1DeviceContext* pD2D1DeviceContext = NULL;
		ID2D1EllipseGeometry* ellipseMask = NULL;

		D2D1_POINT_2F centerPoint = D2D1::Point2F(
			centerX,
			centerY
		);

		HRESULT hr = pCanvas->RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			hr = pCanvas->Direct2DPointers.Direct2DFactory->CreateEllipseGeometry(
				D2D1::Ellipse(
					centerPoint,
					radiusX,
					radiusY
				),
				&ellipseMask
			);
		}
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
			pCanvas->RenderTarget->BeginDraw();
			pD2D1DeviceContext->PushLayer(layerParams, NULL);
		}

		SafeRelease(&ellipseMask);
		pD2D1DeviceContext->Release();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT PopLayer(struct Direct2DCanvas* pCanvas)
	{
		pCanvas->RenderTarget->PopLayer();
		HRESULT hr = pCanvas->RenderTarget->EndDraw();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawImageFromFilename(struct Direct2DCanvas* pCanvas, PCWSTR filename, int startX, int startY, int width, int height)
	{
		IWICBitmapDecoder* pWICBitmapDecoder = NULL;
		IWICBitmapFrameDecode* pWICBitmapFrameDecode = NULL;
		IWICFormatConverter* pWICFormatConverter = NULL;
		ID2D1Bitmap* pD2D1Bitmap = NULL;
		ID2D1Effect* pBitmapSourceEffect = NULL;
		ID2D1DeviceContext* pD2D1DeviceContext = NULL;

		HRESULT hr = pCanvas->RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateDecoderFromFilename(
				filename,
				NULL,
				GENERIC_READ,
				WICDecodeMetadataCacheOnLoad,
				&pWICBitmapDecoder
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pWICBitmapDecoder->GetFrame(0, &pWICBitmapFrameDecode);
		}
		if (SUCCEEDED(hr))
		{
			hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateFormatConverter(&pWICFormatConverter);
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

	struct TextLayoutResult {
		IDWriteTextLayout4* pDWriteTextLayout;
		int lineCount; // Lines of text
		int top; // Top edge of text block from edge of canvas
		int left; // Left edge of text block from edge of canvas
		double height; // Height of text block
		double width; // Width of text block
		float lineSpacing; // Line-spacing for font used
		float baseline; // Baseline for font used
		double lineHeight;
		double lineHeightEm;
	};

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateTextLayoutFromString(
			struct Direct2DCanvas* pCanvas,
			PCWSTR text,
			int startX,
			int startY,
			float width,
			float height,
			bool justifyCentered,
			PCWSTR fontName,
			float fontSize,
			int fontWeight,
			PCWSTR localeName,
			struct TextLayoutResult* textLayoutResult
		)
	{
		ID2D1DeviceContext4* pD2D1DeviceContext = NULL;
		IDWriteTextFormat3* pDWriteTextFormat3 = NULL;
		UINT32 len = wcslen(text);
		DWRITE_TEXT_METRICS1 metrics;

		HRESULT hr = pCanvas->RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);

		DWRITE_FONT_WEIGHT weight = DWRITE_FONT_WEIGHT_NORMAL;
		if (fontWeight == 700) {
			weight = DWRITE_FONT_WEIGHT_BOLD;
		}
		else if (fontWeight == 600) {
			weight = DWRITE_FONT_WEIGHT_SEMI_BOLD;
		}
		else if (fontWeight == 500) {
			weight = DWRITE_FONT_WEIGHT_MEDIUM;
		}
		if (SUCCEEDED(hr))
		{
			IDWriteTextFormat* pDWriteTextFormat = NULL;
			hr = pCanvas->Direct2DPointers.DirectWriteFactory->CreateTextFormat(
				fontName,
				NULL,
				weight,
				DWRITE_FONT_STYLE_NORMAL,
				DWRITE_FONT_STRETCH_NORMAL,
				fontSize,
				localeName,
				&pDWriteTextFormat
			);
			pDWriteTextFormat3 = (IDWriteTextFormat3*)pDWriteTextFormat;
		}
		if (SUCCEEDED(hr) && justifyCentered)
		{
			hr = pDWriteTextFormat3->SetTextAlignment(
				DWRITE_TEXT_ALIGNMENT_CENTER
			);
		}
		if (SUCCEEDED(hr))
		{
			textLayoutResult->lineSpacing = fontSize * 1.3f;
			textLayoutResult->baseline = fontSize * 0.8f;
			hr = pDWriteTextFormat3->SetLineSpacing(DWRITE_LINE_SPACING_METHOD_DEFAULT, textLayoutResult->lineSpacing, textLayoutResult->baseline);
		}
		if (SUCCEEDED(hr))
		{
			IDWriteTextLayout* pDWriteTextLayout;
			hr = pCanvas->Direct2DPointers.DirectWriteFactory->CreateTextLayout(
				text,
				len,
				pDWriteTextFormat3,
				width,
				height,
				&pDWriteTextLayout
			);
			textLayoutResult->pDWriteTextLayout = (IDWriteTextLayout4*)pDWriteTextLayout;
		}
		if (SUCCEEDED(hr))
		{
			hr = textLayoutResult->pDWriteTextLayout->GetMetrics(&metrics);
			textLayoutResult->lineCount = metrics.lineCount;
		}
		if (SUCCEEDED(hr))
		{
			if (textLayoutResult->lineCount == 1)
			{
				hr = pDWriteTextFormat3->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);
			}
		}
		if (SUCCEEDED(hr))
		{
			hr = textLayoutResult->pDWriteTextLayout->GetMetrics(&metrics);
			textLayoutResult->lineCount = metrics.lineCount;
			textLayoutResult->top = startY + (double)metrics.top;
			textLayoutResult->height = max(metrics.heightIncludingTrailingWhitespace, 0) > 0 ? (double)metrics.heightIncludingTrailingWhitespace : (double)metrics.height;
			textLayoutResult->left = startX + (double)metrics.left;
			textLayoutResult->width = max(metrics.widthIncludingTrailingWhitespace, 0) > 0 ? (double)metrics.widthIncludingTrailingWhitespace : (double)metrics.width;
		}
		SafeRelease(&pDWriteTextFormat3);
		pD2D1DeviceContext->Release();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawTextLayout(struct Direct2DCanvas* pCanvas, struct TextLayoutResult* textLayoutResult, int startX, int startY, ID2D1SolidColorBrush* pSolidColorBrush)
	{
		ID2D1DeviceContext4* pD2D1DeviceContext = NULL;

		HRESULT hr = pCanvas->RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			pD2D1DeviceContext->DrawTextLayout(
				D2D1::Point2F(startX, startY),
				textLayoutResult->pDWriteTextLayout,
				pSolidColorBrush,
				D2D1_DRAW_TEXT_OPTIONS_ENABLE_COLOR_FONT
			);
			pD2D1DeviceContext->Release();
		}
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseTextLayout(struct TextLayoutResult* textLayoutResult)
	{
		SafeRelease(&textLayoutResult->pDWriteTextLayout);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT SaveImage(struct Direct2DCanvas* pCanvas, PCWSTR filename)
	{

		IWICStream* pWICStream = NULL;
		IWICBitmapEncoder* pWICBitmapEncoder = NULL;
		IWICBitmapFrameEncode* pWICBitmapFrameEncode = NULL;
		WICPixelFormatGUID pixelFormat = GUID_WICPixelFormatDontCare;

		HRESULT hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateStream(&pWICStream);

		if (SUCCEEDED(hr))
		{
			hr = pWICStream->InitializeFromFilename(
				filename,
				GENERIC_WRITE
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateEncoder(
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
			D2D1_SIZE_U renderTargetSize = pCanvas->RenderTarget->GetPixelSize();
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
			hr = pWICBitmapFrameEncode->WriteSource(pCanvas->Bitmap, NULL);
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

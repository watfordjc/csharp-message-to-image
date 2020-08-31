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

		D2D1_FACTORY_OPTIONS factoryOptions;
		//factoryOptions.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

		HRESULT hr = D2D1CreateFactory(
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			//factoryOptions,
			&pD2D1Factory
		);
		if (FAILED(hr))
		{
			return NULL;
		}
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
		if (FAILED(hr))
		{
			return NULL;
		}
		return pWICImagingFactory;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseImagingFactory(IWICImagingFactory* pWICImagingFactory)
	{
		SafeRelease(&pWICImagingFactory);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		IDWriteFactory7* CreateDWriteFactory()
	{
		IDWriteFactory7* pDWriteFactory = NULL;

		HRESULT hr = DWriteCreateFactory(
			DWRITE_FACTORY_TYPE_SHARED,
			__uuidof(IDWriteFactory7),
			reinterpret_cast<IUnknown**>(&pDWriteFactory)
		);
		if (FAILED(hr))
		{
			return NULL;
		}
		return pDWriteFactory;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDWriteFactory(IDWriteFactory7* pDWriteFactory)
	{
		SafeRelease(&pDWriteFactory);
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
		if (FAILED(hr))
		{
			return NULL;
		}
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
		ID2D1RenderTarget* pD2D1RenderTarget = NULL;

		D2D1_RENDER_TARGET_PROPERTIES targetProperties = D2D1::RenderTargetProperties();
		D2D1_PIXEL_FORMAT desiredFormat = D2D1::PixelFormat(
			DXGI_FORMAT_B8G8R8A8_UNORM,
			D2D1_ALPHA_MODE_PREMULTIPLIED
		);
		targetProperties.pixelFormat = desiredFormat;

		HRESULT hr = pD2D1Factory->CreateWicBitmapRenderTarget(
			pWICBitmap,
			targetProperties,
			&pD2D1RenderTarget
		);
		if (FAILED(hr))
		{
			return NULL;
		}
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
		if (FAILED(hr))
		{
			return NULL;
		}
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
		void DrawLine(ID2D1RenderTarget* pD2D1RenderTarget, ID2D1SolidColorBrush* pD2D1SolidColorBrush, int startX, int startY, int stopX, int stopY, float lineWidth)
	{
		pD2D1RenderTarget->DrawLine(
			D2D1::Point2F(startX, startY),
			D2D1::Point2F(stopX, stopY),
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

		D2D1_POINT_2F centerPoint = D2D1::Point2F(
			centerX,
			centerY
		);

		pD2D1RenderTarget->GetFactory(&pD2D1Factory);
		HRESULT hr = pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			hr = pD2D1Factory->CreateEllipseGeometry(
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

		HRESULT hr = pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			hr = pWICImagingFactory->CreateDecoderFromFilename(
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

	struct TextLayoutResult {
		IDWriteTextLayout* pDWriteTextLayout;
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
			IDWriteFactory7* pDWriteFactory,
			ID2D1RenderTarget* pD2D1RenderTarget,
			PCWSTR text,
			int startX,
			int startY,
			int width,
			int height,
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

		HRESULT hr = pD2D1RenderTarget->QueryInterface(
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
			hr = pDWriteFactory->CreateTextFormat(
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
			hr = pDWriteFactory->CreateTextLayout(
				text,
				len,
				pDWriteTextFormat3,
				width,
				height,
				&textLayoutResult->pDWriteTextLayout
			);
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
		HRESULT DrawTextLayout(ID2D1RenderTarget* pD2D1RenderTarget, struct TextLayoutResult* textLayoutResult, int startX, int startY, ID2D1SolidColorBrush* pD2D1SolidColorBrush)
	{
		ID2D1DeviceContext4* pD2D1DeviceContext = NULL;

		HRESULT hr = pD2D1RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			pD2D1DeviceContext->DrawTextLayout(
				D2D1::Point2F(startX, startY),
				textLayoutResult->pDWriteTextLayout,
				pD2D1SolidColorBrush,
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

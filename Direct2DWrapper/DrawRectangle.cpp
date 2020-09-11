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
		// Pointers for Direct2D
		ID2D1Factory1* Direct2DFactory;
		ID2D1Device* Direct2DDevice;
		ID2D1DeviceContext* Direct2DDeviceContext;
		IDWriteFactory7* DirectWriteFactory;
		//IWICImagingFactory* WICImagingFactory;

		// Pointers for Direct3D
		ID3D11Device* Direct3DDevice;
		D3D_FEATURE_LEVEL* Direct3DFeatureLevel;
		ID3D11DeviceContext* Direct3DDeviceContext;

		// Pointers for DXGI
		IDXGIDevice* DXGIDevice;
		IDXGIFactory2* DXGIFactory;
	};

	struct Direct2DCanvas
	{
		IDXGISwapChain1* DXGISwapChain;
		IDXGISurface2* Surface;
		ID2D1Bitmap1* Bitmap;
		ID2D1RenderTarget* RenderTarget;
		struct Direct2DPointers Direct2DPointers;
	};

	struct FontSettings
	{
		float FontSize;
		int FontWeight;
		bool JustifyCentered;
		PCWSTR FontName;
		PCWSTR LocaleName;
	};

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateD2D1Factory(struct Direct2DPointers* pDirect2DPointers)
	{
		D2D1_FACTORY_OPTIONS factoryOptions;
		//factoryOptions.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

		HRESULT hr = D2D1CreateFactory<ID2D1Factory1>(
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
		HRESULT CreateD3D11Device(struct Direct2DPointers* pDirect2DPointers)
	{
		UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

		D3D_FEATURE_LEVEL featureLevels[] =
		{
			D3D_FEATURE_LEVEL_11_1,
			D3D_FEATURE_LEVEL_11_0,
			D3D_FEATURE_LEVEL_10_1,
			D3D_FEATURE_LEVEL_10_0,
			D3D_FEATURE_LEVEL_9_3,
			D3D_FEATURE_LEVEL_9_2,
			D3D_FEATURE_LEVEL_9_1
		};

		HRESULT hr = D3D11CreateDevice(
			nullptr,									// specify null to use the default adapter
			D3D_DRIVER_TYPE_HARDWARE,
			0,
			creationFlags,								// optionally set debug and Direct2D compatibility flags
			featureLevels,								// list of feature levels this app can support
			ARRAYSIZE(featureLevels),					// number of possible feature levels
			D3D11_SDK_VERSION,
			&pDirect2DPointers->Direct3DDevice,         // returns the Direct3D device created
			pDirect2DPointers->Direct3DFeatureLevel,    // returns feature level of device created
			&pDirect2DPointers->Direct3DDeviceContext   // returns the device immediate context
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseD3D11Device(struct Direct2DPointers* pDirect2DPointers)
	{
		SafeRelease(&pDirect2DPointers->Direct3DDeviceContext);
		SafeRelease(&pDirect2DPointers->Direct3DDevice);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDXGIDevice(struct Direct2DPointers* pDirect2DPointers)
	{
		HRESULT hr = pDirect2DPointers->Direct3DDevice->QueryInterface(
			__uuidof(IDXGIDevice),
			reinterpret_cast<void**>(&pDirect2DPointers->DXGIDevice)
		);
		if (SUCCEEDED(hr))
		{
			hr = CreateDXGIFactory2(
				DXGI_CREATE_FACTORY_DEBUG,
				__uuidof(IDXGIFactory2),
				reinterpret_cast<void**>(&pDirect2DPointers->DXGIFactory)
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pDirect2DPointers->Direct2DFactory->CreateDevice(
				pDirect2DPointers->DXGIDevice,
				&pDirect2DPointers->Direct2DDevice
			);
		}
		if (SUCCEEDED(hr))
		{
			hr = pDirect2DPointers->Direct2DDevice->CreateDeviceContext(
				D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
				&pDirect2DPointers->Direct2DDeviceContext
			);
		}
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDXGIDevice(struct Direct2DPointers* pDirect2DPointers)
	{
		SafeRelease(&pDirect2DPointers->Direct2DDeviceContext);
		SafeRelease(&pDirect2DPointers->Direct2DDevice);
		pDirect2DPointers->DXGIFactory->Release();
		pDirect2DPointers->DXGIDevice->Release();
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT CreateDXGISwapChain(struct Direct2DPointers* pDirect2DPointers, UINT width, UINT height, struct Direct2DCanvas* pCanvas)
	{
		pCanvas->Direct2DPointers = *pDirect2DPointers;

		DXGI_SWAP_CHAIN_DESC1 description = {};
		description.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
		description.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		description.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
		description.BufferCount = 2;
		description.SampleDesc.Count = 1;
		description.AlphaMode = DXGI_ALPHA_MODE_PREMULTIPLIED;
		description.Width = width;
		description.Height = height;

		HRESULT hr = pDirect2DPointers->DXGIFactory->CreateSwapChainForComposition(
			pDirect2DPointers->DXGIDevice,
			&description,
			nullptr,
			&pCanvas->DXGISwapChain
		);
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseDXGISwapChain(struct Direct2DCanvas* pCanvas)
	{
		SafeRelease(&pCanvas->DXGISwapChain);
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
		HRESULT CreateRenderTarget(struct Direct2DCanvas* pCanvas)
	{
		D2D1_RENDER_TARGET_PROPERTIES targetProperties = D2D1::RenderTargetProperties();
		D2D1_PIXEL_FORMAT desiredFormat = D2D1::PixelFormat(
			DXGI_FORMAT_B8G8R8A8_UNORM,
			D2D1_ALPHA_MODE_PREMULTIPLIED
		);
		targetProperties.pixelFormat = desiredFormat;

		D2D1_BITMAP_PROPERTIES1 properties = {};
		properties.pixelFormat.alphaMode = D2D1_ALPHA_MODE_PREMULTIPLIED;
		properties.pixelFormat.format = DXGI_FORMAT_B8G8R8A8_UNORM;
		properties.bitmapOptions =
			D2D1_BITMAP_OPTIONS_TARGET |
			D2D1_BITMAP_OPTIONS_CANNOT_DRAW;

		HRESULT hr = pCanvas->DXGISwapChain->GetBuffer(
			0, // index
			__uuidof(IDXGISurface2),
			reinterpret_cast<void**>(&pCanvas->Surface)
		);
		if (SUCCEEDED(hr))
		{
			pCanvas->Direct2DPointers.Direct2DDeviceContext->CreateBitmapFromDxgiSurface(
				pCanvas->Surface,
				properties,
				&pCanvas->Bitmap
			);
		}
		if (SUCCEEDED(hr))
		{
			pCanvas->Direct2DPointers.Direct2DFactory->CreateDxgiSurfaceRenderTarget(
				pCanvas->Surface,
				targetProperties,
				&pCanvas->RenderTarget
			);
		}
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void ReleaseRenderTarget(struct Direct2DCanvas* pCanvas)
	{
		SafeRelease(&pCanvas->RenderTarget);
		SafeRelease(&pCanvas->Bitmap);
		pCanvas->Surface->Release();
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
		void DrawLine(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, D2D1_POINT_2F HeadingSeparatorPoint1, D2D1_POINT_2F HeadingSeparatorPoint2, float lineThickness)
	{
		pCanvas->RenderTarget->DrawLine(
			HeadingSeparatorPoint1,
			HeadingSeparatorPoint2,
			pD2D1SolidColorBrush,
			lineThickness
		);
	}

	DIRECT2DWRAPPER_C_FUNCTION
		void DrawRectangle(struct Direct2DCanvas* pCanvas, ID2D1SolidColorBrush* pD2D1SolidColorBrush, D2D1_RECT_F bounds)
	{
		pCanvas->RenderTarget->FillRectangle(
			bounds,
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
		HRESULT DrawImageFromFilename(struct Direct2DCanvas* pCanvas, PCWSTR filename, D2D1_POINT_2F originPoint, D2D1_RECT_F bounds)
	{
		//IWICBitmapDecoder* pWICBitmapDecoder = NULL;
		//IWICBitmapFrameDecode* pWICBitmapFrameDecode = NULL;
		//IWICFormatConverter* pWICFormatConverter = NULL;
		//ID2D1Bitmap* pD2D1Bitmap = NULL;
		//ID2D1Effect* pBitmapSourceEffect = NULL;
		//ID2D1DeviceContext* pD2D1DeviceContext = NULL;

		//HRESULT hr = pCanvas->RenderTarget->QueryInterface(
		//	__uuidof(ID2D1DeviceContext),
		//	reinterpret_cast<void**>(&pD2D1DeviceContext)
		//);
		//if (SUCCEEDED(hr))
		//{
		//	hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateDecoderFromFilename(
		//		filename,
		//		NULL,
		//		GENERIC_READ,
		//		WICDecodeMetadataCacheOnLoad,
		//		&pWICBitmapDecoder
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pWICBitmapDecoder->GetFrame(0, &pWICBitmapFrameDecode);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pCanvas->Direct2DPointers.WICImagingFactory->CreateFormatConverter(&pWICFormatConverter);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pWICFormatConverter->Initialize(
		//		pWICBitmapFrameDecode,
		//		GUID_WICPixelFormat32bppPBGRA,
		//		WICBitmapDitherTypeNone,
		//		NULL,
		//		0.f,
		//		WICBitmapPaletteTypeMedianCut
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pD2D1DeviceContext->CreateBitmapFromWicBitmap(
		//		pWICFormatConverter,
		//		NULL,
		//		&pD2D1Bitmap
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pD2D1DeviceContext->CreateEffect(
		//		CLSID_D2D1BitmapSource,
		//		&pBitmapSourceEffect
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pBitmapSourceEffect->SetValue(
		//		D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
		//		pWICFormatConverter
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	D2D1_SIZE_F originalSize = pD2D1Bitmap->GetSize();
		//	D2D1_VECTOR_2F resizeVector = D2D1::Vector2F(
		//		bounds.right / originalSize.width,
		//		bounds.bottom / originalSize.height
		//	);
		//	hr = pBitmapSourceEffect->SetValue(
		//		D2D1_BITMAPSOURCE_PROP_SCALE,
		//		resizeVector
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	hr = pBitmapSourceEffect->SetValue(
		//		D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
		//		D2D1_BITMAPSOURCE_INTERPOLATION_MODE_CUBIC
		//	);
		//}
		//if (SUCCEEDED(hr))
		//{
		//	pD2D1DeviceContext->DrawImage(
		//		pBitmapSourceEffect,
		//		originPoint,
		//		bounds,
		//		D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC,
		//		D2D1_COMPOSITE_MODE_SOURCE_OVER
		//	);
		//}
		//SafeRelease(&pBitmapSourceEffect);
		//SafeRelease(&pD2D1Bitmap);
		//SafeRelease(&pWICFormatConverter);
		//SafeRelease(&pWICBitmapFrameDecode);
		//SafeRelease(&pWICBitmapDecoder);
		//pD2D1DeviceContext->Release();
		//return hr;
		return 0;
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
			D2D1_RECT_F bounds,
			struct FontSettings* fontSettings,
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
		if (fontSettings->FontWeight == 700) {
			weight = DWRITE_FONT_WEIGHT_BOLD;
		}
		else if (fontSettings->FontWeight == 600) {
			weight = DWRITE_FONT_WEIGHT_SEMI_BOLD;
		}
		else if (fontSettings->FontWeight == 500) {
			weight = DWRITE_FONT_WEIGHT_MEDIUM;
		}
		if (SUCCEEDED(hr))
		{
			IDWriteTextFormat* pDWriteTextFormat = NULL;
			hr = pCanvas->Direct2DPointers.DirectWriteFactory->CreateTextFormat(
				fontSettings->FontName,
				NULL,
				weight,
				DWRITE_FONT_STYLE_NORMAL,
				DWRITE_FONT_STRETCH_NORMAL,
				fontSettings->FontSize,
				fontSettings->LocaleName,
				&pDWriteTextFormat
			);
			pDWriteTextFormat3 = (IDWriteTextFormat3*)pDWriteTextFormat;
		}
		if (SUCCEEDED(hr) && fontSettings->JustifyCentered)
		{
			hr = pDWriteTextFormat3->SetTextAlignment(
				DWRITE_TEXT_ALIGNMENT_CENTER
			);
		}
		if (SUCCEEDED(hr))
		{
			textLayoutResult->lineSpacing = fontSettings->FontSize * 1.3f;
			textLayoutResult->baseline = fontSettings->FontSize * 0.8f;
			hr = pDWriteTextFormat3->SetLineSpacing(DWRITE_LINE_SPACING_METHOD_DEFAULT, textLayoutResult->lineSpacing, textLayoutResult->baseline);
		}
		if (SUCCEEDED(hr))
		{
			IDWriteTextLayout* pDWriteTextLayout;
			hr = pCanvas->Direct2DPointers.DirectWriteFactory->CreateTextLayout(
				text,
				len,
				pDWriteTextFormat3,
				bounds.right,
				bounds.bottom,
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
			textLayoutResult->top = (double)metrics.top;
			textLayoutResult->height = max(metrics.heightIncludingTrailingWhitespace, 0) > 0 ? (double)metrics.heightIncludingTrailingWhitespace : (double)metrics.height;
			textLayoutResult->left = (double)metrics.left;
			textLayoutResult->width = max(metrics.widthIncludingTrailingWhitespace, 0) > 0 ? (double)metrics.widthIncludingTrailingWhitespace : (double)metrics.width;
		}
		SafeRelease(&pDWriteTextFormat3);
		pD2D1DeviceContext->Release();
		return hr;
	}

	DIRECT2DWRAPPER_C_FUNCTION
		HRESULT DrawTextLayout(struct Direct2DCanvas* pCanvas, struct TextLayoutResult* textLayoutResult, D2D1_POINT_2F originPoint, ID2D1SolidColorBrush* pD2D1SolidColorBrush)
	{
		ID2D1DeviceContext4* pD2D1DeviceContext = NULL;

		HRESULT hr = pCanvas->RenderTarget->QueryInterface(
			__uuidof(ID2D1DeviceContext),
			reinterpret_cast<void**>(&pD2D1DeviceContext)
		);
		if (SUCCEEDED(hr))
		{
			pD2D1DeviceContext->DrawTextLayout(
				originPoint,
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
		HRESULT SaveImage(struct Direct2DCanvas* pCanvas, PCWSTR filename)
	{
		WICPixelFormatGUID pixelFormat = GUID_WICPixelFormat32bppBGRA;

		ID3D11Texture2D* backBuffer;
		HRESULT hr = pCanvas->DXGISwapChain->GetBuffer(
			0,
			__uuidof(ID3D11Texture2D),
			reinterpret_cast<void**>(&backBuffer)
		);
		if (SUCCEEDED(hr))
		{
			using namespace DirectX;
			hr = SaveWICTextureToFile(
				pCanvas->Direct2DPointers.Direct3DDeviceContext,
				backBuffer,
				GUID_ContainerFormatPng,
				filename,
				&pixelFormat,
				nullptr,
				true
			);
		}
		backBuffer->Release();
		return hr;
	}

} // namespace Direct2DWrapper

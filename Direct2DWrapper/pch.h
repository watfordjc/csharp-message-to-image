// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"

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
#include <d2d1_3.h>
#include <d2d1_3helper.h>
#include <d2d1effects.h>
#include <d2d1effecthelpers.h>
#pragma comment(lib, "dxguid.lib")
#include <dcommon.h>
#include <dwrite.h>
#pragma comment(lib, "dwrite.lib")
#include <dwrite_1.h>
#include <dwrite_2.h>
#include <dwrite_3.h>
#include <wincodec.h>
// Direct3D
#include <dxgi1_3.h>
#pragma comment(lib, "dxgi.lib")
#include <d3d11_2.h>
#pragma comment(lib, "d3d11.lib")
#include <dcomp.h>
#pragma comment(lib, "dcomp.lib")
// DirectX Tool Kit
#include "ScreenGrab11.h"


// From https://github.com/Microsoft/DirectXTK/wiki/ThrowIfFailed
#include <stdio.h>
#include <exception>

namespace DX
{
    // Helper class for COM exceptions
    class com_exception : public std::exception
    {
    public:
        com_exception(HRESULT hr) : result(hr) {}

        const char* what() const override
        {
            static char s_str[64] = {};
            sprintf_s(s_str, "Failure with HRESULT of %08X",
                static_cast<unsigned int>(result));
            return s_str;
        }

    private:
        HRESULT result;
    };

    // Helper utility converts D3D API failures into exceptions.
    inline void ThrowIfFailed(HRESULT hr)
    {
        if (FAILED(hr))
        {
            throw com_exception(hr);
        }
    }
}
// https://github.com/Microsoft/DirectXTK/wiki/ThrowIfFailed

#endif //PCH_H

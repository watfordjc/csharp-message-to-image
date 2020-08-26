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

DIRECT2DWRAPPER_C_FUNCTION
double Add(int a, double b);

# csharp-message-to-image-library

csharp-message-to-image-library is a test/sample C# solution with an ordinary (non-MFC) C++ DLL project.

The purpose of this solution is to work out how to create a C++ DLL and utilise it in C#.

This repository is an off-shoot of [csharp-stream-controller](https://github.com/watfordjc/csharp-stream-controller)

## Direct2DWrapper

This project is a non-MFC C++ DLL project.

It exports a limited number of functions that are abstractions of some Direct2D and DirectWrite functions.

## MessageToImageLibrary

This is the main C# WPF .NET Core 3.1 project.

It is (now) a C# wrapper around **Direct2DWrapper** providing a limited set of features so that a message (such as a Tweet) can be transformed into a PNG file.

In order to use the library, the **Direct2DWrapper** project will need building to generate Direct2DWrapper.dll and a link file created in **MessageToImageLibrary\lib** pointing at the Direct2DWrapper.dll file.

### Direct2DWrapper.dll

The C# project needs to be able to find Direct2DWrapper.dll.

1. In Visual Studio 2019, choose **Debug** or **Release** for Solution Configurations and **x64** or **x86** for Solution Platforms.
2. Right-click the **Direct2DWrapper** project and click **Build**.
3. Right-click the **lib** folder in the **MessageToImageLibrary** project.
4. In the context menu: **Add**, **Existing Item&hellip;**
5. Change the file types drop down to **Executable Files** (or all files, you're looking for a .dll)
6. Browse to the root directory for the solution - if you're in the directory **csharp-message-to-image-library\MessageToImageLibrary**, you need to go up a directory.
7. Find the DLL you built in step 1. If you chose Debug and x64, it will likely be in **x64\Debug**. Select the **Direct2DWrapper.dll** file.
8. Click the arrow next to the **Add** button, and click **Add as Link**.
9. Right-click the link file **lib\Direct2DWrapper.dll** and click **Properties**.
10. Change **Copy to Output Directory** to **Copy if newer**.
11. Run the **MessageToImageLibrary** project. If something is still wrong, you'll get a DllNotFoundException.

### Expected Output

**MessageToImageLibrary** exports a generic ```Add(int a, double b)``` function found in **Direct2DWrapper**.

If the projects are built correctly, the Visual Studio debug **Output** window should include the following output on instantiation of a C# ```MessageToImageLibrary.Direct2DWrapper``` instance:

```
Result of external Add(10, 3) = 13.
```
 

```csharp
class Test()
{
    private readonly MessageToImageLibrary.Direct2DWrapper d2dWrapper;

    Test()
    {
        Trace.WriteLine("Testing DLL...");
        d2dWrapper = new MessageToImageLibrary.Direct2DWrapper();
    }
}
```

The expected output listed above may be out of date as documentation changes typically lag code changes. Check recent commits for changes.
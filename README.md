# csharp-text-formatter

csharp-text-formatter is a test/sample C# solution with an ordinary (non-MFC) C++ DLL project.

The purpose of this solution is to work out how to create a C++ DLL and utilise it in C#.

This repository is an off-shoot of [csharp-stream-controller](https://github.com/watfordjc/csharp-stream-controller)

## Direct2DWrapper

This project is a non-MFC C++ DLL project.

At the time of writing it has one exported function:

```cpp
double Add(int a, double b);
```

## TextFormatter

This is the main C# WPF .NET Core 3.1 project.

At the time of writing, it has the following code inside the ```MainWindow.xaml.cs``` ```MainWindow()``` method:

```csharp
int a = 10;
double b = 3.0;
double result = Interop.UnsafeNativeMethods.Add(a, b);
Trace.WriteLine($"Result of external Add({a}, {b}) = {result}.");
```

In order to run the project, the **Direct2DWrapper** project will need building to generate Direct2DWrapper.dll and a link file created in **TextFormatter\lib** pointing at the Direct2DWrapper.dll file.

### Direct2DWrapper.dll

In order for the C# project to successfully run, it needs to be able to find Direct2DWrapper.dll.

1. In Visual Studio 2019, choose **Debug** or **Release** for Solution Configurations and **x64** or **x86** for Solution Platforms.
2. Right-click the **Direct2DWrapper** project and click **Build**.
3. Right-click the **lib** folder in the **TextFormatter** project.
4. In the context menu: **Add**, **Existing Item&hellip;**
5. Change the file types drop down to **Executable Files** (or all files, you're looking for a .dll)
6. Browse to the root directory for the solution - if you're in the directory **csharp-text-formatter\TextFormatter**, you need to go up a directory.
7. Find the DLL you built in step 1. If you chose Debug and x64, it will likely be in **x64\Debug**. Select the **Direct2DWrapper.dll** file.
8. Click the arrow next to the **Add** button, and click **Add as Link**.
9. Right-click the link file **lib\Direct2DWrapper.dll** and click **Properties**.
10. Change **Copy to Output Directory** to **Copy if newer**.
11. Run the **TextFormatter** project. If something is still wrong, you'll get a DllNotFoundException.

### Expected Output

If the projects are built correctly, the Visual Studio debug **Output** window should include the following output:

```
Result of external Add(10, 3) = 13.
```

The expected output listed above may be out of date as documentation changes typically lag code changes. Check recent commits for changes.
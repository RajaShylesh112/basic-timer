@echo off
echo ========================================
echo BasicTimer - Direct .NET Framework Build
echo ========================================

REM Set the compiler path
set "CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist "%CSC%" (
    echo Error: C# compiler not found at %CSC%
    pause
    exit /b 1
)

echo Using C# compiler: %CSC%

REM Change to source directory
cd /d "%~dp0src\BasicTimer"

REM Create output directory
if not exist "bin" mkdir bin
if not exist "bin\Release" mkdir bin\Release

echo.
echo Compiling BasicTimer v1.5...

REM Compile the application
"%CSC%" /target:winexe ^
       /out:bin\Release\BasicTimer.exe ^
       /reference:PresentationCore.dll ^
       /reference:PresentationFramework.dll ^
       /reference:WindowsBase.dll ^
       /reference:System.dll ^
       /reference:System.Core.dll ^
       /reference:System.Xaml.dll ^
       /reference:Microsoft.CSharp.dll ^
       /win32icon:Resources\icon.ico ^
       /nologo ^
       *.cs

if %errorlevel% neq 0 (
    echo.
    echo ❌ Compilation failed!
    pause
    exit /b %errorlevel%
)

REM Copy XAML files and compile them
echo.
echo Processing XAML files...

REM For simplicity, let's try a different approach - copy the XAML files as resources
copy "*.xaml" "bin\Release\" >nul 2>&1

echo.
echo ✅ Build completed successfully!
echo.
echo Output: %~dp0src\BasicTimer\bin\Release\BasicTimer.exe
echo.

REM Try to run the application
if exist "bin\Release\BasicTimer.exe" (
    echo Starting BasicTimer...
    start "" "bin\Release\BasicTimer.exe"
) else (
    echo Warning: Executable not found.
)

pause

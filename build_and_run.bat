@echo off
echo ====================================
echo BasicTimer Build Environment Check
echo ====================================

echo Checking for .NET Framework build tools...

REM Check for various MSBuild locations
set "FOUND_MSBUILD="

REM VS 2022
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Visual Studio 2022 Community
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Visual Studio 2022 Professional
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Visual Studio 2022 Enterprise
)

REM VS 2019
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Visual Studio 2019 Community
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Visual Studio 2019 Professional
)

REM Build Tools for VS
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Build Tools for Visual Studio 2019
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "FOUND_MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    echo Found: Build Tools for Visual Studio 2022
)

REM Older MSBuild
if exist "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
    if "%FOUND_MSBUILD%"=="" (
        set "FOUND_MSBUILD=C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
        echo Found: MSBuild 14.0 ^(VS 2015^)
    )
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" (
    if "%FOUND_MSBUILD%"=="" (
        set "FOUND_MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
        echo Found: Visual Studio 2017 Professional
    )
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
    if "%FOUND_MSBUILD%"=="" (
        set "FOUND_MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
        echo Found: Visual Studio 2017 Community
    )
)

echo.
if "%FOUND_MSBUILD%"=="" (
    echo ❌ No MSBuild found!
    echo.
    echo Please install one of the following:
    echo - Visual Studio 2019/2022 ^(Community, Professional, or Enterprise^)
    echo - Build Tools for Visual Studio 2019/2022
    echo - .NET Framework SDK
    echo.
    echo Download from: https://visualstudio.microsoft.com/downloads/
    pause
    exit /b 1
) else (
    echo ✅ MSBuild found at:
    echo %FOUND_MSBUILD%
    echo.
    echo Building BasicTimer...
    
    cd /d "%~dp0src"
    "%FOUND_MSBUILD%" BasicTimer.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal
    
    if %errorlevel% neq 0 (
        echo.
        echo ❌ Build failed!
        pause
        exit /b %errorlevel%
    ) else (
        echo.
        echo ✅ Build completed successfully!
        echo.
        echo Executable location:
        echo %~dp0src\BasicTimer\bin\Release\net461\BasicTimer.exe
        echo.
        
        REM Try to run the built application
        if exist "%~dp0src\BasicTimer\bin\Release\net461\BasicTimer.exe" (
            echo Starting BasicTimer...
            start "" "%~dp0src\BasicTimer\bin\Release\net461\BasicTimer.exe"
        ) else (
            echo Warning: Executable not found at expected location.
        )
    )
)

pause

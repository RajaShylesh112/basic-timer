@echo off
echo ==========================================
echo BasicTimer - Auto .NET SDK Installation
echo ==========================================

REM Check if dotnet is already available
dotnet --version >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ .NET SDK already installed!
    goto :build
)

echo .NET SDK not found. Installing...

REM Create temp directory
set "TEMP_DIR=%TEMP%\dotnet-install"
if not exist "%TEMP_DIR%" mkdir "%TEMP_DIR%"

echo Downloading .NET 6.0 SDK installer...

REM Download the .NET installer script
powershell -Command "Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile '%TEMP_DIR%\dotnet-install.ps1'"

if not exist "%TEMP_DIR%\dotnet-install.ps1" (
    echo ❌ Failed to download .NET installer
    pause
    exit /b 1
)

echo Installing .NET SDK to user profile...

REM Install .NET SDK to user profile (no admin required)
powershell -ExecutionPolicy Bypass -File "%TEMP_DIR%\dotnet-install.ps1" -Channel 6.0 -InstallDir "%USERPROFILE%\.dotnet"

REM Add to PATH for current session
set "PATH=%USERPROFILE%\.dotnet;%PATH%"

REM Verify installation
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET installation failed
    echo.
    echo Alternative: Download .NET SDK manually from:
    echo https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET SDK installed successfully!

:build
echo.
echo ==========================================
echo Building BasicTimer with .NET SDK
echo ==========================================

REM Change to project directory
cd /d "%~dp0src"

REM Check if we have a solution file
if not exist "BasicTimer.sln" (
    echo ❌ Solution file not found
    pause
    exit /b 1
)

echo Building BasicTimer...

REM Build the project
dotnet build BasicTimer.sln --configuration Release --verbosity minimal

if %errorlevel% neq 0 (
    echo ❌ Build failed!
    pause
    exit /b %errorlevel%
)

echo ✅ Build completed successfully!
echo.

REM Find and display the executable location
for /r %%i in (BasicTimer.exe) do (
    if exist "%%i" (
        echo Executable: %%i
        echo.
        echo Starting BasicTimer...
        start "" "%%i"
        goto :done
    )
)

echo ❌ Executable not found in expected location

:done
pause

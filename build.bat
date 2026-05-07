@echo off
REM Build script for BasicTimer

REM Try to find MSBuild
set "MSBUILD_PATH="

if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
) else (
    echo MSBuild not found. Trying dotnet build...
    cd /d "%~dp0src"
    dotnet build BasicTimer.sln --configuration Release
    if %errorlevel% neq 0 (
        echo Build failed! Please install Visual Studio or Build Tools.
        pause
        exit /b %errorlevel%
    ) else (
        echo Build completed successfully with dotnet!
        echo Executable is at: %~dp0src\BasicTimer\bin\Release\net461\BasicTimer.exe
        pause
        exit /b 0
    )
)

echo Using MSBuild at: %MSBUILD_PATH%

REM Change to project directory
cd /d "%~dp0src"

REM Build the solution
"%MSBUILD_PATH%" BasicTimer.sln /p:Configuration=Release /p:Platform="Any CPU"

if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b %errorlevel%
)

echo Build completed successfully!
echo Executable is at: %~dp0src\BasicTimer\bin\Release\net461\BasicTimer.exe

pause

@echo off
echo ========================================
echo Starting Frontend Only
echo ========================================
echo.

cd "Electro\electro-frontend"

if not exist node_modules (
    echo.
    echo Installing dependencies first...
    echo This may take a few minutes...
    echo.
    call npm install
    if errorlevel 1 (
        echo.
        echo ERROR: Failed to install dependencies!
        echo Please check your internet connection and try again.
        pause
        exit /b 1
    )
    echo.
    echo Dependencies installed successfully!
    echo.
)

if not exist .env.local (
    echo Creating .env.local file...
    echo NEXT_PUBLIC_API_URL=http://localhost:5008/api > .env.local
    echo .env.local created!
    echo.
)

echo ========================================
echo Starting Frontend Server...
echo ========================================
echo.
echo Frontend will run on: http://localhost:3000
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

call npm run dev

pause


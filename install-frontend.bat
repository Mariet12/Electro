@echo off
echo ========================================
echo Installing Frontend Dependencies
echo ========================================
echo.

cd "Electro\electro-frontend"

if exist node_modules (
    echo Dependencies already installed!
    echo.
) else (
    echo Installing npm packages...
    echo This may take a few minutes...
    echo.
    call npm install
    if errorlevel 1 (
        echo.
        echo ERROR: Failed to install dependencies!
        pause
        exit /b 1
    )
    echo.
    echo Installation complete!
    echo.
)

echo ========================================
echo Creating .env.local file if needed...
echo ========================================
if not exist .env.local (
    echo NEXT_PUBLIC_API_URL=http://localhost:5008/api > .env.local
    echo Created .env.local file
) else (
    echo .env.local already exists
)

echo.
echo ========================================
echo Setup Complete!
echo ========================================
echo.
echo Now you can run: start-frontend.bat
echo.
pause


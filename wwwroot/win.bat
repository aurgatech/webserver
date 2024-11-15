set BAT_FILE_PATH=%~dp0

cd %BAT_FILE_PATH%

call npm install
call npx vite build

if exist "..\app\wwwroot" rmdir /S /Q "..\app\wwwroot"
mkdir "..\app\wwwroot" 
robocopy dist "..\app\wwwroot" /E
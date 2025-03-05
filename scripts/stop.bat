@echo off
REM Остановка Nginx
taskkill /IM nginx.exe /F

REM Остановка процессов веб-приложения Valuator
taskkill /IM dotnet.exe /F

echo Success
pause
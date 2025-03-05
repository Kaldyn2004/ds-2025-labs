@echo off
REM Запуск экземпляров веб-приложения Valuator

REM Запуск первого экземпляра на порту 5001
start "" cmd /k "cd /d %~dp0\..\Valuator && dotnet run --urls http://localhost:5001"

REM Запуск второго экземпляра на порту 5002
start "" cmd /k "cd /d %~dp0\..\Valuator && dotnet run --urls http://localhost:5002"

REM Запуск Nginx
cd /d %~dp0\..\nginx
start nginx.exe

echo Система запущена
pause
@echo off
REM Запуск экземпляров веб-приложения Valuator и RankCalculator

REM Запуск первого экземпляра Valuator на порту 5001
start "Valuator 5001" cmd /c "cd /d %~dp0\..\Valuator && dotnet run --urls http://localhost:5001 && pause"
start "Valuator 5002" cmd /c "cd /d %~dp0\..\Valuator && dotnet run --urls http://localhost:5002 && pause"

timeout /t 5 >nul

REM Запуск первого экземпляра RankCalculator
start "RankCalculator 1" cmd /c "cd /d %~dp0\..\RankCalculator && dotnet run && pause"
start "RankCalculator 2" cmd /c "cd /d %~dp0\..\RankCalculator && dotnet run && pause"

timeout /t 5 >nul

REM Запуск Nginx
cd /d %~dp0\..\nginx
start nginx.exe

echo Done
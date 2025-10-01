@echo off

start "WebAPI" dotnet watch run --project ./WebAPI/WebAPI.csproj --urls "https://localhost:7096;http://localhost:5096"

start "Frontend" npm run dev --prefix ./frontend/
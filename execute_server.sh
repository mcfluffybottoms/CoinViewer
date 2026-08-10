#!/bin/bash

if [ -f "server" ]; then
    echo "Запуск скомпилированного сервера..."
    ./server
elif [ -f "server.go" ]; then
    echo "Запуск Go сервера..."
    go run server.go
elif [ -f "server.py" ]; then
    echo "Запуск Python сервера..."
    python3 server.py
elif [ -f "server.js" ]; then
    echo "Запуск JavaScript сервера..."
    node server.js
elif [ -f "server.class" ]; then
    echo "Запуск Java сервера..."
    java server
elif [ -f "server.cs" ]; then
    echo "Запуск С# клиента..."
    dotnet run --no-build server.cs
else
    echo "Не найден файл сервера для запуска"
    exit 1
fi
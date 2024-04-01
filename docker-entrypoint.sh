#!/usr/bin/env bash
# Install certificate
./certificate-tool add --file $1 --password $2
# Then you can run the app
dotnet ./ConsoleApp.dll
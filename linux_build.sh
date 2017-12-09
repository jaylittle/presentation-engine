#!/bin/bash
dotnet restore
if [ -f pengine_current.tgz ]
then
  rm pengine_current.tgz
fi
cd PEngine.Core.Tests
dotnet clean
dotnet test
cd ../PEngine.Core.Web
dotnet clean
dotnet publish -c Release
cd ..
tar -C PEngine.Core.Web/bin/Release/netcoreapp2.0/publish -czvf pengine_current.tgz ./

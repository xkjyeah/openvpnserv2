#!/bin/sh

xbuild /p:Configuration=Release "/p:Platform=Any CPU" OpenVpnService.sln 
xbuild /p:Configuration=Release "/p:Platform=x86" OpenVpnService.sln 
xbuild /p:Configuration=Release "/p:Platform=x64" OpenVpnService.sln 

xbuild /p:Configuration=Debug "/p:Platform=Any CPU" OpenVpnService.sln 
xbuild /p:Configuration=Debug "/p:Platform=x86" OpenVpnService.sln 
xbuild /p:Configuration=Debug "/p:Platform=x64" OpenVpnService.sln 

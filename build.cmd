msbuild /p:Configuration=Release "/p:Platform=x86"
msbuild /p:Configuration=Release "/p:Platform=x64"
msbuild /p:Configuration=Release "/p:Platform=Any CPU"

msbuild /p:Configuration=Debug "/p:Platform=x86"
msbuild /p:Configuration=Debug "/p:Platform=x64"
msbuild /p:Configuration=Debug "/p:Platform=Any CPU"


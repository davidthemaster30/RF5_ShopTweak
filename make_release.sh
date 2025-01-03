rm -rf obj
rm -rf bin

dotnet build RF5_ShopTweak.csproj -f net6.0 -c Release

zip -j 'RF5_ShopTweak_v1.3.0.zip' './bin/Release/net6.0/RF5_ShopTweak.dll'
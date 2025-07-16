rm -rf obj
rm -rf bin

dotnet build RF5_ShopTweak.csproj -f net6.0 -c Release

mkdir -p BepInEx/plugins
mkdir -p BepInEx/config

cp './bin/Release/net6.0/RF5_ShopTweak.dll' BepInEx/plugins/RF5_ShopTweak.dll
cp './bin/Release/net6.0/RF5_ShopTweak.ini' BepInEx/plugins/RF5_ShopTweak.ini

zip -r 'RF5_ShopTweak_v1.3.0.zip' BepInEx

rm -rf BepInEx

cp './bin/Release/net6.0/RF5_ShopTweak.dll' '/data/Steam/steamapps/common/Rune Factory 5/BepInEx/plugins'
cp './bin/Release/net6.0/RF5_ShopTweak.ini' '/data/Steam/steamapps/common/Rune Factory 5/BepInEx/plugins'
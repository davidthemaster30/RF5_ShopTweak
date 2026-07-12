rm -rf obj
rm -rf bin

CSPROJ_PATH="RF5_ShopTweak.csproj"

dotnet build $CSPROJ_PATH -f net6.0 -c Release

VERSION=$(grep -oP '(?<=<Version>)[^<]+' "$CSPROJ_PATH" || true)
PROJECTNAME=$(grep -oP '(?<=<AssemblyName>)[^<]+' "$CSPROJ_PATH" || true)
ZIP_NAME="${PROJECTNAME}_v${VERSION}.zip"

mkdir -p BepInEx/plugins
mkdir -p BepInEx/config

cp './bin/Release/net6.0/RF5_ShopTweak.dll' BepInEx/plugins/RF5_ShopTweak.dll
cp './bin/Release/net6.0/RF5_ShopTweak.ini' BepInEx/plugins/RF5_ShopTweak.ini

zip -j "${ZIP_NAME}" BepInEx

rm -rf BepInEx

git tag "v${VERSION}"
git push origin "v${VERSION}"
 
gh release create "v${VERSION}" "${ZIP_NAME}" \
  --title "v${VERSION}" \
  --generate-notes
  
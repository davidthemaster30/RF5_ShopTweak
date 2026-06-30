using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace RF5_ShopTweak;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess(GAME_PROCESS)]
public class ShopTweakPlugin : BasePlugin
{
	internal static List<CustomShop> Shops { get; private set; } = new();
	internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("ShopTweak");
	private const string GAME_PROCESS = "Rune Factory 5.exe";
	private static string FILENAME = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + MyPluginInfo.PLUGIN_NAME + ".ini";

	public override void Load()
	{
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loading!");

		Shops = IniParser.ParseFile(FILENAME);

		foreach (var shop in Shops)
		{
			Log.LogInfo(shop.ToString());
		}

		new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();

		//Harmony.CreateAndPatchAll(typeof(ShopDataTableHandler));
		Harmony.CreateAndPatchAll(typeof(ShopDataTableChangePage));
		//Harmony.CreateAndPatchAll(typeof(UIShopControllerChangePage));
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loaded!");
	}
}

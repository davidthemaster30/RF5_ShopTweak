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

	static public new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("ShopTweak");
	private const string GAME_PROCESS = "Rune Factory 5.exe";
	private static string FILENAME = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + MyPluginInfo.PLUGIN_NAME + ".ini";

	public override void Load()
	{
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loading!");

		Shops = IniParser.ParseFile(FILENAME);

		foreach (var shop in Shops)
		{
			Log.LogInfo($"Shop {shop.ShopType.ToString()}");

			foreach (var page in shop.Pages)
			{
				Log.LogInfo($"Page {page.Name}");
				foreach (var item in page.Items)
				{
					Log.LogInfo($"ItemsToAdd {item.Id}:{item.Level}");
				}
			}
		}

		new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
		Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loaded!");
	}
}

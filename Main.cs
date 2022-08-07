using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using RF5SHOP;

namespace RF5_ShopTweak
{
	[BepInPlugin(GUID, NAME, VERSION)]
	[BepInProcess(GAME_PROCESS)]
	public class Main : BasePlugin
	{
		#region PluginInfo
		private const string GUID = "A465AF6E-9709-261F-EDCA-7E777C4AD8C1";
		private const string NAME = "RF5_ShopTweak";
		private const string VERSION = "1.1";
		private const string GAME_PROCESS = "Rune Factory 5.exe";
		#endregion

		static public new ManualLogSource Log;
		private static string FILENAME = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + NAME + ".ini";
		public static new IniParser Config;

		public override void Load()
		{
			Log = base.Log;
			Config = new IniParser(FILENAME);
			new Harmony(GUID).PatchAll();
		}
	}
}

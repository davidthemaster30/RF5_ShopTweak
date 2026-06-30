using System.Text.RegularExpressions;
using RF5SHOP;

namespace RF5_ShopTweak;
#pragma warning disable CA1307 // Specify StringComparison for correctness
#pragma warning disable CA1310 // Specify StringComparison for correctness
#pragma warning disable S4058 // Specify StringComparison for correctness
internal static class IniParser
{
	public static bool IsShopName(this string line)
	{
		return line.StartsWith('[') && line.EndsWith(']');
	}

	public static string GetShopName(this string line)
	{
		return line[1..^1];
	}

	public static int GetPageNumber(this string value)
	{
		var extractedInt = Regex.Match(value, @"\d+").Value;

		return int.Parse(extractedInt);
	}

	public static float GetPriceMultiplier(this string value)
	{
		if (float.TryParse(value, out float multiplier))
		{
			return multiplier;
		}
		else
		{
			ShopTweakPlugin.Log.LogWarning($"Bad PriceMultiplier in ini file : {value}");
			return 1;
		}
	}

	public static NpcShopType ConvertOrThrow(this string shopName)
	{
		if (!Enum.TryParse<NpcShopType>(shopName, out var shopType))
		{
			throw new Exception($"Invalid NpcShopType {shopName}");
		}

		return shopType;
	}

	private record State
	{
		internal List<CustomShop> shops = [];
		internal NpcShopType? currentNpcShopType;
		internal DeferredListProcessor currentDeferredList = new();
		internal float currentPriceMultiplier;

		internal State()
		{
			Reset();
		}

		internal void Reset()
		{
			currentNpcShopType = null;
			currentDeferredList = new();
			currentPriceMultiplier = -1.0f;
		}

		internal void HandleCurrentList()
		{
			if (currentNpcShopType is not null)
			{
				if (!currentDeferredList.IsEmpty())
				{
					shops.Add(new CustomShop
					{
						PriceMultiplier = currentPriceMultiplier,
						ShopTweaks = currentDeferredList,
						ShopType = (NpcShopType)currentNpcShopType
					});
					Reset();
				}
				else
				{
					ShopTweakPlugin.Log.LogDebug($"currentNpcShopType [{currentNpcShopType}] has no actions.");
				}
			}
		}
	}

	internal static List<CustomShop> ParseFile(string fileName)
	{
		var myState = new State();
		
		string currentNewPageName = string.Empty;

		foreach (var line in File.ReadLines(fileName))
		{
			var trimmedLine = line.Trim();

			// Skip empty lines or comments.
			if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
			{
				continue;
			}
			ShopTweakPlugin.Log.LogDebug($"trimmedLine : {trimmedLine}");
			// Handle sections (shops).
			if (trimmedLine.IsShopName())
			{
				myState.HandleCurrentList();

				var shopName = trimmedLine.GetShopName();
				myState.currentNpcShopType = shopName.ConvertOrThrow();
				continue;
			}

			if (myState.currentNpcShopType is null)
			{
				ShopTweakPlugin.Log.LogDebug($"Invalid ini format. Missing shop type before line: {trimmedLine}");
				continue;
			}

			// Handle key-value pairs.
			var delimiterIndex = trimmedLine.IndexOf('='); //Need first IndexOf for the ReplaceAction
			if (delimiterIndex <= -1)
			{
				continue;
			}

			var key = trimmedLine[..delimiterIndex].Trim();
			var value = trimmedLine[(delimiterIndex + 1)..].Trim();

			switch (key)
			{
				case string k when k.StartsWith("NewPageName"):
					currentNewPageName = value;
					break;

				case string k when k.StartsWith("NewPage"):
					if (string.IsNullOrEmpty(currentNewPageName))
					{
						ShopTweakPlugin.Log.LogError($"Invalid ini format. Missing page name before line: {trimmedLine}");
						currentNewPageName = "Unknown";
					}

					myState.currentDeferredList.Enqueue(CustomActionFactory.MakeAddPage(value, currentNewPageName));
					currentNewPageName = string.Empty;
					break;

				case string k when k.StartsWith("AddItem"):
					myState.currentDeferredList.Enqueue(CustomActionFactory.MakeAddItems(value, GetPageNumber(key)));
					currentNewPageName = string.Empty;
					break;

				case string k when k.StartsWith("RemoveItem"):
					myState.currentDeferredList.Enqueue(CustomActionFactory.MakeRemoveItems(value, GetPageNumber(key)));
					currentNewPageName = string.Empty;
					break;

				case string k when k.StartsWith("ReplaceItem"):
					myState.currentDeferredList.Enqueue(CustomActionFactory.MakeReplaceItems(value, GetPageNumber(key)));
					currentNewPageName = string.Empty;
					break;

				case "PriceMultiplier":
					myState.currentPriceMultiplier = value.GetPriceMultiplier();
					currentNewPageName = string.Empty;
					break;

				default:
					ShopTweakPlugin.Log.LogWarning($"Unhandled line in ini file : {trimmedLine}");
					break;
			}

		}

		myState.HandleCurrentList();

		return myState.shops;
	}


}
#pragma warning restore CA1307 // Specify StringComparison for correctness
#pragma warning restore CA1310 // Specify StringComparison for correctness
#pragma warning restore S4058 // Specify StringComparison for correctness
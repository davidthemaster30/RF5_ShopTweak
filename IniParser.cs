using RF5SHOP;

namespace RF5_ShopTweak;
#pragma warning disable CA1307 // Specify StringComparison for correctness
#pragma warning disable CA1310 // Specify StringComparison for correctness
internal static class IniParser
{
	internal static List<CustomShop> ParseFile(string fileName)
	{
		List<CustomShop> shops = [];
		CustomShop? currentShop = null;
		CustomShopPageAction? currentPage = null;

		foreach (var line in File.ReadLines(fileName))
		{
			var trimmedLine = line.Trim();

			// Skip empty lines or comments.
			if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
			{
				continue;
			}

			// Handle sections (shops).
			if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
			{
				var shopName = trimmedLine[1..^1]; // Remove brackets.
				if (!Enum.TryParse<NpcShopType>(shopName, out var shopType))
				{
					throw new Exception($"Invalid NpcShopType {trimmedLine}");
				}

				if (currentShop is not null)
				{
					shops.Add(currentShop);
					currentPage = null;
				}

				currentShop = new CustomShop { ShopType = shopType };
				continue;
			}

			if (currentShop is null)
			{
				throw new Exception($"Invalid ini format. Missing shop type before line: {trimmedLine}");
			}

			// Handle key-value pairs.
			var delimiterIndex = trimmedLine.IndexOf('=');
			if (delimiterIndex <= -1)
			{
				continue;
			}

			var key = trimmedLine[..delimiterIndex].Trim();
			var value = trimmedLine[(delimiterIndex + 1)..].Trim();


			switch (key)
			{
				case string k when k.StartsWith("NewPageName"):
					if (currentPage is not null)
					{
						currentShop.Pages.Add(currentPage);
					}

					currentPage = new CustomShopPageAdd { Name = value };
					break;

				case string k when k.StartsWith("NewPage"):
					if (currentPage is not CustomShopPageAdd)
					{
						throw new Exception($"Invalid ini format. Missing page name before line: {trimmedLine}");
					}

					var newpageItems = ParseItems(value);
					(currentPage as CustomShopPageAdd)?.Items.AddRange(newpageItems);
					break;

				case string k when k.StartsWith("AddItem"):
					if (currentPage is null)
					{
						throw new Exception($"Invalid ini format. Missing page name before line: {trimmedLine}");
					}

					var pageItems = ParseItems(value);
					(currentPage as CustomShopAddItems)?.Items.AddRange(pageItems);
					break;

				case string k when k.StartsWith("ReplaceItem"):
					if (currentPage is null)
					{
						throw new Exception($"Invalid ini format. Missing page name before line: {trimmedLine}");
					}

					var replaceItems = ParseItems(value);
					(currentPage as CustomShopPageReplace)?.Items.AddRange(replaceItems);
					currentShop.Pages.Add(currentPage);
					break;

				case "PriceMultiplier":
					if (float.TryParse(value, out float multiplier))
					{
						currentShop.PriceMultiplier = multiplier;
					}
					else
					{
						ShopTweakPlugin.Log.LogWarning($"Bad PriceMultiplier in ini file : {trimmedLine}");
					}
					break;

				default:
					ShopTweakPlugin.Log.LogWarning($"Unhandled line in ini file : {trimmedLine}");
					break;
			}

		}

		if (currentShop is not null)
		{
			shops.Add(currentShop);
		}

		return shops;
	}

	private static List<CustomShopItem> ParseItems(string value)
	{
		var items = new List<CustomShopItem>();

		foreach (var itemString in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = itemString.Split('+');

			if (!int.TryParse(parts[0], out int id))
			{
				ShopTweakPlugin.Log.LogWarning($"Invalid item ID specified : {itemString}");
				continue;
			}

			if (parts.Length == 2 && int.TryParse(parts[1], out int level))
			{
				items.Add(new CustomShopItem { Id = (ItemID)id, Level = level });
				continue;
			}

			items.Add(new CustomShopItem { Id = (ItemID)id });
		}

		return items;
	}
}
#pragma warning restore CA1307 // Specify StringComparison for correctness
#pragma warning restore CA1310 // Specify StringComparison for correctness
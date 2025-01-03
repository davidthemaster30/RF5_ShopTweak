using Define;
using HarmonyLib;
using RF5SHOP;

namespace RF5_ShopTweak;

[HarmonyPatch(typeof(UIShopController), nameof(UIShopController.SetShopTable))]
public class UIShopControllerChangePage
{
	static int CalcPrices(ItemID itemId, int itemLv = 1)
	{
		ItemDataTable data = ItemDataTable.GetDataTable(itemId);
		Main.Log.LogDebug($"itemid:{(int)itemId}, itemlv:{itemLv}, item prices: shop:{data.ShopPrice}, sell:{data.SellPrice}, calc:{data.GetShopPrice(itemLv)}");

		return data.ShopPrice;
	}

	static void HandleAddItem(string category, NpcShopTable __result)
	{
		for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
		{
			foreach (string item in Main.Config.GetString(category, $"AddItem{i + 1}", "").Split(','))
			{
				// itemId+itemLv
				string[] itemIdAndLevel = item.Trim().Split('+');
				if (!int.TryParse(itemIdAndLevel[0], out int itemId))
				{
					continue;
				}

				ShopItem shopItem = new ShopItem
				{
					ItemId = (ItemID)itemId,
					prices = 100,   // 实际价格=这个数值*商店价格/100
				};
				if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out int itemLv))
				{
					shopItem.itemLv = itemLv;
				}
				else
				{
					shopItem.itemLv = 1;
				}

				__result.ShopCatalogPages[i].shopItems.Add(shopItem);

				Main.Log.LogDebug($"AddItem category:{category}, page:{i + 1}, itemId:{itemId}, itemLv:{shopItem.itemLv}, prices:{shopItem.prices}");
			}
		}
	}

	static void HandleNewPage(string category, NpcShopTable __result)
	{
		for (int i = 0; i < 99; ++i)
		{
			ShopCatalogPage page = new ShopCatalogPage
			{
				name = Main.Config.GetString(category, $"NewPageName{i + 1}", ""),
				shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>(),
			};

			foreach (string item in Main.Config.GetString(category, $"NewPage{i + 1}", "").Split(','))
			{
				string[] itemIdAndLevel = item.Trim().Split('+');
				if (!int.TryParse(itemIdAndLevel[0], out int itemId))
				{
					continue;
				}

				ShopItem shopItem = new ShopItem
				{
					ItemId = (ItemID)itemId,
					prices = 100,
				};
				if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out int itemLv))
				{
					shopItem.itemLv = itemLv;
				}
				else
				{
					shopItem.itemLv = 1;
				}

				page.shopItems.Add(shopItem);

				Main.Log.LogDebug($"NewPage category:{category}, page:{__result.ShopCatalogPages.Count + i + 1}, pageName:{page.name}, itemId:{itemId}, itemLv:{shopItem.itemLv}, prices:{shopItem.prices}");
			}

			if (page.name.Length > 0 && page.shopItems.Count > 0)
			{
				__result.ShopCatalogPages.Add(page);
				if (__result.ShopNpcTalks.Count > 0)
				{
					__result.ShopNpcTalks.Add(__result.ShopNpcTalks[0]);
				}
			}
			else
			{
				break;
			}
		}
	}

	static void HandleRemoveItem(string category, NpcShopTable __result)
	{
		for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
		{
			foreach (string item in Main.Config.GetString(category, $"RemoveItem{i + 1}", "").Split(','))
			{
				if (!int.TryParse(item.Trim(), out int itemId))
				{
					continue;
				}

				for (int i2 = 0; i2 < __result.ShopCatalogPages[i].shopItems.Count; ++i2)
				{
					if (__result.ShopCatalogPages[i].shopItems[i2].ItemId != (ItemID)itemId)
					{
						continue;
					}

					Main.Log.LogDebug($"RemoveItem category:{category}, page:{i + 1}, itemId:{itemId}");

					__result.ShopCatalogPages[i].shopItems.RemoveAt(i2--);
				}
			}
		}
	}

	static void HandleReplaceItem(string category, NpcShopTable __result)
	{
		for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
		{
			foreach (string item in Main.Config.GetString(category, $"ReplaceItem{i + 1}", "").Split(','))
			{
				string[] itemIdToItemId = item.Trim().Split('=');
				if (itemIdToItemId.Length < 2)
				{
					break;
				}

				string[] newItemIdAndLevel = itemIdToItemId[1].Split('+');

				if (!int.TryParse(itemIdToItemId[0], out int oldItemId) ||
					!int.TryParse(newItemIdAndLevel[0], out int newItemId))
				{
					continue;
				}

				foreach (ShopItem shopItem in __result.ShopCatalogPages[i].shopItems)
				{
					if (shopItem.ItemId != (ItemID)oldItemId)
					{
						continue;
					}

					shopItem.ItemId = (ItemID)newItemId;
					shopItem.conditions?.Clear();
					shopItem.id = 0;
					shopItem.storyLineFrag = GameFlagData.None;
					if (newItemIdAndLevel.Length > 1 && int.TryParse(newItemIdAndLevel[1], out int itemLv))
					{
						shopItem.itemLv = itemLv;
					}
					else
					{
						shopItem.itemLv = 1;
					}

					shopItem.prices = 100;

					Main.Log.LogDebug($"ReplaceItem category:{category}, page:{i + 1}, oldItemId:{oldItemId}, newItemId:{newItemId}, itemLv:{shopItem.itemLv}, prices:{shopItem.prices}");
				}
			}
		}
	}
	static void Postfix(UIShopController __instance)
	{
		if (__instance.ShopMenuType != ShopMenuType.ITEM)
		{
			return;
		}

		string category = __instance.shopType.ToString();
		HandleAddItem(category, __instance.NpcShopTable);
		HandleNewPage(category, __instance.NpcShopTable);
		HandleReplaceItem(category, __instance.NpcShopTable);
		HandleRemoveItem(category, __instance.NpcShopTable);

		__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
		__instance.ChangePagesGroup.SetActive(__instance.NpcShopTable.ShopCatalogPages.Count > 1);

		float rate = Main.Config.GetFloat(category, "PriceMultiplier", -1.0f);
		if (rate >= 0.0f)
		{
			__instance.UIShopControl.discountRate = rate;
		}

		Main.Log.LogDebug($"SetShopTable shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}, pageMax:{__instance.pageMax}, discountRate:{__instance.UIShopControl.discountRate}");

		foreach (ShopCatalogPage page in __instance.NpcShopTable.ShopCatalogPages)
		{
			Main.Log.LogDebug($"Page:{page.name}, itemCount:{page.shopItems.Count}");
		}
	}
}

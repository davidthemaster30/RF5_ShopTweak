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
		ShopTweakPlugin.Log.LogDebug($"itemid:{(int)itemId}, itemlv:{itemLv}, item prices: shop:{data.ShopPrice}, sell:{data.SellPrice}, calc:{data.GetShopPrice(itemLv)}");

		return data.ShopPrice;
	}

	static void HandleShop(CustomShop shop, UIShopController __instance)
	{
		NpcShopTable __result = __instance._NpcShopTable;
		ShopTweakPlugin.Log.LogInfo($"HandleNewPage {shop.ShopType}");

		ShopTweakPlugin.Log.LogInfo($"Shop {shop.ShopType.ToString()}");

		foreach (var page in shop.Pages)
		//foreach (string item in ShopTweakPlugin.Config.GetString(category, $"NewPage{i}", "").Split(','))
		{
			ShopCatalogPage catalogPage = new ShopCatalogPage
			{
				name = page.Name,
				shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>(),
			};
			ShopTweakPlugin.Log.LogDebug($"NewPage {page.Name} ShopCatalogPage");

			foreach (var item in page.Items)
			{
				ShopTweakPlugin.Log.LogDebug($"  Item {item.Id}:{item.Level}");

				ShopItem shopItem = new ShopItem
				{
					ItemId = item.Id,
					itemLv = item.Level,
					prices = 100,
				};

				catalogPage.shopItems.Add(shopItem);

				ShopTweakPlugin.Log.LogDebug($"  NewPage pageName:{catalogPage.name}, itemId:{shopItem.ItemId}, itemLv:{shopItem.itemLv}, prices:{shopItem.prices}");
			}

			if (catalogPage.name.Length > 0 && catalogPage.shopItems.Count > 0)
			{
				__result.ShopCatalogPages.Add(catalogPage);
				if (__result.ShopNpcTalks.Count > 0)
				{
					__result.ShopNpcTalks.Add(__result.ShopNpcTalks[0]);
				}
				ShopTweakPlugin.Log.LogDebug($"  NewPage page:{__result.ShopCatalogPages.Count + 1}, pageName:{catalogPage.name}");
			}
			else
			{
				ShopTweakPlugin.Log.LogWarning("  NewPage had no name or items category");
			}
		}


		ShopTweakPlugin.Log.LogInfo($"HandleNewPage {shop.ShopType} End");

		ShopTweakPlugin.Log.LogDebug($"1SetShopTable shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}, pageMax:{__instance.pageMax}, discountRate:{__instance.UIShopControl.discountRate}");

		foreach (ShopCatalogPage page in __instance.NpcShopTable.ShopCatalogPages)
		{
			ShopTweakPlugin.Log.LogDebug($"1Page:{page.name}, itemCount:{page.shopItems.Count}");
		}

		//__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
		//__instance.ChangePagesGroup.SetActive(true);

		float rate = shop.PriceMultiplier;
		if (rate >= 0.0f)
		{
			__instance.UIShopControl.discountRate = rate;
		}

		ShopTweakPlugin.Log.LogDebug($"2SetShopTable shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}, pageMax:{__instance.pageMax}, discountRate:{__instance.UIShopControl.discountRate}");

		foreach (ShopCatalogPage page in __instance.NpcShopTable.ShopCatalogPages)
		{
			ShopTweakPlugin.Log.LogDebug($"2Page:{page.name}, itemCount:{page.shopItems.Count}");
		}
	}

	static void Postfix(UIShopController __instance)
	{
		if (__instance.ShopMenuType != ShopMenuType.ITEM)
		{
			return;
		}

		NpcShopType category = __instance.shopType;
		var shop = ShopTweakPlugin.Shops.FirstOrDefault(x => x.ShopType == category);
		if (shop is null || shop.Pages.Count == 0)
		{
			return;
		}
		ShopTweakPlugin.Log.LogDebug($"Postfix SetShopTable shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}, pageMax:{__instance.pageMax}, discountRate:{__instance.UIShopControl.discountRate}");


		HandleShop(shop, __instance);
	}
}

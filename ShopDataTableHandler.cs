using HarmonyLib;
using RF5SHOP;

namespace RF5_ShopTweak;

[HarmonyPatch]
internal static class ShopDataTableHandler
{
	private static bool _processed = false;

    [HarmonyPatch(typeof(UIShopController), "SetShopTable")]
    [HarmonyPostfix]
	internal static void UIShopControllerTweaks(UIShopController __instance)
	{
		ShopTweakPlugin.Log.LogDebug($"UIShopController.SetShopTable");
		if (__instance is null || __instance.ShopMenuType != ShopMenuType.ITEM || __instance.NpcShopTable == null)
		{
			return;
		}

        if (_processed)
		{
			ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks Skipping already processed shop:{__instance.NpcShopTable.name}");
			return;
		}

		ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks Start shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}");

		var shop = ShopTweakPlugin.Shops.FirstOrDefault(x => x.ShopType == __instance.shopType);
		shop?.ShopTweaks.ApplyAll(__instance.NpcShopTable);
		_processed = true;

		ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks End shopType:{__instance.shopType}, shopPages:{__instance.NpcShopTable.ShopCatalogPages.Count}");

		foreach (ShopCatalogPage page in __instance.NpcShopTable.ShopCatalogPages)
		{
			ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks Page Debug:{page.name}, itemCount:{page.shopItems.Count}");
		}

		__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
		__instance.ChangePagesGroup.SetActive(__instance.NpcShopTable.ShopCatalogPages.Count > 1);

		if (shop?.PriceMultiplier >= 0.0f)
		{
			__instance.UIShopControl.discountRate = shop.PriceMultiplier;
			ShopTweakPlugin.Log.LogDebug($"ShopDataTableHandler applied discountRate:{shop.PriceMultiplier}");
		}
	}

	[HarmonyPatch(typeof(UIShopController), "CloseShop")]
    [HarmonyPostfix]
	internal static void UIShopControllerTweaksCleanup(UIShopController __instance)
	{
		ShopTweakPlugin.Log.LogDebug($"UIShopController.CloseShop");
		_processed = false;
	}

}



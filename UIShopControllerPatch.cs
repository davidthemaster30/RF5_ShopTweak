using HarmonyLib;
using RF5SHOP;
using Define;
using UnityEngine;
using UnityEngine.Events;

namespace RF5_ShopTweak;

[HarmonyPatch]
internal class ShopDataTableChangePage
{
	[HarmonyPatch(typeof(ShopDataTable), nameof(ShopDataTable.GetTable))]
	[HarmonyPostfix]
	static void ApplyShopTweaks(NpcShopType type, ref NpcShopTable __result)
	{
		ShopTweakPlugin.Log.LogDebug($"ShopDataTable.GetTable type {type}, __result {__result}");
		if (__result is null)
		{
			return;
		}

		ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks Start shopType:{type}, shopPages:{__result.ShopCatalogPages.Count}");

		var shop = ShopTweakPlugin.Shops.FirstOrDefault(x => x.ShopType == type);
		shop?.ShopTweaks.ApplyAll(ref __result);

		ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks End shopType:{type}, shopPages:{__result.ShopCatalogPages.Count}");

		foreach (ShopCatalogPage page in __result.ShopCatalogPages)
		{
			ShopTweakPlugin.Log.LogDebug($"ApplyShopTweaks Page Debug:{page.name}, itemCount:{page.shopItems.Count}");
		}
	}
}

[HarmonyPatch]
internal static class ShopDataTableHandler
{
	[HarmonyPatch(typeof(UIShopController), nameof(UIShopController.SetShopTable))]
	[HarmonyPostfix]
	internal static void UIShopControllerTweaks(UIShopController __instance)
	{
		ShopTweakPlugin.Log.LogDebug($"UIShopController.SetShopTable");
		if (__instance is null || __instance.ShopMenuType != ShopMenuType.ITEM)
		{
			return;
		}

		__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
		__instance.ChangePagesGroup.SetActive(__instance.NpcShopTable.ShopCatalogPages.Count > 1);

		var shop = ShopTweakPlugin.Shops.FirstOrDefault(x => x.ShopType == __instance.shopType);

		if (shop?.PriceMultiplier >= 0.0f)
		{
			__instance.UIShopControl.discountRate = shop.PriceMultiplier;
			ShopTweakPlugin.Log.LogDebug($"ShopDataTableHandler applied discountRate:{shop.PriceMultiplier}");
		}
		ShopTweakPlugin.Log.LogDebug($"ShopDataTableHandler currentShopType:{__instance.shopType}");
	}
}



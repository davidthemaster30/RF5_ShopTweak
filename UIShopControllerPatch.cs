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
		} //UITextDic.DICID SHOPCAT_CrystalShop_Koueki_00

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
	internal static NpcShopType? currentShopType;
	//

	// [HarmonyPatch(typeof(UIShopController), nameof(UIShopController.OpenShop))]
	// [HarmonyPostfix]
	// internal static void SaveCurrentShop(UIShopController __instance)
	// {
	// 	ShopTweakPlugin.Log.LogDebug($"UIShopController.OpenShop");
	// 	if (__instance is not null && __instance.ShopMenuType != ShopMenuType.ITEM)
	// 	{
	// 		currentShopType = __instance.shopType;
	// 		ShopTweakPlugin.Log.LogDebug($"SaveCurrentShop currentShopType:{currentShopType}");
	// 	}
	// }

	// [HarmonyPatch(typeof(UIShopController), nameof(UIShopController.OpenShop), new Type[] { typeof(NPCID), typeof(NpcShopType) , typeof(UnityAction) })]
	// [HarmonyPostfix]
	// internal static void SaveCurrentShop(NPCID _npcId, NpcShopType npcShopType, UnityAction EndCallback, UIShopController __instance)
	// {
	// 	ShopTweakPlugin.Log.LogDebug($"UIShopController.OpenShop, _npcId {_npcId}, npcShopType {npcShopType}");
	// 	if (__instance is not null && __instance.ShopMenuType != ShopMenuType.ITEM)
	// 	{
	// 		currentShopType = __instance.shopType;
	// 		ShopTweakPlugin.Log.LogDebug($"SaveCurrentShop currentShopType:{currentShopType}");
	// 	}
	// }

	// [HarmonyPatch(typeof(UIShopController), nameof(UIShopController.CloseShop))]
	// [HarmonyPostfix]
	// internal static void RemoveCurrentShop(UIShopController __instance)
	// {
	// 	ShopTweakPlugin.Log.LogDebug($"UIShopController.CloseShop");
	// 	if (currentShopType is not null)
	// 	{
	// 		ShopTweakPlugin.Log.LogDebug($"RemoveCurrentShop currentShopType:{currentShopType}");
	// 		currentShopType = null;
	// 	}
	// }

	[HarmonyPatch(typeof(UIShopController), nameof(UIShopController.SetShopTable))]
	[HarmonyPostfix]
	internal static void UIShopControllerTweaks(UIShopController __instance)
	{
		ShopTweakPlugin.Log.LogDebug($"UIShopController.SetShopTable");
		if (currentShopType is null || __instance is null)
		{
			return;
		}

		__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
		__instance.ChangePagesGroup.SetActive(__instance.NpcShopTable.ShopCatalogPages.Count > 1);

		var shop = ShopTweakPlugin.Shops.FirstOrDefault(x => x.ShopType == ShopDataTableHandler.currentShopType);

		if (shop?.PriceMultiplier >= 0.0f)
		{
			__instance.UIShopControl.discountRate = shop.PriceMultiplier;
		}
		ShopTweakPlugin.Log.LogDebug($"UIShopControllerTweaks currentShopType:{currentShopType}");
	}
}

// [HarmonyPatch]
// [HarmonyPatch(typeof(UIShopController), nameof(UIShopController.SetShopTable))]
// public class UIShopControllerChangePage
// {
// 	static void Postfix(UIShopController __instance)
// 	{
// 		ShopTweakPlugin.Log.LogDebug($"OLD UIShopController.SetShopTable");
// 	}

// }


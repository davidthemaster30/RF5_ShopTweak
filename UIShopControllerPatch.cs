using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RF5SHOP;
using Define;
using UnityEngine.Events;

namespace RF5_ShopTweak
{
	[HarmonyPatch(typeof(UIShopController), nameof(UIShopController.SetShopTable))]
	public class UIShopControllerChangePage
	{
		static void Postfix(UIShopController __instance)
		{
			string category = __instance.shopType.ToString();
			HandleAddItem(category, __instance.NpcShopTable);
			HandleNewPage(category, __instance.NpcShopTable);
			HandleReplaceItem(category, __instance.NpcShopTable);
			HandleRemoveItem(category, __instance.NpcShopTable);

			__instance.pageMax = __instance.NpcShopTable.ShopCatalogPages.Count;
			__instance.ChangePagesGroup.SetActive(__instance.NpcShopTable.ShopCatalogPages.Count > 1);

			Main.Log.LogDebug(string.Format("SetShopTable shopType:{0}, shopPages:{1}, pageMax:{2}",
				__instance.shopType, __instance.NpcShopTable.ShopCatalogPages.Count, __instance.pageMax
			));

			foreach (ShopCatalogPage page in __instance.NpcShopTable.ShopCatalogPages)
				Main.Log.LogDebug(string.Format("Page:{0}, itemCount:{1}", page.name, page.shopItems.Count));
		}

		// 增加物品
		static void HandleAddItem(string category, NpcShopTable __result)
		{
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("AddItem{0}", i + 1), "").Split(','))
				{
					// itemId+itemLv
					string[] itemIdAndLevel = item.Trim().Split('+');
					if (!int.TryParse(itemIdAndLevel[0], out int itemId))
						continue;

					ShopItem shopItem = new ShopItem();
					shopItem.ItemId = (ItemID)itemId;
					if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out int itemLv))
						shopItem.itemLv = itemLv;
					else
						shopItem.itemLv = 1;
					shopItem.prices = CalcPrices(shopItem.ItemId, shopItem.itemLv);
					__result.ShopCatalogPages[i].shopItems.Add(shopItem);

					Main.Log.LogDebug(string.Format("AddItem category:{0}, page:{1}, itemId:{2}, itemLv:{3}, prices:{4}",
						category, i + 1, itemId, shopItem.itemLv, shopItem.prices
					));
				}
			}
		}

		// 替换物品
		static void HandleReplaceItem(string category, NpcShopTable __result)
		{
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("ReplaceItem{0}", i + 1), "").Split(','))
				{
					// oldId=newId+newLv
					string[] itemIdToItemId = item.Trim().Split('=');
					if (itemIdToItemId.Length < 2)
						break;

					// newId+newLv
					string[] newItemIdAndLevel = itemIdToItemId[1].Split('+');

					if (!int.TryParse(itemIdToItemId[0], out int oldItemId) ||
						!int.TryParse(newItemIdAndLevel[0], out int newItemId))
						continue;

					foreach(ShopItem shopItem in __result.ShopCatalogPages[i].shopItems)
					{
						if (shopItem.ItemId != (ItemID)oldItemId)
							continue;

						shopItem.ItemId = (ItemID)newItemId;
						shopItem.conditions?.Clear();
						shopItem.id = 0;
						shopItem.storyLineFrag = GameFlagData.None;
						if (newItemIdAndLevel.Length > 1 && int.TryParse(newItemIdAndLevel[1], out int itemLv))
							shopItem.itemLv = itemLv;
						else
							shopItem.itemLv = 1;
						shopItem.prices = CalcPrices(shopItem.ItemId, shopItem.itemLv);

						Main.Log.LogDebug(string.Format("ReplaceItem category:{0}, page:{1}, oldItemId:{2}, newItemId:{3}, itemLv:{4}, prices:{5}",
							category, i + 1, oldItemId, newItemId, shopItem.itemLv, shopItem.prices
						));
					}
				}
			}
		}

		// 增加新页
		static void HandleNewPage(string category, NpcShopTable __result)
		{
			for (int i = 0; i < 99; ++i)
			{
				ShopCatalogPage page = new ShopCatalogPage
				{
					name = Main.Config.GetString(category, string.Format("NewPageName{0}", i + 1), ""),
					shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>(),
				};

				foreach (string item in Main.Config.GetString(category, string.Format("NewPage{0}", i + 1), "").Split(','))
				{
					// itemId+itemLv
					string[] itemIdAndLevel = item.Trim().Split('+');
					if (!int.TryParse(itemIdAndLevel[0], out int itemId))
						continue;
					
					ShopItem shopItem = new ShopItem();
					shopItem.ItemId = (ItemID)itemId;
					if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out int itemLv))
						shopItem.itemLv = itemLv;
					else
						shopItem.itemLv = 1;
					shopItem.prices = CalcPrices(shopItem.ItemId, shopItem.itemLv);
					page.shopItems.Add(shopItem);

					Main.Log.LogDebug(string.Format("NewPage category:{0}, page:{1}, pageName:{2}, itemId:{3}, itemLv:{4}, prices:{5}",
						category, __result.ShopCatalogPages.Count + i + 1, page.name, itemId, shopItem.itemLv, shopItem.prices
					));
				}

				if (page.name.Length > 0 && page.shopItems.Count > 0)
				{
					__result.ShopCatalogPages.Add(page);
					if (__result.ShopNpcTalks.Count > 0)
						__result.ShopNpcTalks.Add(__result.ShopNpcTalks[0]);
				}
				else
				{
					break;
				}
			}
		}

		// 删除物品
		static void HandleRemoveItem(string category, NpcShopTable __result)
		{
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("RemoveItem{0}", i + 1), "").Split(','))
				{
					if (!int.TryParse(item.Trim(), out int itemId))
						continue;

					for (int i2 = 0; i2 < __result.ShopCatalogPages[i].shopItems.Count; ++i2)
					{
						if (__result.ShopCatalogPages[i].shopItems[i2].ItemId != (ItemID)itemId)
							continue;

						Main.Log.LogDebug(string.Format("RemoveItem category:{0}, page:{1}, itemId:{2}",
							category, i + 1, itemId
						));

						__result.ShopCatalogPages[i].shopItems.RemoveAt(i2--);
					}
				}
			}
		}

		static int CalcPrices(ItemID itemId, int itemLv = 1)
		{
			ItemDataTable data = ItemDataTable.GetDataTable(itemId);
			Main.Log.LogDebug(string.Format("item prices: shop:{0}, sell:{1}, calc:{2}",
				data.ShopPrice, data.SellPrice, data.GetShopPrice(itemLv)
			));

			return data.GetShopPrice(itemLv);
		}
	}
}

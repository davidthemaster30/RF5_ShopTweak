using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RF5SHOP;

namespace RF5_ShopTweak
{
	[HarmonyPatch(typeof(ShopDataTable), nameof(ShopDataTable.GetTable))]
	public class ShopDataTablePatch
	{
		public static void Postfix(NpcShopType type, NpcShopTable __result)
		{
			string category = type.ToString();
			HandleAddItem(category, __result);
			// HandleNewPage(category, __result);
			// HandleReplaceItem(category, __result);
			// HandleRemoveItem(category, __result);
		}

		static void HandleAddItem(string category, NpcShopTable __result)
		{
			// 增加物品
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("AddItem{0}", i + 1), "").Split(','))
				{
					// itemId+itemLv
					string[] itemIdAndLevel = item.Trim().Split('+');
					if (int.TryParse(itemIdAndLevel[0], out int buffer))
					{
						ShopItem shopItem = new ShopItem();
						shopItem.ItemId = (ItemID)buffer;
						if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out buffer))
							shopItem.itemLv = buffer;
						else
							shopItem.itemLv = 1;
						shopItem.prices = shopItem.GetPrices();
						__result.ShopCatalogPages[i].shopItems.Add(shopItem);

						Main.Log.LogInfo(string.Format("AddItem category:{0}, page:{1}, itemId:{2} itemLv:{3}",
							category, i + 1, (int)shopItem.ItemId, shopItem.itemLv
						));
					}
				}
			}
		}

		static void HandleReplaceItem(string category, NpcShopTable __result)
		{
			// 替换物品
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("ReplaceItem{0}", i + 1), "").Split(','))
				{
					// oldId=newId+newLv
					string[] itemIdToItemId = item.Trim().Split('=');
					if (itemIdToItemId.Length < 2)
						break;

					string[] newItemIdAndLevel = itemIdToItemId[1].Split('+');

					if (int.TryParse(itemIdToItemId[0], out int buffer))
					{
						for (int i2 = 0; i2 < __result.ShopCatalogPages[i].shopItems.Count; ++i2)
						{
							if (__result.ShopCatalogPages[i].shopItems[i2].ItemId != (ItemID)buffer)
								continue;

							if(int.TryParse(newItemIdAndLevel[0], out buffer))
							{
								ShopItem shopItem = new ShopItem();
								ItemID oldItemId = __result.ShopCatalogPages[i].shopItems[i2].ItemId;
								shopItem.ItemId = (ItemID)buffer;

								if (newItemIdAndLevel.Length > 1 && int.TryParse(newItemIdAndLevel[1], out buffer))
									shopItem.itemLv = buffer;

								shopItem.prices = shopItem.GetPrices();
								__result.ShopCatalogPages[i].shopItems.RemoveAt(i2);
								__result.ShopCatalogPages[i].shopItems.Insert(i2, shopItem);

								Main.Log.LogInfo(string.Format("ReplaceItem category:{0}, page:{1}, itemId:{2}, newItemId:{3}, itemLv:{4}",
									category, i + 1, (int)oldItemId, (int)shopItem.ItemId, shopItem.itemLv
								));
							}
						}
					}
				}
			}
		}

		static void HandleNewPage(string category, NpcShopTable __result)
		{
			// 增加新页
			for(int i = 0; i < 99; ++i)
			{
				ShopCatalogPage page = new ShopCatalogPage();
				page.name = Main.Config.GetString(category, string.Format("NewPageName{0}", i + 1), "");

				foreach(string item in Main.Config.GetString(category, string.Format("NewPage{0}", i + 1), "").Split(','))
				{
					// itemId+itemLv
					string[] itemIdAndLevel = item.Trim().Split('+');
					if (int.TryParse(itemIdAndLevel[0], out int buffer))
					{
						ShopItem shopItem = new ShopItem();
						shopItem.ItemId = (ItemID)buffer;
						if (itemIdAndLevel.Length > 1 && int.TryParse(itemIdAndLevel[1], out buffer))
							shopItem.itemLv = buffer;
						else
							shopItem.itemLv = 1;
						shopItem.prices = shopItem.GetPrices();
						page.shopItems.Add(shopItem);

						Main.Log.LogInfo(string.Format("NewPage category:{0}, page:{1}, pageName:{2}, itemId:{2}, itemLv:{3}",
							category, __result.ShopCatalogPages.Count + i + 1, page.name, (int)shopItem.ItemId, shopItem.itemLv
						));
					}
				}

				__result.ShopCatalogPages.Add(page);
				__result.ShopNpcTalks.Add(__result.ShopNpcTalks[0]);    // 不确定是否需要
			}
		}

		static void HandleRemoveItem(string category, NpcShopTable __result)
		{
			// 删除物品
			for (int i = 0; i < __result.ShopCatalogPages.Count; i++)
			{
				foreach (string item in Main.Config.GetString(category, string.Format("RemoveItem{0}", i + 1), "").Split(','))
				{
					if (int.TryParse(item.Trim(), out int buffer))
					{
						for (int i2 = 0; i2 < __result.ShopCatalogPages[i].shopItems.Count; ++i2)
						{
							if (__result.ShopCatalogPages[i].shopItems[i2].ItemId != (ItemID)buffer)
								continue;

							Main.Log.LogInfo(string.Format("RemoveItem category:{0}, page:{1}, itemId:{2}",
								category, i + 1, (int)__result.ShopCatalogPages[i].shopItems[i2].ItemId
							));

							__result.ShopCatalogPages[i].shopItems.RemoveAt(i2);
							--i2;
						}
					}
				}
			}
		}
	}
}

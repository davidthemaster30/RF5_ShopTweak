using RF5SHOP;

namespace RF5_ShopTweak;

public static class ListExtensions
{
    //Il2CppSystem.Predicate
    static Predicate<ShopItem> ByItemID(ItemID itemID)
    {
        return delegate (ShopItem item)
        {
            return item.ItemId == itemID;
        };
    }

    public static (int i, int j) GetIndexOf(this Il2CppSystem.Collections.Generic.List<ShopCatalogPage> pages, ItemID item)
    {
        if (pages is null)
        {
            return (-1, -1);
        }

        for (int i = 0; i < pages.Count; i++)
        {
            for (int j = 0; j < pages[i].shopItems.Count; j++)
            {
                if (pages[i].shopItems[j].ItemId == item)
                {
                    return (i, j);
                }
            }
        }
        return (-1, -1);
    }

    public static int GetIndexOf(this ShopCatalogPage page, ItemID item)
    {
        if (page is null)
        {
            return -1;
        }

        for (int i = 0; i < page.shopItems.Count; i++)
        {
            if (page.shopItems[i].ItemId == item)
            {
                return i;
            }
        }

        return -1;
    }
}

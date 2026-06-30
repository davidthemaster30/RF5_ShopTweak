using RF5SHOP;
using Define;

namespace RF5_ShopTweak;

internal class CustomShopReplaceItemAction : ICustomShopPageAction
{
    private readonly ItemID _oldItem;
    private readonly int _pageNumber;
    private readonly ShopItem _shopItem;
    public override string ToString()
    {
        return $"CustomShopReplaceItemAction Replace {_oldItem}({(int)_oldItem}) with {_oldItem}({(int)_oldItem})+{_shopItem.itemLv} on page {_pageNumber}";
    }

    internal CustomShopReplaceItemAction(int pageNumber, ItemID oldItem, ItemID newItem, int itemLevel = 1)
    {
        _pageNumber = pageNumber;
        _oldItem = oldItem;

        _shopItem = new ShopItem
        {
            ItemId = newItem,
            prices = 100,   // Actual price = this value * store price / 100
            itemLv = itemLevel,
            id = 0,
            storyLineFrag = GameFlagData.None
        };
    }

    public void Apply(ref NpcShopTable shop)
    {
        if (shop is null || shop.ShopCatalogPages.Count <= 0 || _pageNumber >= shop.ShopCatalogPages.Count)
        {
            return;
        }

        var index = shop.ShopCatalogPages[_pageNumber].GetIndexOf(_oldItem);

        if (index == -1)
        {
            ShopTweakPlugin.Log.LogDebug($"RemoveItem could not find itemId:{(int)_oldItem} to replace in page:{_pageNumber + 1}");
            return;
        }

        ShopItem shopItem = shop.ShopCatalogPages[_pageNumber].shopItems[index];

        shopItem.ItemId = _shopItem.ItemId;
        shopItem.conditions?.Clear();
        shopItem.id = _shopItem.id;
        shopItem.storyLineFrag = _shopItem.storyLineFrag;
        shopItem.itemLv = _shopItem.itemLv;
        shopItem.prices = _shopItem.prices;

        ShopTweakPlugin.Log.LogDebug($"Apply ReplaceItem page:{_pageNumber}, oldItemId:{_oldItem}, newItemId:{_shopItem.ItemId}, itemLv:{shopItem.itemLv}, prices:{shopItem.prices}");
    }
}

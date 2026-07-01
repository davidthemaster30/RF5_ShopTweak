using RF5SHOP;

namespace RF5_ShopTweak;

internal class CustomShopAddItemAction : ICustomShopPageAction
{
    private readonly int _pageNumber;
    private readonly ShopItem _shopItem;
    public override string ToString()
    {
        return $"CustomShopAddItemAction Add {_shopItem.ItemId}({(int)_shopItem.ItemId})+{_shopItem.itemLv} on page {_pageNumber}";
    }

    internal CustomShopAddItemAction(int pageNumber, ItemID itemId, int itemLevel = 1, int price = 100)
    {
        _pageNumber = pageNumber;

        _shopItem = new ShopItem
        {
            ItemId = itemId,
            prices = price,
            itemLv = itemLevel
        };
    }

    public void Apply(ref NpcShopTable shop)
    {
        if (shop is null || shop.ShopCatalogPages.Count <= 0 || _pageNumber >= shop.ShopCatalogPages.Count)
        {
            return;
        }

        shop.ShopCatalogPages[_pageNumber - 1].shopItems.Add(_shopItem);
        ShopTweakPlugin.Log.LogDebug($"Apply AddItem page:{_pageNumber}, itemId:{(int)_shopItem.ItemId}, itemLv:{_shopItem.itemLv}, prices:{_shopItem.prices}");
    }
}

using RF5SHOP;

namespace RF5_ShopTweak;

internal class CustomShopRemoveItemAction : ICustomShopPageAction
{
    private readonly ItemID _itemID;
    private readonly int _pageNumber;
    public override string ToString()
    {
        return $"CustomShopRemoveItemAction Remove {_itemID}({(int)_itemID}) on page {_pageNumber}";
    }

    internal CustomShopRemoveItemAction(int pageNumber, ItemID itemID)
    {
        _pageNumber = pageNumber;
        _itemID = itemID;
    }

    public void Apply(ref NpcShopTable shop)
    {
        if (shop is null || shop.ShopCatalogPages.Count <= 0 || _pageNumber >= shop.ShopCatalogPages.Count)
        {
            return;
        }

        var index = shop.ShopCatalogPages[_pageNumber].GetIndexOf(_itemID);

        if (index == -1)
        {
            ShopTweakPlugin.Log.LogDebug($"RemoveItem could not find itemId:{(int)_itemID} to remove in page:{_pageNumber + 1}");
            return;
        }

        shop.ShopCatalogPages[_pageNumber].shopItems.RemoveAt(index);
        ShopTweakPlugin.Log.LogDebug($"Apply RemoveItem page:{_pageNumber + 1}, itemId:{(int)_itemID}");
    }

}

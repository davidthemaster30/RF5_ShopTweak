using RF5SHOP;

namespace RF5_ShopTweak;

internal class CustomShopRemoveItemAction : ICustomShopPageAction
{
    private readonly ItemID _itemID;
    private readonly int _pageNumber;
    private int _pageIndex => _pageNumber - 1;
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
        if (shop is null || shop.ShopCatalogPages.Count <= 0 || _pageNumber > shop.ShopCatalogPages.Count)
        {
            return;
        }

        var itemIndex = shop.ShopCatalogPages[_pageIndex].GetIndexOf(_itemID);

        if (itemIndex == -1)
        {
            ShopTweakPlugin.Log.LogWarning($"RemoveItem could not find itemId:{(int)_itemID} to remove in page:{_pageNumber}");
            return;
        }

        shop.ShopCatalogPages[_pageIndex].shopItems.RemoveAt(itemIndex);
        ShopTweakPlugin.Log.LogInfo($"Apply RemoveItem page:{_pageNumber}, itemId:{(int)_itemID}");
    }

}

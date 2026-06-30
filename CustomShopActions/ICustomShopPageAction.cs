using RF5SHOP;

namespace RF5_ShopTweak;

internal interface ICustomShopPageAction
{
    //NpcShopTable is used instead of the ShopCatalogPages directly
    //for the add page action which needs to modify NpcShopTable
    public void Apply(ref NpcShopTable shop);
}

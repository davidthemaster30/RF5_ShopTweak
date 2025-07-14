using RF5SHOP;

namespace RF5_ShopTweak;

internal interface CustomShopPageAction
{

}

internal record CustomShop
{
    internal NpcShopType ShopType { get; init; }
    internal List<CustomShopPageAction> Pages { get; } = [];
    internal float PriceMultiplier { get; set; } = 1;
}

internal record CustomShopPageAdd : CustomShopPageAction
{
    internal string Name { get; init; } = string.Empty;
    internal List<CustomShopItem> Items { get; init; } = [];
}

internal record CustomShopPageReplace : CustomShopPageAction
{
    internal int PageNumber { get; init; } = 1;
    internal List<CustomShopItem> Items { get; init; } = [];
}

internal record CustomShopPageRemove : CustomShopPageAction
{
    internal int PageNumber { get; init; } = 1;
    internal List<ItemID> Items { get; init; } = [];
}

internal record CustomShopAddItems : CustomShopPageAction
{
    internal int PageNumber { get; init; } = 1;
    internal List<CustomShopItem> Items { get; init; } = [];
}

internal record CustomShopItem
{
    internal ItemID Id { get; init; }
    internal int Level { get; init; } = -1;
}


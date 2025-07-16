using RF5SHOP;

namespace RF5_ShopTweak;

internal record CustomShop
{
    internal NpcShopType ShopType { get; init; }
    internal List<CustomShopPageAdd> Pages { get; } = [];
    internal float PriceMultiplier { get; set; } = 1;
}

internal record CustomShopPageAdd
{
    internal string Name { get; init; } = string.Empty;
    internal List<CustomShopItem> Items { get; init; } = [];
}

internal record CustomShopItem
{
    internal ItemID Id { get; init; }
    internal int Level { get; init; } = -1;
}

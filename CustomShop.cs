using RF5SHOP;

namespace RF5_ShopTweak;

internal record CustomShop
{
    internal NpcShopType ShopType { get; init; }
    internal DeferredListProcessor ShopTweaks { get; init; }
    internal float PriceMultiplier { get; set; } = 1;
    public override string ToString()
    {
        return $"CustomShop {{ NpcShopType {ShopType} with {ShopTweaks.ToString()} and PriceMultiplier {PriceMultiplier} }}";
    }
}


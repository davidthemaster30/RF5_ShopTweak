using RF5SHOP;

namespace RF5_ShopTweak;

internal static class CustomActionFactory
{
    internal static CustomShopAddPageAction MakeAddPage(string value, string? pageName)
    {
        var parsedItems = ParseShopItems(value);
        var items = new Il2CppSystem.Collections.Generic.List<ShopItem>();
        foreach (var parsedItem in parsedItems)
        {
            items.Add(parsedItem);
        }
        ShopTweakPlugin.Log.LogDebug($"CustomActionFactory.MakeAddPage");
        return new CustomShopAddPageAction(pageName, items);
    }

    internal static List<ICustomShopPageAction> MakeRemoveItems(string value, int pageNumber)
    {
        List<ICustomShopPageAction> actions = new();
        foreach (var parsedItem in ParseShopItems(value))
        {
            actions.Add(new CustomShopRemoveItemAction(pageNumber, parsedItem.ItemId));
        }
        ShopTweakPlugin.Log.LogDebug($"CustomActionFactory.MakeRemoveItems");
        return actions;
    }

    internal static List<ICustomShopPageAction> MakeReplaceItems(string value, int pageNumber)
    {
        List<ICustomShopPageAction> actions = new();
        foreach (var parsedItem in ParseShopItems(value))
        {
            actions.Add(new CustomShopReplaceItemAction(pageNumber, parsedItem.ReplacedItemId, parsedItem.ItemId, parsedItem.itemLv, parsedItem.Prices));
        }
        ShopTweakPlugin.Log.LogDebug($"CustomActionFactory.MakeReplaceItems");
        return actions;
    }

    internal static List<ICustomShopPageAction> MakeAddItems(string value, int pageNumber)
    {
        List<ICustomShopPageAction> actions = new();
        foreach (var parsedItem in ParseShopItems(value))
        {
            actions.Add(new CustomShopAddItemAction(pageNumber, parsedItem.ItemId, parsedItem.itemLv, parsedItem.Prices));
        }
        ShopTweakPlugin.Log.LogDebug($"CustomActionFactory.MakeAddItems");
        return actions;
    }

    private sealed record ParsedItem
    {

        internal ItemID ItemId { get; init; }
        internal int itemLv { get; init; } = 1;
        internal ItemID ReplacedItemId { get; init; }
        internal int Prices { get; init; } = 100;
        public static ShopItem ToShopItem(ParsedItem item)
        {
            return new ShopItem
            {
                ItemId = item.ItemId,
                itemLv = item.itemLv,
                prices = item.Prices
            };
        }

        public static implicit operator ShopItem(ParsedItem item)
        {
            return ToShopItem(item);
        }
    }

    private static IEnumerable<ParsedItem> ParseShopItems(string value)
    {
        List<ParsedItem> parsedItems = [];
        foreach (var itemString in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            int replacedItemId = -1;
            int level = 1;
            int prices = 100;
            string[]? parts;

            if (itemString.IndexOf('=') != -1)
            {
                //Replacement
                if (!int.TryParse(itemString.Split('=').FirstOrDefault(), out replacedItemId))
                {
                    ShopTweakPlugin.Log.LogWarning($"Couldn't parse replacedItemId : {itemString}, skipping");
                    continue;
                }

                parts = itemString[(itemString.LastIndexOf('=') + 1)..].Split('+');
            }
            else
            {
                parts = itemString.Split('+');
            }

            if (!int.TryParse(parts[0], out int id))
            {
                ShopTweakPlugin.Log.LogWarning($"Invalid item ID specified : {itemString}"); //2152=2171+6
                continue;
            }

            if (parts.Length >= 2 && !int.TryParse(parts[1], out level))
            {
                ShopTweakPlugin.Log.LogWarning($"Couldn't parse itemLevel : {itemString}");

                if (parts.Length == 3 && !int.TryParse(parts[2], out prices))
                {
                    ShopTweakPlugin.Log.LogWarning($"Couldn't parse prices : {itemString}");
                }
            }

            parsedItems.Add(new ParsedItem
            {
                ItemId = (ItemID)id,
                itemLv = level,
                ReplacedItemId = (ItemID)replacedItemId,
                Prices = prices
            });
        }

        return parsedItems;
    }
}
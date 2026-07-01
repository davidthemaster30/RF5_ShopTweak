using RF5SHOP;
using System.Text;
using Il2CppInterop.Runtime;

namespace RF5_ShopTweak;

internal class CustomShopAddPageAction : ICustomShopPageAction
{
    private readonly ShopCatalogPage _page;
    private const string Empty = "Empty";
    private const string Unknown = "Unknown";
    private static readonly ShopCatalogPage _empty = new ShopCatalogPage
    {
        name = Empty,
        shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>()
    };

    public override string ToString()
    {
        if (_page == _empty)
        {
            return "CustomShopAddPageAction Empty page!";
        }

        StringBuilder sb = new();
        sb.AppendLine($"CustomShopAddPageAction for page {_page.name} with items:");
        foreach (var item in _page.shopItems)
        {
            sb.AppendLine($"{item.ItemId}({(int)item.ItemId})+{item.itemLv}");
        }
        return sb.ToString();
    }

    internal CustomShopAddPageAction(Il2CppSystem.Collections.Generic.List<ShopItem> items) : this(Unknown, items) { }
    internal static ShopCatalogPage Clone(ShopCatalogPage page)
    {
        var _shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>();

        foreach (var item in page.shopItems)
        {
            _shopItems.Add(Clone(item));
        }

        var new_page = new ShopCatalogPage
        {
            name = page.name,
            shopItems = _shopItems
        };

        return new_page;
    }

    internal static ShopCatalogPage Clone(ShopCatalogPage page, NpcShopTable shop)
    {
        var _shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>();

        foreach (var item in page.shopItems)
        {
            _shopItems.Add(Clone(item));
        }

        var new_page = new ShopCatalogPage
        {
            name = page.name,
            shopItems = _shopItems
        };

        return new_page;
    }

    internal static ShopItem Clone(ShopItem item)
    {
        return new ShopItem
        {
            ItemId = item.ItemId,
            itemLv = item.itemLv,
            prices = item.prices
        };
    }

    internal CustomShopAddPageAction(string name, Il2CppSystem.Collections.Generic.List<ShopItem> items)
    {
        if (items is null || items.Count <= 0)
        {
            _page = _empty;
            return;
        }

        _page = new ShopCatalogPage
        {
            name = name,
            shopItems = items
        };
    }

    public static unsafe void AddShopCatalogPage(Il2CppSystem.Collections.Generic.List<ShopCatalogPage> list, ShopCatalogPage page)
    {
        System.IntPtr pagePtr = IL2CPP.Il2CppObjectBaseToPtrNotNull(page);
        System.IntPtr unboxedPtr = IL2CPP.il2cpp_object_unbox(pagePtr);

        void** args = stackalloc void*[1];
        args[0] = (void*)unboxedPtr;

        System.IntPtr exception = System.IntPtr.Zero;
        IL2CPP.il2cpp_runtime_invoke(
            // Get the Add method pointer from the list's class
            IL2CPP.GetIl2CppMethod(
                IL2CPP.il2cpp_object_get_class(IL2CPP.Il2CppObjectBaseToPtrNotNull(list)),
                false, "Add", "System.Void", new[] { "RF5SHOP.ShopCatalogPage" }),
            IL2CPP.Il2CppObjectBaseToPtrNotNull(list),
            args,
            ref exception);
        Il2CppException.RaiseExceptionIfNecessary(exception);
    }

    public void Apply(ref NpcShopTable shop)
    {
        if (shop is null || shop.ShopCatalogPages.Count <= 0)
        {
            return;
        }

        if (_page.name != Empty)
        {
            AddShopCatalogPage(shop.ShopCatalogPages, _page);
            if (shop.ShopNpcTalks.Count > 0)
            {
                shop.ShopNpcTalks.Add(shop.ShopNpcTalks[0]);
            }
            ShopTweakPlugin.Log.LogInfo($"Apply NewPage page:{shop.ShopCatalogPages.Count}, pageName:{_page.name} with {_page.shopItems.Count} items");
        }
    }
}

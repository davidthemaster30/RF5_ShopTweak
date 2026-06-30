using RF5SHOP;
using System.Text;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;

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

        var new_page = CreateShopCatalogPage();
        new_page.name = page.name;
        new_page.shopItems = _shopItems;

        return new_page;
    }

    internal static ShopCatalogPage Clone(ShopCatalogPage page, NpcShopTable shop)
    {
        var _shopItems = new Il2CppSystem.Collections.Generic.List<ShopItem>();

        foreach (var item in page.shopItems)
        {
            _shopItems.Add(Clone(item));
        }

        var new_page = CreateShopCatalogPage(shop);
        new_page.name = page.name;
        new_page.shopItems = _shopItems;

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

    public static ShopCatalogPage CreateShopCatalogPage(NpcShopTable shop)
    {
        // Borrow a valid native struct pointer from an existing page
        var existingPage = shop.ShopCatalogPages[0];
        System.IntPtr existingPtr = IL2CPP.Il2CppObjectBaseToPtrNotNull(existingPage);

        // Unbox to get the raw struct pointer
        System.IntPtr rawStructPtr = IL2CPP.il2cpp_object_unbox(existingPtr);

        // Box a copy of it — same size, same layout, guaranteed valid
        System.IntPtr newBoxed = IL2CPP.il2cpp_value_box(Il2CppClassPointerStore<ShopCatalogPage>.NativeClassPtr, rawStructPtr);

        return new ShopCatalogPage(newBoxed);
    }

    internal CustomShopAddPageAction(string name, Il2CppSystem.Collections.Generic.List<ShopItem> items)
    {
        if (items is null || items.Count <= 0)
        {
            _page = _empty;
            return;
        }

        _page = CreateShopCatalogPage();
        _page.name = name;
        _page.shopItems = items;
    }

    public static ShopCatalogPage CreateShopCatalogPage()
    {
        unsafe
        {
            byte* stackPtr = stackalloc byte[(int)(uint)IL2CPP.il2cpp_class_value_size(Il2CppClassPointerStore<ShopCatalogPage>.NativeClassPtr, ref *(uint*)null)];
            var boxed = IL2CPP.il2cpp_value_box(Il2CppClassPointerStore<ShopCatalogPage>.NativeClassPtr, (System.IntPtr)stackPtr);
            return new ShopCatalogPage(boxed);
        }
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
            var clonedpage = Clone(_page, shop);
            ShopTweakPlugin.Log.LogDebug($"Apply NewPage page hashcode:{_page.GetHashCode()}");
            ShopTweakPlugin.Log.LogDebug($"Apply NewPage typeof(ShopCatalogPage).IsValueType:{typeof(ShopCatalogPage).IsValueType}");
            ShopTweakPlugin.Log.LogDebug($"Apply NewPage Adding page:{this.ToString()}");
            //shop.ShopCatalogPages.Add(clonedpage);
            AddShopCatalogPage(shop.ShopCatalogPages, clonedpage);
            if (shop.ShopNpcTalks.Count > 0)
            {
                shop.ShopNpcTalks.Add(shop.ShopNpcTalks[0]);
            }
            ShopTweakPlugin.Log.LogDebug($"Apply NewPage page:{shop.ShopCatalogPages.Count}, pageName:{_page.name} with {_page.shopItems.Count} items");

            // Check if the added page is still intact on the native side, this is crashing
            var addedPage = shop.ShopCatalogPages[shop.ShopCatalogPages.Count - 1];
            ShopTweakPlugin.Log.LogDebug($"Added page hashcode: {addedPage.GetHashCode()}");
            ShopTweakPlugin.Log.LogDebug($"Same object? {ReferenceEquals(_page, addedPage)}");
            ShopTweakPlugin.Log.LogDebug($"Items count after add: {addedPage.shopItems?.Count ?? -1}");
        }
    }
}

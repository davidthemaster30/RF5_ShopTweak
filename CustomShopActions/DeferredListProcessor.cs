using RF5SHOP;
using System.Text;

namespace RF5_ShopTweak;

internal sealed class DeferredListProcessor
{
    internal bool IsEmpty() => _actions.Count == 0;
    private readonly List<ICustomShopPageAction> _actions = new();
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (var action in _actions)
        {
            sb.AppendLine(action.ToString());
        }
        return sb.ToString();
    }

    internal void Enqueue(ICustomShopPageAction action)
    {
        ShopTweakPlugin.Log.LogDebug($"Enqueued {action.ToString()}");
        _actions.Add(action);
    }

    internal void Enqueue(System.Collections.Generic.List<ICustomShopPageAction> actions)
    { 
        ShopTweakPlugin.Log.LogDebug($"Enqueueing {actions.Count}");

        foreach (var action in actions)
        {
            ShopTweakPlugin.Log.LogDebug($"Enqueued {action.ToString()}");
        }
        _actions.AddRange(actions);
    }

    internal void ApplyAll(ref NpcShopTable shop)
    {
        foreach (var action in _actions)
        {
            action.Apply(ref shop);
        }
        _actions.Clear();
    }
}

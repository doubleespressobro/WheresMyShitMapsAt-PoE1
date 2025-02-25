using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using System.Numerics;

namespace WheresMyShitMapsAt.Types;

public readonly record struct MapInfo(NormalInventoryItem Item, Mods Mods) { }

public readonly record struct MapHighlightInfo(
    Vector2 Center,
    bool HasBadMod,
    bool HasGoodMod,
    NormalInventoryItem Item);
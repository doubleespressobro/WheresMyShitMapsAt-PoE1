using System;
using System.Collections.Generic;
using System.Linq;
using WheresMyShitMapsAt.Core;
using WheresMyShitMapsAt.Types;
using WheresMyShitMapsAt.Cache;
using WheresMyShitMapsAt.Settings;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory;
using ExileCore.Shared.Enums;
using ExileCore;
using ExileCore.Shared.Helpers;

namespace WheresMyShitMapsAt;

public sealed class WheresMyShitMapsAt : BaseSettingsPlugin<WheresMyShitMapsAtSettings>
{
    private static WheresMyShitMapsAt _instance;
    private readonly HighlightCache _highlightCache;
    private readonly MapHighlighter _highlighter;
    private NormalInventoryItem _previewItem = null;

    public static WheresMyShitMapsAt Instance { get => _instance; set => _instance = value; }

    public WheresMyShitMapsAt()
    {
        _highlightCache = new HighlightCache();
        _highlighter = new MapHighlighter();

        Instance = this;
    }

    public override bool Initialise()
    {
        _highlighter.Initialise(Graphics);

        return true;
    }

    public override Job Tick()
    {
        if (!Settings.Enable.Value)
            return null;

        var newHighlights = new Dictionary<long, MapHighlightInfo>();

        ProcessInventory(newHighlights);
        ProcessStash(newHighlights);

        _highlightCache.Update(newHighlights);

        if (Settings.PreviewHotkey.PressedOnce())
        {
            var element = GameController.IngameState.UIHoverElement;
            if (element?.AsObject<Element>() is { } hoveredElement)
            {
                _previewItem = hoveredElement.AsObject<NormalInventoryItem>();
            }
        }

        return null;
    }

    public NormalInventoryItem GetPreviewItem() => _previewItem;

    private void ProcessInventory(Dictionary<long, MapHighlightInfo> highlights)
    {
        if (!Settings.FilterInventory.Value || !GameController.IngameState.IngameUi.InventoryPanel.IsVisible)
            return;

        var inventoryItems = GameController.IngameState.IngameUi
            .InventoryPanel[InventoryIndex.PlayerInventory]
            .VisibleInventoryItems;

        ProcessItems(inventoryItems, highlights);
    }

    private void ProcessStash(Dictionary<long, MapHighlightInfo> highlights)
    {
        if (!Settings.FilterStash.Value || !GameController.IngameState.IngameUi.StashElement.IsVisible)
            return;

        var stashItems = GameController.IngameState.IngameUi
            .StashElement
            .VisibleStash
            .VisibleInventoryItems;

        ProcessItems(stashItems, highlights);
    }

    private void ProcessItems(
        IEnumerable<NormalInventoryItem> items,
        Dictionary<long, MapHighlightInfo> highlights)
    {
        foreach (var item in items.Where(IsValidMap))
        {
            var mods = item.Item.GetComponent<Mods>();
            var modMatch = MapModMatcher.MatchMods(mods, Settings.Entries);

            if (modMatch.HasAnyMatch)
            {
                highlights[item.Item.Address] = new MapHighlightInfo(
                    Center: item.GetClientRectCache.Center.ToVector2Num(),
                    HasBadMod: modMatch.HasBadMod,
                    HasGoodMod: modMatch.HasGoodMod,
                    Item: item
                );
            }
        }
    }

    public override void Render()
    {
        if (!Settings.Enable.Value)
            return;

        if (!Settings.FilterInventory.Value && !Settings.FilterStash.Value)
            return;

        _highlighter.RenderHighlights(_highlightCache.GetCurrentHighlights());
    }

    private static bool IsValidMap(NormalInventoryItem inventoryItem)
    {
        try
        {
            return inventoryItem?.Item != null
                && inventoryItem.Item.TryGetComponent(out Mods mods)
                && mods.Identified
                && inventoryItem.Item.TryGetComponent(out Map _);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
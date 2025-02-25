using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using WheresMyShitMapsAt.Settings;

namespace WheresMyShitMapsAt.Core;

public sealed class MapModMatcher
{
    public readonly record struct ModMatchResult(bool HasBadMod, bool HasGoodMod)
    {
        public bool HasAnyMatch => HasBadMod || HasGoodMod;
    }

    public static ModMatchResult MatchMods(Mods mods, IEnumerable<TableEntry> entries)
    {
        var activeEntries = entries.Where(x => x.Active).ToList();

        if (activeEntries.Count == 0)
            return new ModMatchResult(false, false);

        var explicitMods = mods.ExplicitMods;
        var badMods = activeEntries.Where(x => x.Type == ModType.Bad);
        var goodMods = activeEntries.Where(x => x.Type == ModType.Good);

        return new ModMatchResult(
            HasBadMod: HasMatchingMod(explicitMods, badMods),
            HasGoodMod: HasMatchingMod(explicitMods, goodMods)
        );
    }

    private static bool HasMatchingMod(
        IEnumerable<ItemMod> explicitMods,
        IEnumerable<TableEntry> targetMods)
    {
        if (!explicitMods.Any() || !targetMods.Any())
            return false;

        var modNames = explicitMods.Select(x => x.Name).ToList();

        return targetMods.Any(entry =>
            modNames.Any(modName =>
                modName.Contains(entry.Name)
            )
        );
    }
}
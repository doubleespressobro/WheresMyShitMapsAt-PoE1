using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WheresMyShitMapsAt.Settings;

public sealed class WheresMyShitMapsAtSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new(false);
    public HotkeyNodeV2 PreviewHotkey { get; set; } = new(Keys.None);
    public ToggleNode FilterStash { get; set; } = new(false);
    public ToggleNode FilterInventory { get; set; } = new(true);
    public List<TableEntry> Entries { get; set; } = [];
    public WheresMyShitMapsAtSettingsMenu Menu { get; set; }

    public WheresMyShitMapsAtSettings()
    {
        Menu = new WheresMyShitMapsAtSettingsMenu(this);
    }
}
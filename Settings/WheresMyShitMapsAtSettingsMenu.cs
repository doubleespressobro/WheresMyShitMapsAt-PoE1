using ExileCore.Shared.Attributes;

namespace WheresMyShitMapsAt.Settings;

[Submenu(RenderMethod = nameof(Render))]
public sealed class WheresMyShitMapsAtSettingsMenu
{
    private readonly TableManager _tableManager;

    public WheresMyShitMapsAtSettingsMenu(WheresMyShitMapsAtSettings settings)
    {
        _tableManager = new TableManager(settings);
    }

    public void Render()
    {
        _tableManager.RenderTable();
    }
}
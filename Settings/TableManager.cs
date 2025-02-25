using ImGuiNET;
using System.Linq;
using System.Numerics;
using ExileCore.PoEMemory.Components;

namespace WheresMyShitMapsAt.Settings;

public sealed class TableManager(WheresMyShitMapsAtSettings settings)
{
    private readonly WheresMyShitMapsAtSettings _settings = settings;
    private string _newBadModName = string.Empty;
    private string _newGoodModName = string.Empty;
    private bool _isModalOpen = false;
    private string _selectedMod = string.Empty;

    private void RenderModTable(ModType modType, ref string newEntryName)
    {
        var entries = _settings.Entries.Where(e => e.Type == modType).ToList();
        var availableSpace = ImGui.GetContentRegionAvail();

        if (ImGui.BeginTable($"##entriesTable{modType}", 3,
            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY,
            new Vector2(availableSpace.X, availableSpace.Y - 30)))
        {
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Active", ImGuiTableColumnFlags.WidthFixed, 60f);
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 70f);
            ImGui.TableHeadersRow();

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var entry = entries[i];
                var index = _settings.Entries.IndexOf(entry);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text(entry.Name);

                ImGui.TableNextColumn();
                bool isActive = entry.Active;
                if (ImGui.Checkbox($"##active{index}", ref isActive))
                {
                    entry.Active = isActive;
                }

                ImGui.TableNextColumn();
                ImGui.PushID(index);
                if (ImGui.Button("Remove"))
                {
                    _settings.Entries.Remove(entry);
                }
                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        var availableWidth = ImGui.GetContentRegionAvail().X;
        var buttonWidth = 60f;
        var spacing = ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetNextItemWidth(availableWidth - buttonWidth - spacing);
        ImGui.InputText($"##newEntry{modType}", ref newEntryName, 100);
        ImGui.SameLine();
        if (ImGui.Button("Add", new Vector2(buttonWidth, 0)) &&
            !string.IsNullOrWhiteSpace(newEntryName))
        {
            _settings.Entries.Add(new TableEntry(newEntryName, modType));
            newEntryName = string.Empty;
        }
    }

    private void RenderPreviewTable()
    {
        var hoveredItem = WheresMyShitMapsAt.Instance.GetPreviewItem();
        var availableSpace = ImGui.GetContentRegionAvail();

        if (ImGui.BeginTable("##previewTable", 2,
            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY,
            new Vector2(availableSpace.X, availableSpace.Y)))
        {
            ImGui.TableSetupColumn("Mod", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 70f);
            ImGui.TableHeadersRow();

            if (hoveredItem != null && hoveredItem.Item.TryGetComponent(out Mods mods))
            {
                foreach (var mod in mods.ExplicitMods)
                {
                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    ImGui.Text(mod.Name);

                    ImGui.TableNextColumn();
                    ImGui.PushID(mod.Name.GetHashCode());
                    if (ImGui.Button("Add"))
                    {
                        _selectedMod = mod.Name;
                        _isModalOpen = true;
                    }
                    ImGui.PopID();
                }
            }

            ImGui.EndTable();
        }

        if (_isModalOpen)
        {
            RenderAddModModal();
        }
    }

    private void RenderAddModModal()
    {
        ImGui.SetNextWindowSize(new Vector2(225, 80));

        ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.Begin("Add Mod", ref _isModalOpen,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.Modal);

        ImGui.Text("Add To:");

        var existsInGoodMods = _settings.Entries.Any(e => e.Type == ModType.Good && e.Name == _selectedMod);
        var existsInBadMods = _settings.Entries.Any(e => e.Type == ModType.Bad && e.Name == _selectedMod);

        if (ImGui.Button("Good Mods", new Vector2(100, 0)) && !existsInGoodMods)
        {
            _settings.Entries.Add(new TableEntry(_selectedMod, ModType.Good));
            _isModalOpen = false;
        }

        if (existsInGoodMods && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Mod already exists in Good Mods");
        }

        ImGui.SameLine();

        if (ImGui.Button("Bad Mods", new Vector2(100, 0)) && !existsInBadMods)
        {
            _settings.Entries.Add(new TableEntry(_selectedMod, ModType.Bad));
            _isModalOpen = false;
        }

        if (existsInBadMods && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Mod already exists in Bad Mods");
        }

        ImGui.End();
    }

    public void RenderTable()
    {
        ImGui.BeginTabBar("##tabs");

        if (ImGui.BeginTabItem("Bad Mods"))
        {
            RenderModTable(ModType.Bad, ref _newBadModName);
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Good Mods"))
        {
            RenderModTable(ModType.Good, ref _newGoodModName);
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Preview"))
        {
            RenderPreviewTable();
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }
}
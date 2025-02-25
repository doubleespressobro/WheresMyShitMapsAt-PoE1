namespace WheresMyShitMapsAt.Settings;

public sealed class TableEntry
{
    public string Name { get; set; }
    public bool Active { get; set; }
    public ModType Type { get; set; }

    public TableEntry(string name, ModType type)
    {
        Name = name;
        Active = false;
        Type = type;
    }
}
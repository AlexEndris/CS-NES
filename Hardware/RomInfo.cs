namespace Hardware;

using Headers;

public sealed record RomInfo
{
    public HeaderFormat Format { get; init; }
    public ushort MapperId { get; init; }
    public byte Submapper { get; init; }
    public Mirroring Mirroring { get; init; }
    public bool FourScreen { get; init; }
    public bool HasBattery { get; init; }
    public int PrgRomSize { get; init; }
    public int ChrRomSize { get; init; }
    public int PrgRamSize { get; init; }
    public int ChrRamSize { get; init; }
    public int PrgNvRamSize { get; init; }

    public string RomName { get; set; }
}

public enum HeaderFormat
{
    INes, Nes2
}
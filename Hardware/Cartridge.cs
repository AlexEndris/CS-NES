using System;

namespace Hardware;

public class Cartridge
{
    public RomInfo RomInfo { get; }

    public IMapper Mapper { get; }
    public byte[] PrgRom { get; }
    public byte[] ChrRom { get; }
    public byte[] PrgRam { get; private set; }

    public bool Interrupt => Mapper.Interrupt;

    private bool storeSaveGame;
    
    public  bool StoreSaveGame
    {
        get
        {
            if (!storeSaveGame)
                return false;
            
            storeSaveGame = false;
            return true;
        }
    }

    private bool loadedSaveGame;

    public Cartridge(RomInfo romInfo, IMapper mapper, byte[] prgMem, byte[] chrMem, byte[] prgRam)
    {
        RomInfo = romInfo;
        Mapper = mapper;
        PrgRom = prgMem;
        ChrRom = chrMem;
        PrgRam = prgRam;
    }

    public bool CpuRead(ushort address, out byte value)
    {
        // Needs to be initialised anyhow
        value = 0;
        if (!Mapper.IsCpuRead(address))
            return false;

        if (address is >= 0x6000 and <= 0x7FFF)
        {
            if (!Mapper.PrgRamEnabled)
                return false;

            value = PrgRam[(address & 0x1FFF)];
            return true;
        }
        
        // If the mapped address doesn't get a value, despite the mapper saying
        // it'll handle the mapping, then the mapper already handled the reading as well
        var mappedAddress = Mapper.CpuRead(address);
        
        if (mappedAddress.HasValue)
            value = PrgRom[mappedAddress.Value];
        
        return true;
    }

    public bool CpuWrite(ushort address, byte value)
    {
        if (!Mapper.IsCpuWrite(address))
            return false;
        
        if (address is >= 0x6000 and <= 0x7FFF)
        {
            if (!Mapper.PrgRamEnabled
                || !Mapper.PrgRamWriteAllowed)
                return false;

            PrgRam[(address & 0x1FFF)] = value;
            SetStoreSaveGame();
            return true;
        }
        
        // If the mapped address doesn't get a value, despite the mapper saying
        // it'll handle the mapping, then the mapper already handled the writing as well
        var mappedAddress = Mapper.CpuWrite(address, value);
        
        if (mappedAddress.HasValue)
            PrgRom[mappedAddress.Value] = value;
        
        return true;
    }

    private void SetStoreSaveGame()
    {
        storeSaveGame = RomInfo.HasBattery;
    }
    
    public void LoadSaveGame(byte[] ram)
    {
        if (!RomInfo.HasBattery
            || loadedSaveGame)
            return;
        
        PrgRam = ram;
        loadedSaveGame = true;
    }
    
    public bool PpuRead(ushort address, out byte value)
    {
        if (Mapper.PpuRead(address, out var mappedAddress))
        {
            value = ChrRom[mappedAddress];
            return true;
        }

        value = 0;
        return false;
    }

    public bool PpuWrite(ushort address, byte value)
    {
        if (!Mapper.PpuWrite(address, out var mappedAddress))
            return false;
        
        ChrRom[mappedAddress] = value;
        return true;
    }
}

namespace Hardware;

public static class Memory
{

    public static bool CrossesPageBoundary(ushort baseAddress, ushort actualAddress)
    {
        return (baseAddress & 0xFF00) != (actualAddress & 0xFF00);
    }
}
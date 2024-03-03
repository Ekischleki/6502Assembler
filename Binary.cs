namespace TASI
{
    public class Binary
    {
        readonly uint romSize;
        SortedList<uint, IReserved> reservedItems = new();

        public byte[] GenerateRom()
        {
            byte[] rom = new byte[romSize];
            foreach(IReserved part in reservedItems.Values)
            {
                byte[] compiled = part.GenerateAssembly().ToArray();
                if (compiled.Length != part.ReservedLength)
                    throw new Exception("Part did not generate correct length assembly.");
                Array.Copy(compiled, 0, rom, part.ReservedStart, part.ReservedLength);
            }
            return rom;
        }
        public uint FindSpace(uint length)
        {
            if (reservedItems.Count == 0)
            {
                return 0;
            }

            uint position = 0;
            for (int i = 0; i < reservedItems.Count; i++)
            {
                var reservedSpace = reservedItems.GetValueAtIndex(i);
                uint space = checked(reservedSpace.ReservedStart - position);
                if (space <= length)
                {
                    return position;
                }
                position = reservedSpace.ReservedStart + reservedSpace.ReservedLength;
            }
            throw new Exception("Couldn't find space.");
        }

        public void Reserve(IReserved reserved)
        {
            if (reserved.IsDynamic)
            {
                uint spaceStart = FindSpace(reserved.ReservedLength);
                reserved.ReservedStart = spaceStart;
            }
            reservedItems.Add(reserved.ReservedStart, reserved);
        }
    }

    public interface IReserved
    {
        public uint ReservedStart { get; set; }
        public uint ReservedLength { get; }
        public bool IsDynamic { get; }

        public IEnumerable<byte> GenerateAssembly();
    }

}

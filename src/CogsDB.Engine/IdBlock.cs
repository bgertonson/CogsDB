using System;

namespace CogsDB.Engine
{
    public class IdBlock
    {
        public IdBlock(ulong blockNumber, int size)
        {
            BlockNumber = blockNumber;
            Size = size;
            CurrentId = (BlockNumber - 1)*(ulong)Size;
        }

        public ulong BlockNumber { get; private set; }
        public int Size { get; private set; }
        private ulong CurrentId { get; set; }

        public bool HasIdsRemaining()
        {
            return CurrentId < (BlockNumber * (ulong)Size);
        }

        public ulong GetId()
        {
            if(!HasIdsRemaining()) throw new IndexOutOfRangeException("This Id Block is out of Ids");
            return ++CurrentId;
        }
    }
}

using System;

namespace CogsDB.Engine
{
    public class IdBlock
    {
        public IdBlock(int blockNumber, int size)
        {
            BlockNumber = blockNumber;
            Size = size;
            CurrentId = (BlockNumber - 1)*Size;
        }

        public string Id { get; set; }

        public int BlockNumber { get; private set; }
        public int Size { get; private set; }
        private int CurrentId { get; set; }

        public bool HasIdsRemaining()
        {
            return CurrentId < (BlockNumber * Size);
        }

        public int GetId()
        {
            if(!HasIdsRemaining()) throw new IndexOutOfRangeException("This Id Block is out of Ids");
            return ++CurrentId;
        }
    }
}

using CogsDB.Engine;
using NUnit.Framework;

namespace Tests.Unit
{
    [TestFixture]
    public class IdBlockTests
    {
        [Test]
        public void GetId_WithFirstBlock_IdIsOne()
        {
            var block = new IdBlock(1, 5);

            var id = block.GetId();

            Assert.AreEqual(1, id);
        }

        [Test]
        public void GetId_ExpendAllIds_LastIdIsFive()
        {
            var block = new IdBlock(1, 5);
            int id = 0;
            while(block.HasIdsRemaining())
            {
                id = block.GetId();
            }

            Assert.AreEqual(5, id);
        }

        [Test]
        public void GetId_LastIdOfBlockAndFirstOfNextBlock_AreSequential()
        {
            var blockA = new IdBlock(1, 5);
            var blockB = new IdBlock(2, 5);

            int lastId = 0;
            int nextId = 0;

            while(blockA.HasIdsRemaining())
            {
                lastId = blockA.GetId();
            }

            nextId = blockB.GetId();

            Assert.AreEqual(1, nextId - lastId);
        }
    }
}

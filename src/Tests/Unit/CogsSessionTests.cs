using CogsDB.Engine;
using NUnit.Framework;
using Rhino.Mocks;
using Tests.Data;

namespace Tests.Unit
{
    [TestFixture]
    public class CogsSessionTests
    {
        [Test]
        public void Load_NonExistentDocument_ReturnsNull()
        {
            var id = "does_not_exist";
            var persister = MockRepository.GenerateMock<ICogsPersister>();
            var serializer = MockRepository.GenerateMock<IDocumentSerializer>();

            var session = new CogsSession(persister, serializer);

            persister.Expect(p => p.Get(id)).Return(null);

            var result = session.Load<Person>(id);

            Assert.IsNull(result);
        }

        [Test]
        public void Load_ExistingDocument_ReturnsDocument()
        {
            var id = "person-1";
            var doc = new Document() {Id = id, Content = "DeserializeMe"};
            var bob = DataFactory.GetPerson("Bob", "Twilliger");
            var persister = MockRepository.GenerateMock<ICogsPersister>();
            var serializer = MockRepository.GenerateMock<IDocumentSerializer>();

            var session = new CogsSession(persister, serializer);

            persister.Expect(p => p.Get(id)).Return(doc);
            serializer.Expect(s => s.Deserialize<Person>(doc.Content)).Return(bob);

            var result = session.Load<Person>(id);

            Assert.AreEqual(bob, result);
        }
    }
}

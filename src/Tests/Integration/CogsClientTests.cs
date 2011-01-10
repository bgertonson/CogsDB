using System;
using System.Linq;
using CogsDB.Engine;
using NUnit.Framework;
using Tests.Data;

namespace Tests.Integration
{
    [TestFixture]
    class CogsClientTests
    {
        [Test]
        public void Put_Person_Get_Person()
        {
            var session = GetCogsSession();
            var person = DataFactory.GetPerson("joe", "Schmoe");
            session.Store(person);
            var id = person.Id;
            session.SubmitChanges();

            var result = session.Load<Person>(id);

            Assert.AreEqual(person.Id, result.Id);
            Assert.AreEqual(person.FirstName, result.FirstName);
            Assert.AreEqual(person.LastName, result.LastName);
            Assert.AreEqual(person.Email, result.Email);

            session.Delete(id);
        }

        [Test]
        public void Put_BunchOfPeople_IdsAreSequential()
        {
            var session = GetCogsSession();
            var first = new[] { "joe", "bob", "frank", "john", "mark", "james", "shawn" };
            var last = new[] { "schmoe", "twilliger", "beans", "jacob", "pheonix", "peach", "dead" };
            var people = first.Select((x, i) => DataFactory.GetPerson(x, last[i])).ToList();
            foreach (var person in people)
            {
                session.Store(person);
            }
            session.SubmitChanges();

            var personA = people.First();
            var personB = people.Last();

            var id1 = Int32.Parse(personA.Id.Replace("Person-", ""));
            var id2 = Int32.Parse(personB.Id.Replace("Person-", ""));

            Assert.AreEqual(6, id2 - id1);

            foreach (var person in people)
            {
                session.Delete(person.Id);
            }
        }

        [Test]
        public void Get_MultipleIds_ReadAllObjects()
        {
            var session = GetCogsSession();
            var insert = new[]
                             {
                                 new Person()
                                     {
                                         Id = "person-1001",
                                         FirstName = "Bob",
                                         LastName = "Twilliger",
                                         Email = "blah1@yahoo.com"
                                     },
                                 new Person
                                     {
                                         Id = "person-1002",
                                         FirstName = "Marcus",
                                         LastName = "Pheonix",
                                         Email = "blah2@yahoo.com"
                                     },
                                 new Person
                                     {
                                         Id = "person-1003",
                                         FirstName = "Simon",
                                         LastName = "Says",
                                         Email = "blah3@yahoo.com"
                                     }
                             };
            try
            {
                foreach (var person in insert)
                {
                    session.Store(person);
                }
                session.SubmitChanges();

                var people =
                    session.Load<Person>(new[]
                                             {
                                                 "person-1001", "person-1002", "person-1003"
                                             });
                Assert.AreEqual(3, people.Count());
            }
            finally
            {
                foreach (var person in insert)
                {
                    session.Delete(person.Id);
                }
            }
        }

        [Test]
        public void Get_Edit_Update()
        {
            var session = GetCogsSession();
            var person = DataFactory.GetPerson("Bob", "Twilliger");
            session.Store(person);
            session.SubmitChanges();

            var id = person.Id;

            person = session.Load<Person>(id);
            var name = "Frank";
            person.FirstName = name;
            
            session.Store(person);
            session.SubmitChanges();

            var refresh = session.Load<Person>(id);
            session.Delete(id);
            session.SubmitChanges();

            Assert.AreEqual(name, refresh.FirstName);
        }

        [Test]
        public void Store_ObjectWithCustomId_ItemCreated()
        {
            var org = new Organization() { Name = "Microsoft", Key = "MS" };
            var session = GetCogsSession();
            session.Store(org);
            session.SubmitChanges();

            var result = session.Load<Organization>("MS");

            Assert.AreEqual(org.Name, result.Name);

            session.Delete(org.Id);
        }

        private CogsSession GetCogsSession()
        {
            var persister = new SqlPersister("integration");
            var serializer = new JsonSerializer();
            return new CogsSession(persister, serializer);
        }


    }
}

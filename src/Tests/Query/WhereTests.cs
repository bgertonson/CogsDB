using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogsDB.Engine;
using CogsDB.Engine.Linq;
using NUnit.Framework;
using Tests.Data;

namespace Tests.Query
{
    [TestFixture]
    public class WhereTests
    {
        [Test]
        public void AddPerson_QueryByEmail_GetResult()
        {
            var session = new CogsSession(new SqlPersister("sqlce"), new JsonSerializer());
            var input = DataFactory.GetPerson("bryan", "gertonson");
            session.Store(input);
            session.SubmitChanges();

            var person = session.Query<Person>().Where(p => p.Email == "bryan.gertonson@gmail.com").First();

            Assert.AreEqual(input.Id, person.Id);
        }
    }
}

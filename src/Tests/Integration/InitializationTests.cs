using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogsDB.Engine;
using CogsDB.Engine.Configuration;
using NUnit.Framework;

namespace Tests.Integration
{
    [TestFixture]
    public class InitializationTests
    {
        [Test]
        public void Configure_Persistence_SqlCE()
        {
//            Cogs.Configure()
//                .Storage(MsSql.CE4(() => null));
//
//             Cogs.Configure()
//                 .Storage(MsSql.CE4(c =>
//                     c.ConnectionString(c =>
//                         c.FromKey("sql"))))
//                 .SessionManagement(c =>
//                     c.InSingletonScope())
//                 .Initialize()
//                 .BuildSessionFactory();
        }
    }
}

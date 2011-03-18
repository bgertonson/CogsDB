using System;
using System.Threading;
using CogsDB.Engine;
using NUnit.Framework;
using Tests.Data;

namespace Tests.Integration
{
    [TestFixture]
    public class ThreadTests
    {
        private ManualResetEvent _reset = new ManualResetEvent(false);
        private int _count = 0;
        private int _expected = 0;
        private object _lock = new object();

        [Test]
        public void Add100People_UsingThreadPool()
        {
            var rnd = new Random();
            var first = new[]
                            {
                                "Bob", "Frank", "Bill", "Martha", "Joan", "Jacob", "Franz", "Gunther", "Gabrielle",
                                "Ashley"
                            };
            var last = new[] { "Twilliger", "Jones", "Smith", "Baker", "Ferdinand", "Cook", "Thompson" };
            var domain = new[] { "gmail", "yahoo", "hotmail" };


            _expected = 100;
            for (int i = 0; i < _expected; i++ )
            {
                var f = first[rnd.Next(first.Length)];
                var l = last[rnd.Next(last.Length)];
                var d = domain[rnd.Next(domain.Length)];

                var person = new Person()
                {
                    FirstName = f,
                    LastName = l,
                    Email = String.Format("{0}.{1}@{2}.com", f, l, d)
                };

                ThreadPool.QueueUserWorkItem(ThreadPoolWork, person);
            }
            _reset.WaitOne();
        }

        private void ThreadPoolWork(object data)
        {
            var person = data as Person;

            var session = GetCogsSession();
            session.Store(person);
            session.SubmitChanges();

            lock (_lock)
            {
                _count++;
                if (_count == _expected)
                    _reset.Set();
            }

        }

        private CogsSession GetCogsSession()
        {
            var persister = new SqlPersister("sqlce");
            var serializer = new JsonSerializer();
            return new CogsSession(persister, serializer);
        }

    }
}

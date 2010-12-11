using System;

namespace Tests.Data
{
    public class DataFactory
    {
        public static Person GetPerson(string first, string last)
        {
            return new Person() { FirstName = first, LastName = last, Email = String.Format("{0}.{1}@gmail.com", first, last)};
        }
    }
}

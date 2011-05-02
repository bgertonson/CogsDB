using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogsDB.Engine.Linq
{
    public static class CogsSessionExtension
    {
        public static IQueryable<T> Query<T>(this CogsSession session)
        {
            return new CogsQuery<T>(new CogsQueryProvider());
        }
    }
}

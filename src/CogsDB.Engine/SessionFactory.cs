using System;

namespace CogsDB.Engine
{
    public class SessionFactory
    {
        public static Func<ICogsSession> GetSession = () => null;
    }
}
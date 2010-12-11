using System;
using System.Web;

namespace CogsDB.Engine
{
    public class WebSessionManager: ICogsSessionManager
    {
        private readonly HttpContextBase _context;
        private const string SessionKey = "COGS_SESSION_KEY";

        public WebSessionManager(HttpContextBase context)
        {
            _context = context;
        }

        public ICogsSession GetSession()
        {
            var session = _context.Items[SessionKey] as ICogsSession;
            if (session != null) return session;
            
            session = SessionFactory.GetSession();
            _context.Items.Add(SessionKey, session);
            return session;
        }
    }

    public class StaticSessionManager: ICogsSessionManager
    {
        private static ICogsSession _session = SessionFactory.GetSession();

        public ICogsSession GetSession()
        {
            return _session;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CogsDB.Engine
{
    public class SessionManagerFactory
    {
        public static Func<ICogsSessionManager> GetSessionManager =
            () => new WebSessionManager(new HttpContextWrapper(HttpContext.Current));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogsDB.Engine.Storage
{
    public abstract class SqlStorageBase: ICogsStorage
    {
        public abstract void Initialize();
    }

    public interface ICogsStorage
    {
        
    }
}

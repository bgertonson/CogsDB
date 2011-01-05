using System;
using System.Collections.Generic;

namespace CogsDB.Engine
{
    public class IdentityServer: IIdentityServer
    {
        private static IDictionary<Type, IdBlock> _idServer = new Dictionary<Type, IdBlock>();
        private object _lock = new object();
        private const int BlockSize = 5;

        public IdentityServer()
        {
        }

        public string GetNextIdentity<T>()
        {
            var server = GetServerForType(typeof (T));
            var id = String.Format("{0}-{1}", typeof (T).Name, server.GetId());
            return id;
        }

        private IdBlock GetServerForType(Type type)
        {
            if (!_idServer.ContainsKey(type)) return InitializeForType(type);
            var block = _idServer[type];
            return !block.HasIdsRemaining() ? GetNextBlock(type) : block;
        }

        private IdBlock InitializeForType(Type type)
        {
            IdBlock block = GetNewBlock(type);
            _idServer.Add(type, block);
            return block;
        }

        private IdBlock GetNextBlock(Type type)
        {
            var block = GetNewBlock(type);
            _idServer[type] = block;
            return block;
        }

        private IdBlock GetNewBlock(Type type)
        {
            var id = GetIdBlockDocumentId(type);
            IdBlock block;
            lock(_lock)
            {
                var storage = new CogsStorage();
                var session = storage.OpenSession();

                block = session.Load<IdBlock>(id);
                if(block == null)
                {
                    block = new IdBlock(0, BlockSize) {Id = id};
                }

                block = new IdBlock(block.BlockNumber + 1, BlockSize) {Id = id};

                session.Store<IdBlock>(block);
                session.SubmitChanges();
            }
            return block;
        }

        private string GetIdBlockDocumentId(Type type)
        {
            return String.Format("identityblock.{0}", type.Name);
        }
    }
}

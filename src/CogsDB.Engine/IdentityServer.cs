using System;
using System.Collections.Generic;

namespace CogsDB.Engine
{
    public class IdentityServer: IIdentityServer
    {
        private static IDictionary<Type, IdBlock> _idServer = new Dictionary<Type, IdBlock>();
        private IIdentityProvider _idProvider;

        public IdentityServer(IIdentityProvider idProvider)
        {
            _idProvider = idProvider;
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
            int blockNumber = _idProvider.GetNextBlock(type.Name);
            return new IdBlock(blockNumber, 5);
        }
    }
}

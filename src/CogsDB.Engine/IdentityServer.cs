using System;
using System.Collections.Generic;
using CogsDB.Engine.Documents;

namespace CogsDB.Engine
{
    public class IdentityServer: IIdentityServer
    {
        private static IDictionary<Type, IdBlock> _idServer = new Dictionary<Type, IdBlock>();
        private ICogsSession _session;
        private object _lock = new object();

        public IdentityServer(ICogsSession session)
        {
            _session = session;
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
            lock (_lock)
            {
                var tracker = _session.Load<IdTrackingDocument>(CogsSystem.Ids.IdentityTrackingDocument);
                if (tracker == null) tracker = BuildTracker();
                var typeTracker = tracker.GetTracker(type);
                typeTracker.Increment();
                (_session as CogsSession).StoreImmediate<IdTrackingDocument>(tracker);
                return new IdBlock(typeTracker.LastBlock, typeTracker.BlockSize);
            }
        }

        private IdTrackingDocument BuildTracker()
        {
            var doc = new IdTrackingDocument();
            return doc;
        }
    }
}

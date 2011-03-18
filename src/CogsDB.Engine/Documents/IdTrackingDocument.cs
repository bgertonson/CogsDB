using System;
using System.Collections.Generic;
using System.Linq;

namespace CogsDB.Engine.Documents
{
    public class IdTrackingDocument
    {
        public IdTrackingDocument()
        {
            Trackers = new List<IdTracker>();
            Id = CogsSystem.Ids.IdentityTrackingDocument;
        }

        public String Id { get; set; }

        public List<IdTracker> Trackers { get; set; }

        public IdTracker GetTracker<T>()
        {
            var type = typeof(T);
            return GetTracker(type);
        }

        public IdTracker GetTracker(Type type)
        {
            var name = type.Name;
            var tracker = Trackers.FirstOrDefault(x => x.Type == name);
            if (tracker == null)
            {
                tracker = new IdTracker { Type = name, LastBlock = 0, BlockSize = 5 };
                Trackers.Add(tracker);
            }
            return tracker;
        }

        public class IdTracker
        {
            public string Type { get; set; }
            public int LastBlock { get; set; }
            public int BlockSize { get; set; }

            public void Increment()
            {
                LastBlock++;
            }
        }
    }
}

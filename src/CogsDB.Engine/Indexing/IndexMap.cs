using System;
using System.Collections.Generic;

namespace CogsDB.Engine.Indexing
{
    public class IndexMap
    {
        private IList<IndexMapEntry> _entries = new List<IndexMapEntry>();
        
        public void AddEntry(IndexMapEntry entry)
        {
            _entries.Add(entry);
        }

        public static IndexMap InferMap<T>()
        {
            var map = new IndexMap();
            Type type = typeof (T);
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.PropertyType.Equals(typeof(String))) continue;

                IndexMapEntry entry;
                var name = propertyInfo.Name;
                if(name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    entry = new IndexMapEntry(name, true, false);
                    map.AddEntry(entry);
                    continue;
                }

                entry = new IndexMapEntry(name, true, false);
                map.AddEntry(entry);
            }
            return map;
        }
    }

    public class IndexMapEntry
    {
        public IndexMapEntry(string field, bool store, bool analyze)
        {
            Field = field;
            Store = store;
            Analyze = analyze;
        }

        public string Field { get; private set; }
        public bool Store { get; private set; }
        public bool Analyze { get; private set; }
    }
}

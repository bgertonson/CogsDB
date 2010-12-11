using System;

namespace CogsDB.Engine
{
    public class Document
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Metadata { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public bool IsNew { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace CogsDB.Management.Models
{
    public class DocumentStoreListViewModel
    {
        public DocumentStoreListViewModel()
        {
            DocumentStores = new List<String>();
        }

        public IEnumerable<String> DocumentStores { get; set; }
    }
}
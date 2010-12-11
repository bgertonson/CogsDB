using System.Collections.Generic;
using CogsDB.Management.Core.Entities;

namespace CogsDB.Management.Models
{
    public class DocumentListViewModel: DocumentBaseViewModel
    {
        public DocumentListViewModel()
        {
            Documents = new List<DocumentSummary>();
        }

        public List<DocumentSummary> Documents { get; set; }
    }
}
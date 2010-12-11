namespace CogsDB.Management.Models
{
    public abstract class DocumentBaseViewModel
    {
        public string DocStore { get; set; }
        public string DocType { get; set; }
        public string DocId { get; set; }
    }
}
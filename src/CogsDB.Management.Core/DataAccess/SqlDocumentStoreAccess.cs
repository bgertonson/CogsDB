using System;
using System.Collections.Generic;
using System.Data;
using CogsDB.Engine;
using CogsDB.Management.Core.Contracts;
using CogsDB.Management.Core.Entities;
using Opsbasoft.Blocks.Data;

namespace CogsDB.Management.Core.DataAccess
{
    public class SqlDocumentStoreAccess: IDocumentStoreAccess
    {
        private const string SELECT_DOC_TYPES = "Select Distinct [type] From [documents]";
        private const string SELECT_DOCS = "Select [id], [create-date], [modify-date] From [documents] Where [type] = @type";
        
        public IEnumerable<string> ListDocumentTypes(string docStore)
        {
            var types = new List<string>();
            var db = DatabaseFactory.CreateDatabase(docStore);
            using(var cmd = db.GetSqlStringCommand(SELECT_DOC_TYPES))
            using(var reader = db.ExecuteReader(cmd))
            while(reader.Read())
            {
                types.Add(DBNullConvert.ToString(reader[0]));
            }
            return types;
        }

        public IEnumerable<DocumentSummary> ListDocuments(string docStore, string docType)
        {
            var docs = new List<DocumentSummary>();
            var db = DatabaseFactory.CreateDatabase(docStore);
            using(var cmd = db.GetSqlStringCommand(SELECT_DOCS))
            {
                db.AddInParameter(cmd, "type", DbType.String, docType);
                using(var reader = db.ExecuteReader(cmd))
                while(reader.Read())
                {
                    var summary = new DocumentSummary()
                                      {
                                          Id = DBNullConvert.ToString(reader[0]),
                                          CreateDate = DBNullConvert.To<DateTime>(reader[1]),
                                          ModifyDate = DBNullConvert.To<DateTime>(reader[2])
                                      };
                    docs.Add(summary);
                }
            }
            return docs;
        }

        public Document GetDocument(string docStore, string docId)
        {
            var persister = new SqlPersister(docStore);
            var document = persister.Get(docId);
            return document;
        }
    }
}

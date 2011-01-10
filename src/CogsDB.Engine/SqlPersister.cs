using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using System.Linq;
using Opsbasoft.Blocks.Data;

namespace CogsDB.Engine
{
    public class SqlPersister : ICogsPersister, IIdentityProvider
    {
        private String Connection { get; set; }
        private IDictionary<string, object> _cache = new Dictionary<string, object>();

        #region [SQL]
        private const String DELETE_SQL = "delete from documents where id = @id";
        private const String GET_SQL = "select [id], [type], [doc], [meta], [create-date], [modify-date] from [documents] where [id] = @id";

        private const String GET_MANY_SQL =
            "select [id], [type], [doc], [meta], [create-date], [modify-date] from [documents] where [id] in ({0})";
        private const string PUT_CREATE_SQL = "insert into documents values (@id, @type, @doc, @meta, @create, @modify)";
        private const string PUT_UPDATE_SQL = "update documents set [doc] = @doc, [meta] = @meta, [modify-date] = @modify where [id] = @id";

        private const string GET_ALL_OF_TYPE_SQL = "select [id], [type], [doc], [meta], [create-date], [modify-date] from [documents] where [type] = @type";

        private const string IDENTITY_SQL =
            @"declare @num int
select @num = [last-block]
from   [id-blocks]
where  [type] = @type

if @num is null
	begin
		insert into [id-blocks] values (@type, 1)
		set @num = 1
	end
else
	begin
		set @num = @num + 1
		update [id-blocks] set [last-block] = @num
		where  [type] = @type
	end
select @num";
        #endregion

        public SqlPersister(string connection)
        {
            Connection = connection;
        }

        public void Put(Document document)
        {
            if (document.IsNew) Create(document);
            else Update(document);
        }

        private void Create(Document document)
        {
            var db = DatabaseFactory.CreateDatabase(Connection);
            using (var cmd = db.GetSqlStringCommand(PUT_CREATE_SQL))
            {
                db.AddInParameter(cmd, "id", DbType.String, document.Id);
                db.AddInParameter(cmd, "type", DbType.String, document.Type);
                db.AddInParameter(cmd, "doc", DbType.String, document.Content);
                db.AddInParameter(cmd, "meta", DbType.String, document.Metadata);
                db.AddInParameter(cmd, "create", DbType.DateTime, document.CreateDate);
                db.AddInParameter(cmd, "modify", DbType.DateTime, document.ModifyDate);

                db.ExecuteNonQuery(cmd);
            }
        }

        private void Update(Document document)
        {
            var db = DatabaseFactory.CreateDatabase(Connection);
            using (var cmd = db.GetSqlStringCommand(PUT_UPDATE_SQL))
            {
                db.AddInParameter(cmd, "id", DbType.String, document.Id);
                db.AddInParameter(cmd, "doc", DbType.String, document.Content);
                db.AddInParameter(cmd, "meta", DbType.String, document.Metadata);
                db.AddInParameter(cmd, "modify", DbType.DateTime, document.ModifyDate);

                db.ExecuteNonQuery(cmd);
            }

        }

        public void Put(IEnumerable<Document> documents)
        {
            foreach (var doc in documents)
            {
                Put(doc);
            }
        }

        public Document Get(string id)
        {
            Document document;
            var db = DatabaseFactory.CreateDatabase(Connection);
            using(var cmd = db.GetSqlStringCommand(GET_SQL))
            {
                db.AddInParameter(cmd, "id", DbType.String, id);
                using (var reader = db.ExecuteReader(cmd, true))
                {
                    if (!reader.Read()) return null;
                    document = HydrateDocument(reader);
                    //TODO: Caching
                }
            }
            return document;
        }

        public IEnumerable<Document> Get(string[] ids)
        {
            //TODO: check cache, build new id array for remaining items
            var @params = ids.Select((id, index) => String.Format("@id{0}", index)).ToArray();
            var sql = String.Format(GET_MANY_SQL, String.Join(",", @params));

            IList<Document> documents = new List<Document>();
            var db = DatabaseFactory.CreateDatabase(Connection);
            using(var cmd = db.GetSqlStringCommand(sql))
            {
                for (int i = 0; i < ids.Length; i++)
                    db.AddInParameter(cmd, @params[i], DbType.String, ids[i]);
                using (var reader = db.ExecuteReader(cmd, true))
                {
                    while (reader.Read())
                    {
                        Document doc = HydrateDocument(reader);
                        //TODO: Caching
                        documents.Add(doc);
                    }
                }
            }
            return documents;
        }

        public IEnumerable<Document> GetAll(string type)
        {
            IList<Document> documents = new List<Document>();

            var db = DatabaseFactory.CreateDatabase(Connection);
            using(var cmd = db.GetSqlStringCommand(GET_ALL_OF_TYPE_SQL))
            {
                db.AddInParameter(cmd, "type", DbType.String, type);
                using(var reader = db.ExecuteReader(cmd))
                {
                    while(reader.Read())
                    {
                        var doc = HydrateDocument(reader);
                        //TODO: Caching
                        documents.Add(doc);
                    }
                }
            }
            return documents;
        }


        public void Delete(string id)
        {
            var db = DatabaseFactory.CreateDatabase(Connection);
            using (var cmd = db.GetSqlStringCommand(DELETE_SQL))
            {
                db.AddInParameter(cmd, "id", DbType.String, id);
                db.ExecuteNonQuery(cmd);
                //TODO: Remove from cache
            }
        }

        public int GetNextBlock(string type)
        {
            int blockNum;

            var db = DatabaseFactory.CreateDatabase(Connection);
            using (var transaction = new TransactionScope())
            using (var cmd = db.GetSqlStringCommand(IDENTITY_SQL))
            {
                db.AddInParameter(cmd, "type", DbType.String, type);
                blockNum = DBNullConvert.To<int>(db.ExecuteScalar(cmd));
                transaction.Complete();
            }
            return blockNum;
        }

        private Document HydrateDocument(IDataReader reader)
        {
            return new Document
                       {
                           Id = DBNullConvert.ToString(reader[0]),
                           Type = DBNullConvert.ToString(reader[1]),
                           Content = DBNullConvert.ToString(reader[2]),
                           Metadata = DBNullConvert.ToString(reader[3]),
                           CreateDate = DBNullConvert.To<DateTime>(reader[4]),
                           ModifyDate = DBNullConvert.To<DateTime>(reader[5])
                       };
        }
    }
}

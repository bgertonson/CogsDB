using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;

namespace CogsDB.Engine.Storage
{
    public static class TransactionScopeConnections
    {
        private static Dictionary<Transaction, Dictionary<string, DbConnection>> transactionConnections = new Dictionary<Transaction, Dictionary<string, DbConnection>>();

        public static DbConnection GetConnection(Database database)
        {
            Transaction transaction = Transaction.Current;

            if (transaction == null)
                return null;

            DbConnection connection = null;

            Dictionary<string, DbConnection> connectionList;
            transactionConnections.TryGetValue(transaction, out connectionList);

            if (connectionList != null)
            {
                connectionList.TryGetValue(database.ConnectionString, out connection);
                if (connection != null)
                    return connection;
            }
            else
            {
                // we need to create a connection list for this transaction
                connectionList = new Dictionary<string, DbConnection>();
                lock (transactionConnections)
                    transactionConnections.Add(transaction, connectionList);
            }

            if (!connectionList.ContainsKey(database.ConnectionString))
            {
                connection = database.GetNewOpenConnection();
                transaction.TransactionCompleted += OnTransactionCompleted;
                connectionList.Add(database.ConnectionString, connection);
            }

            return connection;
        }

        private static void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            Dictionary<string, DbConnection> connectionList;

            transactionConnections.TryGetValue(e.Transaction, out connectionList);
            if (connectionList == null)
                return;

            lock (transactionConnections)
                transactionConnections.Remove(e.Transaction);

            foreach (DbConnection connection in connectionList.Values)
            {
                connection.Dispose();
            }
        }
    }
}
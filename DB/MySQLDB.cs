using MySqlConnector;
using Testing.Model.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.RecordSource;
using System.Transactions;
using System.Windows;

namespace Testing.DB
{

    public class MySQLDB<M> : IMySQLDB where M : AbstractModel, IDB<M>, new() {
        private MySqlConnection Connection;
        private MySqlCommand Command = null!;
        public M Record { get; set; } = new();
        public RecordSource<M> RecordSource { get; set; } = new();
        public bool IsConnected { get; private set; }
        private MySqlTransaction? Transaction;

        public MySQLDB()=>
        Connection = new MySqlConnection(MainDB.ConnectionString);

        public void OpenConnection()
        {
            Connection.Open();
            IsConnected = true;
        }

        public void CloseConnection()
        {
            Connection.Close();
            IsConnected = false;
        }

        #region Transaction
        public void StartTransaction()
        {
            OpenConnection();
            RunStatement("SET autocommit = 0;");
            Transaction = Connection.BeginTransaction();
            Command = new();
            Command.Connection = Connection;
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
            Command.Dispose();
            RunStatement("SET autocommit = 1;");
            CloseConnection();
        }

        public void RollBack()
        {
            Transaction.Rollback();
            Command.Dispose();
            RunStatement("SET autocommit = 1;");
            CloseConnection();
        }

        public object UpdateTransaction(object? obj)
        {
            M record = (M)obj;
            Command.CommandText = record.SQLQuery(QueryType.UPDATE);
            Command.Transaction = Transaction;
            record.Params(Command.Parameters);
            Command.ExecuteScalar();
            Command.Parameters.Clear();
            record.IsDirty = false;
            return Record;
        }

        public object InsertTransaction(object? obj)
        {
            M record = (M)obj;
            Command.CommandText = record.SQLQuery(QueryType.INSERT);
            Command.Transaction = Transaction;
            record.Params(Command.Parameters);
            Command.ExecuteScalar();
            Command.Parameters.Clear();
            record.SetPrimaryKey(LastInsertedID2());
            record.IsDirty = false;
            return Record;
        }

        #endregion
        #region UpdateStatement
        public void Update(M? record, string sql)
        {
            Record = record;
            Command = new(sql, Connection);
            Record.Params(Command.Parameters);
            Command.ExecuteNonQuery();
            Record.IsDirty = false;
            RecordSource.UpdateRecord(Record);
            Command.Parameters.Clear();
            Command.Dispose();
        }

        public void Update(M record) => Update(record, record.SQLQuery(QueryType.UPDATE));

        #endregion

        #region InsertStatement
        public void Insert(M? record, string sql)
        {
            Record = record;
            Command = new(sql, Connection);
            Record.Params(Command.Parameters);
            try
            {
                Command.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("QUERY FAILED","TIME OUT");
                return;
            }
            Command.Parameters.Clear();
            Record.SetPrimaryKey(LastInsertedID());
            Record.IsDirty= false;
            RecordSource.InsertRecord(Record);
            Command.Dispose();
        }
        public void Insert(M record) => Insert(record, record.SQLQuery(QueryType.INSERT));
        #endregion


        private UInt64 LastInsertedID()
        {
            Command = new("SELECT LAST_INSERT_ID();", Connection);
            return (UInt64)Command.ExecuteScalar();
        }

        private UInt64 LastInsertedID2()
        {
            Command.CommandText= "SELECT LAST_INSERT_ID();";
            return (UInt64)Command.ExecuteScalar();
        }



        #region SelectStatement
        public RecordSource<M> Select(string sql)
        {
            RecordSource<M> res = new();
            Command = new(sql, Connection);
            var result = Command.ExecuteReader();
            while (result.Read())
            {
                res.InsertRecord(Record.GetRecord(result));
            }
            return res;
        }

        public void Select()
        {
            RecordSource.Clear();
            Command = new(Record.SQLQuery(QueryType.SELECT), Connection);
            var result = Command.ExecuteReader();
            while (result.Read())
            {
                RecordSource.InsertRecord(Record.GetRecord(result));
            }
        }
        #endregion

        //Command = new("PRAGMA foreign_keys = ON;", Connection);
        //Command.ExecuteNonQuery();

        #region DeleteStatement
        public void Delete(M record)=>Delete(record, record.SQLQuery(QueryType.DELETE));

        public void Delete()=>Delete(Record.SQLQuery(QueryType.DELETE));

        public void Delete(string sql)=>Delete(null,sql);

        public void Delete(M? record, string sql)
        {
            if (!RecordIsSet(record)) return;
            Command = new(sql, Connection);
            Record.Params(Command.Parameters);
            Command.ExecuteNonQuery();
            Command.Parameters.Clear();
            Command.Dispose();
            RecordSource.DeleteRecord(Record);
        }

        public void DeleteRecords(IEnumerable<M> source,string sql)
        {
            Record = new();                        
            Command = new(sql, Connection);
            Record.Params(Command.Parameters);
            Command.ExecuteNonQuery();
            foreach (M record in source)
            {               
                RecordSource.DeleteRecord(record);
            }
            Command?.Parameters.Clear();
            Command?.Dispose();
        }

        #endregion

        public bool IsNewRecord() => Record.IsNewRecord;

        public void AddChildreSource(IRecordSource source)
        {
            RecordSource.Children.Add(source);
        }

        public void RunStatement(string sql)
        {
            Command = new(sql, Connection);
            Command.ExecuteNonQuery();
        }

        private bool RecordIsSet(M? record) {
            if (record != null) Record = record;
            return (Record != null);
        }
    }

    #region IMySqlDB
    public interface IMySQLDB
    {
        public void CommitTransaction();
        public void RollBack();
        public void StartTransaction();
        public object? InsertTransaction(object? obj);
        public object? UpdateTransaction(object? obj);
    }

    #endregion
}

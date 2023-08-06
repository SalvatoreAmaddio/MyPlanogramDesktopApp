using MySqlConnector;
using Testing.Model.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DB
{
    public interface IDB<M> where M : AbstractModel
    {
        public string SQLQuery(QueryType Query);
        public M GetRecord(MySqlDataReader reader);
        public void Params(MySqlParameterCollection param);
        public void SetPrimaryKey(UInt64 ID);
    }

    public enum QueryType
    {
        SELECT,
        UPDATE,
        DELETE,
        INSERT
    }
}

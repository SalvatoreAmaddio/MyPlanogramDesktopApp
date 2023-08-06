using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.DB;
using Testing.Model.Abstracts;

namespace MyPlanogramDesktopApp.Model
{
    public class Department : AbstractModel, IDB<Department>
    {
        #region backprop
        int _departmentid;
        string _departmentname=string.Empty;
        #endregion

        #region Props
        public int DepartmentID { get=>_departmentid; set=>Set<int>(ref value,ref _departmentid,nameof(DepartmentID)); }
        public string DepartmentName { get => _departmentname; set => Set<string>(ref value, ref _departmentname, nameof(DepartmentName)); }
        #endregion
        
        public Department() { }

        public Department(int departmentid) 
        { 
            DepartmentID= departmentid;
            IsDirty = false;
        }

        public Department(MySqlDataReader reader)
        {
            DepartmentID = reader.GetInt32(0);
            DepartmentName = reader.GetString(1);
            IsDirty = false;
        }

        public override bool IsNewRecord => DepartmentID==0;

        public override void SetForeignKeys()
        {
        }

        public static bool IsAncillary(string val) => val.ToLower().Contains("ancillary");

        public string SQLQuery(QueryType Query)
        {
            switch (Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Department;";
                case QueryType.DELETE:
                    return "DELETE FROM Department WHERE DepartmentID=@DepartmentID;";
                case QueryType.UPDATE:
                    return "UPDATE Department SET DepartmentName=@DepartmentName WHERE DepartmentID=@DepartmentID;";
                case QueryType.INSERT:
                    return "INSERT INTO Department (DepartmentName) VALUES (@DepartmentName);";
                default: return string.Empty;
            }
        }

        public Department GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(DepartmentID),DepartmentID);
            param.AddWithValue(nameof(DepartmentName), DepartmentName);
        }

        public void SetPrimaryKey(ulong ID)=>DepartmentID = (int)ID;

        public override bool Equals(object? obj)=>obj is Department department && DepartmentID == department.DepartmentID;

        public override int GetHashCode()=>HashCode.Combine(DepartmentID);

        public override string? ToString() => DepartmentName;
    }
}

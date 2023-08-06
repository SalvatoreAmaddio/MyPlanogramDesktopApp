using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller.AbstractControllers;
using Testing.DB;

namespace MyPlanogramDesktopApp.Controller
{
    public class DepartmentController : AbstractDataListController<Department>
    {
        public DepartmentController() 
        {
            RecordSource.ReplaceRange(MainDB.DBDepartment.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
        }

        public override MySQLDB<Department> DB() => MainDB.DBDepartment;
    }
}

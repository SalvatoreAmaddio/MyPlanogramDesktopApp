using Microsoft.Win32;
using MvvmHelpers.Commands;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;
using Testing.Utilities;

namespace MyPlanogramDesktopApp.Controller
{
    #region MainClass
    public class DeliveryImporterController : AbstractImporterController
    {

        public DeliveryImporterController()=> 
        ButtonLabel = "IMPORT DELIVERY";

        public override IMySQLDB? DBType() => new MySQLDB<Item>();

        public override IRecordSource MainDBSource()=>
        MainDB.DBItem.RecordSource;

        public override async void OpenFileExplorer()
        {
            if (DialogCancelled()) return;
            SetFilePathAndName();

            SetCancelCMD();
            ClearProgress();

            await Task.Run(() =>
            {
                ExcelManager = new DeliveryExcelReader(this, FilePath);
                IsWorking = true;
                ExcelManager.Read();
                IsWorking = false;
                ExcelManager.CommitOrRollBack();
                ResetCMD();
            });
            Transfer();
        }
    }
    #endregion

    #region DeliveryExcelReader
    public class DeliveryExcelReader : AbstractExcelManager
    {
        public override AbstractChecker Checker { get; set; } = new ItemChecker();

        public DeliveryExcelReader(DeliveryImporterController controller, string path) : base(path, controller)
        {
        }


        public override void ReadLogic(int currentRow, int currentColumn)
        {
            if (!CellIsNull(currentRow, 1) && !CellIsNull(currentRow, 2) && !CellIsNull(currentRow, 3))
            {

                var okay=Checker.AddItem(CellValue(currentRow, 1), CellValue(currentRow, 2), CellValue(currentRow, 3).ToLower());
                if (okay)
                {
                        Checker.InsertTransaction();
                        ReportProgress();
                }
            }
        }
    }
    #endregion

    #region ItemChecker
    public class ItemChecker : AbstractChecker
    {
        public Item? Item;

        public override bool AddItem(params string[] values)
        {
            if (!Sys.IsNumeric(values[0])) return false;
            if (Department.IsAncillary(values[2])) return false;
            Item = new(values[0], values[1], values[2]);
            if (!Exists())
            {
                Source.InsertRecord(Item);
                return true;
            }
            return false;
        }

        public override string? ToString() =>
        $"{Item?.ItemID} - {Item?.SKU} - {Item?.ItemName} - {Item?.Department?.DepartmentName} - {Item?.Department?.DepartmentID}";

        public override object? Model() => Item;
    }
    #endregion
}

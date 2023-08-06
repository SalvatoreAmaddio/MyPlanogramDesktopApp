using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller.AbstractControllers;
using Testing.DB;
using Testing.RecordSource;
using Testing.Utilities;

namespace MyPlanogramDesktopApp.Controller
{
    #region MainClass
    public class BarcodeImporterController : AbstractImporterController
    {
        public BarcodeImporterController() 
        {
            ButtonLabel = "IMPORT BARCODES";
        }

        public override IMySQLDB? DBType() => new MySQLDB<Barcode>();

        public override IRecordSource MainDBSource() => MainDB.DBBarcode.RecordSource;

        public async override void OpenFileExplorer()
        {
            if (DialogCancelled()) return;
            SetFilePathAndName();

            SetCancelCMD();
            ClearProgress();

            await Task.Run(() =>
            {
                ExcelManager = new BarcodeExcelReader(this, FilePath);
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

    #region 
    public class BarcodeExcelReader : AbstractExcelManager
    {

        public BarcodeExcelReader(BarcodeImporterController controller, string filepath) : base(filepath,controller) { 
        
        }

        public override AbstractChecker Checker { get; set; } = new BarcodeChecker();

        public override void ReadLogic(int currentRow, int currentColumn)
        {
            if (!CellIsNull(currentRow, 1) && !CellIsNull(currentRow, 2) && !CellIsNull(currentRow, 3))
            {
                var okay = Checker.AddItem(CellValue(currentRow, 1), CellValue(currentRow, 2), CellValue(currentRow, 3));
                if (okay)
                {
                    Checker.InsertTransaction();
                    ReportProgress();
                }
            }

        }
    }
    #endregion

    #region BarcodeChecker
    public class BarcodeChecker : AbstractChecker
    {
        private Barcode? CurrentBarcode;
        private RecordSource<Item> Items = new(MainDB.DBItem.RecordSource);

        public override bool AddItem(params string[] values)
        {
            if (!Sys.IsNumeric(values[0])) return false;
            CurrentBarcode = new(values[0], values[1], values[2]);
            if (!CurrentBarcode.ProperLength()) return false;
//            if (!CurrentBarcode.IsValid()) return false;
            if (Exists()) return false;
            else
                if (ItemExists())
                {
                    Source.InsertRecord(CurrentBarcode);
                    return true;
                }
            return false;
        }

        public bool ItemExists()
        {
            Item? item= Items.FirstOrDefault(s => s.Equals(CurrentBarcode?.Product));
            CurrentBarcode.Product = item;
            return CurrentBarcode.Product != null;
        }
        public override object? Model() => CurrentBarcode;

        public override string? ToString()=>
        $"CODE: {CurrentBarcode?.Code} - ITEM: {CurrentBarcode?.Product?.ItemName}, SKU: {CurrentBarcode?.Product?.SKU}";
    }
    #endregion
}

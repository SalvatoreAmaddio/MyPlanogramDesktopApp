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
    public class ImportOfferController : AbstractImporterController
    {
        public ImportOfferController()=> ButtonLabel = "IMPORT OFFERS";


        public override IMySQLDB? DBType() => new MySQLDB<Item>();

        public override IRecordSource MainDBSource() => MainDB.DBItem.RecordSource;

        public async override void OpenFileExplorer()
        {
            if (DialogCancelled()) return;
            SetFilePathAndName();

            SetCancelCMD();
            ClearProgress();

            await Task.Run(() =>
            {
                ExcelManager = new OfferExcelReader(this, FilePath);
                IsWorking = true;
                ExcelManager.Read();
                IsWorking = false;
                ExcelManager.CommitOrRollBack();
                ResetCMD();
            });
            Transfer();
        }
    }

    #region 
    public class OfferExcelReader : AbstractExcelManager
    {

        public OfferExcelReader(ImportOfferController controller, string filepath) : base(filepath, controller)
        {

        }

        public override AbstractChecker Checker { get; set; } = new OfferChecker();

        public override void ReadLogic(int currentRow, int currentColumn)
        {
            if (!CellIsNull(currentRow, 1) && !CellIsNull(currentRow, 2))
            {
                var okay = Checker.AddItem(CellValue(currentRow, 1), CellValue(currentRow, 2));
                if (okay)
                {
                    Checker.UpdateTransaction();
                    ReportProgress("UPDATING");
                }
            }

        }
    }
    #endregion

    #region BestsellerChecker
    public class OfferChecker : AbstractChecker
    {
        private Item? CurrentItem;
        private Offer? CurrentOffer;
        private RecordSource<Item> Items = MainDB.DBItem.RecordSource;
        private string? SKU;
        private string? Offer;

        public override bool AddItem(params string?[] values)
        {
            SKU = values[0]?.ToString().Trim();
            Offer = values[1]?.ToString().ToLower().Trim();
            if (string.IsNullOrEmpty(SKU)) return false;
            if (string.IsNullOrEmpty(Offer)) return false;

            if (!Sys.IsNumeric(SKU)) return false;
            CurrentItem = Items.FirstOrDefault(s => s.SKU.Equals(SKU));
            if (!ItemExists()) return false;
            CurrentOffer=MainDB.DBOffer.RecordSource.FirstOrDefault(s=>s.OfferName.ToLower().Equals(Offer));
            if (!OfferExists()) return false;
            CurrentItem.Offer = CurrentOffer;
            return true;
        }

        public bool ItemExists() => CurrentItem != null;
        public bool OfferExists() => CurrentOffer != null;

        public override object? Model() => CurrentItem;

        public override string? ToString() =>
        $"ITEM: {CurrentItem} is on offer: {CurrentOffer}";
    }
    #endregion
}

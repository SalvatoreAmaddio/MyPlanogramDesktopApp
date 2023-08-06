using MyPlanogramDesktopApp.Model;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Controller
{
    public class BarcodeListController : AbstractDataListController<Barcode>
    {
        public ItemListController ItemListController { get; } = new();
        BarcodeFilter _barcodeFilter=new(string.Empty);
        public BarcodeFilter BarcodeFilter { get => _barcodeFilter; set => Set<BarcodeFilter>(ref value, ref _barcodeFilter,nameof(BarcodeFilter)); }
        public bool ItemForm { get; set; } = false;

        public BarcodeListController()
        {
            SelectedRecord = RecordSource.FirstOrDefault();
            AfterPropChanged += BarcodeListController_AfterPropChanged;
        }

        private void BarcodeListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Search)))
            {
                BarcodeFilter = new(e.GetValue<string>().ToLower());
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }

            if (e.PropIs(nameof(SelectedRecord)) && ItemForm)
            {
                e.GetValue<Barcode>().Generate();
                return;
            }
        }

        public override MySQLDB<Barcode> DB() => MainDB.DBBarcode;

        public override void DeleteRecord(Barcode record)
        {
            if (ConfirmDialog()== MessageBoxResult.No) return;
            MainDB.DBBarcode.OpenConnection();
            MainDB.DBBarcode.Delete(record);
            MainDB.DBBarcode.CloseConnection();
        }

        public override Barcode CreateNewRecord() => new Barcode();

        public void RemoveInvalidBarcodes()
        {
            MainDB.DBBarcode.OpenConnection();
            RecordSource<Barcode> bar = new(MainDB.DBBarcode.RecordSource);
            foreach (var x in bar)
            {
                if (!x.IsValid())
                {
                    MainDB.DBBarcode.Delete(x);
                }
            }
            MainDB.DBBarcode.CloseConnection();
            MessageBox.Show("Done");
        }
     
    }

}

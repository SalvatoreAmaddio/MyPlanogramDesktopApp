using MvvmHelpers.Commands;
using Testing.Controller.AbstractControllers;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;
using System.Collections;

namespace MyPlanogramDesktopApp.Controller
{

    public class ItemListController : AbstractDataListController<Item> {

        public OfferListController OfferListController { get;} = new();
        public DepartmentController DepartmentController { get;} = new();
        public RecordSource<Barcode> Barcodes { get; }= new();
        private ItemListFilter _itemlistfilter=null!;
        public ItemListFilter ItemListFilter { get => _itemlistfilter; set => Set<ItemListFilter>(ref value, ref _itemlistfilter,nameof(ItemListFilter)); }

        bool _nobarcode = false;
        public bool NoBarcode { get => _nobarcode; set => Set<bool>(ref value, ref _nobarcode, nameof(NoBarcode)); }

        public ItemListController() {
            RecordSource.ReplaceRange(MainDB.DBItem.RecordSource);
            Barcodes.ReplaceRange(MainDB.DBBarcode.RecordSource);
            MainDB.DBBarcode.AddChildreSource(Barcodes);
            SelectedRecord = RecordSource.FirstOrDefault();
            ItemListFilter = new(string.Empty, NoBarcode);
            AfterPropChanged += ItemListController_AfterPropChanged;
        }

        private void ItemListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Search)))
            {
                ItemListFilter = new(e.GetValue<string>(),NoBarcode);
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }

            if (e.PropIs(nameof(NoBarcode))) {
                ItemListFilter = new(Search, e.GetValue<bool>());
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }

        }

        public override MySQLDB<Item> DB() => MainDB.DBItem;
    }

    #region ItemListFilter
    public class ItemListFilter : AbstractFilterAndSort
    {
        public override IRecordSource RecordSource => MainDB.DBItem.RecordSource;
        private bool NoBarcode = false;

        public ItemListFilter(string value,bool excludebarcode) 
        {
            SkipSort = true;
            Search = value.ToLower();
            NoBarcode = excludebarcode;
        }


        public override bool Criteria(object record)
        {
            Item item = (Item)record;
            bool cond1 = item.ToString().ToLower().Contains(Search);
            
            if (NoBarcode)
            {
                bool cond2=item.BarcodeFilter.Count == 0;
                return cond1 && cond2;
            }

            return cond1;
        }

        public override void SortSource(IEnumerable ItemsSource)
        {

        }
    }
    #endregion
}

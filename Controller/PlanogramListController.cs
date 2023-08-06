using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using MvvmHelpers.Commands;
using MyPlanogramDesktopApp.Model;
using MyPlanogramDesktopApp.View.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Controller
{
    public class PlanogramListController : AbstractDataListController<Planogram>
    {

        public RecordSource<Bay> Bays { get; set; } = new();
        public RecordSource<Section> Sections { get; set; } = new();
        public RecordSource<Shelf> Shelves { get; set; } = new();
        public RecordSource<Item> Items { get; set; } = new();
        public RecordSource<Department> Departments { get; set; } = new();
        public ICommand TransferItemCMD { get; }
        public ICommand DropPlanogramCMD { get; }
        public ICommand OpenNewFormItemCMD { get; }

        Section _selectedsection = null!;
        public Section SelectedSection { get => _selectedsection; set => Set<Section>(ref value, ref _selectedsection, nameof(SelectedSection)); }

        Bay _selectedbay = null!;
        public Bay SelectedBay { get => _selectedbay; set => Set<Bay>(ref value, ref _selectedbay, nameof(SelectedBay)); }

        Shelf _selectedshelf = null!;
        public Shelf SelectedShelf { get => _selectedshelf; set => Set<Shelf>(ref value, ref _selectedshelf, nameof(SelectedShelf)); }

        BayFilter _bayFilter = null!;
        public BayFilter BayFilter { get => _bayFilter; set => Set<BayFilter>(ref value, ref _bayFilter, nameof(BayFilter)); }

        ShelvesFilter _shelfFilter = null!;
        public ShelvesFilter ShelfFilter { get => _shelfFilter; set => Set<ShelvesFilter>(ref value, ref _shelfFilter, nameof(ShelfFilter)); }

        ItemFilter _itemFilter = null!;
        public ItemFilter ItemFilter { get => _itemFilter; set => Set<ItemFilter>(ref value, ref _itemFilter, nameof(ItemFilter)); }

        FilterPlanogram _planogramfilter = null!;
        public FilterPlanogram PlanogramFilter { get => _planogramfilter; set => Set<FilterPlanogram>(ref value, ref _planogramfilter, nameof(PlanogramFilter)); }

        Department? _selecteddepartment;
        public Department? SelectedDepartment { get => _selecteddepartment; set => Set<Department?>(ref value, ref _selecteddepartment,nameof(SelectedDepartment)); }

        string _searchItem =string.Empty;
        public string SearchItem { get => _searchItem; set => Set<string>(ref value, ref _searchItem, nameof(SearchItem)); }

        string _scrolltosku = string.Empty;
        public string ScrollToSKU { get => _scrolltosku; set => Set<string>(ref value, ref _scrolltosku, nameof(ScrollToSKU)); }
        public ICommand SaveAllRecordCMD { get; }

        public PlanogramListController() {
            AllowNewRecord = false;
            Departments.ReplaceRange(MainDB.DBDepartment.RecordSource);
            MainDB.DBDepartment.RecordSource.Children.Add(Departments);

            Items.ReplaceRange(MainDB.DBItem.RecordSource);
            MainDB.DBItem.RecordSource.Children.Add(Items);

            Sections.ReplaceRange(MainDB.DBSection.RecordSource);
            MainDB.DBSection.RecordSource.Children.Add(Sections);

            Bays.ReplaceRange(MainDB.DBBay.RecordSource);
            MainDB.DBBay.RecordSource.Children.Add(Bays);

            Shelves.ReplaceRange(MainDB.DBShelf.RecordSource);
            MainDB.DBShelf.RecordSource.Children.Add(Shelves);

            AfterPropChanged += PlanogramListController_AfterPropChanged;
            SelectedSection = Sections.FirstOrDefault();
            TransferItemCMD = new Command<Item>(TransferItem);
            DropPlanogramCMD = new Command(DropPlanogram);
            OpenNewFormItemCMD = new Command(OpenNewFormItem);
            SaveAllRecordCMD = new Command(SaveAllRecord);
        }
        

        private void SaveAllRecord()
        {
            DB().StartTransaction();
             foreach (var record in RecordSource)
            {
                if (record.IsDirty)
                {
                    DB().UpdateTransaction(record);
                }
            }
            DB().CommitTransaction();
            PlanogramFilter = new(SelectedShelf);
            ItemFilter = new(SearchItem, RecordSource);
        }

        private void OpenNewFormItem()
        {
            ItemForm itemform = new();
            itemform.ShowDialog();
        }

        private void DropPlanogram()
        {
            if (ConfirmDialog("Are you sure you want to drop this shelf?")== MessageBoxResult.No) return;
            List<Planogram> list = new(RecordSource);
            DB().OpenConnection();
            foreach(var record in list)
            {
                RecordSource.DeleteRecord(record);
                DB().Delete(record);
            }
            DB().CloseConnection();
        }

        private void TransferItem(Item item)
        {
            Planogram planogram = new();
            planogram.SetSectionTriggered = true;
            planogram.Product = item;
            planogram.Shelf = SelectedShelf;
            planogram.OrderList = RecordSource.Count + 1;
            planogram.Section = SelectedSection;
            planogram.Bay = SelectedBay;
            planogram.Shelf=SelectedShelf;
            planogram.SetSectionTriggered = false;
            planogram.IsDirty = true;
            SaveRecord(planogram);            
        }

        private void PlanogramListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SelectedSection)))
            {
                BayFilter = new(e.GetValue<Section>(),true);
                return;
            }

            if (e.PropIs(nameof(SelectedBay)))
            {
                ShelfFilter = new(e.GetValue<Bay>(), true,false);
                return;
            }

            if (e.PropIs(nameof(SelectedShelf)))
            {
                PlanogramFilter= new(e.GetValue<Shelf>());
                return;
            }

            if (e.PropIs(nameof(SearchItem)))
            {
                ItemFilter = new(e.GetValue<string>(),RecordSource,SelectedDepartment);
                return;
            }


            if (e.PropIs(nameof(SelectedDepartment)))
            {
                ItemFilter = new(SearchItem, RecordSource, e.GetValue<Department>());
                return;
            }

            if (e.PropIs(nameof(Search)))
            {
                PlanogramFilter = new(SelectedShelf,e.GetValue<string>());
                return;
            }

            if (e.PropIs(nameof(ScrollToSKU)))
            {
                string search = e.GetValue<string>().ToLower();
                if (string.IsNullOrEmpty(e.GetValue<string>()))
                {
                    SelectedRecord = RecordSource.FirstOrDefault();
                    return;
                }
                SelectedRecord = RecordSource.FirstOrDefault(s=>s.Product.SKU.StartsWith(search));
                return;
            }
        }
        
        public override void SaveRecord(Planogram? record)
        {
            bool IsNewRecord = record.IsNewRecord;
            base.SaveRecord(record);

            if (!IsNewRecord)
            {
                PlanogramFilter = new(SelectedShelf);
            }
            ItemFilter = new(SearchItem, RecordSource);
        }

        public void OpenItemForm()
        {
            ItemForm itemform = new(SelectedRecord);
            itemform.ShowDialog();
        }

        public void OpenItemForm(Item item)
        {
            ItemForm itemform = new(item);
            itemform.ShowDialog();
        }

        public override void OnMovingRow(MoveRecordEvtArgs e)
        {
            var count = RecordSource.Count;
            Model.Planogram From = (Model.Planogram)e.ObjectFrom;
            Model.Planogram To = (Model.Planogram)e.ObjectTo;
            var val = To.OrderList;
            int order = 0;

            From.OrderList = val;
            
            if ((val+1) >= count)
                To.OrderList--;
            else
                To.OrderList++;

            PlanogramFilter = new();

            List<Planogram> records = new(RecordSource);
            MainDB.DBPlanogram.StartTransaction();

            foreach (var record in records)
            {
                order++;
                record.OrderList = order;
                MainDB.DBPlanogram.UpdateTransaction(record);
            }
            
            MainDB.DBPlanogram.CommitTransaction();
            PlanogramFilter = new();
            SelectedRecord = From;
        }

        public override MySQLDB<Planogram> DB() => MainDB.DBPlanogram;

    }
 
    #region ItemFilter
    public class ItemFilter : AbstractFilterAndSort
    {
        RecordSource<Planogram> Source2 { get; }
        public override IRecordSource RecordSource => MainDB.DBItem.RecordSource;
        Department? SelectedDepartment;

        public ItemFilter(string search, RecordSource<Planogram> source) {
            Search = search.ToLower();
            Source2 = source;
        }

        public ItemFilter(string search, RecordSource<Planogram> source, Department? department) : this(search, source)
        {
            SelectedDepartment = department;
        }

        public override bool Criteria(object record)
        {
            if (SelectedDepartment==null)
            {
                return
                    (!Source2.Any(s => s.Product.Equals(record)))
                        && record.ToString().ToLower().Contains(Search);
            }
            Item item = record as Item;
            return
            (!Source2.Any(s => s.Product.Equals(item)))
            && record.ToString().ToLower().Contains(Search) && item.Department.Equals(SelectedDepartment);

        }

        public override void SortSource(IEnumerable ItemsSource)
        {
            Source = MainDB.DBItem.RecordSource.NewSortedSource(ItemsSource.Cast<Item>().OrderBy(s => Int32.Parse(s.SKU)));
            SortRan = true;
        }
    }

    #endregion

    #region FilterPlanogram
    public class FilterPlanogram : AbstractFilterAndSort {
        Shelf? Shelf;

        public override IRecordSource RecordSource => MainDB.DBPlanogram.RecordSource;

        public FilterPlanogram(Shelf shelf) {
            Shelf=shelf;
            SelectItem = true;
        }

        public FilterPlanogram() {
            SkipFilter = true;
            SelectItem = true;
        }

        public FilterPlanogram(Shelf shelf, string search) : this(shelf)=>Search = search.ToLower();

        public override bool Criteria(object record)
        {

            return (string.IsNullOrEmpty(Search)) ?
                   ((Planogram)record).Shelf.ShelfID == Shelf?.ShelfID :
                   ((Planogram)record).Product.ToString().ToLower().Contains(Search);
            ;
        }

        public override void SortSource(IEnumerable ItemsSource)
        {
            Source = RecordSource.NewSortedSource(ItemsSource.Cast<Planogram>().OrderBy(s => s.OrderList));
            SortRan = true;
        }
    }

    #endregion
}

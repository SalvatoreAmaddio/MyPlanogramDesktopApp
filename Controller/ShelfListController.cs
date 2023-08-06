using MvvmHelpers.Commands;
using MyPlanogramDesktopApp.Customs;
using MyPlanogramDesktopApp.Model;
using MyPlanogramDesktopApp.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class ShelfListController : AbstractDataListController<Shelf>
    {
        public bool DoSort { get; set; } = false;
        public RecordSource<Bay> Bays { get; set; } = new();
        public RecordSource<Section> Sections { get; set; } = new();

        Section _selectedsection = null!;
        public Section SelectedSection { get => _selectedsection; set => Set<Section>(ref value, ref _selectedsection, nameof(SelectedSection)); }

        Bay _selectedbay = null!;
        public Bay SelectedBay { get => _selectedbay; set => Set<Bay>(ref value, ref _selectedbay, nameof(SelectedBay)); }

        BayFilter _bayfilter = null!;
        public BayFilter BayFilter { get => _bayfilter; set => Set<BayFilter>(ref value, ref _bayfilter, nameof(BayFilter)); }

        ShelvesFilter _shelvesfilter = null!;
        public ShelvesFilter ShelvesFilter { get => _shelvesfilter; set => Set<ShelvesFilter>(ref value, ref _shelvesfilter, nameof(ShelvesFilter)); }
        public SettingController SettingController { get; } = new();

        public ShelfListController() {
            Sections.ReplaceRange(MainDB.DBSection.RecordSource);
            MainDB.DBSection.RecordSource.Children.Add(Sections);
            RecordSource.ReplaceRange(MainDB.DBShelf.RecordSource);
            Bays.ReplaceRange(MainDB.DBBay.RecordSource);
            MainDB.DBBay.RecordSource.Children.Add(Bays);
            AfterPropChanged += ShelfListController_AfterPropChanged;            
            SelectedSection = Sections.FirstOrDefault();
        }

        public void TurnSettingControllerOn()
        {
            SettingController.ParentController = this;
            DoSort = true;
            ShelvesFilter = new(SelectedBay, true, DoSort);
        }

        public override MySQLDB<Shelf> DB() => MainDB.DBShelf;

        private void ShelfListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SelectedSection)))
            {
                BayFilter = new(e.GetValue<Section>(), true);
                SelectedBay = Bays.FirstOrDefault();
                return;
            }

            if (e.PropIs(nameof(SelectedBay)))
            {
                if (SettingController != null)
                {
                    Fixture.Elements.Clear();
                }
                ShelvesFilter = new(e.GetValue<Bay>(), true,DoSort);
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }
        }

        public override Shelf CreateNewRecord()=> new Shelf(SelectedBay, RecordSource.Count+1);

    }

    #region ShelfvesFilter
    public class ShelvesFilter : AbstractFilterAndSort
    {
        Bay Bay;
        public bool DoSort { get; set; }

        public ShelvesFilter(Bay bay, bool selectitem,bool dosort)
        {
            Bay = bay;
            SelectItem = selectitem;
            DoSort = dosort;
        }

        public override IRecordSource RecordSource => MainDB.DBShelf.RecordSource;

        public override bool Criteria(object record)=>
        ((Shelf)record).Bay.Equals(Bay);

        public override void SortSource(IEnumerable ItemsSource)
        {
            if (!DoSort) return;
            Source = MainDB.DBShelf.RecordSource.NewSortedSource(ItemsSource.Cast<Shelf>().OrderByDescending(s => s.ShelfNum));
            SortRan = true;
        }
    }
    #endregion
}

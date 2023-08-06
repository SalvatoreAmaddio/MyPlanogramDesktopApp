using MyPlanogramDesktopApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Controller
{
    public class BayListController : AbstractDataListController<Bay>
    {
        public RecordSource<Section> Sections { get; set; } = new();
        Section _selectedsection = null!;
        public Section SelectedSection { get=>_selectedsection; set=>Set<Section>(ref value, ref _selectedsection,nameof(SelectedSection)); }

        BayFilter _bayfilter=null!;
        public BayFilter BayFilter { get => _bayfilter; set => Set<BayFilter>(ref value, ref _bayfilter,nameof(BayFilter)); }

        public BayListController() {
            Sections.ReplaceRange(MainDB.DBSection.RecordSource);
            MainDB.DBSection.RecordSource.Children.Add(Sections);
            RecordSource.ReplaceRange(MainDB.DBBay.RecordSource);
            AfterPropChanged += BayListController_AfterPropChanged;
            SelectedSection = Sections.FirstOrDefault();
        }

        private void BayListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (!e.PropIs(nameof(SelectedSection))) return;
            BayFilter = new(e.GetValue<Section>(), true);
        }

        public override MySQLDB<Bay> DB() => MainDB.DBBay;

        public override Bay CreateNewRecord()=>new Bay(RecordSource.Count + 1, SelectedSection);

    }

    #region BayFilter
    public class BayFilter : AbstractFilterAndSort
    {
        Section Section;
        public override IRecordSource RecordSource => MainDB.DBBay.RecordSource;

        public BayFilter(Section section, bool selectitem)
        {
            Section = section;
            SelectItem = selectitem;
        }

        public override bool Criteria(object record)=>
        ((Bay)record).Section.Equals(Section);

        public override void SortSource(IEnumerable ItemsSource)
        {
            Source=MainDB.DBBay.RecordSource.NewSortedSource(ItemsSource.Cast<Bay>().OrderBy(s => s.BayNum));
            SortRan = true;
        }
    }

    #endregion
}

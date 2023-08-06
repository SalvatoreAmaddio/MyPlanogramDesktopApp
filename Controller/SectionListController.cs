using MyPlanogramDesktopApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Controller
{
    public class SectionListController : AbstractDataListController<Section>
    {
        SectionFilter _sectionfilter = null!;
        public SectionFilter SectionFilter { get => _sectionfilter; set => Set<SectionFilter>(ref value, ref _sectionfilter,nameof(SectionFilter)); }

        public SectionListController() { 
            RecordSource.ReplaceRange(MainDB.DBSection.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            SectionFilter = new(string.Empty);
            AfterPropChanged += SectionListController_AfterPropChanged;
        }

        private void SectionListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (!e.PropIs(nameof(Search))) return;
            SectionFilter = new(e.GetValue<string>());
            SelectedRecord = RecordSource.FirstOrDefault();
        }

        public override MySQLDB<Section> DB() => MainDB.DBSection;

    }

    #region SectionFilter
    public class SectionFilter : AbstractFilterAndSort
    {
        public SectionFilter(string value) =>Search = value.ToLower();

        public override IRecordSource RecordSource => MainDB.DBSection.RecordSource;

        public override bool Criteria(object record)=>
        record.ToString().ToLower().Contains(Search);

        public override void SortSource(IEnumerable ItemsSource)
        {
        }
    }
    #endregion
}

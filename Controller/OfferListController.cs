using MyPlanogramDesktopApp.Model;
using System.Collections;
using System.Linq;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Controller
{
    public class OfferListController : AbstractDataListController<Offer> {

        OfferFilter _offerfilter = null!;
        public OfferFilter OfferFilter 
        {
            get => _offerfilter;
            set => Set<OfferFilter>(ref value, ref _offerfilter,nameof(OfferFilter));
        }

        public OfferListController()
        {
            RecordSource.ReplaceRange(MainDB.DBOffer.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            OfferFilter = new(string.Empty);
            AfterPropChanged += OfferListController_AfterPropChanged;
        }

        public override MySQLDB<Offer> DB() => MainDB.DBOffer;

        private void OfferListController_AfterPropChanged(object? sender, Testing.Model.Abstracts.PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Search)))
            {
                OfferFilter = new(e.GetValue<string>());
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }
        }
}

    #region OfferFilter
    public class OfferFilter : AbstractFilterAndSort
    {
        public OfferFilter(string val)=>Search = val.ToLower();

        public override IRecordSource RecordSource => MainDB.DBOffer.RecordSource;

        public override bool Criteria(object record)=>
        record.ToString().ToLower().Contains(Search);

        public override void SortSource(IEnumerable ItemsSource)
        {

        }
    }
    #endregion
}

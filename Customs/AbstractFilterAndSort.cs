using MyPlanogramDesktopApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;

namespace Testing.Customs
{
    public abstract class AbstractFilterAndSort : AbstractNotifier
    {
        public bool NotUsed { get => !FilterRan && !SortRan; }
        public bool FilterRan { get; set; } = false;
        public bool SortRan { get; set; } = false;
        public bool SkipFilter { get; set; } = false;
        public bool SkipSort { get; set; } = false;
        public string? Search;
        public IEnumerable? Source { get; set; }
        public bool SelectItem { get; set; } = false;
        public abstract bool Criteria(object record);
        public abstract void SortSource(IEnumerable ItemsSource);
        public abstract IRecordSource RecordSource { get; }
        int _count=0;
        public int Count { get => _count; set => Set<int>(ref value, ref _count, nameof(Count)); }

        public virtual void FilterSource()
        {
            var range = RecordSource.NewFilteredSource(s => Criteria(s));
            Source = range;
            Count = range.RecordCount();
            FilterRan = true;
        }

    }
}

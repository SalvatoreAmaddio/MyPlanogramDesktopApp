using Testing.Controller.AbstractControllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Testing.Model.Abstracts;

namespace Testing.RecordSource
{
    public interface IRecordSource : IEnumerable
    {
        public event EventHandler<CascadeEventArgs>? CascadeTriggered;
        public List<IRecordSource> Children { get; set; }
        public void InsertRecord(object Record);
        public void InsertRecords(IEnumerable source);
        public object DeleteRecord(object Record);
        public void UpdateRecord(object Record);
        public int RecordCount();
        public IController Controller { get; set; }
        public IRecordSource NewSource();
        public bool Find(Func<AbstractModel, bool> predicate);
        public IRecordSource NewFilteredSource(Func<AbstractModel, bool> predicate);
        public IRecordSource NewSortedSource(IEnumerable source);
        public int GetIndexOf(object record);
        public IEnumerable Enumerable();
        public IRecordSource Copy();
    }
}

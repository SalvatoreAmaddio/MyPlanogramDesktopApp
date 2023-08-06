using Testing.Controller.AbstractControllers;
using Microsoft.VisualBasic;
using MvvmHelpers;
using Testing.Model.Abstracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Collections;
using System.Windows.Media.Animation;

namespace Testing.RecordSource
{

    public class RecordSource<M> : ObservableRangeCollection<M>, IRecordSource where M : AbstractModel, new() {
        public IController Controller { get; set; } = null!;
        public bool RefreshRecordTracker { get; set; } = false;
        public List<IRecordSource> Children { get; set; } = new();
        public event EventHandler<CascadeEventArgs>? CascadeTriggered;

        public RecordSource()
        {
          
        }

        public RecordSource(IEnumerable<M> range) : base(range)
        {
        }


        public void DeleteRecords(IEnumerable source)
        {
            foreach(var record in source)
            {
                DeleteRecord(record);   
            }
        }

        public object DeleteRecord(object Record)
        {
            int index = this.IndexOf((M)Record);

            foreach (IRecordSource source in Children)
            {
                CascadeTriggered?.Invoke(this,new(Record));
                source.DeleteRecord(Record);
            }

            Remove((M)Record);
            M temp = new();
            try
            {
                temp = this[index];
            }
            catch
            {
            }

            if (!RefreshRecordTracker) return temp;
            Controller.UpdateRecordTracker(temp, this);
            return temp;
        }

        public void InsertRecords(IEnumerable source)
        {
            var s = source.Cast<M>();
            ReplaceRange(s);
            foreach (IRecordSource source2 in Children)
            {
                source2.InsertRecords(s);
            }
        }

        public bool Find(Func<AbstractModel, bool> predicate)=>
        this.Any(predicate);

        public void InsertRecord(object Record)
        {
            if (this.Any(s => s.Equals(Record))) return;
            Add((M)Record);
            foreach (IRecordSource source in Children)
            {
                source.InsertRecord(Record);
            }
            if (!RefreshRecordTracker) return;
            Controller.UpdateRecordTracker((M)Record, this);
        }

        public void UpdateRecord(object Record)
        {
            var index=this.IndexOf((M)Record);
            if (index == -1) return;
            var x = this[index];
            x = (M)Record;
            if (RefreshRecordTracker)
            {
                Controller.UpdateRecordTracker(x, this);
            }

            foreach (IRecordSource source in Children)
            {
                source.UpdateRecord(Record);
            }
        }

        public int RecordCount()=>this.Count;

        public IRecordSource Copy()=>
        new RecordSource<M>(this);

        public IRecordSource NewSource()
        {
            RecordSource<M> temp = new(this);
            temp.Controller = Controller;
            Children.Add(temp);
            return temp; 
        }

        public IRecordSource NewFilteredSource(Func<AbstractModel, bool> predicate)
        {
            RecordSource<M> temp = new(this.Where(predicate).Cast<M>());
            temp.Controller = Controller;
            Children.Add(temp);
            return temp;
        }

        public IRecordSource NewSortedSource(IEnumerable source)
        {
            RecordSource<M> temp = new(source.Cast<M>());
            temp.Controller = Controller;
            Children.Add(temp);
            return temp;
        }

        public override string? ToString()=>
        Count.ToString();

        public int GetIndexOf(object record) => IndexOf((M)record);

        public IEnumerable Enumerable()=>
        this.AsEnumerable<M>();
    }

    #region CascadeEvent
    public class CascadeEventArgs : EventArgs
    {
        public object? Record { get; set; }
        public CascadeEventArgs(object? record) => Record = record;

    }
    #endregion
}

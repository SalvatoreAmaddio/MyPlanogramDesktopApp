using MvvmHelpers.Commands;
using Testing.Model.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Testing.Customs;
using Testing.RecordSource;
using Testing.DB;
using MyPlanogramDesktopApp.Model;

namespace Testing.Controller.AbstractControllers
{
    public abstract class AbstractDataController<M> : AbstractNotifier, IController where M : AbstractModel, IDB<M>, new()
    {
        public ICommand ClearRecordCMD { get; set; }
        public ICommand SaveRecordCMD { get; set; }
        public ICommand DeleteRecordCMD { get; set; }
        public RecordSource<M> RecordSource { get; set; } = new();

        M? _record;
        public M? Record { get => _record; set => Set(ref value, ref _record, nameof(Record)); }

        RecordTracker<M> _recordtracker = null!;
        public RecordTracker<M> RecordTracker
        {
            get => _recordtracker;
            set => Set(ref value, ref _recordtracker, nameof(RecordTracker));
        }

        public bool AllowNewRecord { get; set; } = true;

        public void Save()=>SaveRecord(Record);

        public AbstractDataController()
        {
            SaveRecordCMD = new Command<M>(SaveRecord);
            ClearRecordCMD = new Command(ClearRecord);
            DeleteRecordCMD = new Command<M>(DeleteRecord);
            AfterPropChanged += AbstractDataController_AfterPropChanged;
            Record = new();
            RecordSource.Controller = this;
            RecordSource.RefreshRecordTracker = true;
            DB().RecordSource.Children.Add(RecordSource);
        }

        public virtual void SetRecord(M? record)
        {
            Record = record;
        }

        public void UpdateRecordTracker(AbstractModel record, IRecordSource recordsource)
        {
            RecordTracker = new(
                                record,
                                recordsource,
                                new Command(GoFirst),
                                new Command(GoPrevious),
                                new Command(GoNext),
                                new Command(GoLast),
                                new Command(GoNew)
                                );
        }

        private void AbstractDataController_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Record)))
            {
                UpdateRecordTracker(e.GetValue<M>(), RecordSource);
            }
        }

        public virtual void GoFirst()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.BOF) return;
            Record = RecordSource.FirstOrDefault();
            Record.IsDirty = false;
        }

        public virtual void GoPrevious()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.BOF) return;

            if (Record.IsNewRecord)
            {
                Record = RecordSource.LastOrDefault();
            }
            else
            {
                Record = RecordSource[RecordSource.IndexOf(Record) - 1];
            }
            Record.IsDirty = false;
        }

        public virtual void RemoveRecordsBy(string sql,Func<M, bool> predicate)
        {
            List<M> records = new(DB().RecordSource.Where(predicate).Cast<M>());
            DB().OpenConnection();
            DB().DeleteRecords(records,sql);
            DB().CloseConnection();
            Record = RecordSource.FirstOrDefault();
        }

        public virtual void UpdateRecordTracker()=>
        UpdateRecordTracker(Record,RecordSource);

        public virtual void GoNext()
        {
            if (Record.IsNewRecord) return;
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.EOF)
            {
                GoNew();
                return;
            }
            var index = RecordSource.IndexOf(Record);
            Record = RecordSource[index + 1];
            Record.IsDirty = false;
        }

        public virtual void GoLast()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.EOF) return;
            Record = RecordSource.LastOrDefault();
            Record.IsDirty = false;
        }

        public virtual void GoNew()
        {
            if (Record.IsNewRecord || !AllowNewRecord) return;
            Record = new();
        }

        public virtual void ClearRecord()
        {
            Record = new();
            Record.IsDirty = false;
        }

        public virtual void SaveRecord(M? record)
        {
            if (record == null) return;

            Record = record;
            if (!Record.IsDirty) return;

            DB().OpenConnection();

            if (Record.IsNewRecord)
            {
               DB().Insert(Record);
            }
            else
            {
               DB().Update(Record);
            }
            DB().CloseConnection();
        }

        public virtual void DeleteRecord(M record)
        {
            if (record == null) return;

        }

        public abstract MySQLDB<M> DB();
        public MessageBoxResult ConfirmDialog() => MessageBox.Show("Are you sure you want to delete this record?", "CONFIRM", MessageBoxButton.YesNo);
        public MessageBoxResult ConfirmDialog(string msg) => MessageBox.Show(msg, "CONFIRM", MessageBoxButton.YesNo);
        public virtual M CreateNewRecord() => new M();
    }

    #region RsMovements
    public enum RsMovements
    {
        First,
        Last,
        Previous,
        Next,
    }
    #endregion

    #region RecordTracker
    public class RecordTracker<M> : AbstractNotifier, IRecordTracker where M : AbstractModel
    {
        int _currentrecord;
        int _recordcount;

        bool _bof;
        public bool BOF { get => _bof; set => Set(ref value, ref _bof, nameof(BOF)); }

        bool _eof;
        public bool EOF { get => _eof; set => Set(ref value, ref _eof, nameof(EOF)); }

        bool _newrecord;
        public bool NewRecord { get => _newrecord; set => Set(ref value, ref _newrecord, nameof(NewRecord)); }

        public IRecordSource RecordSource { get; set; }
        public int CurrentRecord { get => _currentrecord; set => Set(ref value, ref _currentrecord, nameof(CurrentRecord)); }
        public int RecordCount { get => _recordcount; set => Set(ref value, ref _recordcount, nameof(RecordCount)); }

        public ICommand GoNewCMD { get; set; } = null!;
        public ICommand GoFirstCMD { get; set; } = null!;
        public ICommand GoNextCMD { get; set; } = null!;
        public ICommand GoLastCMD { get; set; } = null!;
        public ICommand GoPreviousCMD { get; set; } = null!;
        public string Records
        {
            get
            {
                if (NewRecord) return "New Record";
                if (RecordCount == 0) return "NO RECORD";
                return $"Records {CurrentRecord} of {RecordCount}";
            }
        }

        public RecordTracker(AbstractModel record, IRecordSource recordsource, ICommand GoFirst, ICommand GoPrevious, ICommand GoNext, ICommand GoLast, ICommand GoNew) : this(record, recordsource)
        {
            GoFirstCMD = GoFirst;
            GoPreviousCMD = GoPrevious;
            GoNextCMD = GoNext;
            GoLastCMD = GoLast;
            GoNewCMD = GoNew;
            if (record != null)
            {
                record.NotifyNewRecord();
            }
        }

        public RecordTracker(AbstractModel record, IRecordSource recordsource) : this((M)record, recordsource)
        {
        }

        public RecordTracker(M record, IRecordSource recordsource)
        {
            RecordSource = recordsource;
            RecordCount = RecordSource.RecordCount();
            CurrentRecord = (record == null) ? 0 : RecordSource.GetIndexOf(record) + 1;
            BOF = CurrentRecord <= 1;
            EOF = CurrentRecord == RecordCount;
            NewRecord = record == null ? false : record.IsNewRecord;
            if (NewRecord && RecordCount > 0)
            {
                BOF = false;
            }
        }

        public override string? ToString()
        {
            if (NewRecord) return "New Record";
            return $"Record {CurrentRecord} of {RecordCount}";
        }

    }
    #endregion
}

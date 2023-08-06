using MvvmHelpers.Commands;
using Testing.Model.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.RecordSource;
using System.Windows;
using Testing.DB;
using System.Security.AccessControl;

namespace Testing.Controller.AbstractControllers
{
    public abstract class AbstractDataListController<M> : AbstractDataController<M> where M : AbstractModel, IDB<M>, new()
    {
        M _selectedrecord = null!;
        public M SelectedRecord { get => _selectedrecord; set => Set(ref value, ref _selectedrecord, nameof(SelectedRecord)); }

        string _search=string.Empty;
        public string Search { get => _search; set => Set<string>(ref value, ref _search,nameof(Search)); }

        public AbstractDataListController() : base()
        {
            AfterPropChanged += AbstractDataListController_AfterPropChanged;
            SelectedRecord = new();
        }

        private void AbstractDataListController_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SelectedRecord)))
            {
                UpdateRecordTracker(e.GetValue<M>(), RecordSource);
            }
        }

        public override void UpdateRecordTracker()=>
        UpdateRecordTracker(SelectedRecord, RecordSource);

        public override void RemoveRecordsBy(string sql, Func<M, bool> predicate)
        {          
            List<M> records = new (DB().RecordSource.Where(predicate).Cast<M>());
            DB().OpenConnection();
            DB().DeleteRecords(records,sql);
            DB().CloseConnection();
            SelectedRecord = RecordSource.FirstOrDefault();
        }

        public override void DeleteRecord(M record)
        {
            SelectedRecord = record;
            if (ConfirmDialog() == MessageBoxResult.No) return;
            DB().OpenConnection();
            int index = RecordSource.IndexOf(record);
            DB().Delete(SelectedRecord);
            DB().CloseConnection();

            if (RecordSource.Count == 0) return;

            if (index == 0)
            {
                SelectedRecord = RecordSource.FirstOrDefault();
                return;
            }

            SelectedRecord = RecordSource[index - 1];
        }

        public override void SaveRecord(M? record)
        {
            if (record == null|| !record.IsDirty) return;
            
            SelectedRecord= record;
            
            DB().OpenConnection();

            if (SelectedRecord.IsNewRecord)
            {
                DB().Insert(SelectedRecord);
                SelectedRecord = RecordSource.LastOrDefault();
                AllowNewRecord = true;
            }
            else
            {
                DB().Update(SelectedRecord);
            }

            DB().CloseConnection();
        }

        private bool ManageNewRecord(RsMovements rsMovements)
        {
            if (!SelectedRecord.IsNewRecord) return false;

                if (!SelectedRecord.IsDirty)
                {
                    RecordSource.Remove(SelectedRecord);
                    switch(rsMovements)
                    {
                        case RsMovements.Previous:
                            GoBack();
                            break;    
                        case RsMovements.First:
                        SelectedRecord = RecordSource.FirstOrDefault();
                            break;
                    case RsMovements.Last:
                        SelectedRecord = RecordSource.LastOrDefault();
                        break;
                }

                AllowNewRecord = true;
                } 
                else SaveChanges();
            return true;
        }

        private void SaveChanges()
            {
            if (SelectedRecord.IsDirty)
            {

                if (ConfirmDialog("Changes to this record have not been saved. Do you want to save them?") == MessageBoxResult.No) return;
                SaveRecord(SelectedRecord);
                SelectedRecord = RecordSource.LastOrDefault();
            }
        }

        private void GoBack() 
        {
            try
            {
                SelectedRecord = RecordSource[RecordSource.IndexOf(SelectedRecord) - 1];
            }
            catch
            {
                SelectedRecord = RecordSource.LastOrDefault();
            }
        } 

        public override void GoNext()
        {
            if (SelectedRecord.IsNewRecord) return;
            if (RecordTracker.RecordCount == 0) return;
            SaveChanges();
            if (RecordTracker.EOF)
            {
                GoNew();
                return;
            }
            var index = RecordSource.IndexOf(SelectedRecord);
            SelectedRecord = RecordSource[index + 1];
            SelectedRecord.IsDirty = false;
        }

        public override void GoLast()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.EOF) return;
            if (ManageNewRecord(RsMovements.Last)) return;
            SaveChanges();
            SelectedRecord = RecordSource.LastOrDefault();
            
        }

        public override void GoNew()
        {
            if (!AllowNewRecord) return;
            var newRecord = CreateNewRecord();
            RecordSource.Add(newRecord);

            if (newRecord.IsDirty)
            {
                SaveRecord(newRecord);
                return;
            }
            SelectedRecord = RecordSource.LastOrDefault();
            UpdateRecordTracker(SelectedRecord, RecordSource);
        }

        public override void GoFirst()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.BOF) return;
            if (ManageNewRecord(RsMovements.First)) return;
            SaveChanges();
            SelectedRecord = RecordSource.FirstOrDefault();
        }

        public override void GoPrevious()
        {
            if (RecordTracker.RecordCount == 0) return;
            if (RecordTracker.BOF) return;
            if (ManageNewRecord(RsMovements.Previous)) return;
            SaveChanges();
            GoBack();
        }

        //used to move rows with mouse
        public virtual void OnMovingRow(Testing.Customs.MoveRecordEvtArgs e)=>
        throw new NotImplementedException();


    }
}

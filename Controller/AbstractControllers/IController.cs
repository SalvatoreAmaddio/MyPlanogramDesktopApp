using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Testing.Model.Abstracts;
using Testing.RecordSource;

namespace Testing.Controller.AbstractControllers
{
    public interface IController 
    { 
        public void UpdateRecordTracker(AbstractModel record, IRecordSource recordsource);
        public void GoNext();
        public void GoLast();
        public void GoNew();
        public void ClearRecord();
        public void GoFirst();
        public ICommand ClearRecordCMD { get; set; }
        public ICommand SaveRecordCMD { get; set; }
        public ICommand DeleteRecordCMD { get; set; }
        public bool AllowNewRecord { get; set; }
        public void Save();
        public MessageBoxResult ConfirmDialog();
        public MessageBoxResult ConfirmDialog(string msg);
    }
}

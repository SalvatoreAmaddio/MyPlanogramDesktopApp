using Microsoft.Win32;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;
using Testing.Utilities;

namespace Testing.Controller.AbstractControllers
{
    public abstract class AbstractImporterController : AbstractNotifier {

        private ICommand _openfileexplorercmd=null!;
        public ICommand OpenFileExplorerCMD { get=>_openfileexplorercmd; set=>Set<ICommand>(ref value, ref _openfileexplorercmd,nameof(OpenFileExplorerCMD)); }

        string _filepath = string.Empty;
        public string FilePath { get => _filepath; set => Set<string>(ref value, ref _filepath, nameof(FilePath)); }

        string _filename = string.Empty;
        public string FileName { get => _filename; set => Set<string>(ref value, ref _filename, nameof(FileName)); }

        public string _reading=string.Empty;
        public string Reading { get => _reading; set => Set<string>(ref value, ref _reading, nameof(Reading)); }

        string _progress = string.Empty;
        public string Progress { get => _progress; set => Set<string>(ref value, ref _progress, nameof(Progress)); }

        string _rowtracker=string.Empty;
        public string RowTracker { get => _rowtracker; set => Set<string>(ref value, ref _rowtracker, nameof(RowTracker)); }

        string _buttonlabel="IMPORT";
        public string ButtonLabel { get => _buttonlabel; set => Set<string>(ref value, ref _buttonlabel,nameof(ButtonLabel)); }

        private OpenFileDialog OpenFileDialog = new() { Filter = "Excel Files (*.xlsx)|*.xlsx"};

        bool _isworking = false;
        public bool IsWorking { get => _isworking; set => Set<bool>(ref value, ref _isworking, nameof(IsWorking)); }

        bool _isenabled = true;
        public bool IsEnabled { get => _isenabled; set => Set<bool>(ref value, ref _isenabled, nameof(IsEnabled)); }

        public AbstractExcelManager ExcelManager { get; set; } = null!;

        public ObservableRangeCollection<string> ProgressList { get; set; } =new();

        public AbstractImporterController()=>OpenFileExplorerCMD = new Command(OpenFileExplorer);

        public abstract void OpenFileExplorer();

        public bool DialogCancelled()=> (bool)!OpenFileDialog.ShowDialog()!;

        public void ReportProgress(string stringa)
        {
            ProgressList.Add(stringa);
            Progress = stringa;
        }

        public void ClearProgress()
        {
            ProgressList.Clear();
            Progress = string.Empty;
        }

        public void SetFilePathAndName()
        {
            FilePath = OpenFileDialog.FileName;
            FileName = OpenFileDialog.SafeFileName;
        }

        public void ResetCMD() 
        {
            ButtonLabel = "IMPORT";
            OpenFileExplorerCMD = new Command(OpenFileExplorer);
        } 
        
        public void SetCancelCMD() 
        {
            ButtonLabel = "CANCEL";
            OpenFileExplorerCMD = new Command(CancelReading);
        }

        private void CancelReading() =>
        ExcelManager.IsCancelled = true;

        public abstract IRecordSource MainDBSource();
        public abstract IMySQLDB? DBType();

        public void Transfer()
        {
            if (ExcelManager.IsCancelled) return;
            MainDBSource().InsertRecords(ExcelManager.Checker.Source);
        }
    }
}

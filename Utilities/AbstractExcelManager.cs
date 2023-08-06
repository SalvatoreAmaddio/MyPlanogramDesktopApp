using Microsoft.Office.Interop.Excel;
using MyPlanogramDesktopApp;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;
using Excel = Microsoft.Office.Interop.Excel;

namespace Testing.Utilities
{

    abstract public class AbstractExcelManager : AbstractNotifier {
        private Excel.Application xlApp = new Excel.Application();
        private Excel.Workbook xlWorkbook=null!;
        private Excel._Worksheet xlWorksheet = null!;
        private Excel.Range xlRange = null!;
        public int RowCount;
        public int ColumnCount;
        public int Count = 1;
        public AbstractImporterController Controller { get; set; } = null!;
        public bool IsCancelled=false;
        abstract public AbstractChecker Checker { get; set; }

        public AbstractExcelManager() 
        { 
        }

        public AbstractExcelManager(string path, int sheetNumber) : this()
        {
            OpenFile(path);
            OpenSheet(sheetNumber);
        }

        public AbstractExcelManager(string path, AbstractImporterController controller) : this(path, 1)
        {
            Controller= controller;
            Checker.Source = controller.MainDBSource().Copy();
            Checker.DB = controller.DBType();
            Checker?.DB?.StartTransaction();
        }

        public AbstractExcelManager(string path) : this() 
        {
            OpenFile(path);
        }

        public void ReportProgress()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Controller.ReportProgress($"{Count}) INSERTING {Checker.ToString()}");
                Count++;
            });
        }

        public void ReportProgress(string description)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Controller.ReportProgress($"{Count}) {description} {Checker.ToString()}");
                Count++;
            });
        }

        public void OpenFile(string path)
        {
            xlWorkbook = xlApp.Workbooks.Open(@path);
        }

        public void OpenSheet(int sheet)
        {
            xlWorksheet = xlWorkbook.Sheets[sheet];
            xlRange = xlWorksheet.UsedRange;
            RowCount = xlRange.Rows.Count;
            ColumnCount = xlRange.Columns.Count; 
        }

        abstract public void ReadLogic(int currentRow, int currentColumn);
        
        public void CommitOrRollBack()
        {
            if (IsCancelled)
            {
                Checker?.DB?.RollBack();
            }
            else
            {
                Checker?.DB?.CommitTransaction();
            }
            Close();
        }

        public virtual Task Read()
        {
            IsCancelled = false;
            Controller.Reading = $"READING {Controller.FileName}...";
            ForInteration();
            App.Current.Dispatcher.Invoke(() =>
            {
                Controller.ReportProgress("Finished");
            });
            return Task.CompletedTask;
        }

        private void ForInteration()
        {
            for (int i = 1; i <= RowCount; i++)
            {
                //for (int j = 1; j <= ColumnCount; j++)
                //{
                if (IsCancelled) 
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Controller.ReportProgress("Reading Cancelled.");
                    });                    
                    break;
                }
                Controller.RowTracker = $"Reading Row: {i} of {RowCount}";
                ReadLogic(i, 1);

                //       }
            }
        }

        public bool CellIsNull(int currentRow, int currentColumn) =>
        xlRange.Cells[currentRow, currentColumn] != null 
        && 
        xlRange.Cells[currentRow, currentColumn].Value2 == null;

        public string CellValue(int currentRow, int currentColumn) => xlRange.Cells[currentRow, currentColumn].Value2.ToString();

        public void Close()
        {
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

        }

    }

    #region AbstractChecker
    public abstract class AbstractChecker {

        public IRecordSource? Source { get; set; }
        public IMySQLDB? DB { get; set; }
        public virtual void UpdateTransaction() => DB?.UpdateTransaction(Model());
        public virtual void InsertTransaction()=>DB?.InsertTransaction(Model());
        public abstract object? Model();        
        public abstract bool AddItem(params string?[] values);
        public virtual bool Exists()=>Source.Find(s => s.Equals(Model()));

    }
    #endregion
}

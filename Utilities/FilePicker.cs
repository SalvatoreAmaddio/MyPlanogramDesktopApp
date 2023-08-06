using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlanogramDesktopApp.Utilities
{
    public class FilePicker
    {
        private OpenFileDialog OpenFileDialog;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        //"Excel Files (*.xlsx)|*.xlsx"

        public FilePicker(string filter="")
        {
            OpenFileDialog = new() { Filter = filter };

        }

        public bool DialogCancelled() => (bool)!OpenFileDialog.ShowDialog()!;
        public void SetFilePathAndName()
        {
            FilePath = OpenFileDialog.FileName;
            FileName = OpenFileDialog.SafeFileName;
        }

    }
}

using MyPlanogramDesktopApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPlanogramDesktopApp.Utilities
{
    public class JSONManager<M> where M : new()
    {
        public string FileName { get; set; } = string.Empty;
        public string Path { get; set; } =$"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}JSONFiles";
        private string JSON_result=string.Empty;
        public M? Object { get; set; }
        public List<M> Objects { get; set; } = new();

        public void Read()
        {
            FilePicker filePicker = new("JSON Files (*.json)|*.json");
            if (filePicker.DialogCancelled()) return;
            filePicker.SetFilePathAndName();
            using (WebClient wc = new())
            {
                string json = wc.DownloadString(filePicker.FilePath);
                Object=JsonConvert.DeserializeObject<M>(json);
            }

        }

        public void ReadList()
        {
            FilePicker filePicker = new("JSON Files (*.json)|*.json");
            if (filePicker.DialogCancelled()) return;
            filePicker.SetFilePathAndName();
            using (WebClient wc = new())
            {
                string json = wc.DownloadString(filePicker.FilePath);
                Objects = JsonConvert.DeserializeObject<List<M>>(json);
            }

        }

        public void WriteList()
        {
            Path = $"{Path}\\{FileName}.json";
            if (File.Exists(Path))
            {
                MessageBoxResult res = MessageBox.Show("This file already exists, do you want to overwrite it?", "CONFIRM", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No) return;
                File.Delete(Path);
            }
            
            JSON_result = JsonConvert.SerializeObject(Objects);
            using (var tw = new StreamWriter(Path, true))
            {
                tw.WriteLine(JSON_result);
                tw.Close();
            }
        }

        public void Write()
        {
            JSON_result = JsonConvert.SerializeObject(Object);
            Path = "{Path}/{FileName}.json";
            using (var tw = new StreamWriter(Path, true))
            {
                tw.WriteLine(JSON_result);
                tw.Close();
            }
        }
    }
}

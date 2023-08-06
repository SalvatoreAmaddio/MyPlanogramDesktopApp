using MyPlanogramDesktopApp.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyPlanogramDesktopApp.Model;
using Testing.DB;
using System.Windows.Media.Media3D;
using System.Threading;

namespace MyPlanogramDesktopApp.View.Item
{
    /// <summary>
    /// Interaction logic for ItemForm.xaml
    /// </summary>
    public partial class ItemForm : Window
    {
        public ItemFormController Controller { get;}  

        public ItemForm()
        {
            InitializeComponent();
            Controller = (ItemFormController)DataContext;
            Controller.Record = new();
        }

        public ItemForm(MyPlanogramDesktopApp.Model.Planogram planogram) : this() => 
        Controller.SetRecord(MainDB.DBItem.RecordSource.FirstOrDefault(s => s.Equals(planogram.Product)));

        public ItemForm(MyPlanogramDesktopApp.Model.Item item) : this() =>
        Controller.SetRecord(item);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)=>
        Controller.Save();

    }
}

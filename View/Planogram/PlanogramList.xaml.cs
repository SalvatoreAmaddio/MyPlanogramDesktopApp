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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyPlanogramDesktopApp.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyPlanogramDesktopApp.View.Item;

namespace MyPlanogramDesktopApp.View.Planogram
{
    /// <summary>
    /// Interaction logic for PlanogramList.xaml
    /// </summary>
    public partial class PlanogramList : Page
    {
        public PlanogramListController Controller { get; }

        public PlanogramList()
        {
            InitializeComponent();
            Controller = (PlanogramListController)DataContext;
        }

        private void ViewCell_MoveRowEvt(object sender, Testing.Customs.MoveRecordEvtArgs e) =>
        Controller.OnMovingRow(e);

        private void Lista_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
        Controller.OpenItemForm();

        private void Lista_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Controller.OpenItemForm((MyPlanogramDesktopApp.Model.Item)((TextBlock)e.OriginalSource).DataContext);
            }
            catch { }
        }

        private void Lista_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                e.Handled = true;
                try
                {
                    Controller.OpenItemForm(Controller.SelectedRecord.Product);
                    e.Handled = true;
                }
                catch { }

            }
        }
    }
}

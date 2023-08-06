using MyPlanogramDesktopApp.Controller;
using MyPlanogramDesktopApp.Model;
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
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.View
{
    /// <summary>
    /// Interaction logic for CheckPage.xaml
    /// </summary>
    public partial class BarcodeList : Page
    {
        public BarcodeListController Controller { get; }

        public BarcodeList()
        {
            InitializeComponent();
            Controller = (BarcodeListController) DataContext;   
        }
    }
}

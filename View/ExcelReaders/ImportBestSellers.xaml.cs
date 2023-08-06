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

namespace MyPlanogramDesktopApp.View.ExcelReaders
{
    /// <summary>
    /// Interaction logic for ImportBestSellers.xaml
    /// </summary>
    public partial class ImportBestSellers : Page
    {
        public BestsellerImporterController Controller { get; }

        public ImportBestSellers()
        {
            InitializeComponent();
            Controller = (BestsellerImporterController) DataContext;
        }
    }
}

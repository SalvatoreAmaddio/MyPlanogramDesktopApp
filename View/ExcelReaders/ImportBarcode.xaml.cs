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
    /// Interaction logic for ImportBarcode.xaml
    /// </summary>
    public partial class ImportBarcode : Page
    {
        public BarcodeImporterController Controller { get; }

        public ImportBarcode()
        {
            InitializeComponent();
            Controller = (BarcodeImporterController)DataContext;
        }
    }
}

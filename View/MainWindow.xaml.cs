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
using Testing.Customs;

namespace MyPlanogramDesktopApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            WindowTracker.ItemList = ItemList;
            WindowTracker.SectionList = SectionList;
            WindowTracker.BayList = BayList;
            WindowTracker.ShelfList = ShelvesList;
            WindowTracker.PlanogramList = PlanogramList;
            WindowTracker.OfferList = OfferList;
            WindowTracker.BarcodeList = BarcodeList;
            WindowTracker.WebPage = PlanViewer;
            WindowTracker.DepartmentView = DepartmentView;
        }
    }
}

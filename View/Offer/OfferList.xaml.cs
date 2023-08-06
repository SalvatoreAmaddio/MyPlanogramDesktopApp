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
using Testing.DB;

namespace MyPlanogramDesktopApp.View.Offer
{
    /// <summary>
    /// Interaction logic for OfferList.xaml
    /// </summary>
    public partial class OfferList : Page
    {
        public OfferListController Controller { get; }

        public OfferList()
        {
            InitializeComponent();
            Controller = (OfferListController) DataContext;
        }

    }
}

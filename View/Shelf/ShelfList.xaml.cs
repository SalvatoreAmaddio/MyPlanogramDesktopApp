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

namespace MyPlanogramDesktopApp.View.Shelf
{
    /// <summary>
    /// Interaction logic for ShelfList.xaml
    /// </summary>
    public partial class ShelfList : Page
    {
        public ShelfListController Controller { get; }

        public ShelfList()
        {
            InitializeComponent();
            Controller = (ShelfListController) DataContext;
        }
    }
}

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

namespace MyPlanogramDesktopApp.View.Section
{
    /// <summary>
    /// Interaction logic for SectionList.xaml
    /// </summary>
    public partial class SectionList : Page
    {
        public SectionListController Controller { get; }

        public SectionList()
        {
            InitializeComponent();
            Controller = (SectionListController)DataContext;
        }
    }
}

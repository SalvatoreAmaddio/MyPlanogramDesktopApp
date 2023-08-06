using MyPlanogramDesktopApp.View;
using MyPlanogramDesktopApp.View.Item;
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
using Testing.DB;
using Testing.Utilities;

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        public Loading()=>InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MainDB.FetchData();
            Sys.OpenUp(new MainWindow(),this);
        }
    }
}

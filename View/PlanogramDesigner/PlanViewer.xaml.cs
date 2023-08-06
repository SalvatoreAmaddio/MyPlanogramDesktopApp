using Aspose.Drawing;
using Microsoft.Win32;
using MyPlanogramDesktopApp.Controller;
using MyPlanogramDesktopApp.Customs;
using MyPlanogramDesktopApp.Model;
using MyPlanogramDesktopApp.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Testing.DB;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.View
{

    public partial class PlanViewer : Page
    {
        public ShelfListController Controller { get; }
       
        public PlanViewer()
        {
            InitializeComponent();
            Controller = (ShelfListController)DataContext;
            Controller.TurnSettingControllerOn();
            SettingController.Fixture2 = Fixture;
        }
    }
}

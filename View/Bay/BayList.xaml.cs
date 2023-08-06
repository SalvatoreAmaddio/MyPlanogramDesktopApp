﻿using MyPlanogramDesktopApp.Controller;
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

namespace MyPlanogramDesktopApp.View.Bay
{
    /// <summary>
    /// Interaction logic for BayList.xaml
    /// </summary>
    public partial class BayList : Page
    {
        public BayListController Controller { get; }

        public BayList()
        {
            InitializeComponent();
            Controller = (BayListController)DataContext;
        }
    }
}

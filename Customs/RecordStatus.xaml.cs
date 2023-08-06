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
using Testing.Model.Abstracts;

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for RecordStatus.xaml
    /// </summary>
    public partial class RecordStatus : Label {

        public RecordStatus()
        {
            InitializeComponent();
        }

        #region IsDirty
        public static readonly DependencyProperty IsDirtyProperty =
        DependencyProperty.Register(
        nameof(IsDirty), typeof(bool), typeof(RecordStatus),

        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback = IsDirtyPropertyChanged
        }
        );

        public static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RecordStatus)d;
            bool IsDirty = (bool)e.NewValue;
            if (IsDirty)
            {
                control.Content = "*";
                control.Foreground = Brushes.Red;
                control.FontWeight = FontWeights.Bold;
            }
            else
            {
                if (control.IsNew) 
                {
                    control.Content = "*";
                    control.Foreground = Brushes.Yellow;
                    control.FontWeight = FontWeights.Bold;
                    return;
                }
                control.Content = "⮞";
                control.Foreground = Brushes.Black;
                control.FontWeight = FontWeights.Black;
            }
        }

        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }
        #endregion

        #region IsNewRecord
        public static readonly DependencyProperty IsNewProperty =
        DependencyProperty.Register(
        nameof(IsNew), typeof(bool), typeof(RecordStatus),

        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = false,
            DefaultValue = false,
            PropertyChangedCallback = IsNewPropertyChanged
        }
        );

        public static void IsNewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RecordStatus)d;
            bool IsNew = (bool)e.NewValue;
            if (IsNew)
            {
                control.Content = "*";
                control.Foreground = Brushes.Yellow;
                control.FontWeight = FontWeights.Bold;
            }
            else
            {
                control.Content = "⮞";
                control.Foreground = Brushes.Black;
                control.FontWeight = FontWeights.Black;
            }
        }

        public bool IsNew
        {
            get => (bool)GetValue(IsNewProperty);
            set => SetValue(IsNewProperty, value);
        }
        #endregion

    }
}

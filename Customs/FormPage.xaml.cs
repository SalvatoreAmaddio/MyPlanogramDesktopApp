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

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for FormPage.xaml
    /// </summary>
    public partial class FormPage : Page
    {
        public FormPage()
        {
            InitializeComponent();
           
        }

        #region RecordTrackerVisibility
        public static readonly DependencyProperty RecordTrackerVisibilityProperty =
        DependencyProperty.Register(
        nameof(RecordTrackerVisibility), typeof(Visibility), typeof(FormPage),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = Visibility.Visible,
            PropertyChangedCallback = RecordTrackerVisibilityPropertyChanged
        }
        );

        public static void RecordTrackerVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var control = (FormPage)d;
            control.RecordTracker.Visibility = (Visibility)e.NewValue;

            switch(control.RecordTracker.Visibility)
            {
                case Visibility.Hidden:
                case Visibility.Collapsed:
                    control.MainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
                break;
                case Visibility.Visible:
                control.MainGrid.RowDefinitions[1].Height = new GridLength(30, GridUnitType.Pixel);
                break;
            }
        }

        public Visibility RecordTrackerVisibility
        {
            get => (Visibility)GetValue(RecordTrackerVisibilityProperty);
            set => SetValue(RecordTrackerVisibilityProperty, value);
        }
        #endregion

        #region Root
        public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(
        nameof(Root), typeof(UIElement), typeof(FormPage),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = RootPropertyChanged
        }
        );

        public static void RootPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var control = (FormPage)d;
            var UIElement = (UIElement)e.NewValue;
            Grid.SetColumn(UIElement, 1);
            control.MainGrid.Children.Add(UIElement);
        }

        public UIElement Root
        {
            get => (UIElement)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }
        #endregion

    }
}

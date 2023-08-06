using MyPlanogramDesktopApp.Controller;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using static System.Net.Mime.MediaTypeNames;

namespace MyPlanogramDesktopApp.Customs
{
    /// <summary>
    /// Interaction logic for DynamicControl.xaml
    /// </summary>
    public partial class DynamicControl : Grid
    {
        private Label Label = new();

        public DynamicControl()
        {
            InitializeComponent();
            ColumnDefinitions.Add(new () { Width=new(85)});
            ColumnDefinitions.Add(new() { Width = new(1,GridUnitType.Star) });
            RowDefinitions.Add(new() { Height=new(30)});
            Children.Add(Label);
            Grid.SetColumn(Label,0);
        }

        #region SettingOptions
        public static readonly DependencyProperty SettingOptionProperty =
        DependencyProperty.Register(
        nameof(SettingOption), typeof(SettingOptions), typeof(DynamicControl),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = SettingOptionsPropertyChanged
        }
        );


        private void CreateTextBox()
        {            
            Binding myBinding = new("Value") { Source = SettingOption, Mode = BindingMode.TwoWay, StringFormat = SettingOption.Format };
            TextBox txt = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            txt.SetBinding(TextBox.TextProperty, myBinding);
            Children.Add(txt);
            Grid.SetColumn(txt, 1);
        }

        private void CreateComboBox()
        {
            List<Stretch> streches = new();
            streches.Add(Stretch.None);
            streches.Add(Stretch.Fill);
            streches.Add(Stretch.UniformToFill);
            streches.Add(Stretch.Uniform);

            Binding myBinding = new("Value") { Source = SettingOption, Mode = BindingMode.TwoWay, StringFormat = SettingOption.Format };
            ComboBox combo = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                ItemsSource = streches,
            };

            combo.SetBinding(ComboBox.SelectedItemProperty, myBinding);
            Children.Add(combo);
            Grid.SetColumn(combo, 1);
        }

        private void CreateLabel()
        {
            Label lbl = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = SettingOption.Value.ToString(),
            };

            Children.Add(lbl);
            Grid.SetColumn(lbl, 1);
        }

        private static void SettingOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            DynamicControl grid = (DynamicControl)d;
            SettingOptions settingoption = (SettingOptions)e.NewValue;
            grid.Label.Content = settingoption.Name;
            switch (settingoption.ControlType)
            {
                case ControlType.TextBox:
                    grid.CreateTextBox();
                    break;
                case ControlType.Label:
                    grid.CreateLabel();
                    break;
                case ControlType.ComboBox:
                    grid.CreateComboBox();
                    break;
                case ControlType.CheckBox:
                    break;
            }
        }

        public SettingOptions SettingOption
        {
            get => (SettingOptions)GetValue(SettingOptionProperty);
            set => SetValue(SettingOptionProperty, value);
        }

        #endregion 
    }
}

using System;
using System.Collections;
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
    /// Interaction logic for ExcelButton.xaml
    /// </summary>
    public partial class ExcelButton : Button
    {
        public ExcelButton()
        {
            InitializeComponent();
          
        }

        #region ContentLabel
        public static readonly DependencyProperty ContentLabelProperty =
        DependencyProperty.Register(
        nameof(ContentLabel), typeof(string), typeof(ExcelButton),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = string.Empty,
            PropertyChangedCallback = ContentLabelPropertyChanged
        }
        );

        private static void ContentLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((TextBlock)(((ExcelButton)d).ButtonStackPanel.Children[1])).Text = e.NewValue.ToString();        
        }

        public string ContentLabel
        {
            get => (string)GetValue(ContentLabelProperty);
            set => SetValue(ContentLabelProperty, value);
        }

        #endregion

    }
}

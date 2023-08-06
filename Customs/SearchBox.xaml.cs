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
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : TextBox
    {
        public Button ClearButton { get; }

        public SearchBox()
        {
            InitializeComponent();
            TextChanged += SearchBox_TextChanged;
            GotFocus += SearchBox_GotFocus;
            LostFocus += SearchBox_LostFocus;
            ApplyTemplate();
            ClearButton = (Button)Template.FindName("ClearButton",this);
            ClearButton.Command= new ClearTextCommand(this);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)=>
            ShowClearButton = Visibility.Collapsed;

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Text.Length > 0)
            {
                ShowClearButton = Visibility.Visible;
                ShowPlaceHolder = Visibility.Collapsed;
                return;
            }
            ShowClearButton = Visibility.Collapsed;
            ShowPlaceHolder = Visibility.Visible;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text.Length>0)
            {
                ShowClearButton = Visibility.Visible;
                ShowPlaceHolder = Visibility.Collapsed;
                return;
            }
            ShowClearButton = Visibility.Collapsed;
            ShowPlaceHolder = Visibility.Visible;
        }

        #region ShowPlaceHolder
        public static readonly DependencyProperty ShowPlaceHolderProperty =
        DependencyProperty.Register(
        nameof(ShowPlaceHolder), typeof(Visibility), typeof(SearchBox),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = Visibility.Visible,
        }
        );

        public Visibility ShowPlaceHolder
        {
            get => (Visibility)GetValue(ShowPlaceHolderProperty);
            set => SetValue(ShowPlaceHolderProperty, value);
        }

        #endregion

        #region ShowClearButton
        public static readonly DependencyProperty ShowClearButtonProperty =
        DependencyProperty.Register(
        nameof(ShowClearButton), typeof(Visibility), typeof(SearchBox),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = Visibility.Collapsed,
        }
        );

        public Visibility ShowClearButton
        {
            get => (Visibility)GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }
        #endregion

        #region PlaceHolderText
        public static readonly DependencyProperty PlaceHolderProperty =
        DependencyProperty.Register(
        nameof(PlaceHolder), typeof(string), typeof(SearchBox),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = string.Empty,
        }
        );

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }
        #endregion

        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(SearchBox),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = new CornerRadius(0),
        }
        );

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        public override string ToString()=>"SearchBox";
    }

    #region ClearTextCommand
    public class ClearTextCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public object TextControl { get;}

        public ClearTextCommand(object searchbox) =>
            TextControl= searchbox;

        public bool CanExecute(object? parameter)=>true;

        public void Execute(object? parameter)
        {
            if (TextControl is Combo)
            {
                Combo combo = (Combo)TextControl;
                combo.Text = string.Empty;
                return;
            }

            if (TextControl is SearchBox) 
                ((SearchBox)TextControl).Text = string.Empty;
        }
    }
    #endregion
}

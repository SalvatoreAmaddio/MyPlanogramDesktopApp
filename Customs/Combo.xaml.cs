using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Testing.Model.Abstracts;
using Testing.RecordSource;

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for Combo.xaml
    /// </summary>
    public partial class Combo : ComboBox
    {
        public SearchBox SearchBox { get; }
        bool LetItemSelection = true;
        bool LetSourceChanged = true;

        public Combo() 
        {
            InitializeComponent();
            ShowClearButton = Visibility.Collapsed;
            ApplyTemplate();
            SearchBox = (SearchBox)Template.FindName("PART_EditableTextBox", this);
            SearchBox.ClearButton.Command = new ClearTextCommand(this);
        }

        private void SelectFirst()
        {
            if (FilterAndSort.SelectItem)
            {
                if (Items.Count > 0)
                    SelectedItem = Items[0];
                return;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
            base.OnSelectionChanged(e);
            if (!LetItemSelection) return;
            object? obj;

            if (e.AddedItems.Count > 0)
            {
                obj = e.AddedItems[0];
                foreach (var record in ItemsSource)
                {
                    if (record.Equals(obj))
                    {
                        LetItemSelection = false;
                        SearchBox.Text = record.ToString();
                        LetItemSelection = true;
                        break;
                    }
                }
            }

            if (e.RemovedItems.Count == 1 && e.AddedItems.Count == 0)
            {
                if (AllowNull)
                {
                    if (string.IsNullOrEmpty(SearchBox.Text))
                    {
                        SearchBox.Text = string.Empty;
                        return;
                    }
                }

                MessageBox.Show($"{SearchBox.Text} does not exist.", "INPUT ERROR");
                obj = e.RemovedItems[0];
                foreach (var item in ItemsSource)
                {
                    if (item.Equals(obj))
                    {
                        SelectedItem = item;
                        break;
                    }
                }
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!LetSourceChanged) return;
                FilterAndSortSource();
        }

        #region AllowNull
        public static readonly DependencyProperty AllowNullProperty =
        DependencyProperty.Register(
        nameof(AllowNull), typeof(bool), typeof(Combo),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
        }
        );

        public bool AllowNull
        {
            get => (bool)GetValue(AllowNullProperty);
            set => SetValue(AllowNullProperty, value);
        }

        #endregion

        #region FilterAndSort
        public static readonly DependencyProperty FilterAndSortProperty =
        DependencyProperty.Register(
        nameof(FilterAndSort), typeof(AbstractFilterAndSort), typeof(Combo) ,       
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = FilterAndSortPropertyChanged
        }
        );

        private static void FilterAndSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((Combo)d).FilterAndSortSource();
        }

        public AbstractFilterAndSort FilterAndSort
        {
            get => (AbstractFilterAndSort)GetValue(FilterAndSortProperty);
            set => SetValue(FilterAndSortProperty, value);
        }

        public void FilterAndSortSource() {
            if (FilterAndSort == null) return;
            if (ItemsSource == null) return;
            LetItemSelection = false;
            LetSourceChanged = false;
            if (!FilterAndSort.SkipFilter)
            {
                FilterAndSort.FilterSource();

                if (FilterAndSort.FilterRan)
                {
                    ItemsSource = FilterAndSort.Source;
                    FilterAndSort.FilterRan = false;
                }
            }

            FilterAndSort.SortSource(ItemsSource);

            if (FilterAndSort.SortRan)
            {
                ItemsSource = FilterAndSort.Source;
                FilterAndSort.SortRan = false;
            }

            LetItemSelection = true;
            SelectFirst();
            LetSourceChanged = true;
        }
        #endregion

        #region PlaceHolderText
        public static readonly DependencyProperty PlaceHolderProperty =
        DependencyProperty.Register(
        nameof(PlaceHolder), typeof(string), typeof(Combo),
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

        #region ShowPlaceHolder
        public static readonly DependencyProperty ShowPlaceHolderProperty =
        DependencyProperty.Register(
        nameof(ShowPlaceHolder), typeof(Visibility), typeof(Combo),
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
        nameof(ShowClearButton), typeof(Visibility), typeof(Combo),
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

    }

}
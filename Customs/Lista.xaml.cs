using MvvmHelpers;
using Testing.Model.Abstracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Testing.RecordSource;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Reflection.PortableExecutable;
using MyPlanogramDesktopApp.Customs;

namespace Testing.Customs
{

    public partial class Lista : ListView
    {
        bool LetSourceChanged = true;

        public Lista()
        {
            InitializeComponent();
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

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!LetSourceChanged) return;
                FilterAndSortSource();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count == 0) return;
            ScrollIntoView(e.AddedItems[0]);
        }

        public ListBoxItem GetListBoxItem(object item) =>
        (ListBoxItem)(ItemContainerGenerator.ContainerFromItem(item));

        public ListBoxItem GetListBoxItem(int index) =>
        (ListBoxItem)(ItemContainerGenerator.ContainerFromItem(Items[index]));

        public object GetLastItem() => Items[Items.Count - 1];
        public void ScrollToLast() => ScrollIntoView(GetLastItem());

        #region FilterAndSort
        public static readonly DependencyProperty FilterAndSortProperty =
        DependencyProperty.Register(
        nameof(FilterAndSort), typeof(AbstractFilterAndSort), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback= FilterAndSortPropertyChanged
        }
        );

        private static void FilterAndSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((Lista)d).FilterAndSortSource();
        }

        public AbstractFilterAndSort FilterAndSort
        {
            get => (AbstractFilterAndSort)GetValue(FilterAndSortProperty);
            set => SetValue(FilterAndSortProperty, value);
        }

        public void FilterAndSortSource()
        {
            if (FilterAndSort == null) return;
            if (ItemsSource == null) return;
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

            SelectFirst();
            LetSourceChanged = true;
        }

        #endregion

        #region ColumnHeaderHeight
        public static readonly DependencyProperty ColumnHeaderHeightProperty =
        DependencyProperty.Register(
        nameof(ColumnHeaderHeight), typeof(double), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = 30.0,
            PropertyChangedCallback = ColumnHeaderHeightPropertyChanged
        }
        );

        public static void ColumnHeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var control = (Lista)d;
            control.ApplyTemplate();
            var template = control.Template;
            var Grid = (Grid)template.FindName("GridContent", control);
            Grid.RowDefinitions[0].Height = new GridLength(double.Parse(e.NewValue.ToString()));
        }

        public double ColumnHeaderHeight
        {
            get => (double)GetValue(ColumnHeaderHeightProperty);
            set => SetValue(ColumnHeaderHeightProperty, value);
        }


        #endregion

        #region ColumnHeader
        public static readonly DependencyProperty HeaderColumnProperty =
        DependencyProperty.Register(
        nameof(HeaderColumn), typeof(FrameworkElement), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = HeaderColumnPropertyChanged
        }
        );

        public static void HeaderColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var control = (Lista)d;
            control.ApplyTemplate();
            var template = control.Template;
            var Grid = (Grid)template.FindName("GridContent", control);
            var UIElement = (FrameworkElement)e.NewValue;
            Grid.SetRow(UIElement, 0);
            Grid.Children.Add(UIElement);
        }

        public FrameworkElement HeaderColumn
        {
            get => (FrameworkElement)GetValue(HeaderColumnProperty);
            set => SetValue(HeaderColumnProperty, value);
        }
        #endregion

        #region UseHeaders
        public static readonly DependencyProperty UseHeaderProperty =
        DependencyProperty.Register(
        nameof(UseHeader), typeof(bool), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = true,
            PropertyChangedCallback = UseHeaderPropertyChanged
        }
        );

        public static void UseHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var control = (Lista)d;
            control.ApplyTemplate();
            var template = control.Template;
            var Grid = (Grid)template.FindName("GridContent", control);
            if (!(bool)e.NewValue)
            {
                Grid.RowDefinitions.RemoveAt(0);
                return;
            }

            var rows=Grid.RowDefinitions.Count;
            if (rows == 2) return;
            if (rows==1)
            {
                RowDefinition row = new RowDefinition();
                row.Height=new GridLength(30, GridUnitType.Star);
                Grid.RowDefinitions.Insert(0, row);
            }
        }

        public bool UseHeader
        {
            get => (bool)GetValue(UseHeaderProperty);
            set => SetValue(UseHeaderProperty, value);
        }

        #endregion
    }

}

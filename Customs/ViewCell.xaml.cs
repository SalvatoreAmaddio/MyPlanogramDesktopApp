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

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for ViewCell.xaml
    /// </summary>
    public partial class ViewCell : Grid
    {
        private MoveRecordEvtArgs args=new();

        public ViewCell()
        {
            InitializeComponent();
            Rec.AllowDrop = true;
            Rec.MouseMove += Rec_MouseMove;
            Rec.Drop += Rec_Drop;
        }

        private void Rec_MouseMove(object sender, MouseEventArgs e)
        {
            args.e = e;
            if (!args.IsMoving) return;
            DragDrop.DoDragDrop((DependencyObject)e.Source, new DataObject(DataFormats.Serializable, DataContext), DragDropEffects.Move);
        }

        private void Rec_Drop(object sender, DragEventArgs e)
        {
            args.ObjectFrom = e.Data.GetData(DataFormats.Serializable);
            args.ObjectTo = ((Label)sender).DataContext;
            _moveRowEvt?.Invoke(this, args);
        }

        event EventHandler<MoveRecordEvtArgs>? _moveRowEvt;

        public event EventHandler<MoveRecordEvtArgs>? MoveRowEvt
        {
            add
            {
                _moveRowEvt += value;
            }
            remove
            {
                _moveRowEvt -= value;
            }
        }

        #region Root
        public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(
        nameof(Root), typeof(UIElement), typeof(ViewCell),
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
            var control = (ViewCell)d;
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

    #region Args
    public class MoveRecordEvtArgs : EventArgs {
        public bool IsMoving { get => e.LeftButton == MouseButtonState.Pressed; }
        public object? ObjectFrom;
        public object? ObjectTo;
        public MouseEventArgs e;
        
    }
    #endregion
}

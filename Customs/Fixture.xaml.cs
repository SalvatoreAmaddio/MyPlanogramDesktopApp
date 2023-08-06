using MyPlanogramDesktopApp.Controller;
using MyPlanogramDesktopApp.Model;
using MyPlanogramDesktopApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Testing.Customs;
using Testing.DB;

namespace MyPlanogramDesktopApp.Customs
{
    /// <summary>
    /// Interaction logic for MyTemplate.xaml
    /// </summary>
    public partial class Fixture : Grid
    {
        public static List<ICommon> Elements = new();
        private IFixture? IFixture;
        public static bool SelectionStarted => (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        public static ListOfSettingModels ListOfSelectedSettingModels = new();
        public static bool IsSelecting => ListOfSelectedSettingModels.Count > 0;

        public Fixture()         
        {
            InitializeComponent();
            Children.Add(MainBorder);
            Grid.SetRow(MainBorder, 0);
            Grid.SetColumn(MainBorder, 0);
        }

        private Border MainBorder = new()
        {
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black,
        };

        public static DependencyProperty Register<T, C>(string propName, bool twoway, T? defaultValue, PropertyChangedCallback? callback = null)
        {
            return DependencyProperty.Register(
            propName,
            typeof(T),
            typeof(C),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = twoway,
                DefaultValue = defaultValue,
                PropertyChangedCallback = callback,
            }
        );
        }

        #region ShelfProperty
        public static readonly DependencyProperty ShelfProperty
        = Register<Shelf, Fixture>(nameof(ShelfModel), true, null, ShelfPropertyChanged);

        private static void ShelfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fixture fixture = (Fixture)d;
            if (e.NewValue == null) return;
            Shelf? shelf = e.NewValue as Shelf;
            fixture.DrawFixture(shelf);
        }

        public Shelf ShelfModel
        {
            get => (Shelf)GetValue(ShelfProperty);
            set => SetValue(ShelfProperty, value);
        }
        #endregion

        private void DrawFixture(Shelf ShelfModel)
        {
            IFixture = (ShelfModel.OnHook) ? new CanvasHooks(ShelfModel) : new ShelfStack(ShelfModel);
            foreach (var product in MainDB.DBPlanogram.RecordSource.Where(s => s.Shelf.Equals(ShelfModel)))
            {
                if (!product.Substitute)
                {
                    int faces = 1;
                    while (faces <= product.Faces)
                    {
                        IFixture.AddPicture(new(product, ContextMenuType.SHELF) { Face = faces });
                        faces++;
                    }
                    faces = 1;
                }
            }
            MainBorder.Child = (UIElement)IFixture;
            IFixture.FixtureHeight = (IFixture.FixtureType.Equals(FixtureType.SHELF)) ? 100 : 500;
            Elements.Add(IFixture);

            Label label = new()
            {
                Foreground = Brushes.White,
                Background = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontStretch = FontStretches.Expanded,
                Content = IFixture?.ToString()
            };
            Children.Add(label);
            Grid.SetRow(label, 1);
            Grid.SetColumn(label, 0);
        }

    }

    #region CanvasHooks
    public class CanvasHooks : Canvas, IFixture, ICommon
    {
        public Border BorderParent { get => (Border)Parent; }
        public Fixture Fixture { get => (Fixture)BorderParent.Parent; }
        public Shelf ShelfModel { get; }
        public int Notch { get => ShelfModel.Notch; }
        public FixtureType FixtureType => FixtureType.HOOKS;
        public static List<HorizontalGrid> Grids { get; } = new();

        public double FixtureHeight
        {
            get => Fixture.RowDefinitions[0].Height.Value;
            set => Fixture.RowDefinitions[0].Height = new(value);
        }

        public CanvasHooks(Shelf shelfmodel) 
        {
            ShelfModel = shelfmodel;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = Brushes.AliceBlue;
            AllowDrop = true;
            Drop += Canvas_Drop;
            MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        }

        public void AddPicture(BorderedPicture picture)
        {
            Children.Add(picture);
            Canvas.SetLeft(picture,0);
            Fixture.Elements.Add(picture);
            picture.IFixture = this;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Equals(e.OriginalSource)) return;
            Fixture.ListOfSelectedSettingModels.ClearAll();
            if (WindowTracker.WebPage.Controller.SettingController.SettingModel != null)
            {
                WindowTracker.WebPage.Controller.SettingController.SettingModel?.BorderedPicture?.AddEvent();
            }

            WindowTracker.WebPage.Controller.SettingController.SettingModel = new(this);
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            var p = e.GetPosition(this);
            BorderedPicture ObjectFrom = (BorderedPicture)e.Data.GetData(DataFormats.Serializable);
            UIElement? element = (ObjectFrom.Parent is Grid) ? (Grid)ObjectFrom.Parent : ObjectFrom;
            SetLeft(element, p.X);
            SetTop(element, p.Y);
            if (WindowTracker.WebPage.Controller.SettingController.SettingModel != null)
            {
                WindowTracker.WebPage.Controller.SettingController.SettingModel?.BorderedPicture?.AddEvent();
            }

            ObjectFrom.RemoveEvent();
            WindowTracker.WebPage.Controller.SettingController.SettingModel = new(ObjectFrom);

        }

        public override string? ToString() =>"ON HOOKS";
        public bool IsEqualsTo(DesignerJSON obj) => Notch == obj.Notch;
    }
    #endregion

    #region ShelfStack 
    public class ShelfStack : StackPanel, IFixture, ICommon
    {
        public Border BorderParent { get => (Border)Parent; }
        public Fixture Fixture { get => (Fixture)BorderParent.Parent; }
        public Shelf ShelfModel { get;}
        public int Notch { get => ShelfModel.Notch; }

        public double FixtureHeight 
        { 
            get => Fixture.RowDefinitions[0].Height.Value; 
            set => Fixture.RowDefinitions[0].Height = new(value);
        }

        public FixtureType FixtureType => FixtureType.SHELF;

        public ShelfStack(Shelf shelfmodel)
        {
            ShelfModel = shelfmodel;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Stretch; 
            VerticalAlignment = VerticalAlignment.Bottom;
            Background = Brushes.White;
            MouseLeftButtonDown += ShelfStack_MouseLeftButtonDown;
        }
        
        public void AddPicture(BorderedPicture picture)
        {
            Children.Add(picture);
            Fixture.Elements.Add(picture);
        }

        private void ShelfStack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Equals(e.OriginalSource)) return;
                Fixture.ListOfSelectedSettingModels.ClearAll();
                if (WindowTracker.WebPage.Controller.SettingController.SettingModel != null)
                {
                    WindowTracker.WebPage.Controller.SettingController.SettingModel?.BorderedPicture?.AddEvent();
                }

                WindowTracker.WebPage.Controller.SettingController.SettingModel = new(this);
         }

        
        public override string? ToString() =>
         $"SHELF: {ShelfModel.ShelfNum} - NOTCH: {ShelfModel.Notch}";

        public bool IsEqualsTo(DesignerJSON obj) => Notch == obj.Notch;
    }

    public enum FixtureType
    {
        SHELF = 1,
        HOOKS = 2,
    }

    public interface ICommon
    {
        public bool IsEqualsTo(DesignerJSON obj);
    }

    public interface IFixture : ICommon
    {
        public Shelf ShelfModel { get; }
        public int Notch { get; }
        public double FixtureHeight { get; set; }
        public FixtureType FixtureType { get; }
        public void AddPicture(BorderedPicture picture);
    }

    #endregion
}

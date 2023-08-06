using MyPlanogramDesktopApp.Controller;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Testing.Customs;
using Testing.Utilities;

namespace MyPlanogramDesktopApp.Customs
{
    /// <summary>
    /// Interaction logic for BorderedPicture.xaml
    /// </summary>
    public partial class BorderedPicture : Border,ICommon
    {
        private BitmapImage BitmapImage = new();
        public Planogram? Planogram;
        public int Face { get; set; }
        public IFixture? IFixture { get; set; }

        public BorderedPicture()
        {
            InitializeComponent();
        }

        public BorderedPicture(Planogram planogram, ContextMenuType ContextMenuType = ContextMenuType.HOOKS)
        {
            InitializeComponent();
            Planogram = planogram;
            BitmapImage.BeginInit();
            BitmapImage.UriSource = new Uri(Planogram?.Product?.PictureURL);
            BitmapImage.EndInit();
            Picture.Source = BitmapImage;
            MakeContextMenuHooks(ContextMenuType);
            VerticalAlignment = VerticalAlignment.Bottom;
        }

        private void MakeContextMenuHooks(ContextMenuType ContextMenuType)
        {
            Picture.ContextMenu = new();
            if (ContextMenuType == ContextMenuType.SHELF)
            {
                MenuItem selectall = new MenuItem() { Header = "Select All" };
                selectall.Click += Selectall_Click; ;
                Picture.ContextMenu.Items.Add(selectall);
                return;
            }
            MenuItem copy = new MenuItem() { Header = "Copy" };
            MenuItem delete = new MenuItem() { Header = "Delete" };
            copy.Click += Copy_Click;
            delete.Click += Delete_Click;
            Picture.ContextMenu.Items.Add(copy);
            Picture.ContextMenu.Items.Add(delete);
        }

        private void Selectall_Click(object sender, RoutedEventArgs e)
        {
            Fixture.ListOfSelectedSettingModels.ClearAll();
            StackPanel stackPanel = (StackPanel)this.Parent;
            foreach(var borderedpicture in stackPanel.Children.Cast<BorderedPicture>())
            {
                SettingModel model = new(borderedpicture);
                Fixture.ListOfSelectedSettingModels.AddModel(model);
                WindowTracker.WebPage.Controller.SettingController.SettingModel = model;
            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)=>DisconnectFromParent();

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            BorderedPicture x = new(this.Planogram);
            if (Canvas == null) 
            {
                MessageBox.Show("Cannot Copy");
                return;
            }
            Canvas?.Children.Add(x);
            var settingmodel= new SettingModel(this);
            Canvas.SetLeft(x, settingmodel.Left+ settingmodel.Width);
            Canvas.SetTop(x, settingmodel.Top);
        }

        public void DisconnectFromParent()
        {
            Panel? panel=this.Parent as Panel;
            panel?.Children.Remove(this);

            if (panel is HorizontalGrid grid)
            {
                grid.Restructure();
            }
            Fixture.Elements.Remove(this);
        }

        public Canvas? Canvas=>this.Parent as Canvas;

        private void Border_MouseEnter(object sender, MouseEventArgs e)=>
        BorderThickness = new(1);

        private void Border_MouseLeave(object sender, MouseEventArgs e)=>
        BorderThickness = new(0);

        public void ForceEvent()=> BorderThickness = new(1);

        public void AddEvent()
        {
            MouseLeave += Border_MouseLeave;
            BorderThickness = new(0);
        }

        public void RemoveEvent()
        {
            try
            {
                MouseLeave -= Border_MouseLeave;
            } catch { }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SettingModel model = new(this);

            if (WindowTracker.WebPage.Controller.SettingController.SettingModel != null)
            {
                if (!Fixture.ListOfSelectedSettingModels.Any(s=>s.Equals(WindowTracker.WebPage.Controller.SettingController.SettingModel))) 
                {
                    WindowTracker.WebPage.Controller.SettingController.SettingModel?.BorderedPicture?.AddEvent();
                }
            }

            if (Fixture.SelectionStarted)
            {
                WindowTracker.WebPage.Controller.SettingController.SettingModel?.BorderedPicture?.RemoveEvent();
                Fixture.ListOfSelectedSettingModels.AddModel(model);             
                model.UpdateSelectedString();
            } 
            else
            {
                Fixture.ListOfSelectedSettingModels.ClearAll();
                RemoveEvent();
            }

            WindowTracker.WebPage.Controller.SettingController.SettingModel = model;

        }

        private async void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
                await Task.Delay(200);
                if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop((DependencyObject)this, new DataObject(DataFormats.Serializable, sender), DragDropEffects.Move);
        }

        public bool IsEqualsTo(DesignerJSON obj) => Planogram?.PlanoID==obj.PlanogramID && Face == obj.Face;

    }

    public enum ContextMenuType
    {
        SHELF=1,
        HOOKS=2,
    }

    public class ListOfSettingModels : List<SettingModel>
    {
        public CanvasHooks? Canvas;

        public void AddModel(SettingModel model)
        {
            model.BorderedPicture?.RemoveEvent();
            if (Canvas==null)
            {
                Canvas = (CanvasHooks)(UIElement)model.BorderedPicture?.IFixture;
            }
            Add(model);
        }    

        public void ClearAll()
        {
            foreach (var model in this)
            {
                model.BorderedPicture?.AddEvent();

            }
            Canvas = null;
            Clear();
        }
    }

    #region HorizontalGrid
    public class HorizontalGrid : Grid,ICommon
    {
        public SettingModel First { get => Fixture.ListOfSelectedSettingModels.First();}
        private int col=0;
        public static int Count=0;
        public string? GridName { get=>ToString(); }
        public int GridNum;

        public HorizontalGrid() 
        {
            Count++;
            GridNum = Count;
            Fixture.Elements.Add(this);
        }

        public void ColumnWidth(int col, double value)=>
        ColumnDefinitions[col].Width = new(value);

        public int GetColumn(UIElement element)=>Grid.GetColumn(element);

        public void Restructure()
        {
            col = 0;
            ColumnDefinitions.Clear();

            if (Children.Count==0)
            {
                Fixture.Elements.Remove(this);
                return;
            }

            foreach (var x in Children.Cast<BorderedPicture>())
            {
                ColumnDefinitions.Add(new() { Width = new(x.Width) });
                Grid.SetColumn(x, col);
                col++;
            }
        }

        public void FillAndSet()
        {
            col = 0;
            Canvas.SetLeft(this, First.Left);
            Canvas.SetTop(this, First.Top);

            foreach (var model in Fixture.ListOfSelectedSettingModels)
            {
                model?.BorderedPicture?.DisconnectFromParent();
                ColumnDefinitions.Add(new() { Width = new(model.BorderedPicture.Width) });
                Children.Add(model.BorderedPicture);
                Fixture.Elements.Add(model.BorderedPicture);
                Grid.SetColumn(model.BorderedPicture, col);
                col++;
            }
        }

        public override string? ToString()=>$"Grid_{GridNum}";

        public bool IsEqualsTo(DesignerJSON obj) => obj.GridNum == GridNum;

    }
    #endregion
}

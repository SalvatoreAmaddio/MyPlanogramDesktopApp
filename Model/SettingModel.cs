using Microsoft.Office.Interop.Excel;
using MyPlanogramDesktopApp.Customs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Testing.Customs;
using Testing.Model.Abstracts;

namespace MyPlanogramDesktopApp.Model {
    public class SettingModel : AbstractModel {

    double _left;
    double _top;
    double _height;
    double _width;
    int _zindex;
    Thickness _margin;
    Stretch _stretch;
    string _selected=string.Empty;

    public Stretch Stretch { get => _stretch; set => Set<Stretch>(ref value, ref _stretch,nameof(Stretch)); }
    public double Left { get => _left; set => Set<double>(ref value, ref _left, nameof(Left)); }
    public double Top { get => _top; set => Set<double>(ref value, ref _top, nameof(Top)); }
    public double Height { get => _height; set => Set<double>(ref value, ref _height, nameof(Height)); }
    public double Width { get => _width; set => Set<double>(ref value, ref _width, nameof(Width)); }
    public int ZIndex { get => _zindex; set => Set<int>(ref value, ref _zindex, nameof(ZIndex)); }
    public Thickness Margin { get => _margin; set => Set<Thickness>(ref value, ref _margin,nameof(Margin)); }
    public BorderedPicture? BorderedPicture;
    public Planogram? Planogram { get=>BorderedPicture?.Planogram; }
    private IFixture? IFixture;
    private Vector Offset;
    public bool IsCanvas { get=>IFixture?.FixtureType==FixtureType.HOOKS; }
    public bool IsShelf { get =>IFixture?.FixtureType == FixtureType.SHELF; }
    public int? Notch {get => IFixture?.Notch;}
    public bool? OnHooks { get => Planogram?.Shelf?.OnHook; }
    public string Selected { get => _selected; set => Set<string>(ref value, ref _selected,nameof(Selected)); }
    public HorizontalGrid? HorizontalGrid { get=>BorderedPicture?.Parent as HorizontalGrid; }

        #region Constructors
        public SettingModel() { }

        public SettingModel(object obj)
        {
//            Selected = (Fixture.SelectionStarted) ? $"{Fixture.ListOfSelectedSettingModels.Count} Selected" : "";
            BorderedPicture = obj as BorderedPicture;
            IFixture = obj as IFixture;
            Height = (IFixture==null) ? 0 : IFixture.FixtureHeight;

            AfterPropChanged +=FixtureAfterPropChanged;

            if (BorderedPicture == null) return;
            AfterPropChanged -= FixtureAfterPropChanged;

            Offset = (BorderedPicture.Parent is HorizontalGrid)
                ? VisualTreeHelper.GetOffset(BorderedPicture.Parent as HorizontalGrid)
                : VisualTreeHelper.GetOffset(BorderedPicture);

            Left = Offset.X;
            Top = Offset.Y;
            Width = BorderedPicture.ActualWidth;
            Height = BorderedPicture.ActualHeight;
            ZIndex = Canvas.GetZIndex(this.BorderedPicture);
            Margin = BorderedPicture.Margin;
            Stretch = BorderedPicture.Picture.Stretch;
            AfterPropChanged += (HorizontalGrid!=null)
                             ? GridBorderedPictureModel
                             : BorderedPictureModelAfterPropChanged;
        }
        #endregion


        public void SetBackOnCanvas()
        {
          CanvasHooks? Canvas= (CanvasHooks?)(UIElement?)BorderedPicture.IFixture;
          Canvas?.AddPicture(BorderedPicture);      
        }

        public void UpdateSelectedString() => Selected = (Fixture.SelectionStarted) ? $"{Fixture.ListOfSelectedSettingModels.Count} Selected" : "";

        #region AfterPropChanged
        private void BorderedPictureModelSelection(object? sender, PropChangedEvtArgs e)
        {
            if (Fixture.SelectionStarted) return;
            if (e.PropIs(nameof(Left)))
            {
                foreach(var element in Fixture.ListOfSelectedSettingModels)
                {
                    Canvas.SetLeft(element.BorderedPicture, e.GetValue<double>());
                }
                return;
            }

            if (e.PropIs(nameof(Top)))
            {
                foreach (var element in Fixture.ListOfSelectedSettingModels)
                {
                    Canvas.SetTop(element.BorderedPicture, e.GetValue<double>());
                }
                return;
            }

            if (e.PropIs(nameof(Width)))
            {
                foreach (var element in Fixture.ListOfSelectedSettingModels)
                {

                  if (element.BorderedPicture.Parent is Grid)
                    {
                        Grid grid = element.BorderedPicture.Parent as Grid;
                        int col = Grid.GetColumn(element.BorderedPicture);
                        grid.ColumnDefinitions[col].Width = new(e.GetValue<double>());
                    }
                    element.BorderedPicture.Width = e.GetValue<double>();
                }
                return;
            }

            if (e.PropIs(nameof(Margin)))
            {
                foreach (var element in Fixture.ListOfSelectedSettingModels)
                {
                    element.BorderedPicture.Margin = e.GetValue<Thickness>();
                }
                    return;
            }

            if (e.PropIs(nameof(Height)))
            {
                foreach (var element in Fixture.ListOfSelectedSettingModels)
                {
                    element.BorderedPicture.Height = e.GetValue<double>();
                }

            return;
            }

            if (e.PropIs(nameof(ZIndex)))
            {
                foreach (var element in Fixture.ListOfSelectedSettingModels)
                {
                    if (element.BorderedPicture.Parent is Grid)
                    {
                        Grid grid = element.BorderedPicture.Parent as Grid;
                        Canvas.SetZIndex(grid, e.GetValue<int>());
                    } else
                    {
                        Canvas.SetZIndex(element.BorderedPicture, e.GetValue<int>());
                    }
                }
                return;
            }
        }

        private void FixtureAfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Height))) 
            {
                IFixture.FixtureHeight = e.GetValue<double>();
                return;
            }

        }

        private void GridBorderedPictureModel(object? sender, PropChangedEvtArgs e)
        {
            HorizontalGrid grid = BorderedPicture.Parent as HorizontalGrid;

            if (e.PropIs(nameof(Left)))
            {
                Canvas.SetLeft(grid, e.GetValue<double>());
                return;
            }

            if (e.PropIs(nameof(Top)))
            {
                Canvas.SetTop(grid, e.GetValue<double>());
                return;
            }

            if (e.PropIs(nameof(Width)))
            {   
                grid?.ColumnWidth(grid.GetColumn(this.BorderedPicture),e.GetValue<double>());
                BorderedPicture.Width = e.GetValue<double>();
                return;
            }

            if (e.PropIs(nameof(Height)))
            {
                BorderedPicture.Height = e.GetValue<double>();
                return;
            }

            if (e.PropIs(nameof(ZIndex)))
            {
                Canvas.SetZIndex(BorderedPicture, e.GetValue<int>());
                return;
            }

            if (e.PropIs(nameof(Margin)))
            {
                BorderedPicture.Margin = e.GetValue<Thickness>();
                return;
            }

            if (e.PropIs(nameof(Stretch)))
            {
                BorderedPicture.Picture.Stretch = e.GetValue<Stretch>();
                return;
            }
        }

        private void BorderedPictureModelAfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (Fixture.IsSelecting)
            {
                BorderedPictureModelSelection(sender, e);
                return;
            }

            if (e.PropIs(nameof(Left)))
            {
                Canvas.SetLeft(BorderedPicture, e.GetValue<double>());
                return;
            }

            if (e.PropIs(nameof(Top)))
            {
                Canvas.SetTop(BorderedPicture, e.GetValue<double>());
                return;
            }

            if (e.PropIs(nameof(Width)))
            {
                BorderedPicture.Width= e.GetValue<double>();
                return;
            }

            if (e.PropIs(nameof(Height)))
            {
                BorderedPicture.Height = e.GetValue<double>();
                return;
            }

            if (e.PropIs(nameof(ZIndex)))
            {
                Canvas.SetZIndex(BorderedPicture, e.GetValue<int>());
                return;
            }

            if (e.PropIs(nameof(Margin)))
            {
                BorderedPicture.Margin = e.GetValue<Thickness>();
                return;
            }

            if (e.PropIs(nameof(Stretch)))
            {
                BorderedPicture.Picture.Stretch = e.GetValue<Stretch>();
                return;
            }
        }
        #endregion

        public static void Recalculate(SettingModel model, DesignerJSON? obj)
        {
            model.Height = obj.Height;
            model.Width = obj.Width;
            model.ZIndex = obj.ZIndex;
            model.Margin = obj.Margin;
            model.Stretch = obj.Stretch;
            model.Left = obj.Left;
            model.Top = obj.Top;
        }

        public void Recalculate()
        {
            Offset = VisualTreeHelper.GetOffset(BorderedPicture);
            Left = Offset.X;
            Top = Offset.Y;
            Width = BorderedPicture.ActualWidth;
            Height = BorderedPicture.ActualHeight;
        }

        public override void SetForeignKeys()
        {
           
        }

        public override bool IsNewRecord => true;

        public override string? ToString() => $"{BorderedPicture?.Planogram?.Product}";
 
    }

}

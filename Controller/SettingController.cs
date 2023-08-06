using Microsoft.Office.Interop.Excel;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MyPlanogramDesktopApp.Customs;
using MyPlanogramDesktopApp.Model;
using MyPlanogramDesktopApp.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Testing.Controller.AbstractControllers;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.Utilities;

namespace MyPlanogramDesktopApp.Controller
{
    public class SettingController : AbstractNotifier
    {
        GridLength _widthcol = new(0);
        string _content = "⮜";
        GridLength _heightcol = new(30);
        int _listwidth=200;

        public string Content { get=>_content; set=>Set<string>(ref value, ref _content,nameof(Content)); }
        public GridLength WidthCol { get => _widthcol; set => Set<GridLength>(ref value, ref _widthcol, nameof(WidthCol)); }
        public GridLength HeightCol { get => _heightcol; set => Set<GridLength>(ref value, ref _heightcol, nameof(HeightCol)); }
        public int ListWidth { get => _listwidth; set => Set<int>(ref value, ref _listwidth, nameof(ListWidth)); }

        public ICommand OpenSettingsCMD { get; }
        public Settings Settings { get; set; } = new();
        public ICommand TabularCMD { get; }
        public ICommand UntabularCMD { get; }
        private JSONManager<DesignerJSON> jSONWriter = new();
        public ICommand SaveCMD { get; }
        public ICommand ReadCMD { get; }
        public ICommand ConvertToPngCMD { get; }
        public static Lista Fixture2 { get; set; }

        SettingModel? _BorderedPictureModel;
        public SettingModel? SettingModel 
        { 
            get=>_BorderedPictureModel;
            set => Set<SettingModel?>(ref value,ref _BorderedPictureModel,nameof(SettingModel));
        }

        public ShelfListController? ParentController { get; set; }

        public SettingController() 
        {
            OpenSettingsCMD = new Command(ToggleSettings);
            TabularCMD = new Command(Tabular);
            UntabularCMD = new Command(Untabular);
            AfterPropChanged += SettingController_AfterPropChanged;
            SaveCMD = new Command(Save);
            ReadCMD = new AsyncCommand(Read);
            ConvertToPngCMD = new AsyncCommand(ConvertToPNG);
        }

        private async Task ConvertToPNG()
        {
            Fixture2.ScrollToLast();
            await Task.Delay(1000);
            List<ListBoxItem> lista = new();
            foreach (var child in Fixture2.Items)
            {
                var x = Fixture2.GetListBoxItem(child);
                lista.Add(x);
            }

            string name = ParentController.SelectedSection + "_BAY_" + ParentController.SelectedBay;
            ImageCreator imageCreator = new(lista, $"C:\\Users\\Salvatore\\Downloads\\Planogram\\Prova\\{name}.png");
            imageCreator.Create();
            MessageBox.Show("Bay Saved", "Done");
        }

        private async Task Read()
        {
            Fixture2.ScrollIntoView(Fixture2.GetLastItem());
            await Task.Delay(1000);

            jSONWriter.ReadList();
            Object? setting;

            foreach(var obj in jSONWriter.Objects)
            {
                setting = Fixture.Elements.FirstOrDefault(s=>s.IsEqualsTo(obj));
                if (setting!=null)
                {
                    SettingModel.Recalculate(new(setting), obj);
                }
            }

            var grids = jSONWriter.Objects.Where(s=>s.GridNum2>0);

            foreach(var grid in grids)
            {
                var range = jSONWriter.Objects.Where(s => s.GridNum == grid.GridNum2);
                
                foreach(var obj in range)
                {
                    setting = Fixture.Elements.FirstOrDefault(s => s.IsEqualsTo(obj));
                    Fixture.ListOfSelectedSettingModels.AddModel(new(setting));
                }

                ReTabular(grid.Left,grid.Top);
            }

        }

        private void Save()
        {
            var section = ParentController?.SelectedSection.SectionName;
            var bay = ParentController?.SelectedBay.BayNum;
            jSONWriter.FileName = $"{section}_BAY_{bay}";
            //jSONWriter.Path = $"C:\\Users\\Salvatore\\Downloads\\Planogram\\Prova";
            foreach (var product in Fixture.Elements)
            {
                if (product.GetType()==typeof(HorizontalGrid))
                {
                    jSONWriter.Objects.Add(new(product as HorizontalGrid));
                }
                else
                {
                    jSONWriter.Objects.Add(new(new SettingModel(product)));
                }
            }


            jSONWriter.WriteList();
        }

        private void ReTabular(double left, double top)
        {
            Canvas? canvas = Fixture.ListOfSelectedSettingModels.Canvas;
            if (canvas == null)
            {
                MessageBox.Show("Cannot Tabular");
                return;
            }
            HorizontalGrid grid = new();
            grid.FillAndSet();
            canvas.Children.Add(grid);
            Fixture.ListOfSelectedSettingModels.ClearAll();
            Canvas.SetLeft(grid,left);
            Canvas.SetTop(grid,top);
        }

        private void Tabular()
        {
            Canvas? canvas = Fixture.ListOfSelectedSettingModels.Canvas;
            if (canvas == null)
            {
                MessageBox.Show("Cannot Tabular");
                return;
            }
            HorizontalGrid grid = new();
            grid.FillAndSet();
            canvas.Children.Add(grid);
            Fixture.ListOfSelectedSettingModels.ClearAll();
        }

        private void Untabular()
        {
            Fixture.ListOfSelectedSettingModels.ClearAll();
            if (SettingModel?.BorderedPicture?.Parent is not HorizontalGrid) return;
            SettingModel?.BorderedPicture?.DisconnectFromParent();
            SettingModel?.SetBackOnCanvas();
            Canvas.SetLeft(SettingModel.BorderedPicture,0);
            Canvas.SetTop(SettingModel.BorderedPicture, 0);

        }

        private void SettingController_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SettingModel))) 
            {
                Settings.ReplaceSet(e.GetValue<SettingModel>());
                return;
            }
        }

        private void ToggleSettings()
        {
            if (WidthCol.Value==0)
            {
                OpenSettings();
            } else
            {
                CloseSettings();
            }
        }

        public void OpenSettings()
        {
            WidthCol = new(1,GridUnitType.Star);
            HeightCol = new(30+200+ListWidth);
            Content = "⮞";
        }

        public void CloseSettings()
        {
            WidthCol = new(0);
            HeightCol = new(30);
            Content = "⮜";
        }
    }

    public class Settings : ObservableRangeCollection<SettingOptions>
    {
        private SettingModel? Model;
        public Settings() { }
        
        public void ReplaceSet(SettingModel? model)
        {
            Clear();
            Model = model;
            if (Fixture.IsSelecting)
            {

                Add(new("Items", $"{Fixture.ListOfSelectedSettingModels.Count} Selected", Model) { ControlType = ControlType.Label });
                if ((bool)(Model?.OnHooks))
                {
                    Add(new("Left", Model?.Left, Model));
                    Add(new("Top", Model?.Top, Model));
                    Add(new("Z Index", Model?.ZIndex, Model) { Format = "" });
                }
                Add(new("Margin", Model?.Margin, Model));
                Add(new("Width", Model?.Width, Model));
                Add(new("Height", Model?.Height, Model));
                return;
            }

            if (Model.IsShelf)
            {
                Add(new("Item Name", "Shelf", Model) { ControlType = ControlType.Label });
                Add(new("Height", Model?.Height, Model));
                return;
            }
            if (Model.IsCanvas)
            {
                Add(new("Item Name", "CANVAS",Model) { ControlType = ControlType.Label });
                Add(new("Height", Model?.Height,Model));
                return;
            }

            
            Add(new("Item Name", Model?.Planogram?.Product?.ItemName, Model) { ControlType=ControlType.Label});
            Add(new("SKU", Model?.Planogram?.Product?.SKU, Model) { ControlType = ControlType.Label });
            Add(new($"Face {Model?.BorderedPicture.Face} of {Model?.Planogram?.Faces}","", Model) { ControlType = ControlType.Label });
            Add(new("Order", Model?.Planogram?.OrderList, Model) { ControlType = ControlType.Label });
            Add(new("Parent", Model?.BorderedPicture.Parent.ToString(), Model) { ControlType = ControlType.Label });

            if ((bool)(Model?.OnHooks))
            {
                Add(new("Left", Model?.Left, Model));
                Add(new("Top", Model?.Top, Model));
                Add(new("Z Index", Model?.ZIndex, Model) { Format = "" });
            }

            Add(new("Margin", Model?.Margin, Model));
            Add(new("Stretch", Model?.Stretch, Model) { ControlType=ControlType.ComboBox});
            Add(new("Width", Model?.Width, Model));
            Add(new("Height", Model?.Height, Model));
        }

    }

    public enum ControlType
    {
        TextBox=0,
        Label=1,
        ComboBox=2,
        CheckBox=3,
    }

    public class SettingOptions : AbstractNotifier
    {
        string _name=string.Empty;
        object? _value;
        string _format="N2";
        ControlType _controlType=ControlType.TextBox;

        public string Format { get => _format; set => Set<string>(ref value, ref _format, nameof(Format)); }
        public string Name { get=>_name; set=>Set<string>(ref value, ref _name,nameof(Name)); }
        public object? Value { get => _value; set => Set<object?>(ref value, ref _value, nameof(Value)); }
        public ControlType ControlType { get => _controlType; set => Set<ControlType>(ref value, ref _controlType, nameof(ControlType)); }
        public SettingModel? Model { get; set; }

        public SettingOptions(string name, object? value, SettingModel? model)
        {
            Name = name;
            Value = value;
            Model = model;
            AfterPropChanged += SettingOptions_AfterPropChanged;
        }

        private void SettingOptions_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Value)))
            {
                switch(Name)
                {
                    case "Top":
                        Model.Top =Convert.ToDouble(e.GetValue<object>().ToString());
                    break;
                    case "Left":
                        Model.Left = Convert.ToDouble(e.GetValue<object>().ToString());
                        break;
                    case "Width":
                        Model.Width = Convert.ToDouble(e.GetValue<object>().ToString());
                        break;
                    case "Height":
                        Model.Height = Convert.ToDouble(e.GetValue<object>().ToString());
                        break;
                    case "Z Index":
                        Model.ZIndex = Convert.ToInt32(e.GetValue<object>().ToString());
                        break;
                    case "Margin":
                        Thickness thickness = new();
                        var s = e.GetValue<object>().ToString();
                        var array = s.Split(",");
                        thickness.Left = Convert.ToInt32(array[0]);
                        thickness.Top = Convert.ToInt32(array[1]);
                        thickness.Right = Convert.ToInt32(array[2]);
                        thickness.Bottom = Convert.ToInt32(array[3]);
                        Model.Margin = thickness;
                        break;
                    case "Stretch":
                        Model.Stretch=(Stretch)e.GetValue<object>();
                        break;
                }

                return;
            }
        }
    }

}

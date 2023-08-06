using Microsoft.Office.Interop.Excel;
using MyPlanogramDesktopApp.Customs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPlanogramDesktopApp.Model
{
    public class DesignerJSON
    {
        [JsonProperty("PlanogramID")]
        public Int64 PlanogramID { get; private set; } = 0;
        [JsonProperty("OnHook")]
        public bool OnHook { get; private set; }=false;
        [JsonProperty("Left")]
        public double Left { get; private set; }
        [JsonProperty("Top")]
        public double Top { get; private set; }
        [JsonProperty("Width")]
        public double Width { get; private set; }
        [JsonProperty("Height")]
        public double Height { get; private set; }
        [JsonProperty("ZIndex")]
        public int ZIndex { get; private set; }
        [JsonProperty("Margin")]
        public Thickness Margin { get; private set; } = new();
        [JsonProperty("Strech")]
        public Stretch Stretch { get; private set; } = Stretch.UniformToFill;
        [JsonProperty("Face")]
        public int Face { get; private set; }
        [JsonProperty("IsCanvas")]
        public bool IsCanvas { get; private set; }
        [JsonProperty("IsShelf")]
        public bool IsShelf { get; private set; }
        [JsonProperty("Notch")]
        public int? Notch { get; private set; }
        [JsonProperty("HorizontalGridName")]
        public string? HorizontalGridName { get; private set; }
        [JsonProperty("GridNum")]
        public int? GridNum { get; private set; }
        [JsonProperty("GridNum2")]
        public int? GridNum2 { get; private set; }

        public DesignerJSON()
        {
        }

        public DesignerJSON(HorizontalGrid grid) 
        {
            HorizontalGridName = grid.GridName;
            GridNum2 = grid.GridNum;
            Vector Offset = VisualTreeHelper.GetOffset(grid);
            Left = Offset.X;
            Top = Offset.Y;
        }

        public DesignerJSON(SettingModel model) {
            if (model.Planogram!=null)
            {
                PlanogramID = model.Planogram.PlanoID;
                OnHook = model.Planogram.Shelf.OnHook;
                Face = model.BorderedPicture.Face;
            }

            Left = model.Left;
            Top = model.Top;
            Width = model.Width;
            IsCanvas = model.IsCanvas;
            IsShelf = model.IsShelf;
            Height = model.Height;
            Notch = model.Notch;
            Stretch = model.Stretch;
            ZIndex = model.ZIndex;
            Margin = model.Margin;

            if (model.HorizontalGrid!=null)
            {
                GridNum = model.HorizontalGrid.GridNum;
            }             
        }

        public override string? ToString() => $"{PlanogramID} - {Width}";
        
        public override bool Equals(object? obj)=>
             (Notch==0) ? obj is DesignerJSON product && PlanogramID == product.PlanogramID && Face == product.Face
             : obj is DesignerJSON Fixture && Notch == Fixture.Notch;            
        
        public override int GetHashCode()=>
        (Notch == 0) ? HashCode.Combine(PlanogramID, Face)
        : HashCode.Combine(Notch);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Testing.Customs;

namespace MyPlanogramDesktopApp.Utilities
{
    public class ImageCreator
    {

        RenderTargetBitmap bitmap;
        DrawingVisual drawingvisual;
        int Width=0;
        int Height=0;
        Size size;
        IEnumerable<ListBoxItem> ListaOfBoxItems;
        PngBitmapEncoder encoder;
        FileStream file;

        public ImageCreator(IEnumerable<ListBoxItem> lista, string output)
        {
            ListaOfBoxItems = lista;
            file = File.Create(output);
        }

        public void Create()
        {
            GetSizes();
            CreateBitmap();
            DrawVisual();
            RenderBitmap();
            SaveAsPng();
    
        }


        public void SaveAsPng()
        {
            encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(file);
            file.Close();
        }

        public void GetSizes()
        {
            foreach (var item in ListaOfBoxItems)
            {
                if (item !=null)
                {
                    size = new(item.ActualWidth, item.ActualHeight);
                    if (size.IsEmpty)
                    {
                        size = new();
                    }

                    Width = 819;
                    //if (Width<(int)size.Width)
                    //{
                    //    Width = (int)size.Width;
                    //}

                    Height = Height + (int)size.Height;

                }
            }
        }

        public void CreateBitmap()=>
        bitmap = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);

        public void RenderBitmap()=>bitmap.Render(drawingvisual);

        public void DrawVisual()
        {
            Point point = new();
            Size size = new();
            drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                foreach(var item in ListaOfBoxItems)
                {
                    if (item !=null)
                    {
                        size.Width = item.ActualWidth;
                        size.Height = item.ActualHeight;
                        context.DrawRectangle(new VisualBrush(item), null, new Rect(point, size));
                        point.Y = point.Y + item.ActualHeight;
                    }
                }
                context.Close();
            }

        }
    }
}

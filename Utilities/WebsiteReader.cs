using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.Model.Abstracts;

namespace MyPlanogramDesktopApp.Utilities
{
    public class WebsiteReader : AbstractNotifier
    {
        string _response = string.Empty;
        string _url = string.Empty;

        public string SKU=string.Empty;
        public string URL { get => _url; set => Set<string>(ref value, ref _url,nameof(URL));}
        public string Response { get => _response; set => Set<string>(ref value, ref _response, nameof(Response)); }
        private HttpClient client = new HttpClient();
        public bool Done =false;
        public bool ReadPhoto = true;

        public WebsiteReader(string url) 
        {
            SKU = url;
            URL=$"https://www.poundland.co.uk/catalogsearch/result/?q={SKU}";
        }

        public async Task Read()
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(URL);
                response.EnsureSuccessStatusCode();
                Response = await response.Content.ReadAsStringAsync();

                if (!Response.Contains($">{SKU}</div>"))
                {
                    Response = Item.DefaultPictureURL;
                    return;
                }

                var photo = @"main product photo";
                var price = "c-product-detail__price\">";

                if (ReadPhoto)
                {
                    var x = Response.Split(photo);
                    Response = x[1];
                    x = Response.Split("src=");
                    Response = x[1];
                    x = Response.Split("/>");
                    Response = x[0];
                    Response = Response.Remove(0, 1);
                    Response = Response.Remove(Response.LastIndexOf('"'), 1);
                } else
                {
                    var x = Response.Split(price);
                    Response = x[1];
                    x = Response.Split("</p>");
                    Response = x[0];
                    Response = Response.Remove(0, 1);
                }
                Done = true;

            }
            catch (Exception e)
            {
                Done = false;
                if (ReadPhoto)
                {
                    Response = "https://www.poundland.co.uk/static/version1676970257/frontend/Poundland/default/en_GB/Magento_Catalog/images/product/placeholder/image.jpg";
                }
                MessageBox.Show(e.Message, "Exception Caught!");
            }
        }
    }
}


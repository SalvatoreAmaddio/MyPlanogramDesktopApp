using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;
using Testing.Utilities;

namespace MyPlanogramDesktopApp.Model
{
    public class Item : AbstractModel, IDB<Item> {

        #region backprop
        Int64 _itemid;
        string _itemname = string.Empty;
        string _sku = string.Empty;
        double _price=1;
        bool _isbs=false;
        string _pictureurl = DefaultPictureURL;
        Offer _offer = Offer.DefaultOffer();
        BarcodeFilter? _barcodefilter;
        bool _asra;
        bool _stop;
        bool _scheduleforchange;
        Department? _department;
        #endregion

        #region Prop
        public Int64 ItemID { get => _itemid; set => Set<Int64>(ref value, ref _itemid, nameof(ItemID)); }
        public string ItemName { get => _itemname; set => Set<string>(ref value, ref _itemname, nameof(ItemName)); }
        public string SKU { get => _sku; set => Set<string>(ref value, ref _sku, nameof(SKU)); }
        public double Price { get => _price; set => Set<double>(ref value, ref _price, nameof(Price)); }
        public bool IsBs { get => _isbs; set => Set<bool>(ref value, ref _isbs, nameof(IsBs)); }
        public string PictureURL { get => _pictureurl; set => Set<string>(ref value, ref _pictureurl, nameof(PictureURL)); }
        public Offer Offer { get => _offer; set => Set<Offer>(ref value, ref _offer, nameof(Offer)); }
        public BarcodeFilter? BarcodeFilter { get=>_barcodefilter; set=>Set<BarcodeFilter>(ref value, ref _barcodefilter,nameof(BarcodeFilter)); }
        public bool ASRA { get => _asra; set => Set<bool>(ref value, ref _asra, nameof(ASRA)); }
        public bool ScheduleForChange { get => _scheduleforchange; set => Set<bool>(ref value, ref _scheduleforchange, nameof(ScheduleForChange)); }
        public bool Stop { get => _stop; set => Set<bool>(ref value, ref _stop, nameof(Stop)); }
        public Department? Department { get => _department; set => Set<Department?>(ref value, ref _department, nameof(Department)); }
        #endregion

        #region Constructors
        public Item(Int64 itemid)
        {
            ItemID = itemid;
            IsDirty = false;
        }

        public Item(string sku, string itemname)
        {
            SKU = sku;
            ItemName=itemname;
            BeforePropChanged += Item_BeforePropChanged;
            BarcodeFilter = new(this);
            IsDirty = false;
        }

        public Item(string sku, string itemname, string department)
        {
            SKU = sku;
            ItemName = itemname;
            Department = MainDB.DBDepartment.RecordSource.FirstOrDefault(s=>s.DepartmentName.ToLower().Contains(department));
            BeforePropChanged += Item_BeforePropChanged;
            BarcodeFilter = new(this);
            IsDirty = false;
        }

        public Item()
        {
            BeforePropChanged += Item_BeforePropChanged;
            BarcodeFilter = new(this);
            IsDirty = false;
        }
        
        public Item(MySqlDataReader reader) : base(reader)
        {
            ItemID = reader.GetInt64(0);
            SKU = reader.GetInt32(1).ToString();
            ItemName = reader.GetString(2);
            Price = reader.GetDouble(3);
            PictureURL = reader.GetString(4);
            Offer = new(reader.GetInt64(5));
            IsBs = reader.GetBoolean(6);
            ASRA = reader.GetBoolean(7);
            ScheduleForChange = reader.GetBoolean(8);
            Stop = reader.GetBoolean(9);
            Department = new(reader.GetInt32(10));            
            BarcodeFilter = new(this);
            IsDirty = false;
            BeforePropChanged += Item_BeforePropChanged;
        }
        #endregion


        private void Item_BeforePropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SKU)))
            {
                e.Value = Sys.RemoveAllEmptySpaces(e.GetValue<string>());
                return;
            }

            if (e.PropIs(nameof(ItemName)))
            {
                e.Value=e.GetValue<string>().ToUpper();
                return;
            }

            if (e.PropIs(nameof(PictureURL)))
            {
                e.Value = e.GetValue<string>().Trim();
                return;
            }
        }
 
        public static string DefaultPictureURL => "https://www.poundland.co.uk/static/version1676970257/frontend/Poundland/default/en_GB/Magento_Catalog/images/product/placeholder/image.jpg";

        #region EqualsToString
        public override bool Equals(object? obj)
        {
            return obj is Item item &&
                   SKU == item.SKU;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SKU);
        }

        public override string? ToString() =>$"{SKU} - {ItemName}";
        #endregion

        #region AbstractModel
        public override bool IsNewRecord =>ItemID == 0;

        public override void SetForeignKeys()
        {
            Offer = MainDB.DBOffer.RecordSource.FirstOrDefault(s => s.Equals(Offer));
            this.BarcodeFilter.FilterSource();
            Department = MainDB.DBDepartment.RecordSource.FirstOrDefault(s=>s.Equals(Department));
            if (Department == null) Department = new();
            IsDirty= false;  
        }
        #endregion

        #region IDB
        public string SQLQuery(QueryType Query)
        {
            switch (Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Item;";
                case QueryType.DELETE:
                    return "DELETE FROM Item WHERE ItemID=@ItemID;";
                case QueryType.UPDATE:
                    return "UPDATE Item SET ItemName=@ItemName,Price=@Price,SKU=@SKU,PictureURL=@PictureURL,IsBs=@IsBs,OfferID=@OfferID,ASRA=@ASRA,ForChange=@ForChange,DoNoSell=@DoNoSell,DepartmentID=@DepartmentID WHERE ItemID=@ItemID;";
                case QueryType.INSERT:
                    return "INSERT INTO Item (ItemName,Price,SKU,PictureURL,IsBs,OfferID,ASRA,ForChange,DoNoSell,DepartmentID) VALUES (@ItemName,@Price,@SKU,@PictureURL,@IsBs,@OfferID,@ASRA,@ForChange,@DoNoSell,@DepartmentID);";
                default: return string.Empty;
            }
        }

        public Item GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(ItemID), ItemID);
            param.AddWithValue(nameof(ItemName), ItemName);
            param.AddWithValue(nameof(SKU), SKU);
            param.AddWithValue(nameof(Price), Price);
            param.AddWithValue("OfferID",Offer?.OfferID);
            param.AddWithValue("DepartmentID", Department?.DepartmentID);
            param.AddWithValue(nameof(PictureURL), PictureURL);
            param.AddWithValue(nameof(IsBs), IsBs);
            param.AddWithValue(nameof(ASRA), ASRA);
            param.AddWithValue("ForChange", ScheduleForChange);
            param.AddWithValue("DoNoSell", Stop);
        }

        public void SetPrimaryKey(ulong ID) => ItemID = (long)ID;
        #endregion


    }

}

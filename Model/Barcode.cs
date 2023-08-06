using Aspose.BarCode.Generation;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Model
{
    public class Barcode : AbstractModel, IDB<Barcode> {

        #region backupprop
        Int64 _barcodeid;
        Item? _product;
        string _code=string.Empty;
        BaseEncodeType? _barcodeType;
        BarCodeImageFormat _imageType;
        string _barcodeSource;
        BitmapImage? _bitimg;
        #endregion

        #region prop
        public Int64 BarcodeID { get => _barcodeid; set => Set<Int64>(ref value, ref _barcodeid,nameof(BarcodeID)); }
        public Item? Product { get => _product; set => Set<Item?>(ref value, ref _product,nameof(Product)); }
        public string Code { get => _code; set => Set<string>(ref value, ref _code,nameof(Code)); }
        public BaseEncodeType? BarcodeType { get => EncodeType(); }
        public BarCodeImageFormat ImageType { get => _imageType; set => Set<BarCodeImageFormat>(ref value, ref _imageType, nameof(ImageType)); }
        public BitmapImage? BitImg { get => _bitimg; set => Set<BitmapImage?>(ref value, ref _bitimg, nameof(BitImg)); } 
        #endregion


        #region Constructors
        public Barcode() 
        { 
            Product = new Item();
            BeforePropChanged += Barcode_BeforePropChanged;
            IsDirty = false;
        }

        public Barcode(string sku, string itemname, string barcode)
        {
            Product = new Item(sku,itemname);
            BeforePropChanged += Barcode_BeforePropChanged;
            Code = barcode;
            IsDirty = false;
        }

        public Barcode(MySqlDataReader reader)
        {
            BarcodeID = reader.GetInt64(0);
            Code = reader.GetString(1);
            Product = new(reader.GetInt64(2));
            IsDirty = false;
        }

        private string GenerateBarcode()
        {
            // Image path
            string imagePath = Code + "." + ImageType;

            // Initilize barcode generator
            BarcodeGenerator generator = new BarcodeGenerator(BarcodeType, Code);

            // Save the image
            generator.Save(imagePath,ImageType);

            return imagePath;
        }
        #endregion

        public bool ProperLength() => Code.Length <= 13;
        private bool IsEven(int num) => num % 2 == 0;
        private bool IsOdd(int num) => num % 2 != 0;

        public bool IsValid()
        {
            char[] digits = Code.ToCharArray();

            int EvenDigits = 0;
            int OddDigits = 0;
            int number;
            int position = 1;

            foreach (char d in digits)
            {
                number = Int32.Parse(d.ToString());
                if (IsOdd(position))
                {
                    OddDigits = OddDigits + number;
                }
                else
                {
                    EvenDigits = EvenDigits + number;
                }

                position++;
            }

            return Int32.Parse((OddDigits + (EvenDigits * 3)).ToString().LastOrDefault().ToString()) == 0;
        }

        private void Barcode_BeforePropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Code)))
            {
                string barcode = e.GetValue<string>();

                if (barcode.StartsWith("*"))
                {
                    barcode = barcode.Remove(0, 1);
                }

                if (barcode.EndsWith("*"))
                {
                    barcode = barcode.Remove(barcode.LastIndexOf("*"), 1);
                }
                e.Value = barcode;
            }
        }

        #region EqualsToString
        public override bool Equals(object? obj)=>obj is Barcode barcode && Code == barcode.Code;

        public override int GetHashCode()=>HashCode.Combine(Code);

        public override string? ToString() => Code;
        #endregion

        public override bool IsNewRecord => BarcodeID == 0;

        public bool ItemExists() 
        {
            Product=MainDB.DBItem.RecordSource.FirstOrDefault(s => s.Equals(Product))!;
            return Product != null;
        }

        #region Query
        public string SQLQuery(QueryType Query)
        {
            switch(Query)
            {
                case QueryType.SELECT:
                        return "SELECT * FROM Barcode;";
                case QueryType.UPDATE:
                    return "UPDATE Barcode SET ItemID=@ItemID, Code=@Code WHERE BarcodeID=@BarcodeID;";
                case QueryType.INSERT:
                    return "INSERT INTO Barcode (ItemID, Code) VALUES(@ItemID,@Code);";
                case QueryType.DELETE:
                    return "DELETE FROM Barcode WHERE BarcodeID=@BarcodeID";
                default: return string.Empty;
            }
        }

        public Barcode GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(BarcodeID),BarcodeID);
            param.AddWithValue("ItemID", Product.ItemID);
            param.AddWithValue(nameof(Code), Code);
        }

        public void SetPrimaryKey(ulong ID)=>BarcodeID = (long)ID;

        public void Generate()
        {
            try
            {
                ImageType = (BarCodeImageFormat)Enum.Parse(typeof(BarCodeImageFormat), "Png");
                string path = Path.GetFullPath(GenerateBarcode());
                var x = new Uri(path);
                BitImg = new BitmapImage(x);
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message,"FAILED");
            }

        }

        private SymbologyEncodeType EncodeType()
        {
            switch(Code.Length)
            {
                case 13:
                    return EncodeTypes.EAN13;
                case 14:
                    return EncodeTypes.EAN14;
                case 8:
                    return EncodeTypes.EAN8;
                case 2:
                    return EncodeTypes.Code128;
                case 11:
                    return EncodeTypes.Code11;
                default:
                    return EncodeTypes.Codabar;
            }
        }

        public override void SetForeignKeys()
        {
            Product = MainDB.DBItem.RecordSource.FirstOrDefault(s=>s.ItemID==Product.ItemID);
            IsDirty = false;
        }
        #endregion
    }

    #region BarcodeFilter 
    public class BarcodeFilter : AbstractFilterAndSort
    {
        public Item? Item { get; }
        private bool Constructor1 = false;
        public override IRecordSource RecordSource => MainDB.DBBarcode.RecordSource;

        public BarcodeFilter(Item? item) 
        {
            Item = item;
            Constructor1=true;
        }

        public BarcodeFilter(string search) => Search = search;

        public override bool Criteria(object? record)
        {
            if (Constructor1)
            {
               return ((Barcode?)record)?.Product?.ItemID == Item?.ItemID;
            }

            Barcode? barcode = (Barcode?)record;
            return barcode.Code.ToLower().StartsWith(Search)
            || barcode.Product.ToString().ToLower().Contains(Search)
             ;
        }

        public override void SortSource(IEnumerable ItemsSource)
        {
            if (Constructor1) return;
            Source = MainDB.DBBarcode.RecordSource.NewSortedSource(ItemsSource.Cast<Barcode>().OrderBy(s => s.Product.ItemName));
            SortRan = true;
        }
    }
    #endregion

}

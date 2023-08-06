using Microsoft.Office.Interop.Excel;
using MyPlanogramDesktopApp.Controller;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Testing.Customs;
using Testing.DB;
using Testing.Model.Abstracts;
using Testing.RecordSource;

namespace MyPlanogramDesktopApp.Model
{
    public class Planogram : AbstractModel, IDB<Planogram>
    {
       
        public bool SetSectionTriggered = false;

        #region backprops
        Int64 _planoid;
        Item? _product;
        Shelf _shelf=null!;
        int _faces;
        bool _substitute;
        int _orderlist;
        Section _section=null!;
        Bay _bay=null!;

        BayFilter _bayFilter=null!;
        ShelvesFilter _shelfFilter = null!;
        #endregion

        #region props

        public Int64 PlanoID { get => _planoid; set => Set<Int64>(ref value, ref _planoid,nameof(PlanoID)); }
        public Item? Product { get => _product; set => Set<Item?>(ref value, ref _product, nameof(Product)); }
        public Shelf Shelf { get => _shelf; set => Set<Shelf>(ref value, ref _shelf, nameof(Shelf)); }

        public Section Section { get => _section; set => Set<Section>(ref value, ref _section, nameof(Section)); }
        public Bay Bay { get => _bay; set => Set<Bay>(ref value, ref _bay, nameof(Bay)); }

        public int Faces { get => _faces; set => Set<int>(ref value, ref _faces,nameof(Faces)); }
        public int OrderList { get => _orderlist; set => Set<int>(ref value, ref _orderlist,nameof(OrderList)); }
        public bool Substitute { get => _substitute; set => Set<bool>(ref value, ref _substitute,nameof(Substitute)); }
        public BayFilter BayFilter { get => _bayFilter; set => Set<BayFilter>(ref value, ref _bayFilter,nameof(BayFilter)); }
        public ShelvesFilter ShelfFilter { get => _shelfFilter; set => Set<ShelvesFilter>(ref value, ref _shelfFilter, nameof(ShelfFilter)); }
        #endregion

        public Planogram() 
        {
            Faces = 1;
            Product = new(0);
            Section = new();
            Bay = new();
            IsDirty= false;
            AfterPropChanged += Planogram_AfterPropChanged;
        }

        public Planogram(MySqlDataReader reader)
        {
            PlanoID = reader.GetInt64(0);
            Product = new(reader.GetInt64(1));
            Shelf = new(reader.GetInt64(2));
            Faces = reader.GetInt32(3);
            Substitute = reader.GetBoolean(4);
            OrderList = reader.GetInt32(6);
            AfterPropChanged += Planogram_AfterPropChanged;
            IsDirty = false;
        }

        private void Planogram_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(Section)))
            {                
                BayFilter = (SetSectionTriggered) ? new(e.GetValue<Section>(),false) : new(e.GetValue<Section>(),true);
                return;
            }

            if (e.PropIs(nameof(Bay)))
            {
                ShelfFilter = (SetSectionTriggered) ? new(e.GetValue<Bay>(),false,false) : new(e.GetValue<Bay>(), true, false);
                return;
            }

        }

        #region AbstractModel
        public override bool IsNewRecord => PlanoID == 0;

        public override void SetForeignKeys()
        {
            SetSectionTriggered = true;
            Product = MainDB.DBItem.RecordSource.FirstOrDefault(s => s.ItemID==Product.ItemID);
            Shelf = MainDB.DBShelf.RecordSource.FirstOrDefault(s => s.Equals(Shelf));
            Bay = MainDB.DBBay.RecordSource.FirstOrDefault(s => s.Equals(Shelf.Bay));
            Section = MainDB.DBSection.RecordSource.FirstOrDefault(s => s.Equals(Bay.Section));
            SetSectionTriggered = false;
            IsDirty = false;
        }
        #endregion

        #region IDB
        public string SQLQuery(QueryType Query)
        {
            switch (Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Planogram;";
                case QueryType.UPDATE:
                    return "UPDATE Planogram SET ItemID=@ItemID, ShelfID=@ShelfID,Faces=@Faces,Substitute=@Substitute,OrderList=@OrderList WHERE PlanoID=@PlanoID;";
                case QueryType.INSERT:
                    return "INSERT INTO Planogram (ItemID, ShelfID,Faces,Substitute,OrderList) VALUES (@ItemID, @ShelfID,@Faces,@Substitute,@OrderList)";
                case QueryType.DELETE:
                    return "DELETE FROM Planogram WHERE PlanoID=@PlanoID;";
                default: return "";
            }
        }

        public Planogram GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(PlanoID), PlanoID);
            param.AddWithValue("ItemID", Product?.ItemID);
            param.AddWithValue("ShelfID", Shelf?.ShelfID);
            param.AddWithValue(nameof(Substitute), Substitute);
            param.AddWithValue(nameof(OrderList), OrderList);
            param.AddWithValue(nameof(Faces), Faces);
        }

        public void SetPrimaryKey(ulong ID) => PlanoID = (long)ID;
        #endregion
        

        #region EqualsHashCodeToString
        public override bool Equals(object? obj)
        {
            return obj is Planogram planogram &&
                   PlanoID == planogram.PlanoID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlanoID);
        }

        public override string? ToString() => $"Planogram {PlanoID}";
        #endregion
    }


}

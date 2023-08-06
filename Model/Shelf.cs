using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.DB;
using Testing.Model.Abstracts;

namespace MyPlanogramDesktopApp.Model
{
    public class Shelf : AbstractModel, IDB<Shelf>
    {
        #region backprop
        Int64 _shelfid;
        int _notch;
        int _shelfnum;
        bool _onhook;
        Bay _bay = null!;
        #endregion

        #region props
        public Int64 ShelfID { get => _shelfid; set => Set<Int64>(ref value, ref _shelfid,nameof(ShelfID)); }
        public int Notch { get => _notch; set => Set<int>(ref value, ref _notch, nameof(Notch)); }
        public int ShelfNum { get => _shelfnum; set => Set<int>(ref value, ref _shelfnum, nameof(ShelfNum)); }
        public bool OnHook { get => _onhook; set => Set<bool>(ref value, ref _onhook, nameof(OnHook)); }
        public Bay Bay { get => _bay; set => Set<Bay>(ref value, ref _bay, nameof(Bay)); }
        #endregion

        public Shelf(Bay bay, int shelfnum)
        {
            Bay = bay;
            ShelfNum = shelfnum;
            Notch = 1;
            IsDirty = (ShelfNum==1) ? true : false;
            AfterPropChanged += Shelf_AfterPropChanged;
        }

        public Shelf() 
        {
            Bay = new Bay();
            ShelfNum = 1;
            AfterPropChanged += Shelf_AfterPropChanged;
            IsDirty = false;
        }
        
        public Shelf(Int64 ShelfID)
        {
            this.ShelfID = ShelfID;
            Bay = new();
            IsDirty = false;
        }

        public Shelf(Shelf Shelf)
        {
            this.ShelfID = Shelf.ShelfID;
            this.ShelfNum = Shelf.ShelfNum;
            Bay = new Bay(Shelf.Bay);
            IsDirty = false;
        }

        public Shelf(MySqlDataReader reader)
        {
            ShelfID = reader.GetInt64(0);
            Notch = reader.GetInt32(1);
            ShelfNum = reader.GetInt32(2);
            OnHook = reader.GetBoolean(3);
            Bay = new() { BayID = reader.GetInt64(4) };
            AfterPropChanged += Shelf_AfterPropChanged;
            IsDirty = false;
        }

        private void Shelf_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(OnHook)))
            {
                Notch = (e.GetValue<bool>()) ? 35 : 1;
                return;
            }
        }

        #region AbstractModel
        public override bool IsNewRecord=> ShelfID == 0;
        public override void SetForeignKeys() {
            Bay = MainDB.DBBay.RecordSource.FirstOrDefault(s => s.Equals(Bay));
            IsDirty = false;
        } 
        #endregion


        #region ToStringEqualsHashCode
        public override bool Equals(object? obj)
        {
            return obj is Shelf shelf &&
                   ShelfID == shelf.ShelfID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ShelfID);
        }

        public override string? ToString() =>$"{ShelfNum}";
        #endregion

        #region IDB
        public string SQLQuery(QueryType Query)
        {
            switch(Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Shelf;";
                case QueryType.DELETE:
                    return "DELETE FROM Shelf WHERE ShelfID=@ShelfID;";
                case QueryType.UPDATE:
                    return "UPDATE Shelf SET Notch=@Notch, ShelfNum=@ShelfNum, OnHook=@OnHook, BayID=@BayID WHERE ShelfID=@ShelfID;";
                case QueryType.INSERT:
                    return "INSERT INTO Shelf (Notch,ShelfNum,OnHook,BayID) VALUES(@Notch,@ShelfNum,@OnHook,@BayID);";
                default:
                    return "";
            }
        }

        public Shelf GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(ShelfID), ShelfID);
            param.AddWithValue(nameof(Notch), Notch);
            param.AddWithValue(nameof(OnHook), OnHook);
            param.AddWithValue(nameof(ShelfNum), ShelfNum);
            param.AddWithValue("BayID", Bay.BayID);
        }

        public void SetPrimaryKey(ulong ID) => ShelfID = (long)ID;
        #endregion

    }
}
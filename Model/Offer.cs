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
    public class Offer : AbstractModel, IDB<Offer>
    {

        #region backprop
        Int64 _offerid;
        string _offername=string.Empty;
        #endregion

        #region Prop
        public Int64 OfferID { get => _offerid; set => Set<Int64>(ref value, ref _offerid,nameof(OfferID)); }
        public string OfferName { get => _offername; set => Set<string>(ref value, ref _offername,nameof(OfferName)); }
        #endregion


        public Offer(Int64 id)
        {
            OfferID = id;
            IsDirty = false;
        }

        public Offer() {
            OfferID = 0;
            OfferName = string.Empty;
            BeforePropChanged += Before_AfterPropChanged;
            IsDirty = false;
        }

        public static Offer DefaultOffer() => new() { OfferID=1,IsDirty=false};

        public Offer(MySqlDataReader reader) {
            OfferID = reader.GetInt64(0);
            OfferName= reader.GetString(1);
            BeforePropChanged += Before_AfterPropChanged;
            IsDirty = false;
        }

        private void Before_AfterPropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(OfferName)))
            {
                e.Value=e.GetValue<string>().ToUpper();
                return;
            }
        }

        #region EqualsToString
        public override bool Equals(object? obj) => obj is Offer offer && OfferID == offer.OfferID;
        public override int GetHashCode() => HashCode.Combine(OfferID);
        public override string? ToString() => OfferName;
        #endregion

        public override bool IsNewRecord=>OfferID == 0;

        #region IDB
        public string SQLQuery(QueryType Query)
        {
            switch (Query) 
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Offer;";
                case QueryType.DELETE:
                    return "DELETE FROM Offer WHERE OfferID=@OfferID;";
                case QueryType.UPDATE:
                    return "UPDATE Offer SET OfferTitle=@OfferName WHERE OfferID=@OfferID;";
                case QueryType.INSERT:
                    return "INSERT INTO Offer (OfferTitle) VALUES(@OfferName);";
                default: return "";
            }
        }

        public Offer GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(OfferID),OfferID);
            param.AddWithValue(nameof(OfferName), OfferName);
        }

        public void SetPrimaryKey(ulong ID) => OfferID = (long)ID;

        public override void SetForeignKeys()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

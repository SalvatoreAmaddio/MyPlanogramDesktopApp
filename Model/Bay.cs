using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Testing.DB;
using Testing.Model.Abstracts;

namespace MyPlanogramDesktopApp.Model
{
    public class Bay : AbstractModel, IDB<Bay>
    {

        #region backprop
        Int64 _bayid;
        Section _section = null!;
        int _baynum;
        string _baytitle=string.Empty;
        string _pictureurl = string.Empty;
        #endregion

        #region prop
        public Int64 BayID { get=>_bayid; set =>Set<Int64>(ref value, ref _bayid,nameof(BayID)); }
        public Section Section { get => _section; set => Set<Section>(ref value, ref _section, nameof(Section)); }
        public int BayNum { get => _baynum; set => Set<int>(ref value, ref _baynum, nameof(BayNum)); }
        public string BayTitle { get => _baytitle; set => Set<string>(ref value, ref _baytitle,nameof(BayTitle)); }
        public string PictureURL { get => _pictureurl; set => Set<string>(ref value, ref _pictureurl, nameof(PictureURL)); }
        #endregion

        public Bay(int baynum, Section section)
        {
            BayNum = baynum;
            Section = section;
            BeforePropChanged += Bay_BeforePropChanged;
            IsDirty = false;
        }

        public Bay()
        {
            Section = new();
            BeforePropChanged += Bay_BeforePropChanged;
        }

        public Bay(Bay Bay)
        {
            this.BayID= Bay.BayID;
            this.BayNum = Bay.BayNum;
            this.Section = new(Bay.Section);
        }

        public Bay(MySqlDataReader reader)
        {
            BayID = reader.GetInt64(0);
            Section = new();
            Section.SectionID= reader.GetInt64(1);
            BayNum = reader.GetInt32(2);
            BayTitle = reader.GetString(3);
            PictureURL=reader.GetString(4);
            BeforePropChanged += Bay_BeforePropChanged;
            IsDirty = false;

        }

        private void Bay_BeforePropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(BayTitle))) 
            {
                e.Value = e.GetValue<string>().ToUpper();
            }
        }

        #region AbstractModel
        public override bool IsNewRecord => BayID == 0;

        public override void SetForeignKeys() {
            Section = MainDB.DBSection.RecordSource.FirstOrDefault(s => s.Equals(Section));
            IsDirty = false;
        } 
        #endregion

        #region IDB
        public Bay GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(BayID), BayID);
            param.AddWithValue(nameof(BayNum), BayNum);
            param.AddWithValue("SectionID", Section.SectionID);
            param.AddWithValue(nameof(BayTitle), BayTitle);
            param.AddWithValue(nameof(PictureURL), PictureURL);
        }

        public void SetPrimaryKey(ulong ID) => BayID = (long)ID;

        public string SQLQuery(QueryType Query)
        {
            switch(Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Bay;";
                case QueryType.DELETE:
                    return "DELETE FROM Bay WHERE BayID=@BayID;";
                case QueryType.INSERT:
                    return "INSERT INTO Bay (SectionID,BayNum,BayTitle,PictureURL) VALUES(@SectionID,@BayNum,@BayTitle,@PictureURL);";
                case QueryType.UPDATE:
                    return "UPDATE Bay SET SectionID=@SectionID,BayNum=@BayNum,BayTitle=@BayTitle,PictureURL=@PictureURL WHERE BayID=@BayID;";
                default: return "";
            }
        }
        #endregion

        #region EqualsToStringHashCode
        public override bool Equals(object? obj)
        {
            return obj is Bay bay &&
                   BayID == bay.BayID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BayID);
        }

        public override string? ToString() => $"{BayNum}";
        #endregion
    }
}

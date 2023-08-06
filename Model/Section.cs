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
    public class Section : AbstractModel, IDB<Section>
    {

        #region backprop
        Int64 _sectionid;
        string _sectionname = string.Empty;
        DateTime _planodate;
        #endregion

        #region prop
        public Int64 SectionID { get=>_sectionid; set => Set<Int64>(ref value, ref _sectionid,nameof(SectionID)); }
        public string SectionName { get => _sectionname; set => Set<string>(ref value, ref _sectionname, nameof(SectionName)); }
        public DateTime PlanoDate { get => _planodate; set => Set<DateTime>(ref value, ref _planodate, nameof(PlanoDate)); }
        #endregion 


        public Section() 
        {
            PlanoDate = DateTime.Today;
            IsDirty = false;
            BeforePropChanged += Section_BeforePropChanged;
        }

        private void Section_BeforePropChanged(object? sender, PropChangedEvtArgs e)
        {
            if (e.PropIs(nameof(SectionName)))
            {
                e.Value = e.GetValue<string>().ToUpper();
            }
        }

        public Section(Section Section) {
            this.SectionID=Section.SectionID;
            this.SectionName = Section.SectionName;
        }

        public Section(MySqlDataReader reader)
        {
            SectionID = reader.GetInt64(0);
            SectionName = reader.GetString(1);
            PlanoDate=reader.GetDateTime(2);
            BeforePropChanged += Section_BeforePropChanged;
            IsDirty = false;
        }


        #region AbstractModel
        public override bool IsNewRecord=>SectionID == 0;

        public override void SetForeignKeys()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region EqualsHashCodeToString
        public override bool Equals(object? obj)
        {
            return obj is Section section &&
                   SectionID == section.SectionID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SectionID);
        }

        public override string? ToString() => $"{SectionName}";
        #endregion

        #region IDB
        public string SQLQuery(QueryType Query)
        {
            switch(Query)
            {
                case QueryType.SELECT:
                    return "SELECT * FROM Section;";
                case QueryType.DELETE:
                    return "DELETE FROM Section WHERE SectionID=@SectionID;";
                case QueryType.UPDATE:
                    return "UPDATE Section SET SectionName=@SectionName, PlanoDate=@PlanoDate WHERE SectionID=@SectionID;";
                case QueryType.INSERT:
                    return "INSERT INTO Section (SectionName,PlanoDate) VALUES (@SectionName,@PlanoDate);";
                default:
                    return string.Empty;
            }
        }

        public Section GetRecord(MySqlDataReader reader) => new(reader);

        public void Params(MySqlParameterCollection param)
        {
            param.AddWithValue(nameof(SectionID),SectionID);
            param.AddWithValue(nameof(SectionName), SectionName);
            param.AddWithValue(nameof(PlanoDate), PlanoDate);
        }

        public void SetPrimaryKey(ulong ID)=>SectionID = (long)ID;
        #endregion

    }
}

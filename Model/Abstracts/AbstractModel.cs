using MyPlanogramDesktopApp.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Model.Abstracts
{
    public abstract class AbstractModel : AbstractNotifier
    {

        bool _isdirty;
        public bool IsDirty { get => _isdirty; set => Set(ref value, ref _isdirty, nameof(IsDirty)); }

        public AbstractModel() { }

        public AbstractModel(MySqlDataReader reader)
        {

        }

        public override void Set<M>(ref M value, ref M backprop, string propName)
        {
            base.Set(ref value, ref backprop, propName);
            if (propName.Equals(nameof(IsDirty))) return;
            IsDirty = true;
        }

        public abstract void SetForeignKeys();

        public abstract bool IsNewRecord { get; }
        public void NotifyNewRecord() => Notify(nameof(IsNewRecord));
    }
}

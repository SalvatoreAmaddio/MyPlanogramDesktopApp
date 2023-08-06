using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing.Model.Abstracts
{
    abstract public class AbstractNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<PropChangedEvtArgs>? AfterPropChanged;
        public event EventHandler<PropChangedEvtArgs>? BeforePropChanged;

        public AbstractNotifier() { }

        public virtual void Set<T>(ref T value, ref T backprop, string propName)
        {
            PropChangedEvtArgs PropChangedEvtArgs = new PropChangedEvtArgs(propName, value, backprop);
            BeforePropChanged?.Invoke(this, PropChangedEvtArgs);
            if (PropChangedEvtArgs.Cancel) return;
            value = (T)PropChangedEvtArgs.Value;
            backprop = value;
            Notify(propName);
            AfterPropChanged?.Invoke(this, PropChangedEvtArgs);
        }

        public void Notify(string propName) => PropertyChanged?.Invoke(this, new(propName));
    }

    #region PropChangedEvtArgs
    public class PropChangedEvtArgs : EventArgs
    {
        public string PropName;
        public object Value;
        public object OldValue = new();
        public bool Cancel { get; set; } = false;
        public bool IsNull { get => Value == null; }

        public PropChangedEvtArgs(string propName, object value)
        {
            PropName = propName;
            Value = value;
        }

        public PropChangedEvtArgs(string propName, object value, object oldvalue) : this(propName, value)
        {
            OldValue = oldvalue;
        }


        public T GetOldValue<T>() => (T)OldValue;
        public T GetValue<T>() => (T)Value;
        public bool PropIs(string propname) => PropName.Equals(propname);
    }
    #endregion
}

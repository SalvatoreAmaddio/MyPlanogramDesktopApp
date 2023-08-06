using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Testing.Customs
{
    public interface IRecordTracker
    {
        public ICommand GoNewCMD { get; set; }
        public ICommand GoFirstCMD { get; set; }
        public ICommand GoNextCMD { get; set; }
        public ICommand GoLastCMD { get; set; }
        public ICommand GoPreviousCMD { get; set; }
        public string Records { get;} 

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for RecordTracker.xaml
    /// </summary>
    public partial class RecordTracker : StackPanel
    {

        public RecordTracker()
        {
            InitializeComponent();
        }

        private void SetButtonsCMD() {
            GoNextButton.Command = IRecordTracker.GoNextCMD;
            GoLastButton.Command = IRecordTracker.GoLastCMD;
            GoPreviousButton.Command = IRecordTracker.GoPreviousCMD;
            GoFirstButton.Command = IRecordTracker.GoFirstCMD;
            GoNewButton.Command = IRecordTracker.GoNewCMD;
            Records.Content= IRecordTracker.Records;
        }

        #region IRecordTracker
        public static readonly DependencyProperty IRecordTrackerProperty =
        DependencyProperty.Register(
        nameof(IRecordTracker), typeof(IRecordTracker), typeof(RecordTracker),

        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = IRecordTrackerPropertyChanged
        }
        );

        public static void IRecordTrackerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RecordTracker)d;
            control.IRecordTracker = (IRecordTracker)e.NewValue;
            control.SetButtonsCMD();
        }

        public IRecordTracker IRecordTracker
        {
            get => (IRecordTracker)GetValue(IRecordTrackerProperty);
            set => SetValue(IRecordTrackerProperty, value);
        }

        #endregion

    }
}

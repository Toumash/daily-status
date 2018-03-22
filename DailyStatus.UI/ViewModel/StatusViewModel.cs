using DailyStatus.Common;
using DailyStatus.Common.Configuration;
using DailyStatus.UI.WpfExtensions;
using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DailyStatus.UI.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        TogglProxy _togglClient;
        DailyStatusConfiguration _config;
        DispatcherTimer LogTimer;
        private TimeSpan _diff;
        private Brush _needle = Brushes.Gray;

        public TimeSpan Diff
        {
            get { return _diff; }
            set
            {
                _diff = value;
                NotifyPropertyChanged(nameof(Diff));
                NotifyPropertyChanged(nameof(TimeDiff));
                NotifyPropertyChanged(nameof(TbTimeDiff));
            }
        }
        public Brush Needle
        {
            get { return _needle; }
            set
            {
                _needle = value;
                NotifyPropertyChanged(nameof(Needle));
            }
        }

        public double TimeDiff { get => _diff.TotalHours; }
        public string TbTimeDiff { get => $"{_diff.TotalHours:0.#} h"; }

        public ICommand CloseButtonCommand { get; } = new RelayCommand((s) => Environment.Exit(0));

        public StatusViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Needle = Brushes.Gray;
                Diff = TimeSpan.FromHours(2.5d);
            }
        }
        public StatusViewModel(TogglProxy togglClient, DailyStatusConfiguration configuration)
        {
            _togglClient = togglClient;
            _config = configuration;

            Needle = Brushes.Transparent;
            Diff = TimeSpan.FromHours(0);

            LogTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2 * 1000)
            };
            LogTimer.Tick += (s, e) =>
            {
                Needle = Brushes.Gray;
                Diff = _togglClient.GetDifference(_config.GetWorkDayConfig());
            };
            LogTimer.Start();
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

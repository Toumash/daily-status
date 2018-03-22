using DailyStatus.Common;
using DailyStatus.Common.Configuration;
using DailyStatus.UI.WpfExtensions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DailyStatus.UI.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private const int REFRESH_INTERNAL_SECONDS = 2;

        TogglProxy _togglClient;
        DailyStatusConfiguration _config;
        DispatcherTimer _timer;

        private TimeSpan _diff;
        private Brush _gaugeNeedle = Brushes.Gray;

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
            get { return _gaugeNeedle; }
            set
            {
                _gaugeNeedle = value;
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

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(REFRESH_INTERNAL_SECONDS)
            };
            _timer.Tick += async (s, e) =>
            {
                Needle = Brushes.Gray;
                Diff = await _togglClient.GetDifference(_config.GetWorkDayConfig());
            };
            _timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

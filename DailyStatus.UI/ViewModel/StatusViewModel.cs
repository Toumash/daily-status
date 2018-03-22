using DailyStatus.Common;
using DailyStatus.Common.Configuration;
using DailyStatus.UI.WpfExtensions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DailyStatus.Common.Extensions;

namespace DailyStatus.UI.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private const int REFRESH_INTERNAL_SECONDS = 1;

        TogglProxy _togglClient;
        DailyStatusConfiguration _config;
        DispatcherTimer _timer;

        private TimeSpan _diff;
        private Brush _gaugeNeedle = Brushes.Gray;
        private string _tbExpected = "";
        private string _tbActual = "";

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

        public string TbExpected
        {
            get => _tbExpected;
            set
            {
                _tbExpected = value;
                NotifyPropertyChanged(nameof(TbExpected));
            }
        }

        public string TbActual
        {
            get => _tbActual;
            set
            {
                _tbActual = value;
                NotifyPropertyChanged(nameof(TbActual));
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
                TbExpected = TimeSpan.FromHours(2.5)
                    .ToWorkingTimeString(8);
                TbActual = TimeSpan.FromHours(2.5)
                    .ToWorkingTimeString(8);
            }
        }

        public StatusViewModel(TogglProxy togglClient, DailyStatusConfiguration configuration)
        {
            _togglClient = togglClient;
            _config = configuration;

            Init();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(REFRESH_INTERNAL_SECONDS)
            };
            _timer.Tick += async (s, e) => await RefreshData();
            _timer.Start();
        }

        private void Init()
        {
            Needle = Brushes.Transparent;
            Diff = TimeSpan.FromHours(0);
            TbExpected = TimeSpan.FromHours(0)
                   .ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
            TbActual = TimeSpan.FromHours(0)
                .ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
        }

        private async Task RefreshData()
        {
            var expected = _togglClient.GetExpectedWorkingTime(_config.GetWorkDayConfig());
            var actual = (await _togglClient.GetWorkingTime());

            TbExpected = expected.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
            TbActual = actual.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
            Diff = _togglClient.GetDifference(expected: expected, sum: actual);
            Needle = Brushes.Gray;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

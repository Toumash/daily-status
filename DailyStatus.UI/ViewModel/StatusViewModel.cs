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
        private const int RefreshIntervalInSeconds = 5;
        public const int LabelsDistanceHours = 8;
        public const int MinimalStaticValueForGauge = -8;

        TogglProxy _togglClient;
        DailyStatusConfiguration _config;
        DispatcherTimer _timer;

        private TimeSpan _diff;
        private Brush _gaugeNeedle;
        private string _tbExpected;
        private string _tbActual;
        private DateTime? _lastUpdated;

        public TimeSpan Diff
        {
            get { return _diff; }
            set
            {
                _diff = value;
                NotifyPropertyChanged(nameof(Diff));
                NotifyPropertyChanged(nameof(TimeDiff));
                NotifyPropertyChanged(nameof(TbTimeDiff));
                NotifyPropertyChanged(nameof(GaugeMinimalValue));
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

        public DateTime? LastUpdateTime
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                NotifyPropertyChanged(nameof(LastUpdateTime));
                NotifyPropertyChanged(nameof(LastUpdateTimeString));
            }
        }

        public string LastUpdateTimeString
        {
            get
            {
                if (!_lastUpdated.HasValue)
                {
                    return $"n/a";
                }
                return $"{_lastUpdated.Value.Hour:00}:{_lastUpdated.Value.Minute:00}:{_lastUpdated.Value.Second:00}";
            }
        }

        public double TimeDiff { get => _diff.TotalHours; }
        public string TbTimeDiff { get => $"{_diff.Hours:0}:{Math.Abs(_diff.Minutes):00}"; }

        public double GaugeMinimalValue
        {
            get
            {
                if (TimeDiff < MinimalStaticValueForGauge)
                {
                    int hours = (int)Math.Floor(TimeDiff);
                    while (hours % LabelsDistanceHours != 0) hours--;

                    return hours;
                }
                else
                {
                    return MinimalStaticValueForGauge;
                }
            }
        }

        private bool _offline = false;
        public bool OfflineMode
        {
            get => _offline;
            set
            {
                _offline = value;
                NotifyPropertyChanged(nameof(OfflineMode));
            }
        }
        private bool _currentlyTracking;
        public bool IsTimerActive
        {
            get => _currentlyTracking;
            set
            {
                _currentlyTracking = value;
                NotifyPropertyChanged(nameof(IsTimerActive));
            }
        }

        public ICommand CloseButtonCommand { get; } = new RelayCommand((s) => Environment.Exit(0));

        public StatusViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Needle = Brushes.Gray;
                Diff = TimeSpan.FromHours(2);
                TbExpected = TimeSpan.FromHours(2.5)
                    .ToWorkingTimeString(8);
                TbActual = TimeSpan.FromHours(2.5)
                    .ToWorkingTimeString(8);
                LastUpdateTime = DateTime.Now;
                OfflineMode = false;
                IsTimerActive = true;
            }
        }

        public StatusViewModel(TogglProxy togglClient, DailyStatusConfiguration configuration)
        {
            _togglClient = togglClient;
            _config = configuration;

            Init();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0)
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
            LastUpdateTime = null;
        }

        private async Task RefreshData()
        {
            _timer.Interval = TimeSpan.FromSeconds(RefreshIntervalInSeconds);
            try
            {
                var actual = (await _togglClient.GetStatus());
                IsTimerActive = actual.IsTimerActive;

                var expected = _togglClient.GetExpectedWorkingTime(_config.GetWorkDayConfig());
                TbExpected = expected.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
                TbActual = actual.TimeInMonth.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
                Diff = _togglClient.GetDifference(expected: expected, sum: actual.TimeInMonth);
                Needle = Brushes.Gray;
                LastUpdateTime = DateTime.Now;
                OfflineMode = false;
            }
            catch (OfflineException ex)
            {
                OfflineMode = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

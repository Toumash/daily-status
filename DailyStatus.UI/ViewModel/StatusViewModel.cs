using DailyStatus.Common;
using DailyStatus.Common.Configuration;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DailyStatus.Common.Extensions;
using System.Collections.ObjectModel;
using System.Linq;
using DailyStatus.Common.Model;
using System.Windows.Input;
using DailyStatus.UI.WpfExtensions;

namespace DailyStatus.UI.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        const int RefreshIntervalInSeconds = 5;
        public const int LabelsDistanceHours = 8;
        public const int MinimalStaticValueForGauge = -16;

        bool firstSync = true;
        readonly TogglProxy _togglClient;
        DailyStatusConfiguration _config;
        readonly DispatcherTimer _timer;

        TimeSpan _diff;
        Brush _gaugeNeedle;
        string _tbExpected;
        string _tbActual;
        DateTime? _lastUpdated;

        TimeSpan _todayHours;
        double _todayGaugeMaxValue = 8;
        ObservableCollection<Workspace> _workspaces = new ObservableCollection<Workspace>();

        public ObservableCollection<Workspace> Workspaces
        {
            get { return _workspaces; }
            set { _workspaces = value; NotifyPropertyChanged(nameof(Workspaces)); }
        }

        Workspace _selectedWorkspace = new Workspace();

        public Workspace SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set
            {
                if (SelectedWorkspace == value) return;
                _selectedWorkspace = value;
                NotifyPropertyChanged(nameof(SelectedWorkspace));
                ScheduleInstantRefresh();
            }
        }

        void ScheduleInstantRefresh()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(0);
        }

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
        public string TbTimeDiff
        {
            get
            {
                var sign = '-';
                sign = _diff.TotalHours < 0 ? sign : ' ';
                return $"{sign}{Math.Abs(_diff.TotalHours):0}:{Math.Abs(_diff.Minutes):00}";
            }
        }

        TimeSpan TodayHours
        {
            get => _todayHours;
            set
            {
                _todayHours = value;
                NotifyPropertyChanged(nameof(TodaysCurrentWork));
                NotifyPropertyChanged(nameof(TodaysCurrentWorkText));
            }
        }
        public double TodaysCurrentWork { get => Math.Min(TodayHours.TotalHours, _todayGaugeMaxValue); }
        public string TodaysCurrentWorkText { get => $"{TodayHours.Hours}:{TodayHours.Minutes:00}"; }

        public ICommand CloseCommand
            => new RelayCommand(o => Environment.Exit(0));


        public double TodayGaugeMaxValue
        {
            get => _todayGaugeMaxValue; set
            {
                _todayGaugeMaxValue = value;
                NotifyPropertyChanged(nameof(TodayGaugeMaxValue));
            }
        }

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

        bool _offline;

        public bool OfflineMode
        {
            get => _offline;
            set
            {
                _offline = value;
                NotifyPropertyChanged(nameof(OfflineMode));
            }
        }
        bool _currentlyTracking;

        public bool IsTimerActive
        {
            get => _currentlyTracking;
            set
            {
                _currentlyTracking = value;
                NotifyPropertyChanged(nameof(IsTimerActive));
            }
        }

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
                _diff = TimeSpan.FromSeconds(0);
                LastUpdateTime = DateTime.Now;
                OfflineMode = false;
                IsTimerActive = true;
                TodayHours = TimeSpan.FromHours(2);
                TodayGaugeMaxValue = 8;
                Workspaces = new ObservableCollection<Workspace>()
                {
                    new Workspace() { Name ="Nexpertis"}
                };
                SelectedWorkspace = Workspaces.First();
            }
        }

        public StatusViewModel(TogglProxy togglClient, DailyStatusConfiguration configuration)
        {
            _togglClient = togglClient;
            _config = configuration;

            Init();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(RefreshIntervalInSeconds)
            };
            _timer.Tick += async (s, e) => await RefreshData();
            _timer.Start();
            ScheduleInstantRefresh();
        }

        void Init()
        {
            TodayGaugeMaxValue = _config.GetWorkDayConfig().NumberOfWorkingHoursPerDay;
            Needle = Brushes.Transparent;
            Diff = TimeSpan.FromHours(0);
            TbExpected = TimeSpan.FromHours(0)
                   .ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
            TbActual = TimeSpan.FromHours(0)
                .ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
            LastUpdateTime = null;
            OfflineMode = false;
            TodayHours = TimeSpan.FromSeconds(0);
            Workspaces = new ObservableCollection<Workspace>(){
                new Workspace() { Name =  "Loading..." }
            };
            this._selectedWorkspace = Workspaces.First();
        }

        async Task RefreshData()
        {
            _timer.Interval = TimeSpan.FromSeconds(RefreshIntervalInSeconds);

            try
            {
                if (firstSync)
                {
                    Workspaces = new ObservableCollection<Workspace>(await _togglClient.GetAllWorkspaces());
                    SelectedWorkspace = Workspaces.First();
                }
                _togglClient.SetWorkspace(SelectedWorkspace);
                var actual = (await _togglClient.GetStatus());
                TodayHours = actual.TodaysHours;
                IsTimerActive = actual.IsTimerActive;

                var expected = _togglClient.GetExpectedWorkingTime(_config.GetWorkDayConfig());
                TbExpected = expected.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
                TbActual = actual.TimeInMonth.ToWorkingTimeString(_config.GetWorkDayConfig().NumberOfWorkingHoursPerDay);
                Diff = _togglClient.GetDifference(expected: expected, sum: actual.TimeInMonth);
                Needle = Brushes.Gray;
                LastUpdateTime = DateTime.Now;
                OfflineMode = false;
            }
            catch (OfflineException)
            {
                OfflineMode = true;
            }
            catch (BadRequestException)
            {
                // ignore, that is our exception and it happens occasionally
                // https://github.com/Toumash/daily-status/issues/19
            }
            firstSync = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

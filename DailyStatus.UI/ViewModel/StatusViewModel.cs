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
using System.Windows.Controls;
using System.Collections.Generic;

namespace DailyStatus.UI.ViewModel
{
  public class StatusViewModel : INotifyPropertyChanged
  {
    const int RefreshIntervalInSeconds = 5;

    bool firstSync = true;
    readonly TogglProxy _togglClient;
    DailyStatusConfiguration _config;
    readonly DispatcherTimer _timer;

    TimeSpan _diff;
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
        var sign = "-";
        sign = _diff.TotalHours < 0 ? sign : "";
        return $"{sign}{Math.Abs(_diff.TotalHours):0}:{Math.Abs(_diff.Minutes):00}";
      }
      set { }
    }

    TimeSpan TodayHours
    {
      get => _todayHours;
      set
      {
        _todayHours = value;
        NotifyPropertyChanged(nameof(TodaysCurrentWork));
      }
    }
    public double TodaysCurrentWork { get => Math.Min(TodayHours.TotalHours, _todayGaugeMaxValue); set { } }

    public ICommand CloseCommand
        => new RelayCommand(o => Environment.Exit(0));


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
      Diff = TimeSpan.FromHours(2);
      _diff = TimeSpan.FromSeconds(0);
      LastUpdateTime = DateTime.Now;
      OfflineMode = false;
      IsTimerActive = true;
      TodayHours = TimeSpan.FromHours(2);
      Workspaces = new ObservableCollection<Workspace>()
                {
                    new Workspace() { Name ="Nexpertis"}
                };
      SelectedWorkspace = Workspaces.First();
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


    public void SelectWorkSpace(Workspace w)
    {
      SelectedWorkspace = w;
      NotifyPropertyChanged(nameof(ContextMenu));
    }

    public List<MenuItem> ContextMenu
    {
      get
      {
        var items = new List<MenuItem>();
        var workspaceItems = Workspaces.Select(w =>
        {
          var item = new MenuItem() { Header = w.Name, Command = new RelayCommand(obj => SelectWorkSpace(w)) };
          if (w == SelectedWorkspace)
          {
            item.Background = new SolidColorBrush(Colors.Green);
          }
          return item;
        });
        items.AddRange(workspaceItems);
        items.Add(new MenuItem() { Header = "Minimize", Command = new RelayCommand((obj) => WindowState = WindowState.Minimized) });
        items.Add(new MenuItem() { Header = "Close", Command = CloseCommand });
        return items;
      }
    }

    private WindowState _windowState = WindowState.Normal;
    public WindowState WindowState
    {
      get { return _windowState; }
      set
      {
        _windowState = value;
        NotifyPropertyChanged(nameof(WindowState));
      }
    }

    void Init()
    {
      Diff = TimeSpan.FromHours(0);
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
          NotifyPropertyChanged(nameof(ContextMenu));
        }
        _togglClient.SetWorkspace(SelectedWorkspace);
        var actual = (await _togglClient.GetStatus());
        TodayHours = actual.TodaysHours;
        IsTimerActive = actual.IsTimerActive;

        var expected = _togglClient.GetExpectedWorkingTime(_config.GetWorkDayConfig());
        Diff = _togglClient.GetDifference(expected: expected, sum: actual.TimeInMonth);
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

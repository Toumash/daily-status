using DailyStatus.Common;
using DailyStatus.Common.Configuration;
using DailyStatus.UI.WpfExtensions;
using System;
using System.Windows.Input;

namespace DailyStatus.UI.ViewModel
{
    class StatusViewModel
    {
        TogglProxy _togglClient;
        DailyStatusConfiguration _config;

        public StatusViewModel(TogglProxy togglClient, DailyStatusConfiguration configuration)
        {
            _togglClient = togglClient;
            _config = configuration;
        }

        private TimeSpan? _diff = null;

        public double TimeDiff
        {
            get
            {
                if (_diff == null)
                {
                    Refresh();
                }
                return _diff.Value.TotalHours;
            }
        }

        public ICommand CloseButtonCommand { get; } = new RelayCommand((s) => Environment.Exit(0));

        public string TbTimeDiff
        {
            get
            {
                if (_diff == null)
                {
                    Refresh();
                }
                return $"{_diff.Value.TotalHours:0.#}";
            }
        }

        public void Refresh()
        {
            _diff = _togglClient.GetDifference(_config.GetWorkDayConfig());
        }
    }
}

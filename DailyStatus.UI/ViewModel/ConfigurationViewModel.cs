using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DailyStatus.Common.Extensions;
using DailyStatus.Common.Model;

namespace DailyStatus.UI.ViewModel
{
    class ConfigurationViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Workspace> _workspaces = new ObservableCollection<Workspace>();

        public ObservableCollection<Workspace> Workspaces
        {
            get => _workspaces;
            set { _workspaces = value; NotifyPropertyChanged(nameof(Workspaces)); }
        }

        private Workspace _selectedWorkspace = new Workspace();
        private bool _monthTargetMode;
        private WorkDay _workDayConfig;
        private int _monthTarget;
        private bool _isScaleRangeLock;
        private int _maxScaleRange;

        public Workspace SelectedWorkspace
        {
            get => _selectedWorkspace;
            set
            {
                if (SelectedWorkspace == value) return;
                _selectedWorkspace = value;
                NotifyPropertyChanged(nameof(SelectedWorkspace));
            }
        }


        public double WorkDayStartHour
        {
            get => _workDayConfig.WorkDayStartHour;
            set
            {
                _workDayConfig.WorkDayStartHour = value;
                NotifyPropertyChanged(nameof(WorkDayStartHour));
            }
        }

        public int NumberOfWorkingHoursPerDay
        {
            get => _workDayConfig.NumberOfWorkingHoursPerDay;
            set
            {
                _workDayConfig.NumberOfWorkingHoursPerDay = value;
                NotifyPropertyChanged(nameof(NumberOfWorkingHoursPerDay));
            }
        }

        public int MonthTarget
        {
            get => _monthTarget;
            set
            {
                _monthTarget = value;
                NotifyPropertyChanged(nameof(MonthTarget));
            }
        }

        public bool MonthTargetMode
        {
            get => _monthTargetMode;
            set
            {
                _monthTargetMode = value;
                NotifyPropertyChanged(nameof(MonthTargetMode));
            }
        }

        public bool IsScaleRangeLock
        {
            get => _isScaleRangeLock;
            set
            {
                _isScaleRangeLock = value;
                NotifyPropertyChanged(nameof(IsScaleRangeLock));
            }
        }

        public int MaxScaleRange
        {
            get => _maxScaleRange;
            set
            {
                _maxScaleRange = value;
                NotifyPropertyChanged(nameof(IsScaleRangeLock));
            }
        }
        public ICommand SaveButtonCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand CancelButtonCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ConfigurationViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Workspaces = new ObservableCollection<Workspace>()
                {
                    new Workspace() { Name ="Nexpertis"}
                };
                SelectedWorkspace = Workspaces.First();
                _workDayConfig = new WorkDay();
                WorkDayStartHour = 8;
                NumberOfWorkingHoursPerDay = 8;
                MaxScaleRange = 16;
                IsScaleRangeLock = true;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

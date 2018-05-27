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
            }
        }

        public ICommand SaveButtonCommand
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
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

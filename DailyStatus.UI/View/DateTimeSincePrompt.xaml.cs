using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DailyStatus.UI.View
{
    public partial class DateTimeSincePrompt : Window, INotifyPropertyChanged
    {
        private string _windowTitle;
        private string _windowPrompt;

        public DateTimeSincePrompt()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyPropertyChanged(nameof(WindowTitle));
            }
        }
        public string WindowPrompt
        {
            get { return _windowPrompt; }
            set
            {
                _windowPrompt = value;
                NotifyPropertyChanged(nameof(WindowPrompt));
            }
        }

        public DateTime? Date
        {
            get
            {
                return DatePicker.SelectedDate;
            }
            set
            {
                DatePicker.SelectedDate = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

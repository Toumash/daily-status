using System.ComponentModel;
using System.Windows;

namespace DailyStatus.UI.View
{
    public partial class DecimalPrompt : Window, INotifyPropertyChanged
    {
        private string _windowTitle;
        private string _windowPrompt;

        public DecimalPrompt()
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

        public decimal Value
        {
            get
            {
                decimal.TryParse(ResponseTextBox.Text, out decimal value);
                return value;
            }
            set
            {
                ResponseTextBox.Text = value.ToString();
                NotifyPropertyChanged(nameof(Value));
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

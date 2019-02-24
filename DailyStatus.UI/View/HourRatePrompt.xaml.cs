using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

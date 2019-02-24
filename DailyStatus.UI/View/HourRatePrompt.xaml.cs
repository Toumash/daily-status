using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for ApiTokenPrompt.xaml
    /// </summary>
    public partial class HourRatePrompt : Window
    {
        public HourRatePrompt()
        {
            InitializeComponent();
        }

        public decimal HourRate
        {
            get
            {
                decimal.TryParse(ResponseTextBox.Text, out decimal value);
                return value;
            }
            set
            {
                ResponseTextBox.Text = value.ToString();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

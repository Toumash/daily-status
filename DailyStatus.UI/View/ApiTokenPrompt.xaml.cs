using System.Windows;

namespace DailyStatus.UI.View
{
    /// <summary>
    /// Interaction logic for ApiTokenPrompt.xaml
    /// </summary>
    public partial class ApiTokenPrompt : Window
    {
        public ApiTokenPrompt()
        {
            InitializeComponent();
        }

        public string ApiToken
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

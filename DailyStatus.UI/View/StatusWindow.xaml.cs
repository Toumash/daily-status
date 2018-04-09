using DailyStatus.Common;
using DailyStatus.UI.ViewModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DailyStatus.UI.View
{
    /// <summary>
    /// Interaction logic for StatusWindow.xaml
    /// </summary>
    public partial class StatusWindow : Window
    {

        private TogglProxy togglClient;
        public StatusWindow()
        {
            InitializeComponent();

            MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };
            Loaded += (s, e) =>
            {
                togglClient = new TogglProxy();
                var apiToken = new WindowsCredentialManager().Get();
                if (string.IsNullOrEmpty(apiToken))
                {
                    do
                    {
                        var apiTokenPrompt = new ApiTokenPrompt
                        {
                            Owner = this
                        };
                        var result = apiTokenPrompt.ShowDialog();

                        if (!result.HasValue || !result.Value)
                        {
                            this.Close();
                            return;
                        }
                        apiToken = apiTokenPrompt.ApiToken;
                        new WindowsCredentialManager().Save(apiToken);
                        togglClient.Configure(apiToken);
                    } while (togglClient.TestConnection());
                }
                else
                {
                    togglClient.Configure(apiToken);
                }
                DataContext = new StatusViewModel(
                   togglClient, new Common.Configuration.DailyStatusConfiguration());
            };
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await togglClient.StartTimer();
        }
    }
}

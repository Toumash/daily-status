using DailyStatus.Common;
using DailyStatus.UI.ViewModel;
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
                var apiToken = new WindowsCredentialManager().Get();
                if (string.IsNullOrEmpty(apiToken))
                {
                    var apiTokenPrompt = new ApiTokenPrompt
                    {
                        Owner = this
                    };
                    apiTokenPrompt.ShowDialog();
                    apiToken = apiTokenPrompt.ApiToken;
                    new WindowsCredentialManager().Save(apiToken);
                }

                // FIXME: Dependency Injection + api key prompt
                var togglClient = new TogglProxy();
                togglClient.Configure(apiToken);
                DataContext = new StatusViewModel(
                   togglClient, new Common.Configuration.DailyStatusConfiguration());
            };
        }
    }
}

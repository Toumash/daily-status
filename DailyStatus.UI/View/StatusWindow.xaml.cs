using DailyStatus.Common.Configuration;
using DailyStatus.Common.Services;
using DailyStatus.UI.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

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
                var togglClient = new TogglProxy();
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
                    } while (!togglClient.TestConnection());
                }
                else
                {
                    togglClient.Configure(apiToken);
                }
                var cfg = SettingsManager.LoadSettings();
                DataContext = new StatusViewModel(togglClient, cfg,this);
            };
        }

        private void ctxMenuClick_Close(object sender, RoutedEventArgs e)
        {
            Close();
            Environment.Exit(0);
        }
    }
}

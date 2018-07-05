using DailyStatus.Common;
using DailyStatus.UI.ViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
                TogglProxy togglClient = new TogglProxy();
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

        void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool panelSwitch;

        void Btn_switch_display_Click(object sender, RoutedEventArgs e)
        {
            if (!panelSwitch)
            {
                this.ag_gauge1.Visibility = Visibility.Hidden;
                tb_Playing.Visibility = Visibility.Hidden;
                tb_TimeDifference.Visibility = Visibility.Hidden;
                tb_timeDiffFullscreen.Visibility = Visibility.Visible;
            }
            else
            {
                this.ag_gauge1.Visibility = Visibility.Visible;
                tb_Playing.Visibility = Visibility.Visible;
                tb_TimeDifference.Visibility = Visibility.Visible;
                tb_timeDiffFullscreen.Visibility = Visibility.Hidden;
            }
            panelSwitch = !panelSwitch;
        }
    }
}

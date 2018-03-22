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
            // FIXME: Dependency Injection + api key prompt
            var togglClient = new Common.TogglProxy();
            togglClient.Configure(new WindowsCredentialManager().Get());
            DataContext = new StatusViewModel(
               togglClient, new Common.Configuration.DailyStatusConfiguration());

            MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };
        }
    }
}

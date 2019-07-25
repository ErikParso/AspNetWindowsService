using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Forms;

namespace AspWinServiceClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly NotifyIcon _notifyIcon;

        public MainWindow()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(path));

            InitializeComponent();
            _notifyIcon = InitializeIcon();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5000/");

            output.Text = $"Arguments:{JsonConvert.SerializeObject(Environment.GetCommandLineArgs())}";
        }

        private NotifyIcon InitializeIcon()
        {
            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Resources/configuration.ico");
            ni.Visible = true;
            ni.Click += (sender, args) =>
            {
                Visibility = Visibility.Visible;
            };
            return ni;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - this.Width;
            Top = desktopWorkingArea.Bottom - this.Height;

            this.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var values = await _httpClient.GetAsync("api/version").Result.Content.ReadAsStringAsync();
            output.Text = "Versions: " + values;
            _notifyIcon.ShowBalloonTip(3000, "Helios client manager", "Values was received", ToolTipIcon.Info);
        }
    }
}

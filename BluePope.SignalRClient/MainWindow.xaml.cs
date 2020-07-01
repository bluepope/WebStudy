using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace BluePope.SignalRClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// BluePope.WebShell 을 먼저 실행해야 테스트 가능함
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection _connection;
        public MainWindow()
        {
            InitializeComponent();

            console.Text = string.Empty;

            _connection = new HubConnectionBuilder()
               .WithUrl("https://localhost:5001/hubs/command")
               .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
               .Build();


            _connection.Reconnecting += (error) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    console.Text += $"연결 끊어짐.. 재연결 시도 중...\n";
                });

                return Task.CompletedTask;
            };

            _connection.Closed += (error) =>
            {
                console.Text += $"연결 끊어짐..\n";
                return Task.CompletedTask;
            };

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    console.Text += WebUtility.HtmlDecode(message);
                    consoleViewer.ScrollToEnd();
                });
            });

            Task.Run(async () =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        console.Text += "연결시도 중\n";
                    });
                    await _connection.StartAsync();
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        console.Text += ex.Message + "\n";
                        consoleViewer.ScrollToEnd();
                    });
                }
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendCommand", cmdInput.Text);
            }
            catch (Exception ex)
            {
                //error
                this.Dispatcher.Invoke(() =>
                {
                    console.Text += ex.Message + "\n";
                    consoleViewer.ScrollToEnd();
                });
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var x = new ChatWindow();
            x.Show();
        }
    }
}

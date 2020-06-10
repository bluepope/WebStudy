using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BluePope.SignalRClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        HubConnection _connection;
        public ChatWindow()
        {
            InitializeComponent();


            var client = new RestClient();
            client.BaseUrl = new Uri("https://localhost:5001/home/login");

            var request = new RestRequest();
            request.AddParameter("id", "안현모");
            request.AddParameter("pw", "히히히");
            
            var response = client.Post(request);
            
            chat.Text = string.Empty;
            _connection = new HubConnectionBuilder()
               .WithUrl("https://localhost:5001/hubs/chat", options => {

                   foreach(var cookie in response.Cookies)
                   {
                       options.Cookies.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                   }
               })
               .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
               .Build();


            _connection.Reconnecting += (error) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    chat.Text += $"연결 끊어짐.. 재연결 시도 중...\n";
                });

                return Task.CompletedTask;
            };

            _connection.Closed += (error) =>
            {
                chat.Text += $"연결 끊어짐..\n";
                return Task.CompletedTask;
            };
        
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    chat.Text += $"{user}: {message}\n";
                    chatViewer.ScrollToEnd();
                });
            });

            Task.Run(async () =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        chat.Text += "연결시도 중\n";
                    });
                    await _connection.StartAsync();
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        chat.Text += ex.Message + "\n";
                        chatViewer.ScrollToEnd();
                    });
                }
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", chatInput.Text);
            }
            catch (Exception ex)
            {
                //error
                this.Dispatcher.Invoke(() =>
                {
                    chat.Text += ex.Message + "\n";
                    chatViewer.ScrollToEnd();
                });
            }
        }
    }
}

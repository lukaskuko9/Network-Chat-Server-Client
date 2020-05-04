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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerClient;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyClient client = null;

        public MainWindow(MyClient client)
        { 
            InitializeComponent();
            this.client = client;
            receive();
        }

        private async void receive()
        {
            this.client.OnMessageReceived += Client_OnMessageReceived;
            await client.Receive();
        }

        private void Client_OnMessageReceived(string str)
        {
            Chat.Text += str + Environment.NewLine;
        }

        private async void btn_Send(object sender, RoutedEventArgs e)
        {
            string msg = chatMessage.Text;
            if (msg == string.Empty)
                return;

            if (client != null)
                await client.Send(msg);
        }
    }
}

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

namespace Network_Chat_Server_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Socket
            InitializeComponent();
        }
    }

    class SimpleClient
    {
        ClientInfo client;
        void Start()
        {
            Socket sock = Sockets.CreateTCPSocket("www.myserver.com", 2345);
            client = new ClientInfo(sock, false); // Don't start receiving yet
            client.OnReadBytes += new ConnectionReadBytes(ReadData);
            client.BeginReceive();
        }

        void ReadData(ClientInfo ci, byte[] data, int len)
        {
            Console.WriteLine("Received " + len + " bytes: " +
               System.Text.Encoding.UTF8.GetString(data, 0, len));
        }
    }
}

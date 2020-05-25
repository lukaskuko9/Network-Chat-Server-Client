using ChatApp.Chat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ClientApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();
      
        public ObservableCollection<ChatMessage> chatMessages
        {
            get => _chatMessages;
            set
            {
                OnPropertyChanged();
                _chatMessages = value;
            }
        }
        ChatUser user = null;
        ChatMessage _message;
        public ChatMessage message
        {
            get => _message;
            set
            {
                OnPropertyChanged();
                _message = value;
            }

        }

        public MainWindow(ChatUser client)
        {
            InitializeComponent();
            this.DataContext = this;
            chatMessage_TextBox.KeyDown += ChatMessage_TextBox_KeyDown;
            user = client;
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            await startReceiving();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void ChatMessage_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                string msg = chatMessage_TextBox.Text;
                await SendMessage(msg);
            }
        }

        private async Task startReceiving()
        {
            if (!user.Client.TcpClient.Connected)
                return; 

            this.user.Client.OnMessageReceived += Client_OnMessageReceived;
            await user.Client.ReceiveAsync(); 

        //user disconnected from server and chose to reconnect
            var c = new ConnectionWindow();
            var client = await c.Connect(user.Username, user.Client.ServerIP, user.Client.ServerPort);

            if (c != null && client != null)
            {
                c.Close();

                MainWindow m = new MainWindow(client);
                m.ShowActivated = true;
                m.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Connection failed", "Unable to reconnect", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
        }

        private void Client_OnMessageReceived(ChatMessage chatMessage)
        {
            chatMessages.Add(chatMessage);
        }

        async Task SendMessage(string msg)
        {  
            if (msg == string.Empty)
                return;

            if(!user.Client.TcpClient.Connected)
            {
                MessageBox.Show("Not connected to server");
                return;
            }
            
            if (user.Client != null)
            {
                ChatMessage chatMessage = new ChatMessage(user, msg);
                await user.Client.SendObjectAsync(chatMessage);
                chatMessage_TextBox.Text = string.Empty;
            }     
        }

        private async void btnClick_Send(object sender, RoutedEventArgs e)
        {
            string msg = chatMessage_TextBox.Text;
            await SendMessage(msg);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label l = (Label)sender;
            MessageBox.Show(l.Content.ToString());
        }
    }
}
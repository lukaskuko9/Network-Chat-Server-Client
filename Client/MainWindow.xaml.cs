using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            receive();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ChatMessage_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                string msg = chatMessage_TextBox.Text;
                SendMessage(msg);
            }
        }

        private async void receive()
        {
            this.user.Client.OnMessageReceived += Client_OnMessageReceived;
            await user.Client.ReceiveAsync();
        }

        private void Client_OnMessageReceived(ChatMessage chatMessage)
        {
            chatMessages.Add(chatMessage);
        }

        async void SendMessage(string msg)
        {  
            if (msg == string.Empty)
                return;

            ChatMessage chatMessage = new ChatMessage(user, msg);

            if (user.Client != null)
                await user.Client.SendAsync(ChatMessage.GetStringFromMessage(chatMessage));

            chatMessage_TextBox.Text = string.Empty;
        }

        private void btnClick_Send(object sender, RoutedEventArgs e)
        {
            string msg = chatMessage_TextBox.Text;
            SendMessage(msg);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label l = (Label)sender;
            MessageBox.Show(l.Content.ToString());
        }
    }
}
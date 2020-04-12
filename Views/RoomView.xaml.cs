using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using Werewolf.Classes.Room;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for RoomView.xaml
    /// </summary>
    public partial class RoomView : UserControl
    {
        private readonly MainWindow _window;
        private bool _hasScroll;

        public RoomView(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            _hasScroll = false;

            ChatBox.Document.Blocks.Clear();
            IPAddressText.Text = ClientRoom.Instance.IPAddressString;

            if (!ClientRoom.Instance.IsHost)
            {
                StartGame.Visibility = Visibility.Hidden;
            }

            ClientRoom.Instance.RoomUserListSet += (sender, e) =>
            {
                foreach (string user in e.UserList)
                    AddUser(user);
            };

            ClientRoom.Instance.RoomUserMessageSent += (sender, e) =>
            {
                AddChatMessage(e.Name, e.Message);
            };

            ClientRoom.Instance.RoomUserJoined += (sender, e) =>
            {
                AddUser(e.Name);
                AddChatMessage(string.Empty, e.Name + " vient de se connecter sur le salon.");
            };

            ClientRoom.Instance.RoomUserLeft += (sender, e) =>
            {
                RemoveUser(e.Name);
                AddChatMessage(string.Empty, e.Name + " s'est déconnecté(e) du salon.");
            };

            ClientRoom.Instance.Listen();
        }

        private void AddChatMessage(string name, string message)
        {
            string nameIfPresent = name.Length == 0 ? name : $"<{name}> ";

            Dispatcher.Invoke(() =>
            {
                ChatBox.Document.Blocks.Add(new Paragraph(new Run($"{nameIfPresent}{message}")));
                if (!_hasScroll || ChatBox.VerticalOffset == (ChatBox.ExtentHeight - ChatBox.ViewportHeight))
                    ChatBox.ScrollToEnd();
            });
        }

        private void AddUser(string name)
        {
            Dispatcher.Invoke(() =>
            {
                UserList.Items.Add(name);
            });
        }

        private void RemoveUser(string name)
        {
            Dispatcher.Invoke(() =>
            {
                UserList.Items.Remove(name);
            });
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            new RoomSettingsWindow(_window, ClientRoom.Instance.IsHost).ShowDialog();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageBox.Text)) return;

            ClientRoom.Instance.Send(ServerRoomClientEvent.ROOM_USER_SEND_MESSAGE, MessageBox.Text);
            MessageBox.Text = string.Empty;
        }

        private void ChatBox_Scroll(object sender, ScrollEventArgs e)
        {
            _hasScroll = true;
        }
    }
}

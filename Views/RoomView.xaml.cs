﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

using Werewolf.Network;
using Werewolf.Network.Events;

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
            IPAddressText.Text = Client.Instance.IPAddressString;

            if (!Client.Instance.IsHost)
            {
                SettingsBtn.Visibility = Visibility.Hidden;
                StartGame.Visibility = Visibility.Hidden;
            }

            Client.Instance.ServerEvents.AddListener<UserListSetEventArgs>((sender, e) =>
            {
                foreach (string user in e.UserList)
                    AddUser(user);
            });

            Client.Instance.ServerEvents.AddListener<ChatMessageSentEventArgs>((sender, e) =>
            {
                AddChatMessage(e.Name, e.Message);
            });

            Client.Instance.ServerEvents.AddListener<UserJoinedEventArgs>((sender, e) =>
            {
                AddUser(e.Name);
                AddChatMessage(string.Empty, e.Name + " vient de se connecter sur le salon.");
            });

            Client.Instance.ServerEvents.AddListener<UserLeftEventArgs>((sender, e) =>
            {
                RemoveUser(e.Name);
                AddChatMessage(string.Empty, e.Name + " s'est déconnecté(e) du salon.");
            });

            Client.Instance.ServerEvents.AddListener<GameStartedEventArgs>((sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    GameView view = _window.SetView<GameView>();
                    view.SetRoles(e.RoleIds);
                    view.SetUsers(UserList.Items.Cast<string>().ToArray());
                });
            });

            Client.Instance.ListenServerEvents();
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
            if (!Client.Instance.IsHost) return;

            GameSettingsWindow gameSettingsWindow = new GameSettingsWindow(_window);
            gameSettingsWindow.SetRoomView(this);
            gameSettingsWindow.ShowDialog();
        }

        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (!Client.Instance.IsHost) return;
            StartGame.IsEnabled = false;

            for (int i = 5; i >= 1; --i)
            {
                Server.Instance.SendEvent(new ChatMessageSentEventArgs(string.Empty, $"Démarrage de la partie dans {i}..."));
                await Task.Delay(1000);
            }

            Server.Instance.SendEvent(new GameStartedEventArgs(Game.Game.Instance.GetRoles().Select((role) => role.Id).ToArray()));
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            Message.Text = Message.Text.Trim();
            if (string.IsNullOrWhiteSpace(Message.Text)) return;

            Client.Instance.SendEvent(new SendChatMessageEventArgs(Message.Text));
            Message.Text = string.Empty;
        }

        private void ChatBox_Scroll(object sender, ScrollEventArgs e) => _hasScroll = true;

        public void EnableStartGameButton() => StartGame.IsEnabled = true;
    }
}

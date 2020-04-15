using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

using Werewolf.Game;
using Werewolf.Network;
using Werewolf.Network.Events;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private readonly MainWindow _window;
        private bool _hasScroll;

        public GameView(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            _hasScroll = false;

            ChatBox.Document.Blocks.Clear();

            Client.Instance.ServerEvents.AddListener<ChatMessageSentEventArgs>((sender, e) =>
            {
                AddChatMessage(e.Name, e.Message);
            });

            Client.Instance.ServerEvents.AddListener<UserLeftEventArgs>((sender, e) =>
            {
                RemoveUser(e.Name);
                AddChatMessage(string.Empty, e.Name + " s'est déconnecté(e) de la partie.");
            });
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

        private void RemoveUser(string name)
        {
            Dispatcher.Invoke(() =>
            {
                UserList.Items.Remove(name);
            });
        }

        private void ChatBox_Scroll(object sender, ScrollEventArgs e) => _hasScroll = true;

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Text = MessageBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(MessageBox.Text)) return;

            Client.Instance.SendEvent(new SendChatMessageEventArgs(MessageBox.Text));
            MessageBox.Text = string.Empty;
        }

        public void SetRoles(int[] roleIds)
        {
            foreach (int id in roleIds)
            {
                Role role = Role.GetRoleById(id);

                ListBoxItem item = new ListBoxItem
                {
                    DataContext = role,
                    Foreground = new SolidColorBrush(role.DefaultTeam.Color),
                    Content = role.Name
                };

                RoleList.Items.Add(item);
            }
        }

        public void SetUsers(string[] users)
        {
            foreach (string user in users)
                UserList.Items.Add(user);
        }
    }
}

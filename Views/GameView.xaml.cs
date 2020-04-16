using System;
using System.Text;
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
                // Tuer le joueur
            });

            Client.Instance.ServerEvents.AddListener<SetRoleEventArgs>((sender, e) =>
            {
                Role role = Role.GetRoleById(e.RoleId);
                Client.Instance.Role = role;

                Dispatcher.Invoke(() =>
                {
                    RoleText.Text = role.Name;
                    RoleText.Foreground = new SolidColorBrush(role.DefaultTeam.Color);

                    Inline inline1 = new Italic(new Run("Vous gagnez avec "));
                    Run team = new Run();

                    if (role.DefaultTeam == Team.Village)
                        team.Text = "le village";
                    else if (role.DefaultTeam == Team.Werewolf)
                        team.Text = "les loup-garous";
                    team.Text += ".";
                    team.Foreground = new SolidColorBrush(role.DefaultTeam.Color);

                    AddChatMessage(new Italic(new Run("Vous gagnez avec ")), new Italic(team));
                    AddChatMessage(new Italic(new Run(role.Description)));
                });
            });

            Client.Instance.ServerEvents.AddListener<TimerUpdatedEventArgs>((sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (e.Seconds < 0)
                        TimerText.Text = string.Empty;
                    else
                        TimerText.Text = TimeSpan.FromSeconds(e.Seconds).ToString("mm\\:ss");
                });
            });

            if (Server.Instance.Started)
            {
                Game.Game.Instance.AssignRolesRandomly();

                foreach (Player player in Game.Game.Instance.GetPlayers())
                    player.User.SendEvent(new SetRoleEventArgs(player.Role.Id));

                Game.Game.Instance.SendEvent(new ChatMessageSentEventArgs(string.Empty, "La première nuit démarre dans 15 secondes..."));
                Game.Game.Instance.StartGameLoop();
            }
        }

        private void AddChatMessage(string name, string message)
        {
            Dispatcher.Invoke(() =>
            {
                Inline inline;

                if (name.Length == 0)
                    inline = new Italic(new Run(message));
                else
                    inline = new Run($"<{name}> {message}");
                AddChatMessage(inline);
            });
        }

        private void AddChatMessage(params Inline[] inlines)
        {
            Paragraph paragraph = new Paragraph();
            foreach (Inline inline in inlines)
                paragraph.Inlines.Add(inline);

            ChatBox.Document.Blocks.Add(paragraph);
            if (!_hasScroll || ChatBox.VerticalOffset == (ChatBox.ExtentHeight - ChatBox.ViewportHeight))
                ChatBox.ScrollToEnd();
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
            Message.Text = Message.Text.Trim();
            if (string.IsNullOrWhiteSpace(Message.Text)) return;

            Client.Instance.SendEvent(new SendChatMessageEventArgs(Message.Text));
            Message.Text = string.Empty;
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

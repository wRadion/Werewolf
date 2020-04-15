using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Werewolf.Game;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for GameSettingsWindow.xaml
    /// </summary>
    public partial class GameSettingsWindow : Window
    {
        private RoomView _roomView;

        public GameSettingsWindow(MainWindow window)
        {
            InitializeComponent();
            Owner = window;
            _roomView = null;

            Closing += (sender, e) =>
            {
                if (!Game.Game.Instance.ValidateRoles())
                {
                    e.Cancel = true;
                    MessageBox.Show("Il doit y avoir plus de villageois que de loup-garous et au moins un loup-garou.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (_roomView != null)
                        _roomView.EnableStartGameButton();
                }
            };

            foreach (Role role in Role.GetAllRoles())
            {
                if (!Game.Game.Instance.ContainsRole(role) || !role.IsUnique)
                    AddRoleAndSort(AvailableRoleList, role);
            }

            foreach (Role role in Game.Game.Instance.GetRoles())
                AddRoleAndSort(ChosenRoleList, role);
        }

        private void AddRoleAndSort(ListBox listBox, Role addedRole)
        {
            ItemCollection collection = listBox.Items;

            ListBoxItem item = new ListBoxItem
            {
                DataContext = addedRole,
                Foreground = new SolidColorBrush(addedRole.DefaultTeam.Color),
                Content = addedRole.Name
            };

            collection.Add(item);
            List<ListBoxItem> list = collection.Cast<ListBoxItem>().ToList();
            list.Sort((item1, item2) => ((Role)item1.DataContext).Name.CompareTo(((Role)item2.DataContext).Name));

            collection.Clear();
            foreach (ListBoxItem listBoxItem in list)
                collection.Add(listBoxItem);
        }

        private void AddRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableRoleList.SelectedItem == null) return;

            int index = AvailableRoleList.SelectedIndex;
            Role role = (Role)(((ListBoxItem)AvailableRoleList.Items[index]).DataContext);

            AddRoleAndSort(ChosenRoleList, role);
            Game.Game.Instance.AddRole(role);
            if (role.IsUnique)
                AvailableRoleList.Items.RemoveAt(index);
        }

        private void RemoveRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ChosenRoleList.SelectedItem == null) return;

            int index = ChosenRoleList.SelectedIndex;
            Role role = (Role)(((ListBoxItem)ChosenRoleList.Items[index]).DataContext);

            ChosenRoleList.Items.RemoveAt(index);
            Game.Game.Instance.RemoveRole(role);
            if (role.IsUnique)
                AddRoleAndSort(AvailableRoleList, role);
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetRoomView(RoomView roomView)
        {
            _roomView = roomView;
        }
    }
}

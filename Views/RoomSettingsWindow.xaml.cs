using System.Windows;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for RoomSettingsWindow.xaml
    /// </summary>
    public partial class RoomSettingsWindow : Window
    {
        public RoomSettingsWindow(MainWindow window)
        {
            InitializeComponent();
            Owner = window;
        }

        private void AddRoleBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveRoleBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

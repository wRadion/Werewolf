using System.Net;
using System.Windows;
using System.Windows.Controls;

using Werewolf.Classes.Room;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private readonly MainWindow _window;

        public MainView(MainWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private void ConnectClient(string ipAddressString = null)
        {
            IPAddress ipAddress = IPAddress.Loopback;

            if (ipAddressString != null)
            {
                bool isValid = IPAddress.TryParse(ipAddressString, out ipAddress);

                if (!isValid)
                {
                    MessageBox.Show("Erreur : l'adresse IP est invalide.", "Erreur - Adresse IP Invalide", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
                ServerRoom.Instance.Start();

            if (ClientRoom.Instance.Connect(UserName.Text, ipAddress))
                _window.SetView<RoomView>();
            else
            {
                MessageBox.Show("Erreur lors de la connexion : le pseudo que vous avez choisi est déjà pris.", "Pseudo déjà pris", MessageBoxButton.OK, MessageBoxImage.Error);
                ClientRoom.Instance.Disconnect();
            }
        }

        private void RoomConnectBtn_Click(object sender, RoutedEventArgs e) => ConnectClient(RoomIPAddress.Text);
        private void RoomCreateBtn_Click(object sender, RoutedEventArgs e) => ConnectClient();
    }
}

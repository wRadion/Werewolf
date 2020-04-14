using System.Net;
using System.Windows;
using System.Windows.Controls;

using Werewolf.Network;

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
            UserName.Text = UserName.Text.Trim();

            if (UserName.Text.Length < 3)
            {
                MessageBox.Show("Votre pseudo doit contenir au moins 3 lettres.", "Erreur - Pseudo invalide", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                Server.Instance.Start();

            if (Client.Instance.Connect(UserName.Text, ipAddress))
                _window.SetView<RoomView>();
            else
            {
                MessageBox.Show("Erreur lors de la connexion : le pseudo que vous avez choisi est déjà pris.", "Pseudo déjà pris", MessageBoxButton.OK, MessageBoxImage.Error);
                Client.Instance.Disconnect();
            }
        }

        private void RoomConnectBtn_Click(object sender, RoutedEventArgs e) => ConnectClient(RoomIPAddress.Text);
        private void RoomCreateBtn_Click(object sender, RoutedEventArgs e) => ConnectClient();
    }
}

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

        private void ConnectClient(string ipAddress = null)
        {
            if (ipAddress == null)
                ServerRoom.Instance.Start();
            ClientRoom.Instance.Connect(ipAddress);
            _window.SetView(new RoomView(_window));
        }

        private void RoomConnectBtn_Click(object sender, RoutedEventArgs e) => ConnectClient(RoomIPAddress.Text);
        private void RoomCreateBtn_Click(object sender, RoutedEventArgs e) => ConnectClient();
    }
}

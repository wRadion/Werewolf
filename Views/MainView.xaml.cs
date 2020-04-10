using System.Windows;
using System.Windows.Controls;

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

        private void RoomConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            _window.SetView(new RoomView(_window));
        }

        private void RoomCreateBtn_Click(object sender, RoutedEventArgs e)
        {
            _window.SetView(new RoomView(_window));
        }
    }
}

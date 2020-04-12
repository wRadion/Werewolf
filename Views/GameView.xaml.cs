using System.Windows;
using System.Windows.Controls;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private readonly MainWindow _window;

        public GameView(MainWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

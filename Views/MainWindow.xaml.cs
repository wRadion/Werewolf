using System.Windows;
using System.Windows.Controls;
using Werewolf.Classes.Room;

namespace Werewolf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closed += ((sender, e) =>
            {
                ClientRoom.Instance.Disconnect();
                ServerRoom.Instance.Stop();
            });

            SetView(new MainView(this));
        }

        public void SetView(UserControl view)
        {
            Main.Child = view;
        }
    }
}

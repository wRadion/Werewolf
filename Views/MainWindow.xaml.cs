using System.Windows;
using System.Windows.Controls;

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
            SetView(new GameView(this));
        }

        public void SetView(UserControl view)
        {
            Main.Child = view;
        }
    }
}

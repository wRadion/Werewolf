using System;
using System.Windows;
using System.Windows.Controls;

using Werewolf.Network;

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
                Utils.MessageBox.IsClosing = true;
                Client.Instance.Disconnect(true);
                Server.Instance.Stop();
            });

            SetView<MainView>();
        }

        public TView SetView<TView>() where TView : UserControl
        {
            TView view = (TView)(Activator.CreateInstance(typeof(TView), this));
            Main.Child = view;
            return view;
        }
    }
}

using System.Windows;

namespace Match3V2
{
    public partial class MainWindow : Window
    {
        private Game _game;

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
        }

        private void StartGame()
        {
            _game = new Game(GameCanvas, 8, 8);
            _game.Start();
        }
    }
}

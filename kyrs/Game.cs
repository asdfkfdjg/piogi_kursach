using System.Windows.Controls;

namespace Match3V2
{
    public class Game
    {
        private readonly Grid _grid;

        public Game(Canvas canvas, int rows, int columns)
        {
            _grid = new Grid(canvas, rows, columns, OnTilesMatched, OnTileMoved);
        }

        public void Start()
        {
            _grid.GenerateGrid();
        }

        private void OnTilesMatched(int count)
        {
            // Увеличить счёт на основе удалённых клеток
        }

        private void OnTileMoved()
        {
            // Логика, которая может быть выполнена после перемещения клетки
        }
    }
}

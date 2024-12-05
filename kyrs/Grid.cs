using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Match3V2
{
    public class Grid
    {
        private readonly Canvas _canvas;
        private readonly int _rows;
        private readonly int _columns;
        private readonly Action<int> _onTilesMatched;
        private readonly Action _onTileMoved;
        private Tile[,] _tiles;
        private Tile _selectedTile;

        public Grid(Canvas canvas, int rows, int columns, Action<int> onTilesMatched, Action onTileMoved)
        {
            _canvas = canvas;
            _rows = rows;
            _columns = columns;
            _onTilesMatched = onTilesMatched;
            _onTileMoved = onTileMoved;
            _tiles = new Tile[rows, columns];
        }

        public void GenerateGrid()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    CreateTile(row, col);
                }
            }

            // Убедиться, что поле без начальных совпадений
            while (RemoveMatchesAndFill())
            {
                // Повторить генерацию до тех пор, пока не останутся совпадения
            }
        }

        private void CreateTile(int row, int col)
        {
            var tile = new Tile(row, col, GetRandomColor(), _canvas, OnTileSelected);
            _tiles[row, col] = tile;
        }

        private void OnTileSelected(Tile clickedTile)
        {
            if (_selectedTile == null)
            {
                _selectedTile = clickedTile;
                _selectedTile.Highlight(true);
            }
            else
            {
                if (AreNeighbors(_selectedTile, clickedTile))
                {
                    PerformSwap(_selectedTile, clickedTile, isUndoable: true);
                }

                _selectedTile.Highlight(false);
                _selectedTile = null;
            }
        }

        private bool AreNeighbors(Tile a, Tile b)
        {
            return (Math.Abs(a.Row - b.Row) == 1 && a.Column == b.Column) ||
                   (Math.Abs(a.Column - b.Column) == 1 && a.Row == b.Row);
        }

        private void PerformSwap(Tile a, Tile b, bool isUndoable)
        {
            // Обновить позиции в массиве _tiles
            SwapTiles(a, b);

            // Запустить анимацию
            a.AnimatePosition(a.Row, a.Column);
            b.AnimatePosition(b.Row, b.Column);

            // После анимации проверить совпадения
            _canvas.Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(1000); // Ждём завершения анимации

                var matchesA = FindMatchForTile(a);
                var matchesB = FindMatchForTile(b);

                if (!matchesA.Any() && !matchesB.Any())
                {
                    // Если совпадений нет, вернуть клетки на исходные позиции
                    if (isUndoable)
                    {
                        PerformSwap(a, b, isUndoable: false);
                    }
                }
                else
                {
                    // Удалить совпадения и заполнить пустоты
                    RemoveTiles(matchesA.Concat(matchesB).ToList());
                    FillEmptySpaces();
                }

                _onTileMoved();
            });
        }

        private void SwapTiles(Tile a, Tile b)
        {
            int tempRow = a.Row;
            int tempCol = a.Column;

            a.SetPosition(b.Row, b.Column);
            b.SetPosition(tempRow, tempCol);

            _tiles[a.Row, a.Column] = a;
            _tiles[b.Row, b.Column] = b;
        }

        private List<Tile> FindMatchForTile(Tile tile)
        {
            var matches = new List<Tile> { tile };

            // Проверка горизонтали
            for (int col = tile.Column - 1; col >= 0 && _tiles[tile.Row, col]?.Color == tile.Color; col--)
                matches.Add(_tiles[tile.Row, col]);
            for (int col = tile.Column + 1; col < _columns && _tiles[tile.Row, col]?.Color == tile.Color; col++)
                matches.Add(_tiles[tile.Row, col]);

            if (matches.Count < 3)
                matches.Clear();

            // Проверка вертикали
            var verticalMatches = new List<Tile> { tile };
            for (int row = tile.Row - 1; row >= 0 && _tiles[row, tile.Column]?.Color == tile.Color; row--)
                verticalMatches.Add(_tiles[row, tile.Column]);
            for (int row = tile.Row + 1; row < _rows && _tiles[row, tile.Column]?.Color == tile.Color; row++)
                verticalMatches.Add(_tiles[row, tile.Column]);

            if (verticalMatches.Count >= 3)
                matches.AddRange(verticalMatches);

            return matches.Distinct().ToList();
        }

        private void RemoveTiles(List<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                _tiles[tile.Row, tile.Column] = null;
                tile.Remove();
            }

            _onTilesMatched(tiles.Count);
        }

        private void FillEmptySpaces()
        {
            for (int col = 0; col < _columns; col++)
            {
                for (int row = _rows - 1; row >= 0; row--)
                {
                    if (_tiles[row, col] == null)
                    {
                        for (int r = row - 1; r >= 0; r--)
                        {
                            if (_tiles[r, col] != null)
                            {
                                _tiles[row, col] = _tiles[r, col];
                                _tiles[r, col] = null;
                                _tiles[row, col].SetPosition(row, col, animate: true);
                                break;
                            }
                        }

                        if (_tiles[row, col] == null)
                        {
                            CreateTile(row, col);
                        }
                    }
                }
            }

            // После заполнения проверить наличие новых совпадений
            while (RemoveMatchesAndFill())
            {
                // Повторять пока совпадения остаются
            }
        }

        private bool RemoveMatchesAndFill()
        {
            var allMatches = new List<Tile>();

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    var matches = FindMatchForTile(_tiles[row, col]);
                    allMatches.AddRange(matches);
                }
            }

            if (allMatches.Any())
            {
                RemoveTiles(allMatches.Distinct().ToList());
                FillEmptySpaces();
                return true;
            }

            return false;
        }

        private Color GetRandomColor()
        {
            var colors = new[] { Colors.Red, Colors.Green, Colors.Blue };
            return colors[new Random().Next(colors.Length)];
        }
    }
}

using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Match3V2
{
    public class Tile
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Color Color { get; private set; }
        private readonly Rectangle _visual;
        private readonly Canvas _canvas;

        public Tile(int row, int column, Color color, Canvas canvas, Action<Tile> onClick)
        {
            Row = row;
            Column = column;
            Color = color;
            _canvas = canvas;

            _visual = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new SolidColorBrush(color)
            };

            _visual.MouseLeftButtonDown += (sender, args) => onClick(this);
            _canvas.Children.Add(_visual);
            UpdateVisualPosition();
        }

        public void SetPosition(int row, int column, bool animate = false)
        {
            Row = row;
            Column = column;
            if (animate)
            {
                AnimatePosition(row, column);
            }
            else
            {
                UpdateVisualPosition();
            }
        }

        public void AnimatePosition(int row, int column)
        {
            // Пример плавной анимации перемещения с задержкой
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 0.3 секунда задержки
            };

            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                UpdateVisualPosition();
            };

            timer.Start();
        }

        public void Highlight(bool enable)
        {
            _visual.Stroke = enable ? Brushes.Yellow : null;
            _visual.StrokeThickness = enable ? 3 : 0;
        }

        public void Remove()
        {
            _canvas.Children.Remove(_visual);
        }

        private void UpdateVisualPosition()
        {
            Canvas.SetLeft(_visual, Column * 50);
            Canvas.SetTop(_visual, Row * 50);
        }
    }
}

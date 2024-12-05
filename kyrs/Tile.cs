using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Match3V2
{
    public class Tile
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public System.Drawing.Color Color { get; private set; }
        private readonly System.Windows.Shapes.Rectangle _visual;
        private readonly Canvas _canvas;

        public Tile(int row, int column, int index, Canvas canvas, Action<Tile> onClick)
        {
            Row = row;
            Column = column;
            _canvas = canvas;

            ImageBrush ib = new ImageBrush();
            //позиция изображения будет указана как координаты левого верхнего угла
            //изображение будет растянуто по размерам прямоугольника, описанного вокруг фигуры
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            switch(index)
            {
                case 0: ib.ImageSource = new BitmapImage(new Uri(@"C:\Users\SAPR\Desktop\piogi\kyrs\pics/favicon.png", UriKind.Absolute)); this.Color = System.Drawing.Color.Red; break;
                case 1: ib.ImageSource = new BitmapImage(new Uri(@"C:\Users\SAPR\Desktop\piogi\kyrs\pics/Emojione_1F48E.svg.png", UriKind.Absolute)); this.Color = System.Drawing.Color.Green; break;
                case 2: ib.ImageSource = new BitmapImage(new Uri(@"C:\Users\SAPR\Desktop\piogi\kyrs\pics/img2.png", UriKind.Absolute)); this.Color = System.Drawing.Color.Blue; break;
            }

            _visual = new System.Windows.Shapes.Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = ib
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MS.Internal.PresentationFramework;

namespace _2584interface
{
    class Tile
    {
        private int size = 100;
        public Tile(int positionX, int positionY, int index)
        {
            this.index = index;
            this.X = positionX;
            this.Y = positionY;

            Image img = new Image();
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(@"D:\code\2584interface\2584interface\resource\th.jpg");
            b.EndInit();
            img.Source = b;
            img.Width = size;
            img.Height = size;
            img.Visibility = System.Windows.Visibility.Visible;
            Canvas.SetTop(img, positionY * size);
            Canvas.SetLeft(img, positionX * size);
            //img.RenderTransform = new TranslateTransform(X * 100, positionY * 100);
            this.img = img;
        }

        /// <summary>
        /// move to particular position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(int x, int y)
        {
            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(size * X, size * x, TimeSpan.FromSeconds(1));
            DoubleAnimation anim2 = new DoubleAnimation(size * Y, size * y, TimeSpan.FromSeconds(1));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);

            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 显示在界面中
        /// </summary>
        public void Show()
        {

        }

        /// <summary>
        /// if can combine, move to the position and create a new tile
        /// </summary>
        /// <param name="tile">target tile</param>
        public void MoveTo(Tile tile)
        {
            MoveTo(tile.X, tile.Y);
        }

        public int X;
        public int Y;
        public int index;
        public Image img;
    }
}

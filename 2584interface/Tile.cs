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
        public Tile(int row, int col, int index)
        {
            this.index = index;
            this.X = col;
            this.Y = row;

            Image img = GetImage(index);
            //Canvas.SetTop(img, Y * size);
            //Canvas.SetLeft(img, X * size);
            //img.RenderTransform = new TranslateTransform(X * 100, positionY * 100);
            this.img = img;
            MoveTo(Y, X);
        }

        public Image GetImage(int index)
        {
            Image img = new Image();
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(string.Format(@"D:\code\2584interface\2584interface\resource\{0}.png", index));
            b.EndInit();
            img.Source = b;
            img.Width = size;
            img.Height = size;
            img.Visibility = System.Windows.Visibility.Visible;
            return img;
        }

        public void SetImage(int index)
        {
            this.img = GetImage(index);
        }

        void BeginAnimation()
        {
            animationEnd = false;
            animation1End = false;
            animation2End = false;
        }

        void EndAnimation()
        {
            if (animation1End && animation2End)
            {
                animationEnd = true;
            }
        }

        /// <summary>
        /// move to particular position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(int row, int col)
        {
            BeginAnimation();
            int x = col;
            int y = row;
            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;
            double animationTime = 0.5;
            DoubleAnimation anim1 = new DoubleAnimation(size * X, size * x, TimeSpan.FromSeconds(animationTime));
            DoubleAnimation anim2 = new DoubleAnimation(size * Y, size * y, TimeSpan.FromSeconds(animationTime));

            anim1.Completed += (sender, eArgs) => { animation1End = true; EndAnimation(); };
            anim2.Completed += (sender, eArgs) => { animation2End = true; EndAnimation(); };

            //DoubleAnimation anim1 = new DoubleAnimation(0, size * (x - X), TimeSpan.FromSeconds(1));
            //DoubleAnimation anim2 = new DoubleAnimation(0, size * (y - Y), TimeSpan.FromSeconds(1));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);

            //img.BeginAnimation(TranslateTransform.XProperty, anim1);
            //img.BeginAnimation(TranslateTransform.YProperty, anim2);

            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 显示在界面中
        /// </summary>
        public void Show()
        {

        }

        public void Remove()
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

        public override string ToString()
        {
            return string.Format("({0}, {1}) {2}", X, Y, index);
        }

        public int X;
        public int Y;
        public int index;
        public Image img;
        public bool animationEnd = true;
        private bool animation1End = true;
        private bool animation2End = true;
    }
}

using System;
using System.Collections;
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
        public static readonly object locker = new object();
        private static int _flag;
        private static Board board;
        private static Canvas canvas;

        public static void SetBoardAndCanvas(Board board, Canvas canvas)
        {
            Tile.board = board;
            Tile.canvas = canvas;
        }

        public static int Flag
        {
            set {
                lock (locker)
                {
                    _flag = value;
                }
            }
            get
            {
                lock (locker)
                {
                    return _flag;
                }
            }
        }

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
            b.UriSource = new Uri(string.Format(@"resource\{0}\{1}.png", App.game, index), UriKind.Relative);
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

        void EndAnimation(int index, Hashtable actions)
        {
            if (animation1End && animation2End)
            {
                animationEnd = true;
                int bit = 1;
                bit <<= index;
                Flag &= ~bit;
            }

            if (Flag != 0)
                return;

            Tile tile;
            List<Action> removeAction = actions["remove"] as List<Action>;
            List<Action> newAction = actions["new"] as List<Action>;

            foreach (Action action in removeAction)
            {
                tile = action.tile;
                canvas.Children.Remove(tile.img);
                //board[action.row, action.col] = null;
            }

            foreach (Action action in newAction)
            {
                tile = action.tile;
                //board[action.row, action.col] = tile;
                canvas.Children.Add(tile.img);
            }
            board.isMoving = false;



        }

        /// <summary>
        /// move to particular position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(int row, int col, int i = -1, Hashtable actions = null)
        {
            BeginAnimation();
            int x = col;
            int y = row;
            TranslateTransform trans = new TranslateTransform();
            img.RenderTransform = trans;
            double animationTime = 0.1;
            DoubleAnimation anim1 = new DoubleAnimation(size * X, size * x, TimeSpan.FromSeconds(animationTime));
            DoubleAnimation anim2 = new DoubleAnimation(size * Y, size * y, TimeSpan.FromSeconds(animationTime));

            if (i != -1)
            {
                anim1.Completed += (sender, eArgs) => { animation1End = true; EndAnimation(i, actions); };
                anim2.Completed += (sender, eArgs) => { animation2End = true; EndAnimation(i, actions); };
            }
                


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

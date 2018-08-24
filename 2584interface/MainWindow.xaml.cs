using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2584interface
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Board b;
        Evil evil;
        Player player;
        public MainWindow()
        {
            InitializeComponent();
            b = new Board(mainCanvas);
            evil = new Evil();
            player = new Player();
        }
        
        //Tile tile;
        //private void Canvas_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //< Image HorizontalAlignment = "Left" Height = "100" VerticalAlignment = "Top" Width = "100" Canvas.Left = "0" Canvas.Top = "0" />
        //    tile = new Tile(0, 0, 2);
        //    mainCanvas.Children.Add(tile.img);
        //    //Image img = new Image();
        //    //BitmapImage b = new BitmapImage();
        //    //b.BeginInit();
        //    //b.UriSource = new Uri(@"D:\code\2584interface\2584interface\resource\th.jpg");
        //    //b.EndInit();
        //    //img.Source = b;
        //    //img.Visibility = Visibility.Visible;
        //    //mainCanvas.Children.Add(img);
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                //tile.MoveTo(tile.X - 1, tile.Y);
                b.MoveLeft();
            }
            else if (e.Key == Key.Right)
            {
                //tile.MoveTo(tile.X + 1, tile.Y);
            }
            else if (e.Key == Key.Down)
            {
                //tile.MoveTo(tile.X, tile.Y + 1);
            }
            else if (e.Key == Key.Up)
            {
                //tile.MoveTo(tile.X, tile.Y - 1);
            }
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //int action = evil.ChooseAction(b);
            //b.TakeEvilAction(action);
            //action = evil.ChooseAction(b);
            //b.TakeEvilAction(action);
            b.TakeEvilAction(0);
            b.TakeEvilAction(2);

            b.TakeEvilAction(5);
            b.TakeEvilAction(6);

            b.TakeEvilAction(8);
            b.TakeEvilAction(9);
            b.TakeEvilAction(10);
            b.TakeEvilAction(11);

            b.TakeEvilAction(13);
            b.TakeEvilAction(14);
            b.TakeEvilAction(15);
        }
    }
}

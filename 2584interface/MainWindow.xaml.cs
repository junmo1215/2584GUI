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
        static Board b;
        static Evil evil;
        static Player player;
        //int _totalScore;
        int totalScore
        {
            set
            {
                score.Content = value;
            }
            get
            {
                return (int)score.Content;
            }
        }
        public MainWindow()
        {
            //Communicate.Config();
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (b.isMoving)
                return;

            int stepScore = -1;
            if (e.Key == Key.Q)
            {
                Direction direction = player.ChooseAction(b);
                if (direction == Direction.Null)
                    return;
                stepScore = b.Move(direction);
            }

            if (e.Key == Key.Left)
            {
                stepScore = b.Move(Direction.Left);
            }
            else if (e.Key == Key.Right)
            {
                stepScore = b.Move(Direction.Right);
            }
            else if (e.Key == Key.Down)
            {
                stepScore = b.Move(Direction.Down);
            }
            else if (e.Key == Key.Up)
            {
                stepScore = b.Move(Direction.Up);
            }

            // 这一步没有动作
            if (stepScore == -1)
            {
                b.isMoving = false;
                return;
            }

            totalScore += stepScore;
        }

        /// <summary>
        /// 动画执行结束之后的操作
        /// </summary>
        public static void OnAnimationEnd()
        {
            int action = evil.ChooseAction(b);
            b.TakeEvilAction(action);
        }

        private void newGame()
        {
            mainCanvas.Children.Clear();
            b = new Board(mainCanvas);
            evil = new Evil();
            player = new Player();

            // 界面相关字段赋初始值
            gameName.Content = App.game;
            this.Title = App.game;
            totalScore = 0;
            //score.Content = totalScore;
            tip.Content = string.Format("Join the numbers and get to the {0} tile!", App.game);

            int action = evil.ChooseAction(b);
            b.TakeEvilAction(action);
            action = evil.ChooseAction(b);
            b.TakeEvilAction(action);
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }
    }
}

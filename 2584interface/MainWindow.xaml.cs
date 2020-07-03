using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
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
using System.Windows.Threading;

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
        int today_game_time
        {
            set
            {
                game_time.Content = value;
            }
            get
            {
                return (int)game_time.Content;
            }
        }
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

            InitializeDB();
            ReadGameTime();
            StartCloseTimer();

            newGame();
        }

        // 初始化数据库
        private void InitializeDB()
        {
            if (File.Exists(App.db_path) == false)
            {
                using (var conn = new SQLiteConnection(App.db_cs))
                {
                    conn.Open();
                    string sql = "CREATE TABLE aa_game_time (date TEXT, time INTEGER)";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
            }
        }

        // 读取今日游戏时长
        private void ReadGameTime()
        {
            using (var conn = new SQLiteConnection(App.db_cs))
            {
                conn.Open();
                var cmd = new SQLiteCommand(string.Format("SELECT time FROM aa_game_time WHERE date = '{0}'", DateTime.Today), conn);
                DataTable dt = new DataTable();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    string sql = string.Format("INSERT INTO aa_game_time('date', 'time') VALUES ('{0}', 0)", DateTime.Today);
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                    today_game_time = 0;
                }
                else
                {
                    today_game_time = Convert.ToInt32(dt.Rows[0]["time"]);
                }
            }
        }

        private void UpdateGameTime()
        {
            using (var conn = new SQLiteConnection(App.db_cs))
            {
                conn.Open();
                var cmd = new SQLiteCommand(string.Format("UPDATE aa_game_time SET time = {0} WHERE date = '{1}'", today_game_time, DateTime.Today), conn);
                cmd.ExecuteNonQuery();
            }
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

        private void exit_with_msg(string str)
        {
            MessageBox.Show(this, str);
            Application.Current.Shutdown();
        }

        private void newGame()
        {
            if (today_game_time > 3600)
            {
                exit_with_msg("今天游戏时长已经超过一小时啦，不允许再玩啦！");
                return;
            }

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        // 开始防沉迷系统计时器
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += clock_tick_callback;
            timer.Start();
        }

        /// <summary>
        /// 每十秒钟检测下当前时间是否在禁止游玩时间中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clock_tick_callback(object sender, EventArgs e)
        {
            if (today_game_time % 10 == 0)
            {
                // 检测游戏时间
                DateTime start_date = DateTime.Parse(App.start_time, System.Globalization.CultureInfo.CurrentCulture);
                DateTime end_date = DateTime.Parse(App.end_time, System.Globalization.CultureInfo.CurrentCulture);

                if (DateTime.Now < end_date && DateTime.Now > start_date)
                {
                    exit_with_msg("不要玩游戏啦，快去睡觉");
                }
            }

            today_game_time += 1;
            if (today_game_time % 10 == 0)
            {
                UpdateGameTime();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _2584interface
{
    enum Direction
    {
        Null,
        Up,
        Down,
        Left,
        Right
    }

    class Action
    {
        public Tile tile;
        public int row;
        public int col;

        public Action(Tile tile, int row, int col)
        {
            this.tile = tile;
            this.row = row;
            this.col = col;
        }
    }

    class Board
    {
        const int POSSIBLE_INDEX = 32;
        const int SIZE = 4;
        private Canvas canvas;

        // 记录动画是否结束
        public bool isMoving = false;

        readonly int[] fibonacci = {
        0, 1, 2, 3, 5, 8, 13, 21,
        34, 55, 89, 144, 233, 377, 610, 987,
        1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368,
        75025, 121393, 196418, 317811,
        514229, 832040, 1346269, 2178309};

        public Tile[,] board;
        Random rnd = new Random();

        /// <summary>
        /// 初始化board，记录盘面的信息和动画相关内容
        /// 盘面信息是一个 4 * 4的数组
        /// 如果某个位置没有内容，则填null
        /// </summary>
        /// <param name="canvas"></param>
        public Board(Canvas canvas)
        {
            this.canvas = canvas;
            board = new Tile[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    board[i, j] = null;
                }
            }
            Tile.SetBoardAndCanvas(this, canvas);
        }

        /// <summary>
        /// 在指定位置生成tile
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public int TakeEvilAction(int action)
        {
            if (action == -1 || action >= SIZE * SIZE)
                throw new ArgumentOutOfRangeException("action", "action can't be -1");
            
            int row = action / 4;
            int col = action % 4;
            if (board[row, col] != null)
                throw new Exception("can't move");

            // 设置生成的概率
            if (rnd.Next(0, 100) > 75)
                board[row, col] = new Tile(row, col, 2);
            else
                board[row, col] = new Tile(row, col, 1);
            canvas.Children.Add(board[row, col].img);
            return 0;
        }

        /// <summary>
        /// 根据tile上的数字判断能否合并
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="hold"></param>
        /// <returns></returns>
        bool CanCombine(int tile, int hold)
        {
            if (App.game == "2584")
                return (tile == 1 && hold == 1) || (tile - hold) == 1 || (tile - hold) == -1;
            else
                return tile == hold;
        }
        
        /// <summary>
        /// 通过在线中的位置获取在盘面中的index
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        int PositionToIndex(int begin, int end, int position)
        {
            return (end - begin) / 4 * position + begin;
        }

        List<Action> MergeList(List<Action> list1, List<Action> list2)
        {
            if (list2 == null) return list1;
            if (list1 == null) return list2;
            List<Action> totalList = new List<Action>(list1.Count + list2.Count);
            totalList.AddRange(list1);
            totalList.AddRange(list2);
            return totalList;
        }

        Hashtable MergeActionsTable(Hashtable newActions, Hashtable oldActions)
        {
            foreach (string str in new string[]{ "move", "new", "remove"})
            {
                oldActions[str] = MergeList(newActions[str] as List<Action>, oldActions[str] as List<Action>);
            }
            return oldActions;
        }

        /// <summary>
        /// 合并某一条直线上的tile
        /// </summary>
        /// <param name="begin">起始位置，这个点在直线上</param>
        /// <param name="end">终点位置，这个点不包含在直线上</param>
        /// <param name="row">tile对应的index数组</param>
        /// <param name="score">这次合并产生的分数</param>
        /// <returns></returns>
        Hashtable CombineLine(int begin, int end, int[] row, out int score)
        {
            // 记录各种动画
            Hashtable actions = new Hashtable();

            int top = 0, hold = 0;
            int hold_position = 0;
            score = 0;
            int tile_index, top_index, hold_index;
            for (int c = 0; c < SIZE; c++)
            {
                int tile = row[c];
                if (tile == 0) continue;
                row[c] = 0;
                if (hold != 0)
                {
                    if (CanCombine(tile, hold))
                    {
                        tile_index = PositionToIndex(begin, end, c);
                        top_index = PositionToIndex(begin, end, top);
                        actions = MergeActionsTable(TileMoveTo(tile_index, top_index), actions);
                        tile = (tile > hold) ? tile : hold;
                        row[top++] = ++tile;

                        // 2048和2584算分方式不同
                        if (App.game == "2584")
                            score += (fibonacci[tile]);
                        else
                            score += (1 << tile);
                        hold = 0;
                    }
                    else
                    {
                        hold_index = PositionToIndex(begin, end, hold_position);
                        top_index = PositionToIndex(begin, end, top);
                        actions = MergeActionsTable(TileMoveTo(hold_index, top_index), actions);

                        row[top++] = hold;
                        hold = tile;
                        hold_position = c;
                    }
                }
                else
                {
                    hold = tile;
                    hold_position = c;
                }
            }
            if (hold != 0)
            {
                if (hold_position != top)
                {
                    tile_index = PositionToIndex(begin, end, hold_position);
                    top_index = PositionToIndex(begin, end, top);
                    actions = MergeActionsTable(TileMoveTo(tile_index, top_index), actions);
                }

                row[top] = hold;
            }

            return actions;
        }

        int GetStep(int begin, int end)
        {
            int result = end - begin;
            if (result >= 4)
                result = 4;
            else if (result > 0)
                result = 1;
            else if (result == 0)
                result = 0;
            else if (result > -4)
                result = -1;
            else
                result = -4;
            return result;
        }

        /// <summary>
        /// 根据盘面上合并这一步的信息，记录需要执行的动画
        /// 执行的动画分为三个部分：
        ///     1. 移动 moveAction，将tile从一个位置移动到另一个位置
        ///     2. 移除 removeAction，从画布中拿掉tile
        ///     3. 新增 newAction，在画布指定位置新增tile
        /// 
        /// 移动过程中，如果路线有其他的tile，需要把相关的tile都清理掉
        /// 移动结束后在终点产生一个新的tile
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Hashtable TileMoveTo(int begin, int end)
        {
            //HashTable<string, List<Action>> actions = new HashSet<>
            Hashtable actions = new Hashtable();
            List<Action> moveAction = new List<Action>();
            List<Action> removeAction = new List<Action>();
            List<Action> newAction = new List<Action>();
            
            int begin_row = begin / SIZE, begin_col = begin % SIZE;
            int end_row = end / SIZE, end_col = end % SIZE;

            Tile beginTile = board[begin_row, begin_col];
            //beginTile.MoveTo(end_row, end_col);
            moveAction.Add(new Action(beginTile, end_row, end_col));

            // 用来找路径中的下一个位置
            int step_col = GetStep(begin_col, end_col);
            int step_row = GetStep(begin_row, end_row);

            if (begin_col == end_col)
                step_col = 0;
            if (begin_row == end_row)
                step_row = 0;

            // 遍历经过的点，查看路线中是否有其他的tile
            List<int> numInPath = new List<int>(2);
            for (int i = end_row, j = end_col; i != begin_row || j != begin_col; i -= step_row, j -= step_col)
            {
                Tile tmpTile = board[i, j];
                if (tmpTile == null) continue;

                numInPath.Add(tmpTile.index);

                board[i, j] = null;
                moveAction.Add(new Action(tmpTile, end_row, end_col));
                removeAction.Add(new Action(tmpTile, end_row, end_col));
            }

            int new_index;
            if (numInPath.Count == 0)
            {
                // 没有其他tile，只需要把自身移动到终点位置
                board[begin_row, begin_col] = null;
                board[end_row, end_col] = beginTile;
            }
            else if (numInPath.Count == 1)
            {
                // 路线上有其他tile，在CombineLine函数中能保证这个tile是能够跟当前tile合并的
                // 直接将两个tile都执行移动到终点的动画，然后清理掉这两个旧的tile，产生一个新的

                // 新tile的index
                new_index = numInPath[0];
                if (new_index <= beginTile.index)
                    new_index = beginTile.index;
                new_index++;

                board[begin_row, begin_col] = null;
                removeAction.Add(new Action(beginTile, begin_row, begin_col));

                Tile new_tile = new Tile(end_row, end_col, new_index);

                board[end_row, end_col] = new_tile;
                newAction.Add(new Action(new_tile, end_row, end_col));
            }
            else
            {
                throw new Exception("count of tile in one move path can't greater then 1");
            }

            actions.Add("move", moveAction);
            actions.Add("new", newAction);
            actions.Add("remove", removeAction);

            return actions;
        }

        /// <summary>
        /// 返回board上index的数组
        /// </summary>
        /// <returns></returns>
        private int[,] BoardValue()
        {
            int[,] result = new int[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (board[i, j] == null)
                        result[i, j] = 0;
                    else
                        result[i, j] = board[i, j].index;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行移动指令
        /// </summary>
        /// <param name="direction">需要移动的方向</param>
        /// <returns>本次移动的得分，-1表示无法移动</returns>
        public int Move(Direction direction)
        {
            isMoving = true;
            Hashtable actions = new Hashtable();
            int totalScore = 0;
            bool isMoved = false;
            int[,] oldValue = BoardValue();
            int boardBackup = board.GetHashCode();
            for (int i = 0; i < SIZE; i++)
            {
                int[] row = new int[SIZE];
                for (int j = 0; j < SIZE; j++)
                {
                    Tile tmpTile;
                    if (direction == Direction.Up)
                        tmpTile = board[j, i];
                    else if (direction == Direction.Down)
                        tmpTile = board[SIZE - j - 1, i];
                    else if (direction == Direction.Left)
                        tmpTile = board[i, j];
                    else if (direction == Direction.Right)
                        tmpTile = board[i, SIZE - j - 1];
                    else
                        throw new ArgumentException();

                    row[j] = tmpTile == null ? 0 : tmpTile.index;
                }

                // 设置每条直线的起点和终点
                int begin, end;
                if (direction == Direction.Up)
                {
                    begin = i;
                    end = i + SIZE * SIZE;
                }
                else if (direction == Direction.Down)
                {
                    begin = i + (SIZE - 1) * SIZE;
                    end = i - SIZE;
                }
                else if (direction == Direction.Left)
                {
                    begin = i * SIZE;
                    end = i * SIZE + SIZE;
                }
                else if (direction == Direction.Right)
                {
                    begin = (i + 1) * SIZE - 1;
                    end = i * SIZE - 1;
                }
                else
                    throw new ArgumentException();

                int rowScore;
                actions = MergeActionsTable(CombineLine(begin, end, row, out rowScore), actions);
                totalScore += rowScore;
            }
            
            PlayActions(actions);

            int[,] newValue = BoardValue();
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (oldValue[i, j] != newValue[i, j])
                        isMoved = true;
                }
            }

            return isMoved ? totalScore : -1;
        }

        /// <summary>
        /// 开始执行动画
        /// </summary>
        /// <param name="actions"></param>
        void PlayActions(Hashtable actions)
        {
            List<Action> moveAction = actions["move"] as List<Action>;

            if (moveAction == null || moveAction.Count == 0)
                return;

            int length = moveAction.Count;

            // flag所有位都为1
            Tile.Flag = 1;
            for (int i = 0; i < length - 1; i++)
            {
                Tile.Flag <<= 1;
                Tile.Flag += 1;
            }

            Action action;
            for (int i = 0; i < length; i++)
            {
                action = moveAction[i];
                action.tile.MoveTo(action.row, action.col, i, actions);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < SIZE; i++)
            {
                for(int j = 0; j < SIZE; j++)
                {
                    sb.Append(board[i, j].index + ", ");
                }
            }
            return sb.ToString();
        }
    }
}

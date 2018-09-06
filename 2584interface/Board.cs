using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _2584interface
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Board
    {
        const int POSSIBLE_INDEX = 32;
        const int ROW = 4;
        const int COL = 4;
        const int SIZE = 4;
        private Canvas canvas;

        readonly int[] fibonacci = {
        0, 1, 2, 3, 5, 8, 13, 21,
        34, 55, 89, 144, 233, 377, 610, 987,
        1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368,
        75025, 121393, 196418, 317811,
        514229, 832040, 1346269, 2178309};

        public Tile[,] board;
        Random rnd = new Random();


        public Board(Canvas canvas)
        {
            this.canvas = canvas;
            board = new Tile[ROW, COL];
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    board[i, j] = null;
                }
            }
        }

        public int TakeEvilAction(int action)
        {
            if (action == -1 || action >= ROW * COL)
                throw new ArgumentOutOfRangeException("action", "action can't be -1");
            
            int row = action / 4;
            int col = action % 4;
            if (board[row, col] != null)
                throw new Exception("can't move");
            if (rnd.Next(0, 100) > 75)
                board[row, col] = new Tile(row, col, 2);
            else
                board[row, col] = new Tile(row, col, 1);
            canvas.Children.Add(board[row, col].img);
            return 0;
        }

        public int TakePlayerAction(int action)
        {
            switch (action)
            {
                case 0:
                    return MoveUp();
                case 1:
                    return MoveDown();
                case 2:
                    return MoveLeft();
                case 3:
                    return MoveRight();
                default:
                    throw new ArgumentOutOfRangeException("action");
            }
        }

        bool CanCombine(int tile, int hold)
        {
            return (tile == 1 && hold == 1) || (tile - hold) == 1 || (tile - hold) == -1;
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

        bool CombineLine(int begin, int end, int[] row, out int score)
        {
            int top = 0, hold = 0;
            int hold_position = 0;
            score = 0;
            bool moved = false;
            int tile_index, top_index, hold_index;
            for (int c = 0; c < COL; c++)
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
                        //Console.WriteLine(string.Format("tile {0} {1}move to {2} {3}", 0, hold, 0, c));
                        TileMoveTo(tile_index, top_index);
                        tile = (tile > hold) ? tile : hold;
                        row[top++] = ++tile;
                        score += (fibonacci[tile]);
                        hold = 0;
                        moved = true;
                    }
                    else
                    {
                        hold_index = PositionToIndex(begin, end, hold_position);
                        top_index = PositionToIndex(begin, end, top);
                        TileMoveTo(hold_index, top_index);

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
                    TileMoveTo(tile_index, top_index);
                    moved = true;
                }

                row[top] = hold;
            }

            return moved;
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

        //private void MoveTiles(Tile tile, int x, int y)
        //{
            
        //}

        private void TileMoveTo(int begin, int end)
        {
            int begin_row = begin / ROW, begin_col = begin % COL;
            int end_row = end / ROW, end_col = end % COL;

            Tile beginTile = board[begin_row, begin_col];
            beginTile.MoveTo(end_row, end_col);

            int step_col = GetStep(begin_col, end_col);
            int step_row = GetStep(begin_row, end_row);

            if (begin_col == end_col)
                step_col = 0;
            if (begin_row == end_row)
                step_row = 0;

            List<int> numInPath = new List<int>(2);
            for (int i = end_row, j = end_col; i != begin_row || j != begin_col; i -= step_row, j -= step_col)
            {
                Tile tmpTile = board[i, j];
                if (tmpTile == null) continue;

                numInPath.Add(tmpTile.index);
                //tmpTile.Remove();
                //var img = tmpTile.img;
                //WaitForAnimation(tmpTile);
                
                canvas.Children.Remove(tmpTile.img);
                board[i, j] = null;
            }

            int new_index;
            if (numInPath.Count == 0)
            {
                board[begin_row, begin_col] = null;
                board[end_row, end_col] = beginTile;
            }
            else if (numInPath.Count == 1)
            {
                new_index = numInPath[0];
                if (new_index <= beginTile.index)
                    new_index = beginTile.index;
                new_index++;

                // 采取先删掉再增加一个的方式
                //System.Threading.Thread.Sleep(100);
                //WaitForAnimation(beginTile);
                //while (beginTile.animationEnd == false)
                //{
                //    System.Threading.Thread.Sleep(100);
                //}
                canvas.Children.Remove(beginTile.img);
                board[begin_row, begin_col] = null;

                Tile new_tile = new Tile(end_row, end_col, new_index);
                board[end_row, end_col] = new_tile;
                canvas.Children.Add(new_tile.img);

                //canvas.Children.Remove(beginTile.img);
                //beginTile.SetImage(new_index);
                //board[begin_row, begin_col] = null;
                //board[end_row, end_col] = beginTile;
                //canvas.Children.Add(beginTile.img);


            }
            else
            {
                throw new Exception("count of tile in one move path can't greater then 1");
            }

            //Tile new_tile = new Tile(end_col, end_row, new_index);
            //board[end_col, end_row] = new_tile;
            //canvas.Children.Add(new_tile.img);
        }

        void WaitForAnimation(Tile tile)
        {
            while (tile.animationEnd == false) ;
        }

        public int Move(Direction direction)
        {
            int totalScore = 0;
            bool isMoved = false;
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
                    end = i * COL - 1;
                }
                else
                    throw new ArgumentException();

                bool moved = CombineLine(begin, end, row, out int rowScore);
                totalScore += rowScore;
                if (moved)
                {
                    isMoved = true;
                }
            }
            return isMoved ? totalScore : -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ROW; i++)
            {
                for(int j = 0; j < COL; j++)
                {
                    sb.Append(board[i, j].index + ", ");
                }
            }
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _2584interface
{
    class Board
    {
        const int POSSIBLE_INDEX = 32;
        const int ROW = 4;
        const int COL = 4;
        private Canvas canvas;

        readonly int[] fibonacci = {
        0, 1, 2, 3, 5, 8, 13, 21,
        34, 55, 89, 144, 233, 377, 610, 987,
        1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368,
        75025, 121393, 196418, 317811,
        514229, 832040, 1346269, 2178309};

        public Tile[,] board;

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
            
            int row = action % 4;
            int col = action / 4;
            if (board[row, col] != null)
                throw new Exception("can't move");

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

        //public int MoveLeft()
        //{
        //    Tile[,] prev = board;
        //    int score = 0;
        //    for (int r = 0; r < 4; r++)
        //    {
        //        Tile[] row =
        //        {
        //            board[r, 0], board[r, 1], board[r, 2], board[r, 3]
        //        };
        //        int top = 0;
        //        int hold = 0;
        //        for (int c = 0; c < 4; c++)
        //        {
        //            Tile tile = row[c];
        //            if (tile == null) continue;
        //            row[c] = null;
        //            if (hold != 0)
        //            {
        //                if (CanCombine(tile.index, hold))
        //                {
        //                    int newIndex = (tile.index > hold) ? tile.index : hold;
        //                    newIndex++;
        //                    row[top++] = new Tile(r, c, newIndex);
        //                    score += fibonacci[newIndex];
        //                    hold = 0;
        //                }
        //                else
        //                {
        //                    row[top++] = hold;
        //                }
        //            }
        //            else
        //            {
        //                hold = tile.index;
        //            }
        //        }
        //    }
        //    return -1;
        //}

        bool CombineLine(int begin_index, int end_index, int[] row, out int score)
        {
            int top = 0, hold = 0;
            score = 0;
            bool moved = false;
            for (int c = 0; c < COL; c++)
            {
                int tile = row[c];
                if (tile == 0) continue;
                row[c] = 0;
                if (hold != 0)
                {
                    if (CanCombine(tile, hold))
                    {
                        tile = (tile > hold) ? tile : hold;
                        row[top++] = ++tile;
                        score += (fibonacci[tile]);
                        hold = 0;
                        //Console.WriteLine(string.Format("tile {0} {1}move to {2} {3}", 0, hold, 0, c));
                    }
                    else
                    {
                        row[top++] = hold;
                        hold = tile;
                    }
                    moved = true;
                }
                else
                {
                    hold = tile;
                }
            }
            if (hold != 0)
                row[top] = hold;

            return moved;
        }

        public int MoveLeft()
        {
            int totalScore = 0;
            bool isMoved = false;
            for(int r = 0; r < ROW; r++)
            {
                //Tile[] row =
                int[] row = new int[COL];
                for (int i = 0; i < COL; i++)
                {
                    if (board[i, r] == null)
                    {
                        row[i] = 0;
                    }
                    else
                    {
                        row[i] = board[i, r].index;
                    }
                }
                //List<int> row = new List<int> {
                //    board[r, 0].index, board[r, 1].index, board[r, 2].index, board[r, 3].index
                //};
                bool moved = CombineLine(r * COL, r * COL + COL, row, out int rowScore);
                totalScore += rowScore;
                if (moved)
                {
                    isMoved = true;
                }
            }
            return isMoved ? totalScore : -1;
        }

        int MoveUp()
        {
            return -1;
        }

        int MoveDown()
        {
            return -1;
        }

        int MoveRight()
        {
            return -1;
        }
    }
}

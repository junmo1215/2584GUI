using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2584interface
{
    class Evil
    {
        private Random rnd = new Random(0);

        public Evil()
        {

        }

        public int ChooseAction(Board b)
        {
            List<int> availableIndex = new List<int>(16);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (b.board[i, j] == null)
                    {
                        availableIndex.Add(i * 4 + j);
                    }
                }
            }
            int randIndex = rnd.Next(0, availableIndex.Count - 1);
            return availableIndex[randIndex];
        }

    }
}

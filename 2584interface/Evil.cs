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
            return rnd.Next(0, 16);
        }

    }
}

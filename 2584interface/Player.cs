using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2584interface
{
    class Player
    {
        public Player()
        {

        }

        /// <summary>
        /// 向配置的服务器发送当前盘面信息
        /// 获取选择的步骤
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Direction ChooseAction(Board b)
        {

            return Direction.Null;
        }
    }
}

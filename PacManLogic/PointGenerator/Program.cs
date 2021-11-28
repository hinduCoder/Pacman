using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int x;
            var result = "";
            do 
            {
                var s = Console.ReadLine();
                var ss = s.Split(' ');
                 x = int.Parse(ss[0]);
                var y = int.Parse(ss[1]);
                result += string.Format("new Point({0}, {1}), ", x, y);
            } while (x != -1);
            Console.Clear();
            Console.WriteLine(result);
        }
    }
}

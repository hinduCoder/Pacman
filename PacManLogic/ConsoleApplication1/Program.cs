using PacManLogic.Helpers;
using System;
using System.Collections.Generic;
using PacManLogic.Heroes;
using System.Diagnostics;
using System.IO;
using PacManLogic.GameField;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var sr = new StreamReader(@"C:\Users\Дом\Documents\GitHub\Pacman1\PacManLogic\PacManLogic\Map.mp");
            var s = sr.ReadToEnd();
            int c = 0;
            foreach (var ch in s)
            {
                if (ch == '3')
                    c++;
            }
            Console.WriteLine(c);
        }

        static int i;
        static void blinky_PositionChanged(object sender, EventArgs e)
        {
            var way = new List<Point>(){new Point(9, 10), new Point(8, 10), new Point(7, 9), new Point(6, 9), new Point(
5, 9), new Point(5, 8), new Point(5, 7), new Point(4, 7), new Point(3, 7), new Point(3, 8), new Point(-1, -1) };
            Debug.WriteLine(((Blinky)sender).Position);
       //     Assert.AreEqual(way[i], ((Blinky)sender).Position);
        }
    }
}

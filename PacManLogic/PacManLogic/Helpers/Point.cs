using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacManLogic.Helpers
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        public static bool operator== (Point o1, Point o2)
        {
            return o1.X == o2.X && o2.Y == o1.Y;
        }
        public static bool operator!= (Point o1, Point o2)
        {
            return !(o1 == o2);
        }

        public Point VerticalOffset(int offset)
        {
            return new Point(X, Y + offset);
        }
        public Point HorizontalOffset(int offset)
        {
            return new Point(X + offset, Y);
        }

        public override bool Equals(object obj)
        {
            return this == (Point)obj;
        }
        public override string ToString()
        {
            return string.Format("({0}; {1})", X, Y);
        }
    }
}

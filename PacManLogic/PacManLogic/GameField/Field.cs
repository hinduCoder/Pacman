using PacManLogic.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using PacManLogic.Heroes;

namespace PacManLogic.GameField
{
    public static class Field
    {
        public static readonly int RightEdge = 20;
        public static readonly int BottomEdge = 20;
        public static readonly int LeftEdge = 0;
        public static readonly int TopEdge = 0;

        public static int DotsCount = 146;

        public static event EventHandler EnergizerAte;

        public static Matrix<PointContent> Map { get; private set; }

        static Field()
        {
            Map = ReadFromFile(@"Map.mp");
        }
        private static Matrix<PointContent> ReadFromFile(string path)
        {
            var stream = new StreamReader(path);
            var result = new Matrix<PointContent>(RightEdge + 1, BottomEdge + 1);
            using (stream)
            {
                int i = 0;
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    for (var j = 0; j < line.Length; j++)
                        result[j,i] = (PointContent)Enum.ToObject(typeof(PointContent), int.Parse(line[j].ToString()));
                    i++;
                }
            }
            return result;
        }

        static bool Tunnel(ref Point position, Direction direction)
        {
            if (position == new Point(0, 9) && direction == Direction.Left)
            {
                position = new Point(20, 9);
                return true;
            }
            if (position == new Point(20, 9) && direction == Direction.Right)
            {
                position = new Point(0, 9);
                return true;
            }
            return false;
        }
        public static bool CanLocate(Point point)
        {
            if (point == new Point(-1, 9) || point == new Point(21, 9))
                return false;
            if (point == new Point(9, 8))
                return false;
            return Map[point] != PointContent.Wall && Map[point] != PointContent.Useless && point != new Point(10, 8);
        }
        public static bool CanLocateOnePointForward(Point point, Direction direction)
        {
            if (point == new Point(0, 9) || point == new Point(20, 9))
                return true;

            if (direction == Direction.Down)
                return CanLocate(point.VerticalOffset(1));
            if (direction == Direction.Up)
                return CanLocate(point.VerticalOffset(-1));
            if (direction == Direction.Right)
                return CanLocate(point.HorizontalOffset(1));
            if (direction == Direction.Left) 
                return CanLocate(point.HorizontalOffset(-1));

            return false;
        }
        public static Point NextPointForward(Point point, Direction direction)
        {
            if (Tunnel(ref point, direction))
                return point;
            var result = new Point();
            switch (direction)
            {
                case Direction.Down: result = point.VerticalOffset(1); break;
                case Direction.Left: result = point.HorizontalOffset(-1); break;
                case Direction.Right: result = point.HorizontalOffset(1); break;
                case Direction.Up: result = point.VerticalOffset(-1); break;
            }
            return result;
        }
        public static Point NextPoint(Ghost ghost, Point target)
        {
            var position = ghost.Position;
            if (Tunnel(ref position, ghost.Direction))
                return position;

            var countOfVariants = 0;
            if (CanLocate(ghost.Position.VerticalOffset(1))) countOfVariants++;
            if (CanLocate(ghost.Position.VerticalOffset(-1))) countOfVariants++;
            if (CanLocate(ghost.Position.HorizontalOffset(1))) countOfVariants++;
            if (CanLocate(ghost.Position.HorizontalOffset(-1))) countOfVariants++;

            if (countOfVariants <= 2)
            {
                return NextStepInternal(ghost.Direction, ghost.Position);
            }

            double upLength = double.MaxValue;
            double downLength = double.MaxValue;
            double leftLength = double.MaxValue;
            double rightLength = double.MaxValue;

            var upPoint = ghost.Position.VerticalOffset(-1);
            var downPoint = ghost.Position.VerticalOffset(1);
            var leftPoint = ghost.Position.HorizontalOffset(-1);
            var rightPoint = ghost.Position.HorizontalOffset(1);
            if (ghost.Direction != Direction.Down && CanLocate(upPoint))
                upLength = Point.Distance(upPoint, target);
            if (ghost.Direction != Direction.Up && CanLocate(downPoint))
            downLength = Point.Distance(downPoint, target);
            if (ghost.Direction != Direction.Right && CanLocate(leftPoint))
                leftLength = Point.Distance(leftPoint, target);
            if (ghost.Direction != Direction.Left && CanLocate(rightPoint))
                rightLength = Point.Distance(rightPoint, target);

            var array = new[] {new Tuple<double, Point>(upLength, upPoint), 
                new Tuple<double, Point>(downLength, downPoint),  
                new Tuple<double, Point>(leftLength, leftPoint),   
                new Tuple<double, Point>(rightLength, rightPoint)};
            Array.Sort(array, delegate(Tuple<double, Point> item1, Tuple<double, Point> item2)
                { if (item1.Item1 < item2.Item1) return -1;
                 if (item1.Item1 > item2.Item1) return 1;
                 return 0;});
            if (array[0].Item1 == array[1].Item1)
            {
                if (array[0].Item1 == upLength)
                    return upPoint;
                if (array[0].Item1 == leftLength)
                    return leftPoint;
                    return downPoint;
            }
            return array[0].Item2;
        }
        public static void EatDot(Point position)
        {
            if (Map[position] == PointContent.Dot || Map[position] == PointContent.Energizer)
            {
                if (Map[position] == PointContent.Dot)
                {
                    DotsCount--;
                    Game.Score += 10;
                }
                else
                {
                    if (EnergizerAte != null)
                    EnergizerAte(null, EventArgs.Empty);
                    Game.Score += 100;
                }
                Map[position] = PointContent.Empty;
            }
        }

        private static Point NextStepInternal(Direction direction, Point position)
        {
            var forward = new Point(); 
            var toLeft = new Point();
            var toRight = new Point();
            switch (direction)
            {
                case Direction.Up:
                    forward = position.VerticalOffset(-1);
                    toLeft = position.HorizontalOffset(-1);
                    toRight = position.HorizontalOffset(1);
                    break;
                case Direction.Down:
                    forward = position.VerticalOffset(1);
                    toLeft = position.HorizontalOffset(1);
                    toRight = position.HorizontalOffset(-1);
                    break;
                case Direction.Right:
                    forward = position.HorizontalOffset(1);
                    toLeft = position.VerticalOffset(-1);
                    toRight = position.VerticalOffset(1);
                    break;
                case Direction.Left:
                    forward = position.HorizontalOffset(-1);
                    toLeft = position.VerticalOffset(1);
                    toRight = position.VerticalOffset(-1);
                    break;
            }
            return CanLocate(forward) ? forward : (CanLocate(toLeft) ? toLeft : toRight);
        }

        public static Point NextRandomPoint(Ghost ghost)
        {
            var position = ghost.Position;
            if (Tunnel(ref position, ghost.Direction))
                return position;

            var list = new List<Point>();
            if (ghost.Direction != Direction.Up && CanLocate(ghost.Position.VerticalOffset(1))) list.Add(ghost.Position.VerticalOffset(1));
            if (ghost.Direction != Direction.Down && CanLocate(ghost.Position.VerticalOffset(-1))) list.Add(ghost.Position.VerticalOffset(-1));
            if (ghost.Direction != Direction.Left && CanLocate(ghost.Position.HorizontalOffset(1))) list.Add(ghost.Position.HorizontalOffset(1));
            if (ghost.Direction != Direction.Right && CanLocate(ghost.Position.HorizontalOffset(-1))) list.Add(ghost.Position.HorizontalOffset(-1));

            if (list.Count <= 1)
            {
                return NextStepInternal(ghost.Direction, ghost.Position);
            }

            var randomizer = new Random();
            var index = randomizer.Next(0, list.Count);
            return list[index];
        }
        public static Direction GetDirection(Point prevPoint, Point curPoint)
        {
            if (prevPoint == new Point(0, 9))
                if (curPoint == new Point(20, 9))
                    return Direction.Left;
                else
                    return Direction.Right;
            if (prevPoint == new Point(20, 9))
                if (curPoint == new Point(0, 9))
                    return Direction.Right;
                else
                    return Direction.Left;

            var diffX = curPoint.X - prevPoint.X;
            var diffY = curPoint.Y - prevPoint.Y;
            if (diffY == 0)
            {
                if (diffX < 0)
                    return Direction.Left;
                return Direction.Right;
            }
            
            if (diffY < 0)
                return Direction.Up;
            return Direction.Down;
            
        }
    }
}

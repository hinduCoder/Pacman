using PacManLogic.GameField;
using PacManLogic.Helpers;
using System;

namespace PacManLogic.Heroes
{
    public class Clyde : Ghost
    {
        public Clyde(Pacman pacman) : base(pacman)
        {
        }
        public override void Move()
        {
            if (Point.Distance(Position, pacman.Position) < 9)
                Move(new Point(Field.RightEdge, Field.BottomEdge + 2));
            else
                Move(pacman.Position);
        }
        public override void SetPositionToStart()
        {
            base.SetPositionToStart();
            Position = new Point(11, 9);

            
        }
        protected override bool ExitFromHome()
        {
            if (Position == new Point(11, 9))
            {
                Direction = Direction.Left;
                Position = new Point(10, 9);
                return false;
            }
            if (Position == new Point(10, 9))
            {
                Direction = Direction.Up;
                Position = new Point(10, 8);
                return false;
            }
            if (Position == new Point(10, 8))
            {
                Position = new Point(10, 7);
                return false;
            }
            return true;
        }
    }
}

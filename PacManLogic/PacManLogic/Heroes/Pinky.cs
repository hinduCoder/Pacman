using PacManLogic.Helpers;
using System;

namespace PacManLogic.Heroes
{
    public class Pinky : Ghost
    {
        public Pinky(Pacman pacman) : base(pacman) 
        {
        }

        public override void Move()
        {
            var direction = pacman.Direction == Direction.Stop ? pacman.PreviousDirection : pacman.Direction;
            switch (direction)
            {
                case Direction.Left: Move(pacman.Position.HorizontalOffset(-4)); break;
                case Direction.Right: Move(pacman.Position.HorizontalOffset(4)); break;
                case Direction.Down: Move(pacman.Position.VerticalOffset(4)); break;
                case Direction.Up: Move(pacman.Position.HorizontalOffset(-4).VerticalOffset(-4)); break;
            }
        }
        public override void SetPositionToStart()
        {
            base.SetPositionToStart();
            Position = new Point(10, 9);
            Direction = Direction.Stop;
        }
        protected override bool ExitFromHome()
        {
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

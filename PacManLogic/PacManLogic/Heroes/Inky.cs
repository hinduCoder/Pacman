using PacManLogic.Helpers;
using System;

namespace PacManLogic.Heroes
{
    public class Inky : Ghost
    {
        private Blinky blinky;

        public Inky(Pacman pacman, Blinky blinky) : base(pacman)
        {
            this.blinky = blinky;
            //SetPositionToStart();
           // Direction = Direction.Up;
        }

        public override void Move()
        {
            var oppositePoint = blinky.Position;
            var center = new Point();
            switch (pacman.Direction)
            {
                case Direction.Up:
                    center = pacman.Position.VerticalOffset(-2); break;
                case Direction.Down:
                    center = pacman.Position.VerticalOffset(2); break;
                case Direction.Left:
                    center = pacman.Position.HorizontalOffset(-2); break;
                case Direction.Right:
                    center = pacman.Position.HorizontalOffset(2); break;
            }
            var target = new Point(center.X + (center.X - oppositePoint.X), center.Y + (center.Y - oppositePoint.Y));
            Move(target);
        } //TODO
        public override void SetPositionToStart()
        {
            base.SetPositionToStart();
            Position = new Point(9, 9);

            
        }
        protected override bool ExitFromHome()
        {
            if (Position == new Point(9, 9))
            {
                Direction = Direction.Right;
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

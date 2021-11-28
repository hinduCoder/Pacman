using PacManLogic.Helpers;
using System;

namespace PacManLogic.Heroes
{
    public class Blinky : Ghost
    {
        public Blinky(Pacman pacman) : base(pacman)
        {

        }

        public override void Move()
        {
            Move(pacman.Position);
        } 
        public override void SetPositionToStart()
        {
            Position = new Point(10, 7);
            Direction = Direction.Up;
        }
        protected override bool ExitFromHome()
        {
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using PacManLogic.GameField;
using PacManLogic.Helpers;

namespace PacManLogic.Heroes
{
    public abstract class Ghost : Hero
    {
        protected readonly Pacman pacman;

        public Mode Mode { get; set; }

        protected Ghost(Pacman pacman)
        {
            this.pacman = pacman;
            OutOfHome = false;
            Direction = Direction.Stop;
            SetPositionToStart();
        }

        public abstract void Move();
        protected abstract bool ExitFromHome();

        public override void SetPositionToStart()
        {
            OutOfHome = false;
            Direction = Direction.Stop;
        }

        bool OutOfHome { get; set; }

        protected void Move(Point target)
        {
            if (!OutOfHome)
            {
                if (ExitFromHome())
                    OutOfHome = true;
                else
                    return;
            }
            var prev = Position;
            if (Mode == Mode.Chase)
                Position = Field.NextPoint(this, target);
            else
                Position = Field.NextRandomPoint(this);
            Direction = Field.GetDirection(prev, Position);
        }
    }

}

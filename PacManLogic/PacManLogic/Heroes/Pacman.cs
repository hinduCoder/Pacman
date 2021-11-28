using PacManLogic.Helpers;
using PacManLogic.GameField;
using System;

namespace PacManLogic.Heroes
{
    public class Pacman : Hero
    {
        public Pacman()
        {
            SetPositionToStart();
        }

        public Direction PreviousDirection { get; private set; }

        public void Turn(Direction direction)
        {
            if (direction != Direction && Field.CanLocateOnePointForward(Position, direction))
            {
                PreviousDirection = Direction = direction;
            }
        }

        public void MoveForward()
        {
            if (Field.CanLocateOnePointForward(Position, Direction))
            {
                Position = Field.NextPointForward(Position, Direction);
                Field.EatDot(Position);
                
            }
            else
            {
                Direction = Direction.Stop;
            }
        }
        public override void SetPositionToStart()
        {
            Position = new Point(10, 15);
            Direction = Direction.Stop;
            PreviousDirection = Direction.Up;
        }
    }
}

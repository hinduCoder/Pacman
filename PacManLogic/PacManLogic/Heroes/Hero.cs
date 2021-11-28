using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacManLogic.Helpers;

namespace PacManLogic.Heroes
{
    public abstract class Hero
    {
        public Point Position { get; protected set; }
        public Direction Direction { get; protected set; }

        public abstract void SetPositionToStart();
    }

}

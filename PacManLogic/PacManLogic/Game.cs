using System;
using System.Collections.Generic;
using System.Threading;
using PacManLogic.GameField;
using PacManLogic.Helpers;
using PacManLogic.Heroes;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace PacManLogic
{
    public static class Game
    {
        public static readonly double Delay = 400;

        public static Mode GhostsMode { get; private set; }
        public static GameStatus Status { get; private set; }

        public static Timer Timer { get; private set; }
        private static Stopwatch stopwatch;
        private static TimeSpan frightModeStart;

        public static Pacman Pacman { get; private set; }
        public static Ghost[] Ghosts = new Ghost[4];

        public static int Lifes { get; private set; }
        public static int Score { get; internal set; }

        static Game()
        {
            Timer = new System.Timers.Timer(Delay / 4);
        }
        public static void Init()
        {
            Pacman = new Pacman();
            Ghosts[0] = new Blinky(Pacman);
            Ghosts[1] = new Pinky(Pacman);
            Ghosts[2] = new Inky(Pacman, (Blinky)Ghosts[0]);
            Ghosts[3] = new Clyde(Pacman);

            GhostsMode = Mode.Chase;

            Lifes = 3;

            Field.EnergizerAte += delegate
                {
                frightModeStart = stopwatch.Elapsed;
                GhostsMode = Mode.Fright;
                for (int index = 0; index < Ghosts.Length; index++)
                {
                    Ghosts[index].Mode = Mode.Fright;
                }
            };

            Timer.Elapsed += Timer_Elapsed;
            Status = GameStatus.Running;
            
            stopwatch = new Stopwatch();
            Timer.Start();
            stopwatch.Start(); 
        }
        public static void TurnPacman(Direction direction)
        {
            Pacman.Turn(direction);
        }

        private static int timerCycle = 0;
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timerCycle == 4)
            {
                MainLoop();
                timerCycle = 0;
            }
            timerCycle++;
        }
        private static void MainLoop()
        {
            if (GhostsMode == Mode.Fright && (stopwatch.Elapsed - frightModeStart).Seconds > 10)
            {
                for (int index = 0; index < Ghosts.Length; index++)
                {
                    Ghosts[index].Mode = Mode.Chase;
                }
                GhostsMode = Mode.Chase;
            }

            foreach (var ghost in Ghosts)
            {
                MoveGhost(ghost);
            }
            Pacman.MoveForward();
            CheckGameOver();
        }
        private static void MoveGhost(Ghost ghost)
        {
            if (ghost is Inky && stopwatch.Elapsed.Seconds < 15 || ghost is Clyde && stopwatch.Elapsed.Seconds < 20)
                return;
            ghost.Move();
        }
        private static bool IsKilled(Ghost ghost)
        {
            return ghost.Position == Pacman.Position || Point.Distance(Pacman.Position, ghost.Position) == 1.0 &&
                   (
                       Pacman.Position.X > ghost.Position.X && Pacman.Direction == Direction.Right &&
                       ghost.Direction == Direction.Left ||
                       Pacman.Position.X < ghost.Position.X && Pacman.Direction == Direction.Left &&
                       ghost.Direction == Direction.Right ||
                       Pacman.Position.Y > ghost.Position.Y && Pacman.Direction == Direction.Down &&
                       ghost.Direction == Direction.Up ||
                       Pacman.Position.Y < ghost.Position.Y && Pacman.Direction == Direction.Up &&
                       ghost.Direction == Direction.Down
                   ); 
        }
        private static void CheckGameOver()
        {
            foreach (var ghost in Ghosts)
            {
                if (IsKilled(ghost))
                {
                    if (GhostsMode == Mode.Fright)
                    {
                        Timer.Stop();
                        stopwatch.Stop();
                        Thread.Sleep(1000);
                        ghost.SetPositionToStart();
                        Thread.Sleep(1000);
                        Timer.Start();
                        stopwatch.Start();
                    }
                    else
                    {
                       
                        Timer.Stop();
                        Thread.Sleep(500);
                        Lifes--;
                        if (Lifes == 0)
                        {
                            Status = GameStatus.Lost;
                            Timer.Elapsed -= Timer_Elapsed;
                            return;
                        }

                        Pacman.SetPositionToStart();
                        foreach (var g in Ghosts)
                        {
                            g.SetPositionToStart();
                        }
                        Thread.Sleep(2000); //TODO
                        stopwatch.Restart();
                        Timer.Start();
                    }
                }
            }
            if (Field.DotsCount == 0)
            {
                Status = GameStatus.Won;
            }
        }
    }
    public enum GameStatus
    {
        NotStarted, Running, Won, Lost 
    }
}

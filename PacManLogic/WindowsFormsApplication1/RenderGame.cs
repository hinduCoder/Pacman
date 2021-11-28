using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using PacManLogic;
using PacManLogic.GameField;
using PacManLogic.Heroes;
using myPoint = PacManLogic.Helpers.Point;
using System.IO;

namespace WindowsFormsApplication1
{
    class RenderGame
    {
        public static readonly int RenderInterval = 10;

        private Graphics graphics;
        private SolidBrush dotBrush = new SolidBrush(Color.Yellow);
        private SolidBrush wallBrush = new SolidBrush(Color.Blue);
        private SolidBrush rectBrush = new SolidBrush(Color.Black);
        private readonly int RectSize = 20;

        public RenderGame()
        {
            Game.Timer.Elapsed += (sender, args) =>
                {
                    if (shift == 0)
                    {
                        shift = -20;
                    }
                    shift += 5;
                };
        }

        public void Render(Graphics graphics)
        {
            this.graphics = graphics;
            switch (Game.Status)
            {
                case GameStatus.NotStarted: RenderStartScreen(); break;
                case GameStatus.Running: RenderField(); break;
                default: RenderFinishScreen(); break;
            }
        }

        void RenderStartScreen()
        {
            graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, 500 , 500);
            graphics.DrawString("Для начала игры \nнажмите \"пробел\" \nДля выхода - Esc",
                new Font(new FontFamily("Times new roman"), 20, FontStyle.Bold), new SolidBrush(Color.Yellow), 90, 170);
        }
        void RenderFinishScreen()
        {
            graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, 500, 500);
            graphics.DrawString(Game.Status == GameStatus.Won ? ":-)" : ":-(",
                new Font(new FontFamily("Times new roman"), 50, FontStyle.Bold), new SolidBrush(Color.Yellow), 70, 100);
            graphics.DrawString(Game.Score.ToString(), new Font(new FontFamily("Times new roman"), 50, FontStyle.Bold), new SolidBrush(Color.Yellow), 240, 100);
            graphics.DrawString("  Чтобы сыграть еще раз, \n      нажмите \"пробел\" \nЧтобы выйти, нажмите Esc", new Font(new FontFamily("Times new roman"), 20, FontStyle.Bold), new SolidBrush(Color.Yellow), 30, 250);
        }
        void RenderField()
        {
            DrawField();
            DrawPacman(Game.Pacman.Position, Game.Pacman.Direction);
            DrawGhost(GhostColor.Red, Game.Ghosts[0]);
            DrawGhost(GhostColor.Pink, Game.Ghosts[1]);
            DrawGhost(GhostColor.Blue, Game.Ghosts[2]);
            DrawGhost(GhostColor.Orange, Game.Ghosts[3]);
        }

        public void OnSpaceDown()
        {
            if (Game.Status != GameStatus.Running)
                try
                {
                    Game.Init();
                }
                catch (TypeInitializationException e)
                {
                    if (e.InnerException is FileNotFoundException)
                    {
                        MessageBox.Show("Поместите файл Map.mp рядом с файлом \nPacman.exe и повторите попытку",
                                        "Файл не найден");
                        Environment.Exit(1);
                    }
                    throw;
                }
        }
        public void OnArrowDown(Direction direction)
        {
            if (Game.Status == GameStatus.Running)
                Game.TurnPacman(direction);
        }
        public void OnEscapeDown()
        {
            if (Game.Status != GameStatus.Running)
                Environment.Exit(0);
        }

        void DrawDot(myPoint position)
        {
            graphics.FillEllipse(dotBrush, RectSize * position.X + (RectSize / 2 - 2), RectSize * position.Y + (RectSize / 2 - 2), 2, 2);
        }
        void DrawEnergizer(myPoint position)
        {
            graphics.FillEllipse(dotBrush, RectSize * position.X, RectSize * position.Y, RectSize - 2, RectSize - 2);
        }
        void DrawWall(myPoint position)
        {
            graphics.FillRectangle(wallBrush, RectSize * position.X, RectSize * position.Y, RectSize, RectSize);
        }
        void DrawRect(myPoint position)
        {
            graphics.FillRectangle(rectBrush, RectSize * position.X, RectSize * position.Y, RectSize, RectSize);
        }
        void DrawField()
        {
            for (int j = Field.TopEdge; j <= Field.BottomEdge; j++)
            {
                for (int i = Field.LeftEdge; i <= Field.RightEdge; i++)
                {
                    DrawRect(new myPoint(i, j));
                    if (Field.Map[i, j] == PointContent.Wall)
                        DrawWall(new myPoint(i, j));
                    else if (Field.Map[i, j] == PointContent.Dot)
                        DrawDot(new myPoint(i, j));
                    else if (Field.Map[i, j] == PointContent.Energizer)
                        DrawEnergizer(new myPoint(i, j));

                }
            }
          
            DrawStatusBar();
        }
        void DrawStatusBar()
        {
            for (int i = Field.LeftEdge; i <= Field.RightEdge; i++)
            {
                DrawRect(new myPoint(i, Field.BottomEdge + 1));
            }
            if (Game.Lifes >= 1)
                DrawPacman(new myPoint(1, Field.BottomEdge + 1), Direction.Left, true);
            if (Game.Lifes >= 2)
                DrawPacman(new myPoint(2, Field.BottomEdge + 1), Direction.Left, true);
            if (Game.Lifes == 3)
                DrawPacman(new myPoint(3, Field.BottomEdge + 1), Direction.Left, true);
            graphics.DrawString(Game.Score.ToString(), new Font("times new roman", 14, FontStyle.Bold), new SolidBrush(Color.White), 18 * RectSize, RectSize * (Field.BottomEdge + 1));
        }

        private Direction oldDirection;
        void DrawPacman(myPoint position, Direction direction, bool image = false)
        {
            var p = image || direction == Direction.Stop || oldDirection != direction ? new Point(RectSize * position.X, RectSize * position.Y) : GetRealPosition(position, direction);
            graphics.FillEllipse(new SolidBrush(Color.Yellow), p.X, p.Y, RectSize, RectSize);
            int startAngle = 0;
            switch (direction)
            {
                case Direction.Up: startAngle = -120; break;
                case Direction.Down: startAngle = 60; break;
                case Direction.Left: startAngle = 150; break;
                case Direction.Right: startAngle = -30; break;
            }
            if (direction != Direction.Stop)
                graphics.FillPie(new SolidBrush(Color.Black), p.X, p.Y, 20, 20, startAngle, 60);
            if (!image)
                oldDirection = direction;
        }
        void DrawGhost(GhostColor color, Ghost ghost)
        {
            Brush brush = new SolidBrush(Color.Transparent);
            Bitmap bitmap = null;
            var resources = new ResourceManager(typeof (MainForm));
            if (Game.GhostsMode == Mode.Chase)
                switch (color)
                {
                    case GhostColor.Blue:
                        bitmap = (Bitmap)resources.GetObject("inky"); break; 
                    case GhostColor.Red:
                        bitmap = (Bitmap)resources.GetObject("blinky"); break; 
                    case GhostColor.Orange:
                        bitmap = (Bitmap)resources.GetObject("clyde"); break; 
                    case GhostColor.Pink:
                        bitmap = (Bitmap)resources.GetObject("pinky"); break;
                } 
            else
            {
                bitmap = (Bitmap) resources.GetObject("ghost");
            }

            var position = ghost.Position;
            var p = GetRealPosition(position, ghost.Direction);
            graphics.DrawImage(bitmap, p.X, p.Y, RectSize, RectSize);
        }
        private int shift;
        private Point GetRealPosition(myPoint position, Direction direction)
        {
            int sx = 0, sy = 0;
            switch (direction)
            {
                case Direction.Down: sy = shift; break;
                case Direction.Up: sy = -shift; break;
                case Direction.Left: sx = -shift; break;
                case Direction.Right: sx = shift; break;
            }
            return new System.Drawing.Point(position.X * 20 + sx, position.Y * 20 + sy);
        }

        
    }
    enum GhostColor
    {
        Red, Pink, Blue, Orange
    }
}

using System;
using System.Windows.Forms;

using PacManLogic.Heroes;
using PacManLogic.GameField;
using PacManLogic;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
        private readonly RenderGame render;
        public MainForm()
        {
            InitializeComponent();
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint , true);
            var timer = new Timer { Interval = 100 };
            timer.Tick += (o, e) => Refresh();
            timer.Start();

            render = new RenderGame();
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                render.OnSpaceDown();
            if (e.KeyCode == Keys.Escape)
                render.OnEscapeDown();
            var direction = Direction.Up;
            switch (e.KeyCode)
            {
                case Keys.Up: direction = Direction.Up; break;
                case Keys.Down: direction = Direction.Down; break;
                case Keys.Left: direction = Direction.Left; break;
                case Keys.Right: direction = Direction.Right; break;
            }
            render.OnArrowDown(direction);
        }
        void Form1_Paint(object sender, PaintEventArgs e)
        {
            render.Render(e.Graphics);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Circle
{
    public partial class Form1 : Form
    {
        const int
            DOT_MIN_SPEED = 1,
            DOT_MAX_SPEED = 10;
        Timer
            updateTimer = new Timer();
        Random
            getRandom = new Random(DateTime.Now.Millisecond);
        Size
            Resolution = new Size(1920, 1080);
        SizeF 
            ResolutionRatio = new SizeF(1f, 1f);
        GraphicsPath
            CirclePath = new GraphicsPath();
        Rectangle
            CircleRectangle = new Rectangle(520, 100, 880, 880),
            SpeedChangerRectangle = new Rectangle(1818, 738, 100, 220);
        int SelectedBall, BallCount = 50;
        float BallSpeed = 1f;
        Region reg;
        Boolean
            Paused, AnyDotSelected, SpeedChangerSelected, CountChangerSelected;
        class Ball
        {
            public PointF Position;
            public float Speed;
            public float Direction;

            public Ball(PointF _Position, float _Speed, float _Direction)
            {
                Position = _Position;
                Speed = _Speed;
                Direction = _Direction;
            }
        }
        List<Ball> Balls = new List<Ball>();

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.BackColor = Color.Gray;
            this.Paint += new PaintEventHandler(pDraw);
            this.KeyDown += new KeyEventHandler(pKeyDown);
            //this.KeyUp += new KeyEventHandler(Program_KeyUp);
            this.MouseDown += new MouseEventHandler(pMouseDown);
            this.MouseMove += new MouseEventHandler(pMouseMove);
            this.MouseUp += new MouseEventHandler(pMouseUp);
            CirclePath.AddEllipse(CircleRectangle);
            reg = new Region(new Rectangle(1200, 400, 400, 400));
            reg.Union(CirclePath);
            reg.Xor(new Rectangle(1415, 750, 50, 50));
            for (int a = 0; a < BallCount; ++a)
                Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 17) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
            updateTimer.Interval = 1;
            updateTimer.Tick += new EventHandler(pUpdate);
            updateTimer.Start();
        }

        PointF getRandomPointInRegion(GraphicsPath _path)
        {
            PointF TP;
        Mark:
            TP = new PointF(getRandom.Next(1920), getRandom.Next(1080));
            if (_path.IsVisible(TP))
                return TP;
            goto Mark;
        }
        PointF getRandomPointInRegion(Region _reg)
        {
            PointF TP;
        Mark:
            TP = new PointF(getRandom.Next(1920), getRandom.Next(1080));
            if (_reg.IsVisible(TP))
                return TP;
            goto Mark;
        }

        void pKeyDown (object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Tab:
                    int w = Screen.PrimaryScreen.Bounds.Width,
                        h = Screen.PrimaryScreen.Bounds.Height;
                    if (Resolution.Width != w || Resolution.Height != h)
                    {
                        Resolution = new Size(w, h);
                        ResolutionRatio.Width = Resolution.Width / 1920f;
                        ResolutionRatio.Height = Resolution.Height / 1080f;
                    }
                    break;
                case Keys.P:
                    if (!Paused)
                        Paused = true;
                    else
                        Paused = false;
                    break;
            }
        }

        void pMouseMove (object sender, MouseEventArgs e)
        {
            if (AnyDotSelected)
                if (reg.IsVisible(e.Location))
                Balls[SelectedBall].Position = e.Location;
                else
                {
                    SelectedBall = -1;
                    AnyDotSelected = false;
                }
            if (SpeedChangerSelected && SpeedChangerRectangle.Contains(e.Location))
                BallSpeed = (958 - e.Y) / 20f;
            if (BallSpeed < DOT_MIN_SPEED)
                BallSpeed = DOT_MIN_SPEED;
            else
                if (BallSpeed > DOT_MAX_SPEED)
                    BallSpeed = DOT_MAX_SPEED;
        }

        void pMouseUp (object sender, MouseEventArgs e)
        {
            AnyDotSelected = false;
            SpeedChangerSelected = false;
        }

        void pMouseDown (object sender, MouseEventArgs e)
        {
            if (!AnyDotSelected)
            {
                double dist;
                for (int a = 0; a < Balls.Count; ++a)
                {
                    double x = Balls[a].Position.X,
                           y = Balls[a].Position.Y;
                    dist = Math.Sqrt(Math.Pow(x - e.X, 2) + Math.Pow(y - e.Y, 2));
                    if (dist < 15)
                    {
                        AnyDotSelected = true;
                        SelectedBall = a;
                        break;
                    }
                }
            }
            if (!SpeedChangerSelected)
                if (SpeedChangerRectangle.Contains(e.Location))
                    SpeedChangerSelected = true;
        }

        void pUpdate (object sender, EventArgs e)
        {
            if (!Paused)
            {
                for (int a = 0; a < Balls.Count; ++a)
                {
                    float nextX = Balls[a].Position.X + (float)Math.Cos(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed,
                          nextY = Balls[a].Position.Y + (float)Math.Sin(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed;
                    PointF PP = new PointF(nextX + 18 * (float)Math.Cos(Math.PI * Balls[a].Direction / 180), nextY + 18 * (float)Math.Sin(Math.PI * Balls[a].Direction / 180));

                    if (!reg.IsVisible(PP))
                    {
                        Balls[a].Direction -= 150 + getRandom.Next(-30, 30);
                    }
                    Balls[a].Position = new PointF(nextX, nextY);
                }
            }
            Invalidate();
        }

        void pDraw (object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.ScaleTransform(ResolutionRatio.Width, ResolutionRatio.Height);

            g.FillRegion(Brushes.CornflowerBlue, reg);

            for (int a = 0; a < Balls.Count; ++a)
            {
                PointF TP = Balls[a].Position;
                SolidBrush TransBrush = new SolidBrush(Color.FromArgb(128, 64, 64, 190));
                g.FillEllipse(TransBrush, TP.X - 6, TP.Y - 6, 22, 22);
                g.DrawEllipse(new Pen(Brushes.AliceBlue, 2), TP.X - 7, TP.Y - 7, 24, 24);
                g.FillEllipse(Brushes.AliceBlue, TP.X, TP.Y, 10, 10);
            }

            g.DrawRectangle(new Pen(Color.CornflowerBlue, 4), new Rectangle(1818, 738, 100, 224));
            g.FillRectangle(new SolidBrush(Color.LimeGreen), 1820, 960 - 20 - BallSpeed * 20, 96, 20 + BallSpeed * 20);

            string output = "";
            //for (int a = 0; a < Balls.Count; ++a)
            //    output += Balls[a].Position.ToString() + ";" + Balls[a].Direction.ToString()+ "\n";
            output += BallSpeed.ToString();
            g.DrawString(output, new Font("Verdana", 13), Brushes.Black, 0, 0);
        }
    }
}

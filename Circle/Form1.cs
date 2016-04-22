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
            BALL_MIN_SPEED = 1,
            BALL_MAX_SPEED = 10,
            BALL_MIN_COUNT = 10,
            BALL_MAX_COUNT = 200;
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
            SpeedChangerRectangle = new Rectangle(1818, 738, 100, 220),
            CountChangerRectangle = new Rectangle(1818, 438, 100, 220);
        class Door
        {
            public Rectangle Rectangle;
            public Point Start;
            public Point End;
            public Point Direction;
            public Door(Rectangle _Rectangle, Point _Start, Point _End, Point _Direction)
            {
                Rectangle = _Rectangle;
                Start = _Start;
                End = _End;
                Direction = _Direction;
            }
            public Door(int x, int y, int w, int h, Point _Start, Point _End, Point _Direction)
            {
                Rectangle = new Rectangle(x, y, w, h);
                Start = _Start;
                End = _End;
                Direction = _Direction;
            }
        }
        int lastTick, lastFrameRate, frameRate,
            SelectedBall = -1, BallCount = 50;
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
        List<Door> Doors = new List<Door>();

        private int CalculateFrameRate()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

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
            Doors.Add(new Door(new Rectangle(new Point(500, 200), new Size(10, 50)), new Point(500, 200), new Point(500, 400), new Point(0, 1)));
            Doors.Add(new Door(new Rectangle(new Point(300, 500), new Size(50, 10)), new Point(200, 500), new Point(400, 500), new Point(1, 0)));
            RegionRefresh();
            for (int a = 0; a < BallCount; ++a)
                Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 17) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
            updateTimer.Interval = 1;
            updateTimer.Tick += new EventHandler(pUpdate);
            updateTimer.Start();
        }

        void RegionRefresh()
        {
            reg = new Region(new Rectangle(1200, 400, 400, 400));
            reg.Union(CirclePath);
            reg.Xor(new Rectangle(1415, 750, 50, 50));
            reg.Union(new Rectangle(300, 300, 300, 50));
            reg.Union(new Rectangle(300, 300, 50, 300));
            reg.Union(new Rectangle(150, 600, 200, 50));
            foreach (Door TD in Doors)
                reg.Xor(TD.Rectangle);
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
            {
                Point[] PP = new Point[4]
                {
                    new Point(e.X - 9, e.Y - 9),
                    new Point(e.X - 9, e.Y + 18),
                    new Point(e.X + 18, e.Y + 18),
                    new Point(e.X + 9, e.Y - 18)
                };
                for (int b = 0; b < 4; ++b)
                    if (reg.IsVisible(PP[b]))
                        Balls[SelectedBall].Position = e.Location;
                    else
                    {
                        SelectedBall = -1;
                        AnyDotSelected = false;
                        break;
                    }
            }
            if (SpeedChangerSelected && SpeedChangerRectangle.Contains(e.Location))
                BallSpeed = (958 - e.Y) / 20f;
            if (BallSpeed < BALL_MIN_SPEED)
                BallSpeed = BALL_MIN_SPEED;
            else
                if (BallSpeed > BALL_MAX_SPEED)
                    BallSpeed = BALL_MAX_SPEED;
            if (CountChangerSelected && CountChangerRectangle.Contains(e.Location))
                BallCount = 658 - e.Y;
            if (BallCount < BALL_MIN_COUNT)
                BallCount = BALL_MIN_COUNT;
            else
                if (BallCount > BALL_MAX_COUNT)
                    BallCount = BALL_MAX_COUNT;
            int dif = BallCount - Balls.Count;
            if (dif > 0)
                for (int a = 0; a < dif; ++a)
                    Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 17) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
            else
                if (dif < 0)
                        Balls.RemoveRange(0, -dif);
        }

        void pMouseUp (object sender, MouseEventArgs e)
        {
            AnyDotSelected = false;
            SpeedChangerSelected = false;
            CountChangerSelected = false;
            SelectedBall = -1;
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
            if (!CountChangerSelected)
                if (CountChangerRectangle.Contains(e.Location))
                    CountChangerSelected = true;
        }

        void pUpdate (object sender, EventArgs e)
        {
            if (!Paused)
            {
                foreach(Door TD in Doors)
                {
                    reg.MakeEmpty();
                    if (TD.Direction.Y != 0)
                        if ((TD.Direction.Y > 0 && TD.Rectangle.Y < TD.End.Y) ||
                             TD.Direction.Y < 0 && TD.Rectangle.Y > TD.Start.Y)
                            TD.Rectangle.Y += TD.Direction.Y * 2;
                        else
                            TD.Direction.Y = -TD.Direction.Y;
                    if (TD.Direction.X != 0)
                        if ((TD.Direction.X > 0 && TD.Rectangle.X < TD.End.X) ||
                             TD.Direction.X < 0 && TD.Rectangle.X > TD.Start.X)
                            TD.Rectangle.X += TD.Direction.X * 2;
                        else
                            TD.Direction.X = -TD.Direction.X;
                    RegionRefresh();
                }
                for (int a = 0; a < Balls.Count; ++a)
                {
                    if (a != SelectedBall)
                    {
                        float nextX = Balls[a].Position.X + (float)Math.Cos(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed,
                              nextY = Balls[a].Position.Y + (float)Math.Sin(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed;
                        int ax = (nextX > Balls[a].Position.X ? 18 : 6), ay = (nextY > Balls[a].Position.Y ? 18 : 12);
                        PointF PP = new PointF(nextX + ax * (float)Math.Cos(Math.PI * Balls[a].Direction / 180), nextY + ay * (float)Math.Sin(Math.PI * Balls[a].Direction / 180));
                        if (!reg.IsVisible(PP))
                            Balls[a].Direction -= 150 + getRandom.Next(-30, 30);
                        Balls[a].Position = new PointF(nextX, nextY);
                    }
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
            g.DrawRectangle(new Pen(Color.CadetBlue, 4), new Rectangle(1818, 438, 100, 224));
            g.FillRectangle(new SolidBrush(Color.Chartreuse), 1820, 660 - 20 - BallCount, 96, 20 + BallCount);
            g.DrawString(Math.Round(BallSpeed, 1).ToString(), new Font("Verdana", 28), Brushes.DarkSlateGray, new PointF(1852 - (BallSpeed != Math.Round(BallSpeed, 1) ? 17 : (BallSpeed == 10 ? 14 : 0)), 920));
            g.DrawString("Velocity", new Font("Verdana", 15), Brushes.DarkKhaki, new PointF(1827, 710));
            g.DrawString("Amount", new Font("Verdana", 15), Brushes.DarkKhaki, new PointF(1827, 410));
            g.DrawString(BallCount.ToString(), new Font("Verdana", 28), Brushes.DarkSlateGray, new PointF(1840 - (BallCount > 99 ? 10 : 0), 620));

            string output = CalculateFrameRate().ToString() + "\n";
            //for (int a = 0; a < Balls.Count; ++a)
            //    output += Balls[a].Position.ToString() + ";" + Balls[a].Direction.ToString()+ "\n";
            //output += BallSpeed.ToString() + "\n" + BallCount.ToString();
            g.DrawString(output, new Font("Verdana", 13), Brushes.Black, 0, 0);
        }
    }
}

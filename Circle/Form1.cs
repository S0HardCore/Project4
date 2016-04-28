using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Circle
{
    public partial class Form1 : Form
    {
        #region Constants
        const int
            BALL_MIN_SPEED = 1,
            BALL_MAX_SPEED = 10,
            BALL_MIN_COUNT = 10,
            BALL_MAX_COUNT = 100,
            HERO_WIDTH = 24,
            HERO_HEIGHT = 24,
            HERO_MAX_LIVES = 6;
        const float
            HERO_TURN_RATE = 4.8f,
            DETECTION_TIME = 0.4f;
        readonly static Size
            HERO_SIZE = new Size(HERO_WIDTH, HERO_HEIGHT),
            ENEMY_VIEW = new Size(125, 125),
            HERO_VIEW = new Size(150, 150);
        readonly static Rectangle
            HERO_INITIAL_POSITION = new Rectangle(150, 600, 50, 50);
        Image[]
            iBars = new Image[]
            {
                Properties.Resources.barGrayLeft,
                Properties.Resources.barGrayMid,
                Properties.Resources.barGrayRight,
                Properties.Resources.barGreenLeft,
                Properties.Resources.barGreenMid,
                Properties.Resources.barGreenRight,
                Properties.Resources.barYellowLeft,
                Properties.Resources.barYellowMid,
                Properties.Resources.barYellowRight
            },
            iBlock = new Image[]
            {
                Properties.Resources.blockGray,
                Properties.Resources.blockWhite
            };
        Image
            iPanel = Properties.Resources.panelBlue,
            iPanelGlass = Properties.Resources.panelGlassTL,
            iCircle = Properties.Resources.circleGray;
        enum ObjectList
        {
            None,
            Key,
            Clock,
            Box
        };
        enum ProgramState
        {
            Paused,
            Active,
            Settings
        };
        static ProgramState GameState;
        static Timer
            updateTimer = new Timer(),
            unstuckTimer = new Timer();
        readonly static Font
            Kristen15 = new Font("Kristen ITC", 15),
            Motorwerk20 = new Font("Nirmala UI", 20);
        readonly static SolidBrush
            viewBrush = new SolidBrush(Color.FromArgb(128, 35, 35, 35)),
            enemyViewBrush = new SolidBrush(Color.FromArgb(128, 190, 190, 190)),
            textRB = new SolidBrush(Color.FromArgb(80, 74, 141)),
            textLB = new SolidBrush(Color.FromArgb(222, 219, 54));
        readonly static Rectangle
            CircleRectangle = new Rectangle(520, 100, 880, 880),
            SpeedChangerRectangle = new Rectangle(1800, 1045, 92, 45),
            CountChangerRectangle = new Rectangle(1800, 980, 92, 45),
            HealthRectangle = new Rectangle(15, 1005, 265, 60);
        #endregion
        class Hero
        {
            public PointF Position;
            public float Direction;
            public int Health = HERO_MAX_LIVES;
            ObjectList LeftHandItem = ObjectList.None;
            ObjectList RightHandItem = ObjectList.None;
            public Hero(float x, float y, float _Direction)
            {
                Position = new PointF(x, y);
                Direction = _Direction;
            }
            public Hero(PointF _Position, float _Direction)
            {
                Position = _Position;
                Direction = _Direction;
            }
            public Hero(Point _Position, float _Direction)
            {
                Position = new PointF(_Position.X, _Position.Y);
                Direction = _Direction;
            }
            public Hero(PointF _Position, double _Direction)
            {
                Position = _Position;
                Direction = (float)_Direction;
            }
            public Hero(Point _Position, double _Direction)
            {
                Position = new PointF(_Position.X, _Position.Y);
                Direction = (float)_Direction;
            }
            public PointF DrawPosition()
            {
                return new PointF(Position.X - HERO_WIDTH / 2, Position.Y - HERO_HEIGHT / 2);
            }
            public float Cos()
            {
                return (float)Math.Cos(Math.PI * Direction / 180);
            }
            public float Sin()
            {
                return (float)Math.Sin(Math.PI * Direction / 180);
            }
            public float Cos(float _Direction)
            {
                return (float)Math.Cos(Math.PI * _Direction / 180);
            }
            public float Sin(float _Direction)
            {
                return (float)Math.Sin(Math.PI * _Direction / 180);
            }
            public float AngleBetween(PointF One, PointF Two)
            {
                double x1 = One.X,
                       x2 = Two.X,
                       y1 = One.Y,
                       y2 = Two.Y;
                double Angle = Math.Atan2(y1 - y2, x1 - x2) / Math.PI * 180;
                Angle = (Angle < 0) ? Angle + 360 : Angle;
                return (float)Angle;
            }
            public PointF[] LeftHand()
            {
                PointF[] Points = new PointF[2];
                Points[0] = new PointF(Position.X + this.Cos(Direction - 90) * 10, Position.Y + this.Sin(Direction - 90) * 8);
                Points[1] = new PointF(Position.X + this.Cos(Direction - 30) * 15, Position.Y + this.Sin(Direction - 30) * 16);
                return Points;
            }
            public PointF[] RightHand()
            {
                PointF[] Points = new PointF[2];
                Points[0] = new PointF(Position.X + this.Cos(Direction + 90) * 10, Position.Y + this.Sin(Direction + 90) * 8);
                Points[1] = new PointF(Position.X + this.Cos(Direction + 30) * 15, Position.Y + this.Sin(Direction + 30) * 16);
                return Points;
            }
            public void DoDamage(Ball _Ball)
            {
                _Ball.Detected = false;
                _Ball.Duration = 0f;
                _Ball.Color = Color.AliceBlue;
                Health--;
                if (Health > 0)
                    Position = new PointF(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25);
                else
                    Application.Exit();
            }
        }
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
        class Ball
        {
            public PointF Position;
            public float Speed;
            public float Direction;
            public float Duration = 0f;
            public Boolean Detected = false;
            public Color Color = Color.AliceBlue;
            //public Region View;

            public Ball(PointF _Position, float _Speed, float _Direction)
            {
                Position = _Position;
                Speed = _Speed;
                Direction = _Direction;
            }
            public Region RefreshView()
            {
                GraphicsPath tempgp = new GraphicsPath();
                Rectangle temprect = new Rectangle((int)Position.X + 6 - ENEMY_VIEW.Width / 2, (int)Position.Y + 6 - ENEMY_VIEW.Height / 2, ENEMY_VIEW.Width, ENEMY_VIEW.Height);
                tempgp.AddPie(temprect, Direction - 45, 90);
                Region tempreg = new Region(tempgp);
                tempreg.Exclude(HERO_INITIAL_POSITION);
                return tempreg;
            }
        }
        #region Variables
        Random
            getRandom = new Random(DateTime.Now.Millisecond);
        Size
            Resolution = new Size(1920, 1080);
        SizeF
            ResolutionRatio = new SizeF(1f, 1f);
        GraphicsPath
            CirclePath = new GraphicsPath();
        PointF[] HeroLastPoints = new PointF[3];
        int lastTick, lastFrameRate, frameRate,
            UnstuckIndex = 0, SelectedBall = -1, BallCount = 50;
        float BallSpeed = 1f;
        static Region reg;
        Boolean
            StuckSystem = false, PressedForward = false, PressedLeft = false, PressedRight = false,
            AnyDotSelected, SpeedChangerSelected, CountChangerSelected;
        
        static List<Ball> Balls = new List<Ball>();
        List<Door> Doors = new List<Door>();
        Hero theHero;
        #endregion

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
            this.BackColor = Color.FromArgb(90, 90, 90);
            this.Paint += new PaintEventHandler(pDraw);
            this.KeyDown += new KeyEventHandler(pKeyDown);
            this.KeyUp += new KeyEventHandler(pKeyUp);
            this.MouseDown += new MouseEventHandler(pMouseDown);
            this.MouseMove += new MouseEventHandler(pMouseMove);
            this.MouseUp += new MouseEventHandler(pMouseUp);
            GameState = ProgramState.Active;
            for (int a = 0; a < HeroLastPoints.Length; ++a)
                HeroLastPoints[a] = HERO_INITIAL_POSITION.Location;
            theHero = new Hero(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25, 0f);
            CirclePath.AddEllipse(CircleRectangle);
            Doors.Add(new Door(new Rectangle(new Point(500, 200), new Size(10, 50)), new Point(500, 200), new Point(500, 400), new Point(0, 1)));
            Doors.Add(new Door(new Rectangle(new Point(300, 500), new Size(50, 10)), new Point(200, 500), new Point(400, 500), new Point(1, 0)));
            RegionRefresh();
            for (int a = 0; a < BallCount; ++a)
                Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 17) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
            updateTimer.Interval = 1;
            unstuckTimer.Interval = 500;
            updateTimer.Tick += new EventHandler(pUpdate);
            unstuckTimer.Tick += new EventHandler(IsHeroStuck);
            updateTimer.Start();
            unstuckTimer.Start();
        }

        void IsHeroStuck(object sender, EventArgs e)
        {
            if (GameState == ProgramState.Active)
            {
                HeroLastPoints[UnstuckIndex] = theHero.Position;
                if (UnstuckIndex < HeroLastPoints.Length - 1)
                    UnstuckIndex++;
                else
                    UnstuckIndex = 0;
            }
        }

        void RegionRefresh()
        {
            reg = new Region(new Rectangle(1200, 400, 400, 400));
            reg.Union(CirclePath);
            reg.Xor(new Rectangle(1415, 750, 50, 50));
            reg.Union(new Rectangle(300, 300, 300, 50));
            reg.Union(new Rectangle(300, 300, 50, 300));
            reg.Union(new Rectangle(150, 600, 200, 50));
            reg.Union(new Rectangle(300, 600, 200, 300));
            reg.Exclude(new Rectangle(300, 650, 150, 3));
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
            foreach (Door TD in Doors)
                if (_reg.IsVisible(TP) && !TD.Rectangle.Contains((int)TP.X, (int)TP.Y) && !new Region(HERO_INITIAL_POSITION).IsVisible(TP))
                    return TP;
            goto Mark;
        }

        void pKeyDown(object sender, KeyEventArgs e)
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
                    if (GameState == ProgramState.Active)
                        GameState = ProgramState.Paused;
                    else if (GameState == ProgramState.Paused)
                        GameState = ProgramState.Active;
                    break;
                case Keys.W:
                    PressedForward = true;
                    break;
                case Keys.A:
                    PressedLeft = true;
                    PressedRight = false;
                    break;
                case Keys.D:
                    PressedRight = true;
                    PressedLeft = false;
                    break;
            }
        }
        void pKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.W:
                    PressedForward = false;
                    break;
                case Keys.A:
                    PressedLeft = false;
                    break;
                case Keys.D:
                    PressedRight = false;
                    break;
            }
        }

        void pMouseMove(object sender, MouseEventArgs e)
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
                    if (reg.IsVisible(PP[b]) && !new Region(HERO_INITIAL_POSITION).IsVisible(PP[b]))
                        Balls[SelectedBall].Position = e.Location;
                    else
                    {
                        SelectedBall = -1;
                        AnyDotSelected = false;
                        break;
                    }
            }
            if (SpeedChangerSelected && SpeedChangerRectangle.Contains(e.Location))
                BallSpeed = (float)Math.Round((e.X - SpeedChangerRectangle.Left) / 9f, 1);
            if (BallSpeed < BALL_MIN_SPEED)
                BallSpeed = BALL_MIN_SPEED;
            else
                if (BallSpeed > BALL_MAX_SPEED)
                    BallSpeed = BALL_MAX_SPEED;
            if (CountChangerSelected && CountChangerRectangle.Contains(e.Location))
                BallCount = (int)Math.Round((e.X - CountChangerRectangle.Left) * 1.1f);
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

        void pMouseUp(object sender, MouseEventArgs e)
        {
            AnyDotSelected = false;
            SpeedChangerSelected = false;
            CountChangerSelected = false;
            SelectedBall = -1;
            if (e.Button == MouseButtons.Right)
                theHero.Position = new PointF(e.X, e.Y);
        }

        void pMouseDown(object sender, MouseEventArgs e)
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
            pMouseMove(sender, e);
        }

        void pUpdate(object sender, EventArgs e)
        {
            switch (GameState)
            {
                case ProgramState.Active:
                    if (!reg.IsVisible(theHero.Position) && StuckSystem)
                        theHero.Position = HeroLastPoints[UnstuckIndex];
                if (PressedLeft)
                    if (theHero.Direction < HERO_TURN_RATE)
                        theHero.Direction = 360;
                    else
                        theHero.Direction -= HERO_TURN_RATE;
                if (PressedRight)
                    if (theHero.Direction > 360 - HERO_TURN_RATE)
                        theHero.Direction = 0;
                    else
                        theHero.Direction += HERO_TURN_RATE;
                if (PressedForward)
                {
                    PointF next = new PointF(theHero.Position.X + 1.33f * theHero.Cos(), theHero.Position.Y + 1.33f * theHero.Sin());
                    PointF[] nextPoints = new PointF[]
                    {
                        new PointF(next.X + HERO_WIDTH / 3 * theHero.Cos(theHero.Direction - 45), next.Y + HERO_HEIGHT / 3 * theHero.Sin(theHero.Direction - 45)),
                        new PointF(next.X + HERO_WIDTH / 3 * theHero.Cos(theHero.Direction + 45), next.Y + HERO_HEIGHT / 3 * theHero.Sin(theHero.Direction + 45)),
                        new PointF(next.X + HERO_WIDTH / 2 * theHero.Cos(), next.Y + HERO_HEIGHT / 2 * theHero.Sin())
                    };
                    if (reg.IsVisible(nextPoints[0]) && reg.IsVisible(nextPoints[1]) && reg.IsVisible(nextPoints[2]))
                        theHero.Position = next;
                }
                foreach (Door TD in Doors)
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
                    if (Balls[a].Detected)
                        if (Balls[a].Duration < DETECTION_TIME)
                            Balls[a].Duration += 0.01f;
                        else
                            theHero.DoDamage(Balls[a]);
                    float x = Balls[a].Position.X + (theHero.Position.X > Balls[a].Position.X ? 8 : 0),
                          y = Balls[a].Position.Y + (theHero.Position.Y > Balls[a].Position.Y ? 8 : 0),
                          dist = (float)Math.Sqrt(Math.Pow(theHero.Position.X - x, 2) + Math.Pow(theHero.Position.Y - y, 2));
                    if (dist > 25 && dist < ENEMY_VIEW.Width / 3 && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position))
                        Balls[a].Direction = theHero.AngleBetween(theHero.Position, Balls[a].Position);
                    if ((dist < 18 && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)) || 
                    (Balls[a].RefreshView().IsVisible(theHero.Position) && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)))
                    {
                        if (dist > 25)
                        {
                            Balls[a].Detected = true;
                            Balls[a].Color = Color.FromArgb(245, 158, 162);
                        }
                        else
                        {
                            theHero.DoDamage(Balls[a]);
                        }
                    }
                    else
                    {
                        Balls[a].Detected = false;
                        Balls[a].Color = Color.AliceBlue;
                    }
                    if (a != SelectedBall)
                    {
                        float nextX = Balls[a].Position.X + (float)Math.Cos(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed,
                              nextY = Balls[a].Position.Y + (float)Math.Sin(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * BallSpeed;
                        int ax = (nextX > Balls[a].Position.X ? 18 : 6), ay = (nextY > Balls[a].Position.Y ? 18 : 12);
                        PointF PP = new PointF(nextX + ax * (float)Math.Cos(Math.PI * Balls[a].Direction / 180), nextY + ay * (float)Math.Sin(Math.PI * Balls[a].Direction / 180));
                        if (!reg.IsVisible(PP) || new Region(HERO_INITIAL_POSITION).IsVisible(PP))
                            Balls[a].Direction -= 150 + getRandom.Next(-30, 30);
                        Balls[a].Position = new PointF(nextX, nextY);
                    }
                }
                break;
                case ProgramState.Settings:
                break;
            }
            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.ScaleTransform(ResolutionRatio.Width, ResolutionRatio.Height);

            g.FillRegion(Brushes.CornflowerBlue, reg);
            #region Initial Rectangle
            g.FillRectangle(new SolidBrush(Color.FromArgb(112, 161, 249)), HERO_INITIAL_POSITION);
            g.FillPolygon(new SolidBrush(Color.FromArgb(110, 159, 247)), new Point[] // bottom
            {
                new Point(HERO_INITIAL_POSITION.X, HERO_INITIAL_POSITION.Bottom),
                new Point(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom + HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom + HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right, HERO_INITIAL_POSITION.Bottom)
            });
            g.FillPolygon(new SolidBrush(Color.FromArgb(110, 159, 247)), new Point[] // top
            {
                new Point(HERO_INITIAL_POSITION.X, HERO_INITIAL_POSITION.Top),
                new Point(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right, HERO_INITIAL_POSITION.Top)
            });
            g.FillPolygon(new SolidBrush(Color.FromArgb(110, 159, 247)), new Point[] // left
            {
                new Point(HERO_INITIAL_POSITION.X, HERO_INITIAL_POSITION.Top),
                new Point(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom + HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.X, HERO_INITIAL_POSITION.Bottom)
            });
            g.FillPolygon(new SolidBrush(Color.FromArgb(110, 159, 247)), new Point[] // right
            {
                new Point(HERO_INITIAL_POSITION.Right, HERO_INITIAL_POSITION.Top),
                new Point(HERO_INITIAL_POSITION.Right + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom + HERO_INITIAL_POSITION.Height / 8),
                new Point(HERO_INITIAL_POSITION.Right, HERO_INITIAL_POSITION.Bottom)
            });
            g.DrawRectangle(new Pen(Color.FromArgb(105, 154, 242)), HERO_INITIAL_POSITION);
            g.DrawLine(new Pen(Color.FromArgb(100, 149, 237)), 144, 594, 150, 600);
            g.DrawLine(new Pen(Color.FromArgb(100, 149, 237)), 144, 655, 150, 649);
            g.DrawLine(new Pen(Color.FromArgb(100, 149, 237)), 205, 594, 199, 600);
            g.DrawLine(new Pen(Color.FromArgb(100, 149, 237)), 205, 655, 200, 650);
            #endregion
            for (int a = 0; a < Balls.Count; ++a)
            {
                PointF TP = Balls[a].Position;
                g.FillRegion(enemyViewBrush, Balls[a].RefreshView());
                SolidBrush TransBrush = new SolidBrush(Color.FromArgb(128, 64, 64, 190));
                g.FillEllipse(TransBrush, TP.X - 6, TP.Y - 6, 22, 22);
                g.DrawEllipse(new Pen(Balls[a].Color, 2), TP.X - 7, TP.Y - 7, 24, 24);
                g.FillEllipse(new SolidBrush(Balls[a].Color), TP.X, TP.Y, 10, 10);
            }
            g.DrawImage(iPanel, 1770, 900, 150, 180);
            g.DrawString("Velocity", Kristen15, textRB, new Point(1804, 1017));
            g.DrawString("Amount", Kristen15, textRB, new Point(1804, 953));
            g.DrawString("Lives", Motorwerk20, textLB, new Point(31, 1015));

            g.DrawImage(iBars[3], SpeedChangerRectangle.Location);
            g.DrawImage(iBars[6], CountChangerRectangle.Location);
            for (int s = 0; s < BALL_MAX_SPEED; ++s)
            {
                g.DrawImage(iBars[1], new Rectangle(SpeedChangerRectangle.X + 6 + s * 8, SpeedChangerRectangle.Y, 8, iBars[1].Height));
                g.DrawImage(iBars[1], new Rectangle(CountChangerRectangle.X + 6 + s * 8, CountChangerRectangle.Y, 8, iBars[4].Height));
            }
            for (float s = 0; s < BallSpeed * 4f; ++s)
                g.DrawImage(iBars[4], new RectangleF(SpeedChangerRectangle.X + 6 + s * 2, SpeedChangerRectangle.Y, 2, iBars[4].Height));
            for (float s = 0; s < BallCount / 2.5f; ++s)
                g.DrawImage(iBars[7], new RectangleF(CountChangerRectangle.X + 6 + s * 2, CountChangerRectangle.Y, 2, iBars[7].Height));
            g.DrawImage(BallSpeed > BALL_MAX_SPEED - 0.25f ? iBars[5] : iBars[2], SpeedChangerRectangle.X + 6 + BALL_MAX_SPEED * 8, SpeedChangerRectangle.Y);
            g.DrawImage(BallCount >= BALL_MAX_COUNT ? iBars[8] : iBars[2], CountChangerRectangle.X + 6 + BALL_MAX_SPEED * 8, CountChangerRectangle.Y);

            g.DrawImage(iPanelGlass, HealthRectangle);
            for (int s = 0; s < HERO_MAX_LIVES; ++s)
                g.DrawImage(s > theHero.Health - 1 ? iBlock[0] : iBlock[1], HealthRectangle.X + 93 + 27 * s, HealthRectangle.Y + 18);

            //Rectangle viewRect = new Rectangle((int)theHero.Position.X - HERO_VIEW.Width / 2 + 1, (int)theHero.Position.Y - HERO_VIEW.Height / 2 + 1, HERO_VIEW.Width, HERO_VIEW.Height);
            //g.FillPie(viewBrush, viewRect, theHero.Direction - 45, 90);
            float x = theHero.Position.X, y = theHero.Position.Y, d = theHero.Direction;
            g.DrawLine(new Pen(Color.Orange, 5), theHero.LeftHand()[0], theHero.LeftHand()[1]);
            g.DrawLine(new Pen(Color.Orange, 5), theHero.RightHand()[0], theHero.RightHand()[1]);
            g.DrawImage(iCircle, new RectangleF(theHero.DrawPosition(), new SizeF(24, 24)));
            g.FillEllipse(Brushes.CornflowerBlue, theHero.Position.X - 3 + theHero.Cos(theHero.Direction - 45) * 6, theHero.Position.Y - 3 + theHero.Sin(theHero.Direction - 45) * 5, 4, 4);
            g.FillEllipse(Brushes.CornflowerBlue, theHero.Position.X - 3 + theHero.Cos(theHero.Direction + 45) * 6, theHero.Position.Y - 3 + theHero.Sin(theHero.Direction + 45) * 5, 4, 4);

            string output = "Status: " + GameState.ToString() + "\nFPS: " + CalculateFrameRate().ToString() + "\n";
            //for (int a = 0; a < Balls.Count; ++a)
            //    output += Balls[a].Position.ToString() + ";" + Balls[a].Direction.ToString()+ "\n";
            //output += BallSpeed.ToString() + "\n" + BallCount.ToString();
            foreach (Ball TB in Balls)
                if (TB.Detected)
                    output += Math.Round(TB.Duration, 2).ToString() + "\n";
            g.TranslateTransform(200, 200);
            g.DrawString(output, new Font("QUARTZ MS", 14), Brushes.Black, 0, 0);
        }
    }
}

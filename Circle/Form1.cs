using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            BALL_MAX_HEALTH = 3,
            BOX_MAX_HEALTH = 5,
            HERO_WIDTH = 24,
            HERO_HEIGHT = 24,
            HERO_MAX_LIVES = 6,
            COLOR_MIN_VALUE = 63,
            COLOR_MAX_VALUE = 192,
            BG_INTERVAL_MIN = 5,
            BG_INTERVAL_MAX = 410,
            PROJECTILE_SIZE = 3,
            PROJECTILE_SPEED = 8;
        const float
            HERO_TURN_RATE = 4.8f,
            DETECTION_TIME = 2.6f,
            PROJECTILE_COOLDOWN = 0.2f,
            DEFAULT_HERO_VELOCITY = 1.5f,
            INCREASED_HERO_VELOCITY = 3.5f,
            AIMING_DURATION = 0.8f;
        readonly static Size
            HERO_SIZE = new Size(HERO_WIDTH, HERO_HEIGHT),
            ENEMY_VIEW = new Size(125, 125),
            HERO_VIEW = new Size(150, 150);
        readonly static Rectangle
            HERO_INITIAL_POSITION = new Rectangle(150, 600, 50, 50);
        static Random
            getRandom = new Random(DateTime.Now.Millisecond);
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
                Properties.Resources.barYellowRight,
                Properties.Resources.barRedLeft,
                Properties.Resources.barRedMid,
                Properties.Resources.barRedRight
            },
            iBlock = new Image[]
            {
                Properties.Resources.blockGray,
                Properties.Resources.blockWhite
            },
            iBox = new Image[]
            {
                Properties.Resources.wooden_box,
                Properties.Resources.steel_box
            };
        static Image
            iPanel = Properties.Resources.panelBlue,
            iPanelGlass = Properties.Resources.panelGlassTL,
            iGlassPanelCorners = Properties.Resources.glassPanelCorners,
            iGlassPanelConsole = Properties.Resources.glassPanelConsole,
            iGlassPanelProjection = Properties.Resources.glassPanel_projection,
            iGlassPanelSimple = Properties.Resources.glassPanelSimple,
            iFlash = Properties.Resources.flash,
            iCircle = Properties.Resources.circleGray,
            iSettingBG = Properties.Resources.metalPanel_blueCorner,
            iCheckBoxEmpty = Properties.Resources.grayBoxEmpty,
            iCheckBox = Properties.Resources.grayBoxChecked;
        enum BoxType
        {
            Wooden,
            Steel
        }
        enum ObjectList
        {
            None,
            Weapon,
            Magnet,
            Key
        };
        enum SuitList
        {
            Default,
            Flash,
            Ninja,
        }
        enum ProgramState
        {
            Paused,
            Active,
            Settings,
            Dialog
        };
        static ProgramState GameState;
        static readonly StringFormat
            TextFormatCenter = new StringFormat(StringFormatFlags.NoClip);
        static Timer
            updateTimer = new Timer(),
            secondaryTimer = new Timer();
        readonly static Font
            Kristen15 = new Font("Kristen ITC", 15),
            Verdana17 = new Font("Verdana", 17),
            Verdana15 = new Font("Verdana", 15),
            Verdana13 = new Font("Verdana", 13),
            Nirmala20 = new Font("Nirmala UI", 20);
        readonly static SolidBrush
            viewBrush = new SolidBrush(Color.FromArgb(128, 35, 35, 35)),
            enemyViewBrush = new SolidBrush(Color.FromArgb(128, 190, 190, 190)),
            textRB = new SolidBrush(Color.FromArgb(80, 74, 141)),
            textLB = new SolidBrush(Color.FromArgb(222, 219, 54));
        static Rectangle
            CircleRectangle = new Rectangle(520, 100, 880, 880),
            CountChangerRectangle = new Rectangle(960, 645, 92, 45),
            SpeedChangerRectangle = new Rectangle(960, 595, 92, 45),
            BGSpeedChangerRectangle = new Rectangle(960, 545, 92, 45),
            HealthRectangle = new Rectangle(15, 1005, 265, 60),
            SettingRectangle = new Rectangle(860, 390, 200, 300),
            AimingCheckBoxRectangle = new Rectangle(950, 425, 32, 30),
            TryAgainRectangle = new Rectangle(800, 480, 320, 120),
            OkRectangle = new Rectangle(840, 550, 80, 30),
            ExitRectangle = new Rectangle(1000, 550, 80, 30);
        #endregion

        class ConsolePrototype
        {
            public Boolean Enabled;

            static private string consoleString;
            static private string consolePrevString;
            static private string consoleLog;
            static private Rectangle CONSOLE_REGION;

            public string getString() { return consoleString; }
            public string getPrevString() { return consolePrevString; }
            public string getLog() { return consoleLog; }
            public int getLength() { return consoleString.Length; }
            public void setString(string String) { consoleString = String; }
            public void setPrevString(string String) { consolePrevString = String; }
            public void setLog(string String) { consoleLog = String; }
            public Rectangle getRegion()
            {
                CONSOLE_REGION = new Rectangle(0, 0, 520, 50);
                return CONSOLE_REGION;
            }
            public void applyCommand()
            {
                consoleString = consoleString.Trim();
                if (!String.IsNullOrEmpty(consoleString))
                    if (consoleString.Length > 8)
                        if (consoleString[0].Equals('W') && consoleString[5].Equals('H'))
                        {
                            string TS1, TS2;
                            int TI1 = Resolution.Width, TI2 = Resolution.Height;
                            TS1 = consoleString.Substring(1, 4);
                            TS2 = consoleString.Substring(6);
                            Int32.TryParse(TS1, out TI1);
                            Int32.TryParse(TS2, out TI2);
                            Resolution.Width = TI1;
                            Resolution.Height = TI2;
                            if (Resolution.Width > 1920)
                                Resolution.Width = 1920;
                            else
                                if (Resolution.Width < 1000)
                                    Resolution.Width = 1000;
                            if (Resolution.Height > 1080)
                                Resolution.Height = 1080;
                            else
                                if (Resolution.Height < 600)
                                    Resolution.Height = 600;
                            consoleLog = Resolution.Width + "x" + Resolution.Height + " done.";
                            ResolutionResize();
                        }
                        else consoleLog = "Error.";
                    else consoleLog = "Error.";

                switch (consoleString)
                {
                    case "SUITFLASH":
                        if (theHero.Suit != SuitList.Flash)
                        {
                            theHero.Suit = SuitList.Flash;
                            theHeroVelocity = INCREASED_HERO_VELOCITY;
                            foreach (Ball TB in Balls)
                                TB.Speed /= 2f;
                        }
                        consoleLog = "Flash suit is dressed.";
                        break;
                    case "SUITNINJA":
                        theHero.Suit = SuitList.Ninja;
                        consoleLog = "Ninja suit is dressed.";
                        break;
                    case "SUITDEFAULT":
                    case "DEFAULTSUIT":
                    case "DEFSUIT":
                    case "DROPSUIT":
                        if (theHero.Suit != SuitList.Default)
                        {
                            theHero.Suit = SuitList.Default;
                            theHeroVelocity = DEFAULT_HERO_VELOCITY;
                            foreach (Ball TB in Balls)
                                TB.Speed *= 2f;
                        }
                        consoleLog = "Default suit is dressed.";
                        break;
                    case "GETMAGNET":
                        theHero.HandsItem = ObjectList.Magnet;
                        consoleLog = "Magnet received.";
                        break;
                    case "GETWEAPON":
                        theHero.HandsItem = ObjectList.Weapon;
                        consoleLog = "Weapon received.";
                        break;
                    case "GETKEY":
                        theHero.HandsItem = ObjectList.Key;
                        consoleLog = "Key received.";
                        break;
                    case "DROPITEM":
                    case "NONEITEM":
                        theHero.HandsItem = ObjectList.None;
                        consoleLog = "Item removed.";
                        break;
                    case "OUTPUT":
                    case "DEBUG":
                    case "INFORMATION":
                    case "INFO":
                    case "DEBUGINFO":
                    case "DEBUGINFORMATION":
                        if (showDI)
                        {
                            showDI = false;
                            consoleLog = "Debug output is disabled.";
                        }
                        else
                        {
                            showDI = true;
                            consoleLog = "Debug output is enabled.";
                        }
                        break;
                    case "AUTO":
                    case "AUTORES":
                    case "AUTORESOLUTION":
                        Resolution = Screen.PrimaryScreen.Bounds.Size;
                        ResolutionResize();
                        consoleLog = "Automatically " + Resolution.Width + "x" + Resolution.Height + " done.";
                        break;
                    case "RESOLUTION":
                    case "DEFAULTRESOLUTION":
                    case "DEFRESOLUTION":
                    case "DEFAULTRES":
                    case "RES":
                    case "DEFRES":
                        Resolution = new Size(1920, 1080);
                        ResolutionResize();
                        consoleLog = "1920x1080 done.";
                        break;
                }
                consolePrevString = consoleString;
                if (consoleString.Length > 0)
                    consoleString = consoleString.Remove(0);
            }
        }

        class Hero
        {
            public PointF Position;
            public float Direction;
            public int Health = HERO_MAX_LIVES;
            public ObjectList HandsItem = ObjectList.None;
            public SuitList Suit = SuitList.Default;
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
                Points[0] = new PointF(Position.X + Cos(Direction - 90) * 10, Position.Y + Sin(Direction - 90) * 8);
                Points[1] = new PointF(Position.X + Cos(Direction - 30) * 15, Position.Y + Sin(Direction - 30) * 16);
                return Points;
            }
            public PointF[] RightHand()
            {
                PointF[] Points = new PointF[2];
                Points[0] = new PointF(Position.X + Cos(Direction + 90) * 10, Position.Y + Sin(Direction + 90) * 8);
                Points[1] = new PointF(Position.X + Cos(Direction + 30) * 15, Position.Y + Sin(Direction + 30) * 16);
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
                    GameState = ProgramState.Dialog;
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
            public Boolean Exist = true;
            public Color Color = Color.AliceBlue;
            public int Health = 3;

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
            public void Damage()
            {
                Health--;
                if (Health < 1)
                    Exist = false;
            }
        }

        class Projectile
        {
            public PointF Position;
            public float Direction;
            public Boolean Exist = true;
            public Projectile(PointF _Position, float _Direction)
            {
                Position = _Position;
                Direction = _Direction;
            }
            public void Move()
            {
                PointF next = new PointF(Position.X + theHero.Cos(Direction) * PROJECTILE_SPEED / (theHero.Suit == SuitList.Flash ? 3 : 1), Position.Y + theHero.Sin(Direction) * PROJECTILE_SPEED / (theHero.Suit == SuitList.Flash ? 3 : 1));
                foreach (Box TB in Boxes)
                    if (TB.Type == BoxType.Wooden && TB.RectangleF().Contains(next))
                        TB.Damage();
                foreach (Ball TB in Balls)
                {
                    float x = TB.Position.X + (Position.X > TB.Position.X ? 8 : 0),
                          y = TB.Position.Y + (Position.Y > TB.Position.Y ? 8 : 0),
                          dist = (float)Math.Sqrt(Math.Pow(Position.X - x, 2) + Math.Pow(Position.Y - y, 2));
                    if (dist < 18)
                    {
                        TB.Damage();
                        Exist = false;
                    }
                }
                if (!reg.IsVisible(next))
                    Exist = false;
                Position = next;
            }
        }

        class Box
        {
            public PointF Position;
            public BoxType Type;
            public int Health = 5;
            public Boolean Exist = true;
            public SizeF Size = new SizeF(32f, 32f);
            public Box(PointF _Position, BoxType _Type)
            {
                Position = _Position;
                Type = _Type;
            }
            public Rectangle Rectangle()
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.Width, (int)Size.Height);
            }
            public RectangleF RectangleF()
            {
                return new RectangleF(Position, Size);
            }
            public void Damage()
            {
                if (Type == BoxType.Wooden)
                    Health--;
                if (Health < 1)
                    Exist = false;
            }
            public void CheckFalling()
            {
                int PointsCounter = 0;
                PointF[] Points = new PointF[4]
                {
                    new PointF(RectangleF().X, RectangleF().Y),
                    new PointF(RectangleF().X, RectangleF().Y + 32),
                    new PointF(RectangleF().X + 32, RectangleF().Y),
                    new PointF(RectangleF().X + 32, RectangleF().Y + 32)
                };
                for (int a = 0; a < 4; ++a)
                    if (!reg.IsVisible(Points[a]))
                        PointsCounter++;
                if (PointsCounter > 3)
                    if (Size.Width > 2f)
                    {
                        Size.Width -= 0.5f;
                        Size.Height -= 0.5f;
                    }
                    else Exist = false;
            }
        }

        class BackGroundColor
        {
            public int R;
            public int G;
            public int B;
            public int RFactor = getRandom.Next(-1, 2);
            public int GFactor = getRandom.Next(-1, 2);
            public int BFactor = getRandom.Next(-1, 2);
            public BackGroundColor(int _R, int _G, int _B)
            {
                R = _R;
                G = _G;
                B = _B;
            }
            public void Increase(int r, int g, int b)
            {
                R += r;
                G += g;
                B += b;
                this.Limits();
            }
            public void Increase(Boolean Factor)
            {
                R += RFactor;
                G += GFactor;
                B += BFactor;
                this.Limits();
            }
            public void Limits()
            {
                if (R > COLOR_MAX_VALUE)
                    RFactor = -1;//R = COLOR_MIN_VALUE;
                else
                    if (R < COLOR_MIN_VALUE)
                        RFactor = 1;//R = COLOR_MAX_VALUE;
                if (G > COLOR_MAX_VALUE)
                    GFactor = -1;//G = COLOR_MIN_VALUE;
                else
                    if (G < COLOR_MIN_VALUE)
                        GFactor = 1;//G = COLOR_MAX_VALUE;
                if (B > COLOR_MAX_VALUE)
                    BFactor = -1;//B = COLOR_MIN_VALUE;
                else
                    if (B < COLOR_MIN_VALUE)
                        BFactor = 1;//B = COLOR_MAX_VALUE;
            }
            public void randomFactors()
            {
            Mark:
                RFactor = getRandom.Next(-1, 2);
                GFactor = getRandom.Next(-1, 2);
                BFactor = getRandom.Next(-1, 2);
                if (RFactor == 0 && GFactor == 0 && BFactor == 0)
                    goto Mark;
            }
            public Color Set()
            {
                return Color.FromArgb(R, G, B);
            }
            public void setRandomColor()
            {
                R = getRandom.Next(COLOR_MIN_VALUE, COLOR_MAX_VALUE);
                G = getRandom.Next(COLOR_MIN_VALUE, COLOR_MAX_VALUE);
                B = getRandom.Next(COLOR_MIN_VALUE, COLOR_MAX_VALUE);
            }
        }

        #region Variables
        static Size
            Resolution = new Size(1920, 1080);
        GraphicsPath
            CirclePath = new GraphicsPath();
        static ConsolePrototype
            Console = new ConsolePrototype();
        PointF[] HeroLastPoints = new PointF[10];
        Point MouseAiming = new Point(960, 540);
        Point ViewOffset = new Point(785, -85);
        int lastTick, lastFrameRate, frameRate,
            UnstuckIndex = 0, SelectedBall = -1, BallCount = 10, BGInterval = 365;
        static float BallSpeed = 1f, ProjectileCooldown = 0f, AimingDuration = 0f,
            theHeroVelocity = 1.5f;
        string AimingInformation = "";
        PointF AimingPosition = new Point(960, 540);
        RectangleF AimingRectangle;
        static Region reg;
        static Boolean
            showDI = false, PressedForward = false, PressedLeft = false, PressedRight = false,
            AnyBallSelected, SpeedChangerSelected, CountChangerSelected, BGSpeedChangerSelected, AimingEnabled = true, MagnetUsing = false;
        static List<Ball> Balls = new List<Ball>();
        static List<Door> Doors = new List<Door>();
        static List<Box> Boxes = new List<Box>();
        static List<Projectile> Projectiles = new List<Projectile>();
        static Hero theHero;
        BackGroundColor BGColor;
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
            InitialSetup();
            updateTimer.Interval = 1;
            secondaryTimer.Interval = BGInterval;
            updateTimer.Tick += new EventHandler(pUpdate);
            secondaryTimer.Tick += new EventHandler(SecondaryLoop);
            updateTimer.Start();
            secondaryTimer.Start();
        }

        private void InitialSetup()
        {
            GameState = ProgramState.Active;
            TextFormatCenter.Alignment = StringAlignment.Center;
            BGColor = new BackGroundColor(90, 90, 90);
            ResolutionResize();
            for (int a = 0; a < HeroLastPoints.Length; ++a)
                HeroLastPoints[a] = HERO_INITIAL_POSITION.Location;
            theHero = new Hero(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25, 0f);
            CirclePath.AddEllipse(CircleRectangle);
            Boxes.Add(new Box(new PointF(300, 600), BoxType.Wooden));
            Boxes.Add(new Box(new PointF(350, 750), BoxType.Steel));
            Doors.Add(new Door(new Rectangle(new Point(500, 200), new Size(10, 50)), new Point(500, 200), new Point(500, 400), new Point(0, 1)));
            Doors.Add(new Door(new Rectangle(new Point(300, 500), new Size(50, 10)), new Point(200, 500), new Point(400, 500), new Point(1, 0)));
            RegionRefresh();
            for (int a = 0; a < BallCount; ++a)
                Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 18) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
        }

        private static void ResolutionResize()
        {
            HealthRectangle.Y = Resolution.Height - 75;
            SettingRectangle.X = Resolution.Width / 2 - 100;
            SettingRectangle.Y = Resolution.Height / 2 - 150;
            SpeedChangerRectangle.X = CountChangerRectangle.X = BGSpeedChangerRectangle.X = Resolution.Width / 2;
            CountChangerRectangle.Y = SettingRectangle.Y + 255;
            SpeedChangerRectangle.Y = SettingRectangle.Y + 205;
            BGSpeedChangerRectangle.Y = SettingRectangle.Y + 155;
            AimingCheckBoxRectangle.X = SettingRectangle.X + 90;
            AimingCheckBoxRectangle.Y = SettingRectangle.Y + 35;
            TryAgainRectangle.X = Resolution.Width / 2 - 160;
            TryAgainRectangle.Y = Resolution.Height / 2 - 120;
            OkRectangle.X = Resolution.Width / 2 - 120;
            OkRectangle.Y = ExitRectangle.Y = TryAgainRectangle.Y + 70;
            ExitRectangle.X = OkRectangle.X + 160;
        }

        void SecondaryLoop(object sender, EventArgs e)
        {
            BGColor.Increase(true);
            this.BackColor = BGColor.Set();
            if (getRandom.Next(50) == 0)
                BGColor.randomFactors();
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
            reg.Union(new Rectangle(1550, 350, 500, 50));
            reg.Exclude(new Rectangle(300, 650, 150, 5));
            reg.Exclude(new Rectangle(560, 350, 340, 4));
            reg.Exclude(new Rectangle(900, 350, 2, 300));
            foreach (Box TB in Boxes)
                if (TB.Exist)
                    reg.Exclude(new RectangleF(TB.Position, new SizeF(32, 32)));
            foreach (Door TD in Doors)
                reg.Xor(TD.Rectangle);
        }

        //PointF getRandomPointInRegion(GraphicsPath _path)
        //{
        //    PointF TP;
        //Mark:
        //    TP = new PointF(getRandom.Next(1920), getRandom.Next(1080));
        //    if (_path.IsVisible(TP))
        //        return TP;
        //    goto Mark;
        //}

        PointF getRandomPointInRegion(Region _reg)
        {
            Point TP;
        Mark:
            TP = new Point(getRandom.Next(1920), getRandom.Next(1080));
            foreach (Door TD in Doors)
            {
                if (_reg.IsVisible(TP) && !TD.Rectangle.Contains(TP) && !HERO_INITIAL_POSITION.Contains(TP))
                    return TP;
            }
            goto Mark;
        }

        void HandItemAction()
        {
            switch (theHero.HandsItem)
            {
                case ObjectList.Weapon:
                    if (ProjectileCooldown >= PROJECTILE_COOLDOWN * (theHero.Suit == SuitList.Flash ? 2.5f : 1))
                    {
                        Projectiles.Add(new Projectile(theHero.Position, theHero.Direction));
                        ProjectileCooldown = 0f;
                    }
                    break;
                case ObjectList.Key:
                    break;
            }
        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                if (GameState != ProgramState.Settings && GameState != ProgramState.Dialog)
                    Application.Exit();
                else
                    GameState = ProgramState.Active;
            if (Console.Enabled)
                #region Console
            {
                if ((e.KeyData >= Keys.A && e.KeyData <= Keys.Z) ||
                    (e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9))
                {
                    string temp = Console.getString();
                    temp += (char)e.KeyValue;
                    Console.setString(temp);
                }
                switch (e.KeyData)
                {
                    case Keys.Back:
                        if (Console.getLength() > 0)
                        {
                            string temp = Console.getString();
                            int tempint = Console.getLength() - 1;
                            temp = temp.Substring(0, tempint);
                            Console.setString(temp);
                        }
                        break;
                    case Keys.Enter:
                        if (!String.IsNullOrEmpty(Console.getString()))
                            Console.applyCommand();
                        break;
                }
            }
            #endregion
            else
                #region Not Console
                switch (e.KeyData)
                {
                    case Keys.F1:
                        if (GameState == ProgramState.Active || GameState == ProgramState.Paused)
                            GameState = ProgramState.Settings;
                        else
                            GameState = ProgramState.Active;
                        break;
                    case Keys.F2:
                        GameState = ProgramState.Dialog;
                        break;
                    case Keys.Space:
                        MagnetUsing = true;
                        HandItemAction();
                        break;
                    case Keys.G:
                        theHero.HandsItem = ObjectList.None;
                        break;
                    case Keys.P:
                        if (GameState == ProgramState.Active)
                            GameState = ProgramState.Paused;
                        else if (GameState == ProgramState.Paused)
                            GameState = ProgramState.Active;
                        break;
                    case Keys.R:
                        foreach (Ball TB in Balls)
                            TB.Position = getRandomPointInRegion(reg);
                        break;
                    case Keys.B:
                        BGColor.randomFactors();
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
                #endregion
        }
        void pKeyUp(object sender, KeyEventArgs e)
        {
            if (!Console.Enabled)
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
                    case Keys.Space:
                        MagnetUsing = false;
                        break;
                }
            if (e.KeyData == Keys.Tab)
                if (!Console.Enabled && GameState != ProgramState.Paused)
                    Console.Enabled = true;
                else
                {
                    Console.Enabled = false;
                    if (!String.IsNullOrEmpty(Console.getString()))
                    {
                        string temp = Console.getString();
                        temp = temp.Remove(0);
                        Console.setString(temp);
                    }
                }
        }

        void pMouseMove(object sender, MouseEventArgs e)
        {
            Point ClickOffset = new Point(e.X - ViewOffset.X, e.Y - ViewOffset.Y);
            if (AnyBallSelected)
            {
                Point[] PP = new Point[4]
                {
                    new Point(ClickOffset.X - 9, ClickOffset.Y - 9),
                    new Point(ClickOffset.X - 9, ClickOffset.Y + 18),
                    new Point(ClickOffset.X + 18, ClickOffset.Y + 18),
                    new Point(ClickOffset.X + 9, ClickOffset.Y - 18)
                };
                for (int b = 0; b < 4; ++b)
                    if (reg.IsVisible(PP[b]) && !new Region(HERO_INITIAL_POSITION).IsVisible(PP[b]))
                        Balls[SelectedBall].Position = ClickOffset;
                    else
                    {
                        SelectedBall = -1;
                        AnyBallSelected = false;
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

            if (BGSpeedChangerSelected && BGSpeedChangerRectangle.Contains(e.Location))
                BGInterval = (e.X - BGSpeedChangerRectangle.Left) * 5;
            if (BGInterval < BG_INTERVAL_MIN)
                BGInterval = BG_INTERVAL_MIN;
            else
                if (BGInterval > BG_INTERVAL_MAX)
                    BGInterval = BG_INTERVAL_MAX;
            secondaryTimer.Interval = BG_INTERVAL_MAX - BGInterval + BG_INTERVAL_MIN;
            if (CountChangerSelected)
            {
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
            MouseAiming = ClickOffset;
        }

        void pMouseUp(object sender, MouseEventArgs e)
        {
            Point ClickOffset = new Point(e.X - ViewOffset.X, e.Y - ViewOffset.Y);
            AnyBallSelected = false;
            SpeedChangerSelected = false;
            CountChangerSelected = false;
            BGSpeedChangerSelected = false;
            SelectedBall = -1;
            if (AimingCheckBoxRectangle.Contains(e.Location))
                if (AimingEnabled)
                    AimingEnabled = false;
                else
                    AimingEnabled = true;
            if (e.Button == MouseButtons.Right)
                theHero.Position = new PointF(ClickOffset.X, ClickOffset.Y);
        }

        void pMouseDown(object sender, MouseEventArgs e)
        {
            AimingInformation = "";
            Point ClickOffset = new Point(e.X - ViewOffset.X, e.Y - ViewOffset.Y);
            if (!AnyBallSelected)
            {
                double dist;
                for (int a = 0; a < Balls.Count; ++a)
                {
                    double x = Balls[a].Position.X,
                           y = Balls[a].Position.Y;
                    dist = Math.Sqrt(Math.Pow(x - ClickOffset.X, 2) + Math.Pow(y - ClickOffset.Y, 2));
                    if (dist < 15)
                    {
                        AnyBallSelected = true;
                        SelectedBall = a;
                        break;
                    }
                }
            }
            if (!SpeedChangerSelected)
                if (SpeedChangerRectangle.Contains(e.Location) && GameState == ProgramState.Settings)
                    SpeedChangerSelected = true;
            if (!CountChangerSelected)
                if (CountChangerRectangle.Contains(e.Location) && GameState == ProgramState.Settings)
                    CountChangerSelected = true;
            if (!BGSpeedChangerSelected)
                if (BGSpeedChangerRectangle.Contains(e.Location) && GameState == ProgramState.Settings)
                    BGSpeedChangerSelected = true;

            if (GameState == ProgramState.Dialog)
            {
                if (OkRectangle.Contains(e.Location))
                {
                    theHero.Health = HERO_MAX_LIVES;
                    theHero.Position = new PointF(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25);
                    theHero.Direction = 0f;
                    BallCount = 10;
                    BallSpeed = 1f;
                    foreach (Ball TB in Balls)
                        TB.Position = getRandomPointInRegion(reg);
                    GameState = ProgramState.Active;
                }
                else
                    if (ExitRectangle.Contains(e.Location))
                        Application.Exit();
            }
            pMouseMove(sender, e);
        }

        void pUpdate(object sender, EventArgs e)
        {
            ViewOffset.X = -((int)theHero.Position.X - Resolution.Width / 2);
            ViewOffset.Y = -((int)theHero.Position.Y - Resolution.Height / 2);
            if (AimingEnabled)
            #region Aiming Information
            {
                if (!String.IsNullOrEmpty(AimingInformation))
                    if (AimingDuration < AIMING_DURATION)
                        AimingDuration += 0.01f;
                    else
                        AimingInformation = "";
                double dist2;
                foreach (Ball TB in Balls)
                {
                    dist2 = Math.Sqrt(Math.Pow(TB.Position.X - MouseAiming.X, 2) + Math.Pow(TB.Position.Y - MouseAiming.Y, 2));
                    if (dist2 < 15 && TB.Exist)
                    {
                        AimingDuration = 0f;
                        AimingInformation = " Object: Enemy\n Health: " + TB.Health.ToString() + "/" + BALL_MAX_HEALTH + "\n Velocity: " + (TB.Speed > theHeroVelocity ? "HIGH" : TB.Speed < theHeroVelocity ? "LOW" : "NORMAL");
                        float width = 3 * iGlassPanelSimple.Width / 4,
                              height = 3 * iGlassPanelSimple.Height / 4;
                        AimingPosition = new PointF(TB.Position.X + ViewOffset.X - width / 2, TB.Position.Y + ViewOffset.Y - height);
                        AimingRectangle = new RectangleF(AimingPosition, new SizeF(width + (TB.Speed == theHeroVelocity ? 20 : -3), height - 5));
                    }
                }
                foreach (Box TB in Boxes)
                {
                    if (TB.Rectangle().Contains(MouseAiming) && TB.Exist)
                    {
                        AimingDuration = 0f;
                        AimingInformation = "Object: " + (TB.Type == BoxType.Wooden ? "Wooden" : "Steel") + " Box\nDurability: " + (TB.Type == BoxType.Wooden ? TB.Health.ToString() + "/" + BOX_MAX_HEALTH : "Infinite");
                        float width = 9 * iGlassPanelSimple.Width / 10,
                              height = iGlassPanelSimple.Height / 2;
                        AimingPosition = new PointF(TB.Position.X + ViewOffset.X - width / 2, TB.Position.Y + ViewOffset.Y - height);
                        AimingRectangle = new RectangleF(AimingPosition, new SizeF(width - (TB.Type == BoxType.Steel ? 1 : 0), height));
                    }
                }
                foreach (Door TD in Doors)
                {
                    if (TD.Rectangle.Contains(MouseAiming))
                    {
                        AimingDuration = 0f;
                        AimingInformation = "Object: Door\nSpeed: Normal\nDirection: " + (TD.Direction.X != 0 ? "Horizontal" : "Vertical");
                        float width = 9 * iGlassPanelSimple.Width / 10,
                              height = 3 * iGlassPanelSimple.Height / 4;
                        AimingPosition = new PointF(TD.Rectangle.X + ViewOffset.X - width / 2, TD.Rectangle.Y + ViewOffset.Y - height);
                        AimingRectangle = new RectangleF(AimingPosition, new SizeF(width + (TD.Direction.X != 0 ? 12 : -3), height - 5));
                    }
                }
                dist2 = Math.Sqrt(Math.Pow(theHero.Position.X - MouseAiming.X, 2) + Math.Pow(theHero.Position.Y - MouseAiming.Y, 2));
                if (dist2 < 15)
                {
                    AimingDuration = 0f;
                    AimingInformation = "Object: You\nEquipment: " + theHero.HandsItem.ToString();
                    float width = 4 * iGlassPanelSimple.Width / 5,
                          height = iGlassPanelSimple.Height / 2;
                    AimingPosition = new PointF(theHero.Position.X + ViewOffset.X - width / 2, theHero.Position.Y + ViewOffset.Y - 1.4f * height);
                    AimingRectangle = new RectangleF(AimingPosition, new SizeF(width + (theHero.HandsItem != ObjectList.None && theHero.HandsItem != ObjectList.Key ? 20 : 0), height));
                }
            }
                #endregion

            switch (GameState)
            {
                case ProgramState.Active:
                    if (reg.IsVisible(theHero.Position))
                        HeroLastPoints[UnstuckIndex] = theHero.Position;
                    if (UnstuckIndex < HeroLastPoints.Length - 1)
                        UnstuckIndex++;
                    else
                        UnstuckIndex = 0;
                    if (!reg.IsVisible(theHero.Position))
                        theHero.Position = HeroLastPoints[UnstuckIndex];

                    ProjectileCooldown += 0.01f;

                    #region Moving
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
                        PointF next = new PointF(theHero.Position.X + theHeroVelocity * theHero.Cos(), theHero.Position.Y + theHeroVelocity * theHero.Sin()),
                               nextEdge = new PointF(theHero.Position.X + 20f * theHero.Cos(), theHero.Position.Y + 20f * theHero.Sin());
                        foreach (Box TB in Boxes)
                            if (TB.Exist)
                                if (TB.Type == BoxType.Steel && theHero.HandsItem == ObjectList.Magnet && MagnetUsing)
                                    if (TB.RectangleF().Contains(nextEdge))
                                    {
                                        TB.Position.X += theHero.Cos() * (theHero.Suit == SuitList.Flash ? 3 : 1);
                                        TB.Position.Y += theHero.Sin() * (theHero.Suit == SuitList.Flash ? 3 : 1);
                                    }
                        PointF[] nextPoints = new PointF[]
                        {
                            new PointF(next.X + HERO_WIDTH / 3 * theHero.Cos(theHero.Direction - 45), next.Y + HERO_HEIGHT / 3 * theHero.Sin(theHero.Direction - 45)),
                            new PointF(next.X + HERO_WIDTH / 3 * theHero.Cos(theHero.Direction + 45), next.Y + HERO_HEIGHT / 3 * theHero.Sin(theHero.Direction + 45)),
                            new PointF(next.X + HERO_WIDTH / 2 * theHero.Cos(), next.Y + HERO_HEIGHT / 2 * theHero.Sin())
                        };
                        if (reg.IsVisible(nextPoints[0]) && reg.IsVisible(nextPoints[1]) && reg.IsVisible(nextPoints[2]))
                            theHero.Position = next;
                    }
                    #endregion

                    #region Projectiles, Doors, Boxes
                    if (Projectiles.Count > 0)
                        for (int p = 0; p < Projectiles.Count; ++p)
                            if (Projectiles[p].Exist)
                                Projectiles[p].Move();
                            else
                                Projectiles.Remove(Projectiles[p]);

                    foreach (Door TD in Doors)
                    {
                        reg.MakeEmpty();
                        if (TD.Direction.Y != 0)
                            if ((TD.Direction.Y > 0 && TD.Rectangle.Y < TD.End.Y) ||
                                 TD.Direction.Y < 0 && TD.Rectangle.Y > TD.Start.Y)
                                TD.Rectangle.Y += TD.Direction.Y * (theHero.Suit == SuitList.Flash ? 1 : 2);
                            else
                                TD.Direction.Y = -TD.Direction.Y;
                        if (TD.Direction.X != 0)
                            if ((TD.Direction.X > 0 && TD.Rectangle.X < TD.End.X) ||
                                 TD.Direction.X < 0 && TD.Rectangle.X > TD.Start.X)
                                TD.Rectangle.X += TD.Direction.X * (theHero.Suit == SuitList.Flash ? 1 : 2);
                            else
                                TD.Direction.X = -TD.Direction.X;
                        RegionRefresh();
                    }

                    foreach (Box TB in Boxes)
                        TB.CheckFalling();
                    #endregion

                    #region Balls
                    for (int a = 0; a < Balls.Count; ++a)
                    {
                        if (!Balls[a].Exist)
                        {
                            Balls.Remove(Balls[a]);
                            BallCount--;
                            break;
                        }
                        if (Balls[a].Detected)
                            if (Balls[a].Duration < DETECTION_TIME)
                                Balls[a].Duration += 0.01f;
                            else
                                theHero.DoDamage(Balls[a]);
                        float x = Balls[a].Position.X + (theHero.Position.X > Balls[a].Position.X ? 8 : 0),
                              y = Balls[a].Position.Y + (theHero.Position.Y > Balls[a].Position.Y ? 8 : 0),
                              dist = (float)Math.Sqrt(Math.Pow(theHero.Position.X - x, 2) + Math.Pow(theHero.Position.Y - y, 2));
                        if (!new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position) && theHero.Suit != SuitList.Ninja)
                            if (dist >= 25 && dist <= ENEMY_VIEW.Width / 4f)
                                Balls[a].Direction = theHero.AngleBetween(theHero.Position, Balls[a].Position);
                            else if (dist >= ENEMY_VIEW.Width / 4f && dist <= ENEMY_VIEW.Width / 3f)
                                Balls[a].Direction = theHero.AngleBetween(theHero.Position, new PointF((theHero.Position.X + Balls[a].Position.X) / 2f, (theHero.Position.Y + Balls[a].Position.Y) / 2f));
                        if ((dist < 18 && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)) ||
                        (Balls[a].RefreshView().IsVisible(theHero.Position) && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)))
                        {
                            if (dist > 18)
                            {
                                if (theHero.Suit != SuitList.Ninja)
                                {
                                    Balls[a].Detected = true;
                                    Balls[a].Color = Color.FromArgb(245, 158, 162);
                                    Balls[a].Direction = theHero.AngleBetween(theHero.Position, Balls[a].Position);
                                }
                            }
                            else
                                theHero.DoDamage(Balls[a]);
                        }
                        else
                        {
                            Balls[a].Detected = false;
                            Balls[a].Duration = 0f;
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
                    #endregion

                    break;
                //case ProgramState.Settings:
                //    break;
            }

            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(ViewOffset.X, ViewOffset.Y);

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
            PointF[] Pos = new PointF[]
            {
                new PointF(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new PointF(HERO_INITIAL_POSITION.Right - 1 + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Top - HERO_INITIAL_POSITION.Height / 8),
                new PointF(HERO_INITIAL_POSITION.Right - 1 + HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom - 1 + HERO_INITIAL_POSITION.Height / 8),
                new PointF(HERO_INITIAL_POSITION.X - HERO_INITIAL_POSITION.Width / 8, HERO_INITIAL_POSITION.Bottom - 1 + HERO_INITIAL_POSITION.Height / 8)
            };
            float[] angles = new float[] { 45, 135, 225, 315 };
            for (int p = 0; p < Pos.Length; ++p)
                g.DrawLine(new Pen(Color.FromArgb(100, 149, 237)), Pos[p], new PointF(Pos[p].X + 9 * theHero.Cos(angles[p]), Pos[p].Y + 9 * theHero.Sin(angles[p])));
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

            //foreach (Door TD in Doors)
            //    g.DrawRectangle(Pens.Black, TD.Rectangle);

            #region Boxes & Projectiles
            foreach (Box TB in Boxes)
                if (TB.Exist)
                    g.DrawImage(iBox[(int)TB.Type], TB.Rectangle());

            foreach (Projectile TP in Projectiles)
                if (TP.Exist)
                    g.FillEllipse(Brushes.Black, TP.Position.X, TP.Position.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
            #endregion

            #region DrawHero
            switch (theHero.HandsItem)
            {
                case ObjectList.None:
                    g.DrawLine(new Pen(Color.Orange, 5), theHero.LeftHand()[0], theHero.LeftHand()[1]);
                    g.DrawLine(new Pen(Color.Orange, 5), theHero.RightHand()[0], theHero.RightHand()[1]);
                    break;
                case ObjectList.Weapon:
                    g.DrawLine(new Pen(Color.FromArgb(190, 70, 70, 70), 5), theHero.LeftHand()[0], theHero.LeftHand()[1]);
                    g.DrawLine(new Pen(Color.FromArgb(190, 70, 70, 70), 5), theHero.RightHand()[0], theHero.RightHand()[1]);
                    break;
                case ObjectList.Magnet:
                    g.DrawLine(new Pen(Color.Blue, 5), theHero.LeftHand()[0], theHero.LeftHand()[1]);
                    g.DrawLine(new Pen(Color.Red, 5), theHero.RightHand()[0], theHero.RightHand()[1]);
                    break;
            }

            switch (theHero.Suit)
            {
                case SuitList.Default:
                    g.DrawImage(iCircle, new RectangleF(theHero.DrawPosition(), new SizeF(24, 24)));
                    break;
                case SuitList.Flash:
                    for (int a = 0; a < HeroLastPoints.Length; ++a)
                        g.FillEllipse(new SolidBrush(Color.FromArgb(10 + 10 * a, 200, 50, 50)), new RectangleF(new PointF(HeroLastPoints[a].X - 12, HeroLastPoints[a].Y - 12), new SizeF(24, 24)));
                    TextureBrush tb = new TextureBrush(iFlash, new Rectangle(0, 0, 50, 50));
                    tb.RotateTransform(theHero.Direction);
                    g.FillEllipse(tb, new RectangleF(theHero.DrawPosition(), new SizeF(24, 24)));
                    
                    break;
                case SuitList.Ninja:
                    g.DrawImage(iCircle, new RectangleF(theHero.DrawPosition(), new SizeF(24, 24)));
                    g.FillEllipse(new SolidBrush(Color.FromArgb(190, 30, 30, 30)), new RectangleF(theHero.DrawPosition(), new SizeF(24, 24)));
                    break;
            }

            g.FillEllipse(theHero.Suit == SuitList.Ninja ? Brushes.Beige : Brushes.CornflowerBlue, theHero.Position.X - 3 + theHero.Cos(theHero.Direction - 45) * 6, theHero.Position.Y - 3 + theHero.Sin(theHero.Direction - 45) * 5, 4, 4);
            g.FillEllipse(theHero.Suit == SuitList.Ninja ? Brushes.Beige : Brushes.CornflowerBlue, theHero.Position.X - 3 + theHero.Cos(theHero.Direction + 45) * 6, theHero.Position.Y - 3 + theHero.Sin(theHero.Direction + 45) * 5, 4, 4);
            #endregion

            //Rectangle viewRect = new Rectangle((int)theHero.Position.X - HERO_VIEW.Width / 2 + 1, (int)theHero.Position.Y - HERO_VIEW.Height / 2 + 1, HERO_VIEW.Width, HERO_VIEW.Height);
            //g.FillPie(viewBrush, viewRect, theHero.Direction - 45, 90);
            
            g.ResetTransform();
            
            if (AimingEnabled)
                if (!String.IsNullOrEmpty(AimingInformation))
                {
                    g.DrawImage(iGlassPanelSimple, AimingRectangle);
                    g.DrawString(AimingInformation, new Font("QUARTZ MS", 12), Brushes.Black, AimingPosition.X + (AimingRectangle.Width >= 9 * iGlassPanelSimple.Width / 10 ? 3 : 7), AimingPosition.Y + 4);
                }

            g.DrawString("Lives", Nirmala20, textLB, new Point(HealthRectangle.X + 16, HealthRectangle.Y + 10));
            g.DrawImage(iPanelGlass, HealthRectangle);
            for (int s = 0; s < HERO_MAX_LIVES; ++s)
                g.DrawImage(s > theHero.Health - 1 ? iBlock[0] : iBlock[1], HealthRectangle.X + 93 + 27 * s, HealthRectangle.Y + 18);

            if (showDI)
            #region Debug Information
            {
                string output = "Status: " + GameState.ToString() + "\nFPS: " + CalculateFrameRate().ToString();
                output += "\nPosition: " + Math.Round(theHero.Position.X).ToString() + "; " + Math.Round(theHero.Position.Y).ToString();
                output += "\nR " + BGColor.R + "; G " + BGColor.G + "; B " + BGColor.B + "\n" + MouseAiming.ToString() + "\n";
                g.DrawString(output, new Font("QUARTZ MS", 14), Brushes.Black, 0, Console.Enabled ? 50 : 0);
            }
            #endregion

            if (Console.Enabled)
            #region Console
            {
                g.DrawString("Console: ", Verdana13, Brushes.Black, 3, 25);
                g.DrawString(Console.getLog(), Verdana13, Brushes.Black, Console.getRegion().X + 3, Console.getRegion().Y);
                g.DrawString(Console.getPrevString(), Verdana13, Brushes.Black, new Rectangle(100, 0, 423, 20), TextFormatCenter);
                g.DrawString(Console.getString(), Verdana13, Brushes.Black, new Rectangle(81, 25, 460, 20));
                g.DrawImage(iGlassPanelConsole, Console.getRegion());
            }
            #endregion

            #region MenuDraw
            switch (GameState)
            {
                case ProgramState.Dialog:
                    g.DrawImage(iGlassPanelCorners, TryAgainRectangle);
                    g.DrawString("Do you want try again ?", Verdana17, Brushes.Black, TryAgainRectangle.X + 18, TryAgainRectangle.Y + 20);
                    g.DrawString("Again", Verdana15, Brushes.Black, OkRectangle.X + 8, OkRectangle.Y + 1);
                    g.DrawString("Exit", Verdana15, Brushes.Black, ExitRectangle.X + 17, ExitRectangle.Y + 1);
                    g.DrawImage(iGlassPanelCorners, OkRectangle);
                    g.DrawImage(iGlassPanelCorners, ExitRectangle);
                    break;
                case ProgramState.Settings:
                    g.DrawImage(iSettingBG, SettingRectangle);
                    g.DrawImage(AimingEnabled ? iCheckBox : iCheckBoxEmpty, AimingCheckBoxRectangle);
                    g.DrawString("Aiming\nInformation", Kristen15, textRB, new Point(AimingCheckBoxRectangle.X - 80, AimingCheckBoxRectangle.Y));
                    g.DrawString("Settings", Verdana15, new SolidBrush(Color.FromArgb(60, 60, 60)), SettingRectangle.X + 2, SettingRectangle.Y);
                    g.DrawString("Velocity", Kristen15, textRB, new Point(SpeedChangerRectangle.X - 90, SpeedChangerRectangle.Y));
                    g.DrawString("Amount", Kristen15, textRB, new Point(CountChangerRectangle.X - 90, CountChangerRectangle.Y));
                    g.DrawString("Rate of \nBackground\nChange", Kristen15, textRB, new Point(BGSpeedChangerRectangle.X - 90, BGSpeedChangerRectangle.Y - 50));

                    g.DrawImage(iBars[3], SpeedChangerRectangle.Location);
                    g.DrawImage(iBars[6], CountChangerRectangle.Location);
                    g.DrawImage(iBars[9], BGSpeedChangerRectangle.Location);
                    for (int s = 0; s < BALL_MAX_SPEED; ++s)
                    {
                        g.DrawImage(iBars[1], new Rectangle(SpeedChangerRectangle.X + 6 + s * 8, SpeedChangerRectangle.Y, 8, iBars[1].Height));
                        g.DrawImage(iBars[1], new Rectangle(CountChangerRectangle.X + 6 + s * 8, CountChangerRectangle.Y, 8, iBars[4].Height));
                        g.DrawImage(iBars[1], new Rectangle(BGSpeedChangerRectangle.X + 6 + s * 8, BGSpeedChangerRectangle.Y, 8, iBars[7].Height));
                    }
                    for (float s = 0; s < BallSpeed * 4f; ++s)
                        g.DrawImage(iBars[4], new RectangleF(SpeedChangerRectangle.X + 6 + s * 2, SpeedChangerRectangle.Y, 2, iBars[4].Height));
                    for (float s = 0; s < BallCount / 2.5f; ++s)
                        g.DrawImage(iBars[7], new RectangleF(CountChangerRectangle.X + 6 + s * 2, CountChangerRectangle.Y, 2, iBars[7].Height));
                    for (float s = 0; s < (BGInterval - 10) / 10f; ++s)
                        g.DrawImage(iBars[10], new RectangleF(BGSpeedChangerRectangle.X + 6 + s * 2, BGSpeedChangerRectangle.Y, 2, iBars[10].Height));
                    g.DrawImage(BallSpeed > BALL_MAX_SPEED - 0.25f ? iBars[5] : iBars[2], SpeedChangerRectangle.X + 6 + BALL_MAX_SPEED * 8, SpeedChangerRectangle.Y);
                    g.DrawImage(BallCount >= BALL_MAX_COUNT ? iBars[8] : iBars[2], CountChangerRectangle.X + 6 + BALL_MAX_SPEED * 8, CountChangerRectangle.Y);
                    g.DrawImage(BGInterval > BG_INTERVAL_MAX + 1 - BG_INTERVAL_MIN ? iBars[11] : iBars[2], BGSpeedChangerRectangle.X + 6 + BALL_MAX_SPEED * 8, BGSpeedChangerRectangle.Y);
                    break;
            }
            #endregion
        }
    }
}

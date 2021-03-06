﻿using System;
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
            BALL_MAX_HEALTH = 3,
            BOX_MAX_HEALTH = 5,
            BOX_DEFAULT_SIZE = 32,
            HERO_WIDTH = 24,
            HERO_HEIGHT = 24,
            HERO_MAX_LIVES = 6,
            COLOR_MIN_VALUE = 63,
            COLOR_MAX_VALUE = 192,
            BG_INTERVAL_MIN = 5,
            BG_INTERVAL_MAX = 410,
            PROJECTILE_SIZE = 3,
            PROJECTILE_SPEED = 8,
            REGION_COUNT = 10,
            TEMPORARY_REGION_MIN_SIZE = 4,
            TEMPORARY_REGION_MAX_SIZE = 100;
        const float
            INTRO_DURATION = 4.8f,
            HERO_TURN_RATE = 4.8f,
            DETECTION_TIME = 1.8f,
            PROJECTILE_COOLDOWN = 0.2f,
            DEFAULT_HERO_VELOCITY = 1.5f,
            INCREASED_HERO_VELOCITY = 3.5f,
            AIMING_DURATION = 0.8f,
            TEMPORARY_REGION_DURATION = 5f,
            TEMPORARY_REGION_COOLDOWN = 1f,
            MESSAGE_DURATION = 4.0f,
            FALLING_TIME_LIMIT = 0.6f;
        readonly static Size
            HERO_SIZE = new Size(HERO_WIDTH, HERO_HEIGHT),
            ENEMY_VIEW = new Size(125, 125),
            HERO_VIEW = new Size(150, 150);
        readonly static Rectangle
            HERO_INITIAL_POSITION = new Rectangle(150, 600, 50, 50);
        static Random
            getRandom = new Random(DateTime.Now.Millisecond);
        readonly static Color[]
            DETECTION_COLORS = new Color[5]
            {
                Color.FromArgb(245, 158, 162),
                Color.FromArgb(255, 126, 130),
                Color.FromArgb(255, 94, 98),
                Color.FromArgb(255, 62, 64),
                Color.FromArgb(255, 30, 32)
            };
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
        };
        enum DoorType
        {
            Blocking,
            Transporting
        };
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
            Ninja
        };
        enum ProgramState
        {
            Paused,
            Active,
            Settings,
            Dialog,
            Intro
        };
        enum DialogType
        {
            GameEnd,
            Information
        };
        static ProgramState GameState;
        static readonly StringFormat
            TextFormatCenter = new StringFormat();
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
            MessageRectangle = new Rectangle(760, 440, 400, 200),
            DoneRectangle = new Rectangle(920, 580, 80, 30),
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
                        theHeroVelocity = INCREASED_HERO_VELOCITY;
                        theHero.Suit = SuitList.Flash;
                        consoleLog = "Flash suit is dressed.";
                        break;
                    case "SUITNINJA":
                        if (theHero.Suit == SuitList.Flash)
                            theHeroVelocity = DEFAULT_HERO_VELOCITY;
                        theHero.Suit = SuitList.Ninja;
                        consoleLog = "Ninja suit is dressed.";
                        break;
                    case "SUITDEFAULT":
                    case "SUITDEF":
                    case "DEFAULTSUIT":
                    case "DEFSUIT":
                    case "DROPSUIT":
                        if (theHero.Suit == SuitList.Flash)
                            theHeroVelocity = DEFAULT_HERO_VELOCITY;
                        theHero.Suit = SuitList.Default;
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
                    case "RESOLUTION":
                        Resolution = Screen.PrimaryScreen.Bounds.Size;
                        ResolutionResize();
                        consoleLog = "Automatically " + Resolution.Width + "x" + Resolution.Height + " done.";
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
            public Rectangle ActualRectangle()
            {
                return new Rectangle((int)Position.X - 14, (int)Position.Y - 14, 25, 25);
            }
            public RectangleF ActualRectangleF()
            {
                return new RectangleF(Position.X - 14, Position.Y - 14, 25.5f, 25.5f);
            }
            public void DoDamage()
            {
                Health--;
                if (Health > 0)
                    Position = new PointF(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25);
                else
                {
                    CurrentDialog = DialogType.GameEnd;
                    GameState = ProgramState.Dialog;
                }
            }
            public void DoDamage(Ball _Ball)
            {
                _Ball.Detected = false;
                _Ball.Duration = 0f;
                _Ball.Color = Color.AliceBlue;
                _Ball.Boost(false);
                this.DoDamage();
            }
        }

        class Door
        {
            public Rectangle Rectangle;
            public Point Start;
            public Point End;
            public Point Direction;
            public DoorType Type;
            public Door(Rectangle _Rectangle, Point _Start, Point _End, Point _Direction, DoorType _Type)
            {
                Rectangle = _Rectangle;
                Start = _Start;
                End = _End;
                Direction = _Direction;
                Type = _Type;
            }
            public Door(int x, int y, int w, int h, Point _Start, Point _End, Point _Direction, DoorType _Type)
            {
                Rectangle = new Rectangle(x, y, w, h);
                Start = _Start;
                End = _End;
                Direction = _Direction;
                Type = _Type;
            }
        }

        class Laser
        {
            public Point RayStart;
            public Point MoveStart;
            public Boolean Moveable = false;
            public Point MoveEnd;
            public Point MoveDirection;
            public Point RayEnd;
            public Point Direction;
            public Laser(Point _Start, Point _Direction)
            {
                RayStart = _Start;
                Direction = _Direction;
                Refresh();
            }
            public Laser(Point _Start, Point _End, Point _Direction, Point _MoveDirection)
            {
                RayStart = MoveStart = _Start;
                MoveEnd = _End;
                Moveable = true;
                Direction = _Direction;
                MoveDirection = _MoveDirection;
                Refresh();
            }
            public void Refresh()
            {
                Point next = RayStart;
                Mark:
                next.Offset(Direction);
                if (theHero.ActualRectangle().Contains(next))
                    theHero.DoDamage();
                foreach (Box TB in Boxes)
                    if (reg.IsVisible(next) && !HERO_INITIAL_POSITION.Contains(next) && !TB.Rectangle().Contains(next))
                        goto Mark;
                RayEnd = next;
            }
        }

        class Ball
        {
            public PointF Position;
            public float Speed;
            public float DefaultSpeed;
            public float Direction;
            public float Duration = 0f;
            public Boolean Detected = false;
            public Boolean Exist = true;
            public Color Color = Color.AliceBlue;
            public int Health = 3;

            public Ball(PointF _Position, float _Speed, float _Direction)
            {
                Position = _Position;
                DefaultSpeed = Speed = _Speed;
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
            public void Unstuck()
            {
                Position = getRandomPointInRegion(reg);
            }
            public void Boost(Boolean Up)
            {
                if (Up)
                    Speed = DefaultSpeed + (Duration + 0.01f) / (theHero.Suit == SuitList.Flash ? 6f : 3f);
                else
                    Speed = DefaultSpeed;
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
                    new PointF(RectangleF().X, RectangleF().Y + BOX_DEFAULT_SIZE),
                    new PointF(RectangleF().X + BOX_DEFAULT_SIZE, RectangleF().Y),
                    new PointF(RectangleF().X + BOX_DEFAULT_SIZE, RectangleF().Y + BOX_DEFAULT_SIZE)
                };
                for (int a = 0; a < 4; ++a)
                    if (!reg.IsVisible(Points[a]))
                        PointsCounter++;
                if (PointsCounter == 0)
                    Size.Width = Size.Height = BOX_DEFAULT_SIZE;
                if (PointsCounter > 3)
                    if (Size.Width > 3f)
                    {
                        Size.Width -= 0.5f;
                        Size.Height -= 0.5f;
                    }
                    else Exist = false;
            }
        }

        class TemporaryRegion
        {
            public Region Rectangle;
            public Point InitialPosition;
            public Point Position;
            public float Duration = -2f;
            public Boolean Exist = true;
            public float Size = TEMPORARY_REGION_MIN_SIZE;
            public TemporaryRegion(Point _Point)
            {
                GraphicsPath TGP = new GraphicsPath();
                InitialPosition = Position = _Point;
                TGP.AddEllipse(Position.X, Position.Y, TEMPORARY_REGION_MIN_SIZE, TEMPORARY_REGION_MIN_SIZE);
                Rectangle = new Region(TGP);
            }
            public void Resize()
            {
                if (Duration < TEMPORARY_REGION_DURATION)
                {
                    Duration += 0.01f;
                    if (Duration <= 2)
                    {
                        if (Size < TEMPORARY_REGION_MAX_SIZE)
                            Size += 0.5f;
                    }
                    else
                        if (Size > TEMPORARY_REGION_MIN_SIZE)
                            Size -= 0.5f;
                }
                if (Duration >= TEMPORARY_REGION_DURATION || Size <= TEMPORARY_REGION_MIN_SIZE)
                    Exist = false;
                Rectangle.Dispose();
                Position = InitialPosition;
                Position.Offset((int)-Size / 2, (int)-Size / 2);
                GraphicsPath TGP = new GraphicsPath();
                TGP.AddEllipse(Position.X, Position.Y, Size, Size);
                Rectangle = new Region(TGP);
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

        class Message
        {
            public string OldText;
            public string NewText;
            public Boolean Completed = false;
            public float Duration = 0f;
            public Message(string _Text)
            {
                OldText = _Text;
            }
            public void Change()
            {
                int index = (int)(OldText.Length * Duration / MESSAGE_DURATION * 2f);
                char[] chars = OldText.ToCharArray();
                for (int c = index; c < OldText.Length; ++c)
                {
                    if (OldText[c] == ' ' || OldText[c] == '.')
                        chars[c] = OldText[c];
                    else
                        if (Char.IsLower(OldText, c))
                            chars[c] = (char)getRandom.Next(97, 122);
                        else
                            chars[c] = (char)getRandom.Next(65, 91);
                }
                NewText = new string(chars);
                if (NewText == OldText)
                    Completed = true;
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
        static Point ViewOffset = new Point(785, -85);
        int lastTick, lastFrameRate, frameRate,
            UnstuckIndex = 0, SelectedBall = -1, BallCount = 10, BGInterval = 365;
        static float BallSpeed = 1f, theHeroVelocity = 1.5f, HeroFallingDuration = 0f, IntroDuration = 0f,
            ProjectileCooldown = 0f, AimingDuration = 0f, TemporaryRegionCoolDown = TEMPORARY_REGION_COOLDOWN;
        string AimingInformation = "";
        static Message CurrentMessage;
        PointF AimingPosition = new Point(960, 540);
        RectangleF AimingRectangle;
        static Region reg;
        static Boolean
            showDI = false, PressedForward = false, PressedLeft = false, PressedRight = false, IntroOver = false,
            AnyBallSelected, SpeedChangerSelected, CountChangerSelected, BGSpeedChangerSelected, AimingEnabled = true, MagnetUsing = false;
        static DialogType CurrentDialog;
        static List<Ball> Balls = new List<Ball>();
        static List<Door> Doors = new List<Door>();
        static List<Laser> Lasers = new List<Laser>();
        static List<Box> Boxes = new List<Box>();
        static List<Projectile> Projectiles = new List<Projectile>();
        static List<TemporaryRegion> TempRegs = new List<TemporaryRegion>();
        static List<Rectangle> MapRects = new List<Rectangle>();
        static List<Region> MapRegs = new List<Region>(),
                            RectsPath = new List<Region>();
        static Hero theHero;
        BackGroundColor BGColor;
        #endregion

        static Point[] qweqwe = new Point[7];

        int CalculateFrameRate()
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
            this.BackColor = Color.FromArgb(240, 255, 255);
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

        void SecondaryLoop(object sender, EventArgs e)
        {
            if (GameState != ProgramState.Intro)
            {
                BGColor.Increase(true);
                this.BackColor = BGColor.Set();
                if (getRandom.Next(50) == 0)
                    BGColor.randomFactors();
            }
        }

        void InitialSetup()
        {
            GameState = ProgramState.Intro;// Active;
            TextFormatCenter.Alignment = StringAlignment.Center;
            BGColor = new BackGroundColor(240, 254, 254);
            Resolution = Screen.PrimaryScreen.Bounds.Size;
            //this.Size = Resolution;
            ResolutionResize();
            for (int a = 0; a < HeroLastPoints.Length; ++a)
                HeroLastPoints[a] = HERO_INITIAL_POSITION.Location;
            theHero = new Hero(HERO_INITIAL_POSITION.X + 25, HERO_INITIAL_POSITION.Y + 25, 0f);
            CirclePath.AddEllipse(CircleRectangle);
            RegionRandom();
            RegionRefresh();
            Boxes.Add(new Box(new PointF(300, 600), BoxType.Wooden));
            Boxes.Add(new Box(new PointF(350, 750), BoxType.Steel));
            Doors.Add(new Door(new Rectangle(new Point(500, 200), new Size(10, 50)), new Point(500, 200), new Point(500, 400), new Point(0, 1), DoorType.Blocking));
            Doors.Add(new Door(new Rectangle(new Point(300, 500), new Size(50, 10)), new Point(200, 500), new Point(400, 500), new Point(1, 0), DoorType.Blocking));
            Doors.Add(new Door(new Rectangle(new Point(450, 875), new Size(50, 25)), new Point(450, 875), new Point(700, 875), new Point(1, 0), DoorType.Transporting));
            Lasers.Add(new Laser(new Point(340, 300), new Point(0, 1)));
            Lasers.Add(new Laser(new Point(900, 450), new Point(900, 650), new Point(-1, 0), new Point(0, 1)));
            for (int a = 0; a < BallCount; ++a)
                Balls.Add(new Ball(getRandomPointInRegion(reg), (float)(getRandom.Next(8, 18) / 10f), (float)getRandom.NextDouble() + getRandom.Next(359)));
        }

        static void RegionRandom()
        {
            if (MapRects.Count > 0 || MapRegs.Count > 0 || RectsPath.Count > 0)
            {
                MapRects.Clear();
                MapRegs.Clear();
                RectsPath.Clear();
            }
            for (int a = 0; a < REGION_COUNT; ++a)
            {
                int Left = getRandom.Next(-ViewOffset.X, Resolution.Width / 2),
                    Top = getRandom.Next(-ViewOffset.Y, Resolution.Height / 2),
                    Width = getRandom.Next(200, 300),
                    Height = getRandom.Next(200, 300);
                int randomdepth = 200;
                Rectangle Rect = new Rectangle(Left, Top, Width, Height);
            Mark:
                for (int b = 0; b < MapRects.Count; ++b)
                    if (randomdepth >= 0)
                    if (MapRects[b].IntersectsWith(Rect))
                    {
                        Left = getRandom.Next(-ViewOffset.X, Resolution.Width / 2);
                        Top = getRandom.Next(-ViewOffset.Y, Resolution.Height / 2);
                        Rect = new Rectangle(Left, Top, Width, Height);
                        randomdepth--;
                        goto Mark;
                    }
                MapRects.Add(Rect);
                MapRegs.Add(new Region(MapRects[a]));
            }
            for (int a = 0; a < REGION_COUNT; ++a)
            {
                int Rand1 = getRandom.Next(REGION_COUNT),
                    Rand2 = getRandomExcept(0, REGION_COUNT, Rand1),
                    X1 = MapRects[Rand1].Left + MapRects[Rand1].Width / 2,
                    Y1 = MapRects[Rand1].Top + MapRects[Rand1].Height / 2,
                    X2 = MapRects[Rand2].Left + MapRects[Rand2].Width / 2,
                    Y2 = MapRects[Rand2].Top + MapRects[Rand2].Height / 2,
                    Width = getRandom.Next(8, 19);
                float Dir2 = theHero.AngleBetween(new PointF(X1, Y1), new PointF(X2, Y2)) - 90,
                      Dir1 = theHero.AngleBetween(new PointF(X2, Y2), new PointF(X1, Y1)) - 90;
                int
                      NX1 = (int)(X1 + Width * theHero.Cos(Dir1)),
                      NY1 = (int)(Y1 + Width * theHero.Sin(Dir1)),
                      _NX1 = (int)(X1 + Width * theHero.Cos(Dir2)),
                      _NY1 = (int)(Y1 + Width * theHero.Sin(Dir2)),
                      NX2 = (int)(X2 + Width * theHero.Cos(Dir2)),
                      NY2 = (int)(Y2 + Width * theHero.Sin(Dir2)),
                      _NX2 = (int)(X2 + Width * theHero.Cos(Dir1)),
                      _NY2 = (int)(Y2 + Width * theHero.Sin(Dir1));
                GraphicsPath tgp = new GraphicsPath();
                Point[] Way = new Point[]
                {
                    new Point(NX1, NY1),
                    new Point(_NX1, _NY1),
                    new Point(NX2, NY2),
                    new Point(_NX2, _NY2)
                };
                //Point[] Way = new Point[]
                //{
                //    new Point(X1 - 16, Y1 + 16),
                //    new Point(X1 - 16, Y1 - 16),
                //    new Point(X2 - 16, Y1 - 16),
                //    new Point(X2 - 16, Y2 - 16),
                //    new Point(X2 + 16, Y2 - 16),
                //    new Point(X2 + 16, Y1 + 16)
                //};
                qweqwe = Way;

                tgp.AddPolygon(Way);
                RectsPath.Add(new Region(tgp));
            }
        }

        static void ResolutionResize()
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
            TryAgainRectangle.Y = Resolution.Height / 2 - 60;
            OkRectangle.X = Resolution.Width / 2 - 120;
            OkRectangle.Y = ExitRectangle.Y = TryAgainRectangle.Y + 70;
            ExitRectangle.X = OkRectangle.X + 160;
            MessageRectangle.X = Resolution.Width / 2 - 200;
            MessageRectangle.Y = Resolution.Height / 2 - 100;
            DoneRectangle.X = MessageRectangle.X + 160;
            DoneRectangle.Y = MessageRectangle.Y + 140;
            //MessageRectangle = new Rectangle(760, 440, 400, 200),
            //DoneRectangle = new Rectangle(920, 580, 80, 30),
        }

        void RegionRefresh()
        {
            reg = new Region(HERO_INITIAL_POSITION);
            for (int s = 0; s < REGION_COUNT; ++s)
                reg.Union(MapRegs[s]);
            for (int s = 0; s < REGION_COUNT; ++s)
                reg.Union(RectsPath[s]);
            //reg = new Region(new Rectangle(1200, 400, 400, 400));
            //reg.Union(CirclePath);
            //reg.Xor(new Rectangle(1415, 750, 50, 50));
            //reg.Union(new Rectangle(300, 300, 300, 50));
            //reg.Union(new Rectangle(300, 300, 50, 300));
            //reg.Union(new Rectangle(150, 600, 200, 50));
            //reg.Union(new Rectangle(300, 600, 200, 300));
            //reg.Union(new Rectangle(1550, 350, 500, 50));
            //reg.Exclude(new Rectangle(300, 650, 150, 5));
            //reg.Exclude(new Rectangle(560, 350, 340, 4));
            //reg.Exclude(new Rectangle(900, 350, 2, 299));
            foreach (TemporaryRegion TTR in TempRegs)
                reg.Union(TTR.Rectangle);
            foreach (Box TB in Boxes)
                if (TB.Exist)
                    reg.Exclude(new RectangleF(TB.Position, new SizeF(TB.Size.Width - 1, TB.Size.Height - 1)));
            foreach (Door TD in Doors)
                if (TD.Type == DoorType.Blocking)
                    reg.Xor(TD.Rectangle);
                else
                    reg.Union(TD.Rectangle);
        }

        void TryAgain()
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

        static PointF getRandomPointInRegion(Region _reg)
        {
            Point TP;
        Mark:
            TP = new Point(getRandom.Next(-ViewOffset.X , 1920), getRandom.Next(-ViewOffset.Y, 1080));
            foreach (Door TD in Doors)
                if (_reg.IsVisible(TP) && !TD.Rectangle.Contains(TP) && !HERO_INITIAL_POSITION.Contains(TP))
                    return TP;
            goto Mark;
        }

        static int getRandomExcept(int Min, int Max, int Except)
        {
            int rand;
        Mark:
            rand = getRandom.Next(Min, Max);
            if (rand == Except)
                goto Mark;
            return rand;
        }

        static int getRandomExcept(int Min, int Max, int[] Except)
        {
            int rand;
        Mark:
            rand = getRandom.Next(Min, Max);
            foreach (int ex in Except)
                if (rand == ex)
                    goto Mark;
            return rand;
        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
            if (GameState != ProgramState.Intro)
            {
                if (e.KeyData == Keys.Escape)
                    if (GameState != ProgramState.Settings && GameState != ProgramState.Dialog)
                        Application.Exit();
                    //GameState = ProgramState.Intro;
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
                            if (GameState != ProgramState.Dialog)
                                if (GameState == ProgramState.Active || GameState == ProgramState.Paused)
                                    GameState = ProgramState.Settings;
                                else
                                    GameState = ProgramState.Active;
                            break;
                        case Keys.F2:
                            theHero.Health = 0;
                            GameState = ProgramState.Dialog;
                            CurrentDialog = DialogType.GameEnd;
                            break;
                        case Keys.F3:
                            RegionRandom();
                            break;
                        case Keys.Enter:
                            if (GameState == ProgramState.Dialog)
                                if (CurrentDialog == DialogType.GameEnd)
                                    TryAgain();
                                else
                                {
                                    GameState = ProgramState.Active;
                                    CurrentMessage.Duration = 0f;
                                }
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
            else
                if (!e.Shift && e.KeyData == Keys.Escape)
                    IntroductionEnd();
        }

        void pKeyUp(object sender, KeyEventArgs e)
        {
            if (GameState != ProgramState.Intro)
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
        }

        void pMouseMove(object sender, MouseEventArgs e)
        {
            if (GameState != ProgramState.Intro)
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
                //if (secondaryTimer.Interval == BG_INTERVAL_MIN)
                //    secondaryTimer.Interval = BG_INTERVAL_MAX;
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
        }

        void pMouseUp(object sender, MouseEventArgs e)
        {
            if (GameState != ProgramState.Intro)
            {
                Point ClickOffset = new Point(e.X - ViewOffset.X, e.Y - ViewOffset.Y);
                if (e.Button != MouseButtons.Right && TemporaryRegionCoolDown >= TEMPORARY_REGION_COOLDOWN && GameState == ProgramState.Active)
                {
                    TempRegs.Add(new TemporaryRegion(ClickOffset));
                    TemporaryRegionCoolDown = 0f;
                }
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
        }

        void pMouseDown(object sender, MouseEventArgs e)
        {
            if (GameState != ProgramState.Intro)
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
                    if (CurrentDialog == DialogType.GameEnd)
                    {
                        if (OkRectangle.Contains(e.Location))
                            TryAgain();
                        else
                            if (ExitRectangle.Contains(e.Location))
                                Application.Exit();
                    }
                    else
                        if (DoneRectangle.Contains(e.Location))
                        {
                            GameState = ProgramState.Active;
                            CurrentMessage.Duration = 0f;
                        }
                    
                pMouseMove(sender, e);
            }
        }

        void pUpdate(object sender, EventArgs e)
        {
            ViewOffset.X = -((int)theHero.Position.X - Resolution.Width / 2);
            ViewOffset.Y = -((int)theHero.Position.Y - Resolution.Height / 2);
            if (IntroDuration >= INTRO_DURATION && !IntroOver)
            {
                IntroductionEnd();
            }
            else
                IntroDuration += 0.01f;

            if (AimingEnabled && GameState != ProgramState.Intro)
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
                        AimingInformation = "Object: " + (TD.Type == DoorType.Blocking ? "Door" : "Vehicle") + "\nSpeed: Normal\nDirection: " + (TD.Direction.X != 0 ? "Horizontal" : "Vertical");
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
                    ProjectileCooldown += 0.01f;
                    TemporaryRegionCoolDown += 0.01f;

                    #region Falling
                    if (reg.IsVisible(theHero.Position))
                    {
                        HeroFallingDuration = 0f;
                        HeroLastPoints[UnstuckIndex] = theHero.Position;
                    }

                    if (UnstuckIndex < HeroLastPoints.Length - 1)
                        UnstuckIndex++;
                    else
                        UnstuckIndex = 0;

                    if (!reg.IsVisible(theHero.Position))
                    {
                        theHero.Position = HeroLastPoints[UnstuckIndex];
                        HeroFallingDuration += 0.01f;
                    }

                    if (HeroFallingDuration > FALLING_TIME_LIMIT)
                    {
                        theHero.Health--;
                        TryAgain();
                    }
                    #endregion    

                    for (int r = 0; r < TempRegs.Count; ++r)
                        if (TempRegs[r].Exist)
                            TempRegs[r].Resize();
                        else
                            TempRegs.Remove(TempRegs[r]);
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
                                TD.Rectangle.Y += TD.Direction.Y * (theHero.Suit == SuitList.Flash || TD.Type == DoorType.Transporting ? 1 : 2);
                            else
                                TD.Direction.Y = -TD.Direction.Y;
                        if (TD.Direction.X != 0)
                            if ((TD.Direction.X > 0 && TD.Rectangle.X < TD.End.X) ||
                                 TD.Direction.X < 0 && TD.Rectangle.X > TD.Start.X)
                                TD.Rectangle.X += TD.Direction.X * (theHero.Suit == SuitList.Flash || TD.Type == DoorType.Transporting ? 1 : 2);
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
                                Balls[a].Direction = theHero.AngleBetween(theHero.Position,
                                    new PointF((theHero.Position.X + Balls[a].Position.X) / 2f, (theHero.Position.Y + Balls[a].Position.Y) / 2f));

                        if ((dist < 18 && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)) ||
                        (Balls[a].RefreshView().IsVisible(theHero.Position) && !new Region(HERO_INITIAL_POSITION).IsVisible(theHero.Position)))
                        {
                            if (dist > 18)
                            {
                                if (theHero.Suit != SuitList.Ninja)
                                {
                                    Balls[a].Detected = true;
                                    Balls[a].Color = DETECTION_COLORS[(int)(Balls[a].Duration / 0.5f)];
                                    Balls[a].Direction = theHero.AngleBetween(theHero.Position, Balls[a].Position);
                                    Balls[a].Boost(true);
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
                            float nextX = Balls[a].Position.X + (float)Math.Cos(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * (theHero.Suit == SuitList.Flash ? BallSpeed / 2 : BallSpeed),
                                nextY = Balls[a].Position.Y + (float)Math.Sin(Math.PI * Balls[a].Direction / 180) * Balls[a].Speed * (theHero.Suit == SuitList.Flash ? BallSpeed / 2 : BallSpeed);
                            int ax = (nextX > Balls[a].Position.X ? 18 : 6), ay = (nextY > Balls[a].Position.Y ? 18 : 12);
                            PointF PP = new PointF(nextX + ax * (float)Math.Cos(Math.PI * Balls[a].Direction / 180), nextY + ay * (float)Math.Sin(Math.PI * Balls[a].Direction / 180));

                            foreach (Door TD in Doors)
                            if (!reg.IsVisible(PP) || new Region(HERO_INITIAL_POSITION).IsVisible(PP) || new Region(TD.Rectangle).IsVisible(nextX, nextY))
                                Balls[a].Direction -= 150 + getRandom.Next(-30, 30);

                            Balls[a].Position = new PointF(nextX, nextY);
                        }
                        foreach (Door TD in Doors)
                            if (new Region(TD.Rectangle).IsVisible(Balls[a].Position))
                                Balls[a].Unstuck();
                    }
                    #endregion

                    #region Lasers
                    foreach (Laser TL in Lasers)
                    {
                        if (TL.Moveable)
                        {
                            if (TL.MoveDirection.Y != 0)
                                if ((TL.MoveDirection.Y > 0 && TL.RayStart.Y < TL.MoveEnd.Y) ||
                                     TL.MoveDirection.Y < 0 && TL.RayStart.Y > TL.MoveStart.Y)
                                    TL.RayStart.Y += TL.MoveDirection.Y;
                                else
                                    TL.MoveDirection.Y = -TL.MoveDirection.Y;
                            if (TL.MoveDirection.X != 0)
                                if ((TL.MoveDirection.X > 0 && TL.RayStart.X < TL.MoveEnd.X) ||
                                     TL.MoveDirection.X < 0 && TL.RayStart.X > TL.MoveStart.X)
                                    TL.RayStart.X += TL.MoveDirection.X;
                                else
                                    TL.MoveDirection.X = -TL.MoveDirection.X;
                        }
                        TL.Refresh();
                    }
                    #endregion

                    break;
                case ProgramState.Dialog:
                    if (CurrentDialog == DialogType.Information)
                        if (CurrentMessage.Duration <= MESSAGE_DURATION)
                            CurrentMessage.Duration += 0.01f;
                        else
                        {
                            GameState = ProgramState.Active;
                            CurrentMessage.Duration = 0f;
                        }
                    break;
            }

            Invalidate();
        }

        static void IntroductionEnd()
        {
            GameState = ProgramState.Dialog;
            CurrentMessage = new Message("You are lost.\nAvoid the lasers and enemies.\nKeep searching a way out.");
            CurrentDialog = DialogType.Information;
            IntroOver = true;
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (GameState == ProgramState.Intro)
                Introduction(g);
            else
            {
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

                #region Balls, Lasers, Doors
                for (int a = 0; a < Balls.Count; ++a)
                {
                    PointF TP = Balls[a].Position;
                    g.FillRegion(enemyViewBrush, Balls[a].RefreshView());
                    SolidBrush TransBrush = new SolidBrush(Color.FromArgb(128, 64, 64, 190));
                    g.FillEllipse(TransBrush, TP.X - 6, TP.Y - 6, 22, 22);
                    g.DrawEllipse(new Pen(Balls[a].Color, 2), TP.X - 7, TP.Y - 7, 24, 24);
                    g.FillEllipse(new SolidBrush(Balls[a].Color), TP.X, TP.Y, 10, 10);
                }

                foreach (Laser TL in Lasers)
                {
                    g.DrawLine(new Pen(Color.FromArgb(24, 255, 255, 255), 3), TL.RayStart, TL.RayEnd);
                    g.DrawLine(Pens.Red, TL.RayStart, TL.RayEnd);
                }

                foreach (Door TD in Doors)
                {
                    if (TD.Type != DoorType.Blocking)
                        g.FillRectangle(new HatchBrush(HatchStyle.Percent30, Color.Black, Color.CornflowerBlue), TD.Rectangle);
                    g.DrawRectangle(new Pen(Color.FromArgb(190, 32, 32, 32)), TD.Rectangle);
                }
                #endregion

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

                g.ResetTransform();

                if (AimingEnabled)
                    if (!String.IsNullOrEmpty(AimingInformation))
                    {
                        g.DrawImage(iGlassPanelSimple, AimingRectangle);
                        g.DrawString(AimingInformation, new Font("QUARTZ MS", 12), Brushes.Black, AimingPosition.X + (AimingRectangle.Width >= 9 * iGlassPanelSimple.Width / 10 ? 3 : 7), AimingPosition.Y + 4);
                    }

                g.DrawImage(iPanelGlass, HealthRectangle);
                g.DrawString("Lives", Nirmala20, textLB, new Point(HealthRectangle.X + 16, HealthRectangle.Y + 10));
                for (int s = 0; s < HERO_MAX_LIVES; ++s)
                    g.DrawImage(s > theHero.Health - 1 ? iBlock[0] : iBlock[1], HealthRectangle.X + 93 + 27 * s, HealthRectangle.Y + 18);

                if (showDI)
                #region Debug Information
                {
                    string output = "Status: " + GameState.ToString() + "\nFPS: " + CalculateFrameRate().ToString();
                    output += "\nPosition: " + Math.Round(theHero.Position.X).ToString() + "; " + Math.Round(theHero.Position.Y).ToString();
                    output += "\nR " + BGColor.R + "; G " + BGColor.G + "; B " + BGColor.B + "\n" + MouseAiming.ToString() + "\n" + Math.Round(HeroFallingDuration, 1).ToString() + "\n";
                    foreach (Point PPP in qweqwe)
                        output += (PPP.X - ViewOffset.X).ToString() + "; " + (PPP.Y - ViewOffset.Y).ToString() + "\n";
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
                        if (CurrentDialog == DialogType.GameEnd)
                        {
                            g.DrawImage(iGlassPanelCorners, TryAgainRectangle);
                            g.DrawString("Do you want try again ?", Verdana17, Brushes.Black, TryAgainRectangle.X + 18, TryAgainRectangle.Y + 20);
                            g.DrawString("Again", Verdana15, Brushes.Black, OkRectangle.X + 8, OkRectangle.Y + 1);
                            g.DrawString("Exit", Verdana15, Brushes.Black, ExitRectangle.X + 17, ExitRectangle.Y + 1);
                            g.DrawImage(iGlassPanelCorners, OkRectangle);
                            g.DrawImage(iGlassPanelCorners, ExitRectangle);
                        }
                        else
                        {
                            g.DrawImage(iGlassPanelCorners, MessageRectangle);
                            CurrentMessage.Change();
                            g.DrawString(CurrentMessage.NewText, Verdana17, Brushes.Black, new Rectangle(MessageRectangle.X + 15, MessageRectangle.Y + 15, MessageRectangle.Width - 30, MessageRectangle.Height - 30), TextFormatCenter);
                            g.DrawImage(iGlassPanelCorners, DoneRectangle);
                            g.DrawString("Done", Verdana15, Brushes.Black, DoneRectangle.X + 12, DoneRectangle.Y + 1);
                        }
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
            //g.TranslateTransform(ViewOffset.X, ViewOffset.Y);
            //g.DrawPolygon(Pens.Black, qweqwe);
            //for (int mp = 0; mp < MapRects.Count; ++mp)
            //    g.DrawString(mp.ToString(), Verdana15, Brushes.Black, MapRects[mp].Location);
        }

        void Introduction(Graphics g)
        {
            int Count = Resolution.Height / 50 + 1;
            Point[] Start = new Point[Count],
                    End = new Point[Count];
            string introstring = "";
            for (int f = 0; f < 8; ++f)
                introstring += (char)getRandom.Next(33, 123);
            if (IntroDuration > INTRO_DURATION / 2f + (Resolution.Height > 800 ? 0.2f : -0.4f))
                introstring = "Welcome";
            g.DrawString(introstring, new Font("QUARTZ MS", 96), Brushes.Black, Resolution.Width / 2 - 345, Resolution.Height / 2 - 95);
            for (int p = 0; p < Count; ++p)
            {
                Start[p] = new Point(p % 2 == 0 ? 5 - (int)(500 * IntroDuration) : Resolution.Width - 5 + (int)(500 * IntroDuration), - (Resolution.Height > 800 ? 450 : 300) + p * 75 - (p % 2 != 0 ? 75 : 0) - (int)(150 * IntroDuration));
                End[p] = new Point(Start[p].X + (p % 2 == 0 ? Resolution.Width - 5 : -Resolution.Width + 5), Start[p].Y + 2 * Resolution.Height / 3);
            }
            for (int p = 0; p < Count; ++p)
            {
                int color = (p > Count / 2 ? 128 - p * 6 : p * 6);
                Pen TempPen = new Pen(Color.FromArgb(color, color, color), 100);
                TempPen.StartCap = TempPen.EndCap = LineCap.Triangle;
                g.DrawLine(TempPen, Start[p], End[p]);
            }
        }
    }
}

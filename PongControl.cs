using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Pong
{
    public partial class PongControl : Form
    {

        public struct Walls
        {
            internal Rectangle TopLeft;
            internal Rectangle TopMid;
            internal Rectangle TopRight;
            internal Rectangle BottomLeft;
            internal Rectangle BottomMid;
            internal Rectangle BottomRight;

        }

        public const int BALL_X_START = 75;
        public const int BALL_Y_START = 160;
        public const int BALL_VELX_START = 7;
        public const int BALL_VELY_START = 0;
        public const int PLAYER_PADDLE_START_X = 60;
        public const int PLAYER_PADDLE_START_Y = 150;
        public const int COMP_PADDLE_START_X = 610;
        public const int COMP_PADDLE_START_Y = 150;

        public const string PADDLE_BEEP_PATH = @"C:\Users\EmperorMung\Documents\Code Projects\Pong\Pong-master\PaddleBeep.wav";
        public const string WALL_BEEP_PATH = @"C:\Users\EmperorMung\Documents\Code Projects\Pong\Pong-master\WallBeep.wav";
        public const string GOAL_SOUND_PATH = @"C:\Users\EmperorMung\Documents\Code Projects\Pong\Pong-master\GoalSound.wav";

        private Walls PongWalls;
        private Ball PongBall;
        private Paddle PlayerPaddle;
        private Paddle CompPaddle;
        private Random rand = new Random();

        System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        private int playerGoals = 0;
        private int compGoals = 0;

        private bool wPressed = false;
        private bool aPressed = false;
        private bool sPressed = false;
        private bool dPressed = false;
        private bool moved = false;


        #region Constructor

        public PongControl()
        {

            InitializeComponent();

            PongBall = new Ball();
            PlayerPaddle = new Paddle(PLAYER_PADDLE_START_X, PLAYER_PADDLE_START_Y);
            CompPaddle = new Paddle(COMP_PADDLE_START_X, COMP_PADDLE_START_Y);
            this.Refresh();

        }

        #endregion

        #region Internal Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode) return;
            base.OnPaint(e);
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Lime);

            PongWalls.TopLeft = new Rectangle(0, 0, 30, 110);
            PongWalls.TopMid = new Rectangle(30, 0, 700 - 45, 30);
            PongWalls.TopRight = new Rectangle(700 - 45, 0, 30, 110);
            PongWalls.BottomLeft = new Rectangle(0, 240, 30, 120);
            PongWalls.BottomMid = new Rectangle(30, 330, 700 - 45, 30);
            PongWalls.BottomRight = new Rectangle(700 - 45, 240, 30, 110);
            e.Graphics.FillRectangle(myBrush, PongWalls.TopLeft);
            e.Graphics.FillRectangle(myBrush, PongWalls.TopMid);
            e.Graphics.FillRectangle(myBrush, PongWalls.TopRight);
            e.Graphics.FillRectangle(myBrush, PongWalls.BottomLeft);
            e.Graphics.FillRectangle(myBrush, PongWalls.BottomMid);
            e.Graphics.FillRectangle(myBrush, PongWalls.BottomRight);

            PongBall.Circle.X += PongBall.velX;
            PongBall.Circle.Y += PongBall.velY;
            PongBall.xLoc = PongBall.Circle.X;
            PongBall.yLoc = PongBall.Circle.Y;
            myBrush.Color = System.Drawing.Color.Yellow;
            e.Graphics.FillEllipse(myBrush, PongBall.Circle);

            myBrush.Color = System.Drawing.Color.Blue;
            e.Graphics.FillRectangle(myBrush, PlayerPaddle.PadRect);

            myBrush.Color = System.Drawing.Color.Red;
            e.Graphics.FillRectangle(myBrush, CompPaddle.PadRect);


            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.TopLeft);
            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.TopMid);
            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.TopRight);
            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.BottomLeft);
            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.BottomMid);
            e.Graphics.DrawRectangle(System.Drawing.Pens.Lime, PongWalls.BottomRight);

            e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, PlayerPaddle.PadRect);

            e.Graphics.DrawRectangle(System.Drawing.Pens.Red, CompPaddle.PadRect);

            e.Graphics.DrawEllipse(System.Drawing.Pens.Purple, PongBall.Circle);

            myBrush.Dispose();
        }



        private void UpdateScreen()
        {

            this.Invalidate();

            CheckForGoal(PongBall);
            MovePaddle(PlayerPaddle);
            MoveCompPaddle(CompPaddle);
            CheckForWallHit();
            CheckBallHitPaddle(PlayerPaddle);
            CheckBallHitPaddle(CompPaddle);

            Update();

        }

        private void CheckBallHitPaddle(Paddle paddle)
        {
            if (PongBall.velX < 0)
            {
                // Towards Player Paddle
                if ((PongBall.xLoc >= paddle.xLoc) && (PongBall.xLoc <= (paddle.xLoc + paddle.PadRect.Width)))
                {
                    // Ball is in Paddle's X region
                    if ((PongBall.yLoc + PongBall.rad) >= paddle.yLoc && PongBall.yLoc <= (paddle.yLoc + paddle.PadRect.Height))
                    {
                        // Ball is in Paddle's Y region
                        PongBall.velX = -1 * PongBall.velX;
                        PongBall.velY = paddle.velY;

                        player.SoundLocation = PADDLE_BEEP_PATH;
                        player.Load();
                        player.Play();
                    }
                }
            }
            else
            {
                if ((PongBall.xLoc + PongBall.rad >= paddle.xLoc) && (PongBall.xLoc + PongBall.rad <= (paddle.xLoc + paddle.PadRect.Width)))
                {
                    // Ball is in Paddle's X region
                    if ((PongBall.yLoc + PongBall.rad) >= paddle.yLoc && PongBall.yLoc <= (paddle.yLoc + paddle.PadRect.Height))
                    {
                        // Ball is in Paddle's Y region
                        PongBall.velX = -1 * PongBall.velX;
                        PongBall.velY = paddle.velY;

                        player.SoundLocation = PADDLE_BEEP_PATH;
                        player.Load();
                        player.Play();
                    }
                }
            }
        }

        private void CheckForWallHit()
        {
            bool wallWashit = false;

            if ((PongBall.Circle.X + PongBall.rad >= PongWalls.TopRight.X) && ((PongBall.Circle.Y + PongBall.rad) <= (PongWalls.TopRight.Y + PongWalls.TopRight.Height)))
            {
                PongBall.Circle.X = PongWalls.TopRight.X - PongBall.rad - 5;
                PongBall.velX = -1 * PongBall.velX;
                wallWashit = true;
            }
            else if (PongBall.Circle.X <= (PongWalls.TopLeft.X + PongWalls.TopLeft.Width) && ((PongBall.Circle.Y + PongBall.rad) <= (PongWalls.TopLeft.Y + PongWalls.TopLeft.Height)))
            {
                PongBall.Circle.X = PongWalls.BottomLeft.X + PongWalls.BottomLeft.Width + 5;
                PongBall.velX = -1 * PongBall.velX;
                wallWashit = true;
            }
            else if ((PongBall.Circle.X + PongBall.rad >= PongWalls.BottomRight.X) && ((PongBall.Circle.Y + PongBall.rad) >= (PongWalls.BottomRight.Y)))
            {
                    PongBall.Circle.X = PongWalls.BottomRight.X - PongBall.rad - 5;
                    PongBall.velX = -1 * PongBall.velX;
                    wallWashit = true;

            }
            else if (PongBall.Circle.X <= (PongWalls.BottomLeft.X + PongWalls.BottomLeft.Width) && ((PongBall.Circle.Y + PongBall.rad) >= (PongWalls.BottomLeft.Y)))
            {
                PongBall.Circle.X = PongWalls.BottomLeft.X + PongWalls.BottomLeft.Width + 5;
                PongBall.velX = -1 * PongBall.velX;
                wallWashit = true;
            }
            else if ((PongBall.Circle.Y + PongBall.rad) <= PongWalls.TopMid.Y + PongWalls.TopMid.Height * 1.7)
            {
                PongBall.Circle.Y = PongWalls.TopMid.Y + PongWalls.TopMid.Height + 5;
                PongBall.velY = -1 * PongBall.velY;
                wallWashit = true;
            }
            else if ((PongBall.Circle.Y + 1.2 * PongBall.rad) >= PongWalls.BottomMid.Y)
            {
                PongBall.Circle.Y = PongWalls.BottomMid.Y - PongBall.rad - 5;
                PongBall.velY = -1 * PongBall.velY;
                wallWashit = true;
            }

            if (wallWashit) {
                player.SoundLocation = WALL_BEEP_PATH;
                player.Load();
                player.Play();
            }
        }

        public void CheckForGoal(Ball ball)
        {
            bool playerScored = false;

            if (ball.Circle.X - ball.rad <= PongWalls.TopLeft.X && ((ball.yLoc + ball.rad) > (PongWalls.TopLeft.Y + PongWalls.TopLeft.Height)) && ((ball.yLoc + ball.rad) < (PongWalls.BottomLeft.Y)))
            {
                compGoals++;
                ResetBoard(playerScored);
                player.SoundLocation = GOAL_SOUND_PATH;
                player.Load();
                player.Play();
            }
            else if (ball.Circle.X + ball.rad >= (PongWalls.TopRight.X + PongWalls.TopRight.Width) && ((ball.yLoc + ball.rad) > (PongWalls.TopRight.Y + PongWalls.TopRight.Height)) && ((ball.yLoc + ball.rad) < (PongWalls.BottomRight.Y)))
            {
                playerScored = true;
                playerGoals++;
                ResetBoard(playerScored);
                player.SoundLocation = GOAL_SOUND_PATH;
                player.Load();
                player.Play();
            }
        }

        public void MovePaddle(Paddle paddle)
        {
            if (wPressed && paddle.velY > -6)
            {
                paddle.velY--;

            }
            else if (!wPressed && paddle.velY < 0)
            {
                paddle.velY += 2;

                if (paddle.velY > 0)
                {
                    paddle.velY = 0;
                }
            }

            if (sPressed && paddle.velY < 6)
            {
                paddle.velY++;

            }
            else if (!sPressed && paddle.velY > 0)
            {
                paddle.velY -= 2;

                if (paddle.velY < 0)
                {
                    paddle.velY = 0;
                }
            }

            if ((paddle.yLoc <= 50  && wPressed)|| (paddle.yLoc >= 260 && sPressed)) {
                paddle.velY = 0;
            }

            paddle.yLoc += paddle.velY;
            paddle.PadRect.Y = paddle.yLoc;
        }

        public void MoveCompPaddle(Paddle paddle) {

            if (PongBall.yLoc < paddle.yLoc && paddle.velY > -1) {
                if (Math.Abs(PongBall.yLoc - paddle.yLoc) > 10)
                {
                    paddle.velY--;
                    
                }
            }
            else if (PongBall.yLoc > paddle.yLoc && paddle.velY < 1)
            {
                if (Math.Abs(PongBall.yLoc - paddle.yLoc) > 10)
                {
                    paddle.velY++;
                    
                }
            }

            if (Math.Abs(PongBall.xLoc - paddle.xLoc) < 20 && (Math.Abs(PongBall.yLoc - paddle.yLoc) < 20))
            {
                paddle.velY += rand.Next(4)  + (-1 * rand.Next(4));
            }
            if (paddle.yLoc <= 50)
            {
                paddle.yLoc = 51;
                paddle.velY = 1;
            } else if ((paddle.yLoc >= 260))
            {
                paddle.yLoc = 259;
                paddle.velY = -1;
            }

            paddle.yLoc += paddle.velY;
            paddle.PadRect.Y = paddle.yLoc;
        }

        public void ResetBoard(bool playerScored)
        {
            PongBall.Reset();
            ResetPaddles();
            Invalidate();
            UpdateScreen();
            PongTimer.Stop();

        }

        public void ResetPaddles() {
            PlayerPaddle.xLoc = PlayerPaddle.PadRect.X = PLAYER_PADDLE_START_X;
            PlayerPaddle.yLoc = PlayerPaddle.PadRect.Y = PLAYER_PADDLE_START_Y;
            PlayerPaddle.velX = 0;
            PlayerPaddle.velY = 0;
            CompPaddle.xLoc = CompPaddle.PadRect.X = COMP_PADDLE_START_X;
            CompPaddle.yLoc = CompPaddle.PadRect.Y = COMP_PADDLE_START_Y;
            CompPaddle.velX = 0;
            CompPaddle.velY = 0;
        }


        #endregion

        #region Event Handlers

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateScreen();
        }

        private void PongControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                PongTimer.Enabled = true;
            }
            if (e.KeyCode == Keys.W)
            {
                wPressed = true;
            }
            if (e.KeyCode == Keys.A)
            {
                aPressed = true;
            }
            if (e.KeyCode == Keys.S)
            {
                sPressed = true;
            }
            if (e.KeyCode == Keys.D)
            {
                dPressed = true;
            }
            moved = (wPressed || aPressed || sPressed || dPressed);
        }

        private void PongControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                wPressed = false;
            }
            if (e.KeyCode == Keys.A)
            {
                aPressed = false;
            }
            if (e.KeyCode == Keys.S)
            {
                sPressed = false;
            }
            if (e.KeyCode == Keys.D)
            {
                dPressed = false;
            }
            moved = (wPressed || aPressed || sPressed || dPressed);
        }

        #endregion

        #region Internal Classes

        public class Ball
        {
            public int xLoc;
            public int yLoc;
            public int rad;
            public int velX;
            public int velY;

            public Rectangle Circle;

            internal Ball()
            {
                xLoc = BALL_X_START;
                yLoc = BALL_Y_START;
                rad = 20;
                velX = BALL_VELX_START;
                velY = BALL_VELY_START;

                Circle = new Rectangle(xLoc, yLoc, rad, rad);
            }

            internal void Reset()
            {
                xLoc = BALL_X_START;
                yLoc = BALL_Y_START;
                rad = 20;
                velX = BALL_VELX_START;
                velY = BALL_VELY_START;
                Circle.X = xLoc;
                Circle.Y = yLoc;
            }
        }

        public class Paddle
        {
            public const int PADDLE_WIDTH = 20;
            public const int PADDLE_HEIGHT = 50;

            public int xLoc;
            public int yLoc;
            public int velX = 0;
            public int velY = 0;
            public Rectangle PadRect;

            internal Paddle(int x, int y)
            {
                xLoc = x;
                yLoc = y;
                PadRect = new Rectangle(x, y, PADDLE_WIDTH, PADDLE_HEIGHT);
            }

        }


        #endregion


    }
}

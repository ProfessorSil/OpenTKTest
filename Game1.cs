using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using QuickFont;

namespace OpenTKTest
{
    class Game1
    {
        public static double WINDOW_FRAMERATE = 60.0;
        public static int WINDOW_WIDTH = 640;
        public static int WINDOW_HEIGHT = 480;
        public static string WINDOW_NAME = "OpenTK Test";
        public static GameWindowFlags WINDOW_FLAGS = GameWindowFlags.Default;

        public OpenTK.GameWindow window;
        Texture2D Texture1, Texture2;
        float rotation = 0;
        public View view;
        public QFont font;

        /// <summary>
        /// This automatically binds Game1's function to window's events
        /// </summary>
        /// <param name="window">The GameWindow that will control this Game1</param>
        public Game1(OpenTK.GameWindow window)
        {
            //I believe this is just a reference to the window object
            this.window = window;
            window.Load += this.Load;
            window.Resize += this.Resize;
            window.RenderFrame += this.Render;
            window.UpdateFrame += this.Update;

            window.VSync = VSyncMode.Off;

            My.Initialize(window);

            Spritebatch.Initialize(window);

            Vector2 o = Calc.RotateAround(new Vector2(4, 3), new Vector2(2, 2), MathHelper.PiOver3);

            view = new View(window, Vector2.Zero, 10.0, 1, 0.0);
            view.enableRounding = false;
        }

        public void Load(object sender, EventArgs e)
        {
            Texture1 = ContentPipe.LoadTexture(@".\Content\shutdown.png");
            Texture2 = ContentPipe.LoadTexture(@".\Content\block1.png");
            font = new QFont("Content\\TIMESBD.ttf", 16);

            Console.WriteLine("Texture1 is " + Texture1.ToString() + ".");
        }

        public void Close()
        {
            
        }

        public void Update(object sender, EventArgs e)
        {
            My.Update();

            if (window.Keyboard[Key.Escape])
            {
                this.Close();
                window.Exit();
            }

            if (My.MouseDown(false))
            {
                rotation += 10;
            }

            view.BasicMovement(5.0f, true, false);
            if (My.KeyDown(Key.E))
            {
                view.Rotate(1);
            }
            if (My.KeyDown(Key.Q))
            {
                view.Rotate(-1);
            }
            if (My.KeyDown(Key.Z))
            {
                view.zoom += 0.01;
            }
            if (My.KeyDown(Key.X))
            {
                view.zoom -= 0.01;
            }
            view.Update();

            My.UpdateAfter();
        }

        public void Render(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Spritebatch.Begin(4.0f, view);

            Spritebatch.Draw(Texture1.Id, new Vector2(0, 0), new Vector2(100, 100), MathHelper.DegreesToRadians(rotation), Color.Red, new Vector2(20, 20));
            Spritebatch.Draw(Texture1.Id, new Vector2(0, 0), new Vector2(100, 100), MathHelper.DegreesToRadians(20));
            Spritebatch.Draw(My.dot.Id, new Vector2(0, 0), new Vector2(20, 20), 0.0f, Color.Purple);

            Spritebatch.End();

            QFont.Begin();
            QFontRenderOptions op = new QFontRenderOptions();
            op.Colour = Color.Red;
            op.DropShadowActive = true;
            op.DropShadowOffset = new Vector2(2, 2);
            font.PushOptions(op);
            font.Print(String.Format(
                "x {0}\ny {1}\nz {2}\nr {3}", 
                Math.Round(view.position.X, 1),
                Math.Round(view.position.Y, 1),
                Math.Round(view.zoom, 4),
                Math.Round(view.rotation, 0)),
                100f, QFontAlignment.Left, new Vector2(0, 0));
            QFont.End();

            window.SwapBuffers();
        }

        public void Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
        }
    }
}

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
        public View view;
        public QFont font;

        #region Variables



        #endregion

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

            view = new View(window, Vector2.Zero);
            view.TopLeft = Vector2.Zero;
            view.enableRounding = false;

            #region Initialization



            #endregion
        }

        public void Load(object sender, EventArgs e)
        {
            font = new QFont("Content\\TIMESBD.ttf", 16);

            #region Loading Stuff



            #endregion
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

            #region Update Stuff



            #endregion

            view.BasicMovement(5.0f, true, false, true, true);
            view.Update();

            My.UpdateAfter();
        }

        public void Render(object sender, EventArgs e)
        {
            Spritebatch.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Spritebatch.Begin(4.0f, view);
            Spritebatch.ClearColor(Color.White, ref view);

            #region Drawing Stuff



            #endregion

            Spritebatch.End();

            QFont.Begin();

            #region Drawing Text
            view.DrawDebug(font, Vector2.Zero, Color.Black);


            #endregion

            QFont.End();

            window.SwapBuffers();
        }

        public void Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
            QFont.InvalidateViewport();
        }
    }
}

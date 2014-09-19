using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTKTest
{
    class Spritebatch
    {
        private static GameWindow window;

        private static Size ScreenSize
        {
            get
            {
                return window.ClientSize;
            }
        }

        public static void Initialize(GameWindow gameWindow)
        {
            //Enable Textures and alpha blending
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            //Set how the alpha blending function works
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            window = gameWindow;

        }

        /// <summary>
        /// Call before spritebatch.Begin
        /// Clears the screen bits to black
        /// </summary>
        public static void Clear(ClearBufferMask mask)
        {
            GL.Clear(mask);
        }
        public static void ClearColor(Color color)
        {
            DrawRectangle(new RectangleF(0, 0, ScreenSize.Width, ScreenSize.Height), color);
        }
        /// <summary>
        /// Call after spritebatch.Begin()
        /// Draws a rectangle over the whole screen with desired color
        /// </summary>
        /// <param name="color"></param>
        public static void ClearColor(Color color, ref View view)
        {
            DrawRectangle(view.ViewPort, color);
        }

        public static void Begin(float depthMax = 4.0f, View view = null)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-window.ClientSize.Width / 2f, window.ClientSize.Width / 2f, window.ClientSize.Height / 2f, -window.ClientSize.Height / 2f, 0.0, depthMax);

            if (view != null)
            {
                view.ApplyTransform();
            }

        }

        public static void End()
        {
            //Doesn't do anything at the moment. window.SwapBuffers() is done externally now
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Draw(Texture2D Texture, Vector2 position, Vector2 size)
        {
            Draw(Texture, position, size, 0f, Color.White, Vector2.Zero, 0f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Draw(Texture2D Texture, Vector2 position, Vector2 size, float rotation)
        {
            Draw(Texture, position, size, rotation, Color.White, Vector2.Zero, 0f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Draw(Texture2D Texture, Vector2 position, Vector2 size, float rotation, Color color)
        {
            Draw(Texture, position, size, rotation, color, Vector2.Zero, 0f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Draw(Texture2D Texture, Vector2 position, Vector2 size, float rotation, Color color, Vector2 origin)
        {
            Draw(Texture, position, size, rotation, color, origin, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation">In Radians</param>
        public static void Draw(Texture2D Texture, Vector2 position, Vector2 size, float rotation, Color color, Vector2 origin, float depth, RectangleF? sourceRec = null)
        {
            GL.BindTexture(TextureTarget.Texture2D, Texture.Id);

            GL.Begin(PrimitiveType.Quads);

            Vector2[] p = new Vector2[4]{Vector2.Zero, Vector2.Zero ,Vector2.Zero, Vector2.Zero};
            p[0] = new Vector2(0, 0);
            p[1] = new Vector2(1, 0);
            p[2] = new Vector2(1, 1);
            p[3] = new Vector2(0, 1);

            Vector2[] tC = new Vector2[4]{Vector2.Zero, Vector2.Zero ,Vector2.Zero, Vector2.Zero};;
            p.CopyTo(tC, 0);
            #region Source Rec
            if (sourceRec.HasValue)
            {
                tC[0] = new Vector2(sourceRec.Value.Left, sourceRec.Value.Top);
                tC[1] = new Vector2(sourceRec.Value.Right, sourceRec.Value.Top);
                tC[2] = new Vector2(sourceRec.Value.Right, sourceRec.Value.Bottom);
                tC[3] = new Vector2(sourceRec.Value.Left, sourceRec.Value.Bottom);
                for (int i = 0; i < 4; i++)
                {
                    tC[i] = new Vector2(tC[i].X / Texture.Width, tC[i].Y / Texture.Height);
                }
            }
            #endregion

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            for (int i = 0; i < 4; i++)
            {
                p[i] = new Vector2(p[i].X * size.X + position.X, p[i].Y * size.Y + position.Y);
                p[i] -= origin;
                if (rotation != 0)
                {
                    p[i] = Calc.RotateAround(p[i], position, rotation);
                }
                //p[i] = p[i] + new Vector2(ScreenSize.Width / 2f, ScreenSize.Height / 2f);
                //p[i] = new Vector2(p[i].X / (ScreenSize.Width / 2f), p[i].Y / (ScreenSize.Height / 2f));


                GL.Color4(color);
                GL.TexCoord2(tC[i]);
                //GL.Vertex2(p[i]);
                GL.Vertex3(new Vector3(p[i].X, p[i].Y, depth));
            }

            GL.End();
        }

        public static void DrawRectangle(Rectangle rec, Color color)
        {
            DrawRectangle(new Vector2(rec.X, rec.Y), new Vector2(rec.Width, rec.Height), color);
        }
        public static void DrawRectangle(RectangleF rec, Color color)
        {
            DrawRectangle(new Vector2(rec.X, rec.Y), new Vector2(rec.Width, rec.Height), color);
        }
        public static void DrawRectangle(Vector2 position, Vector2 size, Color color)
        {
            DrawRectangle(position.X, position.Y, size.X, size.Y, color, 0, Vector2.Zero, 0);
        }
        public static void DrawRectangle(Vector2 position, Vector2 size, Color color, float rotation)
        {
            DrawRectangle(position.X, position.Y, size.X, size.Y, color, rotation, Vector2.Zero, 0);
        }
        public static void DrawRectangle(Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin)
        {
            DrawRectangle(position.X, position.Y, size.X, size.Y, color, rotation, origin, 0);
        }
        public static void DrawRectangle(Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin, float depth)
        {
            DrawRectangle(position.X, position.Y, size.X, size.Y, color, rotation, origin, depth);
        }

        public static void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            Draw(My.dot, new Vector2(x, y), new Vector2(width, height), 0, color, Vector2.Zero, 0);
        }
        public static void DrawRectangle(float x, float y, float width, float height, Color color, float rotation)
        {
            Draw(My.dot, new Vector2(x, y), new Vector2(width, height), rotation, color, Vector2.Zero, 0);
        }
        public static void DrawRectangle(float x, float y, float width, float height, Color color, float rotation, Vector2 origin)
        {
            Draw(My.dot, new Vector2(x, y), new Vector2(width, height), rotation, color, origin, 0);
        }
        public static void DrawRectangle(float x, float y, float width, float height, Color color, float rotation, Vector2 origin, float depth)
        {
            Draw(My.dot, new Vector2(x, y), new Vector2(width, height), rotation, color, origin, depth);
        }

        public static void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness = 1f, float depth = 0f)
        {
            DrawRectangle(p1, new Vector2(Calc.Distance(p1, p2), thickness), color, (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X), new Vector2(0, thickness / 2f), depth);
        }
    }
}

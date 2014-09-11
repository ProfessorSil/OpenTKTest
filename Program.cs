using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTKTest
{
    class Program
    {
        //This makes it so we can use COM in windows.
        //Is needed since GameWindow is dependent on Windows Forms which need this.
        [STAThread]
        static void Main(string[] args)
        {
            GameWindow window = new GameWindow(Game1.WINDOW_WIDTH, Game1.WINDOW_HEIGHT, GraphicsMode.Default, Game1.WINDOW_NAME, Game1.WINDOW_FLAGS);
            Game1 game = new Game1(window);

            window.Run(Game1.WINDOW_FRAMERATE);
        }
    }
}

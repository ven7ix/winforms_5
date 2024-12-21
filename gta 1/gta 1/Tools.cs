using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal static class Tools
    {
        public static int TileSize;
        public static Point ScreenSize;
        public static Point ScreenCentre;

        public static void Initialize(int tileSize, Point screenSize)
        {
            TileSize = tileSize;
            ScreenSize = screenSize;
            ScreenCentre = new Point(screenSize.X / 2, screenSize.Y / 2);
        }

        public static Point ConvertPosition_GAME_SCREEN(Point gamePosition)
        {
            Point screenPosition = new Point
            {
                X = gamePosition.X * TileSize + ScreenCentre.X,
                Y = -gamePosition.Y * TileSize + ScreenCentre.Y
            };

            return screenPosition;
        }

        public static Point ConvertPosition_SCREEN_GAME(Point screenPosition)
        {
            Point gamePosition = new Point();

            if (screenPosition.X < ScreenCentre.X)
                gamePosition.X = -Math.Abs(ScreenCentre.X - screenPosition.X) / TileSize;
            else
                gamePosition.X = Math.Abs(ScreenCentre.X - screenPosition.X) / TileSize;

            if (screenPosition.Y < ScreenCentre.Y)
                gamePosition.Y = Math.Abs(ScreenCentre.Y - screenPosition.Y) / TileSize;
            else
                gamePosition.Y = -Math.Abs(ScreenCentre.Y - screenPosition.Y) / TileSize;

            return gamePosition;
        }
    }
}
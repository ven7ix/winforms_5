using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal class Map
    {
        public static Tile[,] WorldMap;
        public static Point WorldMapSize;

        public struct Tile
        {
            public Point Position;
            public int SpriteID;
            public Rectangle Bounds;

            public Tile(Point position, int spriteID)
            {
                this.Position = position;
                this.SpriteID = spriteID;
                Size size = new Size(Tools.TileSize, Tools.TileSize);
                Bounds = new Rectangle(position, size);
            }
        }

        public static void LoadMapFromFile()
        {
            Regex regLINES = new Regex("\r\n");
            string[] mapLines = regLINES.Split(Properties.Resources.gta_map);

            WorldMapSize = new Point(mapLines[0].Length, mapLines.Length);
            WorldMap = new Tile[WorldMapSize.X, WorldMapSize.Y];

            for (int y = 0; y < WorldMapSize.Y; y++)
            {
                int yOpposite = WorldMapSize.Y - 1 - y;

                for (int x = 0; x < WorldMapSize.X; x++)
                {
                    Tile tile = new Tile(Tools.ConvertPosition_GAME_SCREEN(new Point(x, yOpposite)), -1);

                    if (mapLines[y][x] == 'X')
                    {
                        tile.SpriteID = 1;
                    }
                    else if (mapLines[y][x] == '_')
                    {
                        tile.SpriteID = 0;
                    }
                    
                    WorldMap[x, yOpposite] = tile;
                }
            }
        }

        public static void RenderWorldMap(Graphics screen)
        {
            for (int x = 0; x < WorldMapSize.X; x++)
            {
                for (int y = 0; y < WorldMapSize.Y; y++)
                {
                    Point newPosition = new Point()
                    {
                        X = WorldMap[x, y].Position.X - (Game.player.Position.X - Tools.ScreenSize.X / 2),
                        Y = WorldMap[x, y].Position.Y - (Game.player.Position.Y - Tools.ScreenSize.Y / 2)
                    };

                    Tile tile = new Tile()
                    {
                        Position = newPosition,
                        Bounds = new Rectangle(newPosition, new Size(Tools.TileSize, Tools.TileSize)),
                        SpriteID = WorldMap[x, y].SpriteID
                    };

                    RenderTile(screen, tile);
                }
            }
        }

        private static void RenderTile(Graphics screen, Tile tile)
        {
            if (tile.SpriteID == 1)
            {
                screen.FillRectangle(Brushes.Red, tile.Bounds);
            }
        }

        public static Tile RandomTile()
        {
            Random mapPosition = new Random();
            Tile randomTile = WorldMap[mapPosition.Next(0, WorldMapSize.X), mapPosition.Next(0, WorldMapSize.Y)];
            return randomTile;
        }
    }
}
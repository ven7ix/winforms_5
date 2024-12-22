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
        public static bool[,] Pavements;
        public static bool[,] Roads;
        public static Point WorldMapSize;

        public struct Tile
        {
            public Point Position;
            public Point PositionInWorldMap;
            public int SpriteID;
            public bool Passable;
            public Rectangle Bounds;

            public Tile(Point position, Point positionInWorldMap, int spriteID, bool passable)
            {
                Position = position;
                PositionInWorldMap = positionInWorldMap;
                SpriteID = spriteID;
                Passable = passable;
                Bounds = new Rectangle(position, new Size(Tools.TileSize, Tools.TileSize));
            }
        }

        public static void LoadMapFromFile()
        {
            Regex regLINES = new Regex("\r\n");
            string[] mapLines = regLINES.Split(Properties.Resources.gta_map);

            WorldMapSize = new Point(mapLines[0].Length, mapLines.Length);
            WorldMap = new Tile[WorldMapSize.X, WorldMapSize.Y];
            Pavements = new bool[WorldMapSize.X, WorldMapSize.Y];
            Roads = new bool[WorldMapSize.X, WorldMapSize.Y];

            for (int y = 0; y < WorldMapSize.Y; y++)
            {
                int yOpposite = WorldMapSize.Y - 1 - y;

                for (int x = 0; x < WorldMapSize.X; x++)
                {
                    Tile tile = new Tile(Tools.ConvertPosition_GAME_SCREEN(new Point(x, yOpposite)), new Point(x, y), -1, true);
                    Pavements[x, yOpposite] = false;
                    Roads[x, yOpposite] = false;

                    if (mapLines[y][x] == '_')
                    {
                        tile.SpriteID = 0;
                    }
                    else if (mapLines[y][x] == 's')
                    {
                        tile.SpriteID = 0;

                        Game.entities.Add(new Weapon(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 8, 100, true));
                    }
                    else if (mapLines[y][x] == 'r')
                    {
                        tile.SpriteID = 0;

                        Game.entities.Add(new Weapon(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 6, 50, true));
                    }
                    else if (mapLines[y][x] == 'X')
                    {
                        tile.SpriteID = 1;
                        tile.Passable = false;
                    }
                    else if (mapLines[y][x] == 'P')
                    {
                        tile.SpriteID = 2;

                        Pavements[x, yOpposite] = true;
                    }
                    else if (mapLines[y][x] == '+')
                    {
                        tile.SpriteID = 2;

                        Pavements[x, yOpposite] = true;
                        Game.entities.Add(new NPC(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16, true));
                    }
                    else if (mapLines[y][x] == 'R')
                    {
                        tile.SpriteID = 3;

                        Roads[x, yOpposite] = true;
                    }
                    else if (mapLines[y][x] == '*')
                    {
                        tile.SpriteID = 3;

                        Roads[x, yOpposite] = true;
                        Game.entities.Add(new Vehicle(tile.Position, new Size(2 * Tools.TileSize, 2 * Tools.TileSize), 100, 4, true));
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
                    Tile tile = new Tile()
                    {
                        Position = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                        Bounds = new Rectangle()
                        {
                            Location = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position), 
                            Size = new Size(Tools.TileSize, Tools.TileSize)
                        },
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
            if (tile.SpriteID == 2)
            {
                screen.FillRectangle(Brushes.LightGray, tile.Bounds);
            }
            if (tile.SpriteID == 3)
            {
                screen.FillRectangle(Brushes.Gray, tile.Bounds);
            }
            if (tile.SpriteID == 4)
            {
                screen.FillRectangle(Brushes.DarkBlue, tile.Bounds);
            }
            if (tile.SpriteID == 5)
            {
                screen.FillRectangle(Brushes.Brown, tile.Bounds);
            }
        }

        public static Tile RandomTile()
        {
            Random random = new Random();

            return WorldMap[random.Next(0, WorldMapSize.X), random.Next(0, WorldMapSize.Y)];
        }
    }
}
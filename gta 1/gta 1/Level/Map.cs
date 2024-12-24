using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static gta_1.Tile;

namespace gta_1
{
    internal class Map
    {
        public static Tile[,] WorldMap;
        public static bool[,] Pavements;
        public static bool[,] Roads;
        public static Point WorldMapSize;

        public static void LoadMapFromFile()
        {
            Regex regLINES = new Regex("\r\n");
            string[] map = regLINES.Split(Properties.Resources.gta_map);

            Array.Reverse(map);

            WorldMapSize = new Point(map[0].Length, map.Length);
            WorldMap = new Tile[WorldMapSize.X, WorldMapSize.Y];
            Pavements = new bool[WorldMapSize.X, WorldMapSize.Y];
            Roads = new bool[WorldMapSize.X, WorldMapSize.Y];

            for (int y = 0; y < WorldMapSize.Y; y++)
            {
                int yOpposite = WorldMapSize.Y - 1 - y;

                for (int x = 0; x < WorldMapSize.X; x++)
                {
                    Tile tile = new Tile(Tools.ConvertPosition_GAME_SCREEN(new Point(x, yOpposite)), new Point(x, y));

                    SetEntityPavementRoad(map[yOpposite][x], tile);

                    WorldMap[x, y] = new Tile(SetTilePassability(map[yOpposite][x], tile));
                }
            }

            //вызываем дважды, чтобы те тайлы, которые рисуются по тегам, отображались нормально
            SetSprites(map);
            SetSprites(map);
        }

        private static void SetSprites(string[] map)
        {
            for (int y = 0; y < WorldMapSize.Y; y++)
            {
                int yOpposite = WorldMapSize.Y - 1 - y;

                for (int x = 0; x < WorldMapSize.X; x++)
                {
                    WorldMap[x, y] = SetTileSprite(map[yOpposite][x], WorldMap[x, y]);
                }
            }
        }

        private static Tile SetTileSprite(char spriteInFile, Tile tile)
        {
            if (tile.MapIndex.X + 1 >= WorldMapSize.X || tile.MapIndex.Y + 1 >= WorldMapSize.Y || tile.MapIndex.X <= 0 || tile.MapIndex.Y <= 0)
                return tile;

            switch (spriteInFile)
            {
                case 'X':
                    tile = SetRoofTile(tile);
                    break;
                case '_':
                    tile = SetGrassTile(tile);
                    break;
                case 's':
                    tile = SetGrassTile(tile);
                    break;
                case 'r':
                    tile = SetGrassTile(tile);
                    break;
                case 'P':
                    tile = SetPavementTile(tile);
                    break;
                case '+':
                    tile = SetPavementTile(tile);
                    break;
                case 'R':
                    tile = SetRoadTile(tile);
                    break;
                case '*':
                    tile = SetRoadTile(tile);
                    break;
            }


            return tile;
        }

        private static void SetEntityPavementRoad(char spriteInFile, Tile tile)
        {
            Pavements[tile.MapIndex.X, tile.MapIndex.Y] = false;
            Roads[tile.MapIndex.X, tile.MapIndex.Y] = false;

            switch (spriteInFile)
            {
                case 's':
                    Game.entities.Add(new Weapon(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 8, 100, true));
                    break;
                case 'r':
                    Game.entities.Add(new Weapon(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 6, 50, true));
                    break;
                case 'P':
                    Pavements[tile.MapIndex.X, tile.MapIndex.Y] = true;
                    break;
                case '+':
                    Pavements[tile.MapIndex.X, tile.MapIndex.Y] = true;
                    Game.entities.Add(new NPC(tile.Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16, true));
                    break;
                case 'R':
                    Roads[tile.MapIndex.X, tile.MapIndex.Y] = true;
                    break;
                case '*':
                    Roads[tile.MapIndex.X, tile.MapIndex.Y] = true;
                    Game.entities.Add(new Vehicle(tile.Position, new Size(2 * Tools.TileSize, 2 * Tools.TileSize), 100, 4, true));
                    break;
            }
        }

        private static Tile SetTilePassability(char spriteInFile, Tile tile)
        {
            switch (spriteInFile)
            {
                case 'X':
                    tile.Passable = false;
                    break;
                default:
                    tile.Passable = true;
                    break;
            }

            return tile;
        }

        public static void RenderTiles(Graphics screen)
        {
            for (int x = 0; x < WorldMapSize.X; x++)
            {
                for (int y = 0; y < WorldMapSize.Y; y++)
                {
                    if (WorldMap[x, y].SptireType == "roof")
                        continue;

                    Tile tile = new Tile(WorldMap[x, y])
                    {
                        Position = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                        Bounds = new Rectangle()
                        {
                            Location = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                            Size = new Size(Tools.TileSize, Tools.TileSize)
                        },
                    };

                    RenderTile(screen, tile);
                }
            }
        }

        private static void RenderTile(Graphics screen, Tile tile)
        {
            if (tile.Sprite == null)
            {
                screen.FillRectangle(Brushes.Purple, tile.Bounds);
                return;
            }

            if (tile.SptireType == "roof")
                return;

            screen.DrawImage(tile.Sprite, tile.Bounds);
            return;
        }

        public static void RenderRoofs(Graphics screen)
        {
            for (int x = 0; x < WorldMapSize.X; x++)
            {
                for (int y = 0; y < WorldMapSize.Y; y++)
                {
                    if (WorldMap[x, y].SptireType != "roof")
                        continue;

                    Tile tile = new Tile(WorldMap[x, y])
                    {
                        Position = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                        Bounds = new Rectangle()
                        {
                            Location = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                            Size = new Size(Tools.TileSize, Tools.TileSize)
                        },
                    };

                    RenderRoof(screen, tile);
                }
            }
        }

        private static void RenderRoof(Graphics screen, Tile tile)
        {
            if (tile.Sprite == null)
            {
                screen.FillRectangle(Brushes.Purple, tile.Bounds);
                return;
            }

            if (tile.SptireType != "roof")
                return;

            Point distanceToPlatyer = new Point()
            {
                X = tile.Position.X - Tools.ScreenCentre.X,
                Y = tile.Position.Y - Tools.ScreenCentre.Y
            };

            tile.Bounds = new Rectangle()
            {
                Location = new Point()
                {
                    X = tile.Position.X - Tools.TileSize / 2 + distanceToPlatyer.X,
                    Y = tile.Position.Y - Tools.TileSize / 2 + distanceToPlatyer.Y
                },
                Size = new Size(2 * Tools.TileSize, 2 * Tools.TileSize)
            };

            screen.DrawImage(tile.Sprite, tile.Bounds);
        }

        public static void RenderWalls(Graphics screen)
        {
            for (int x = 0; x < WorldMapSize.X; x++)
            {
                for (int y = 0; y < WorldMapSize.Y; y++)
                {
                    if (WorldMap[x, y].SptireType != "roof")
                        continue;

                    Tile tile = new Tile(WorldMap[x, y])
                    {
                        Position = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                        Bounds = new Rectangle()
                        {
                            Location = Tools.GetPositionRelativeToPlayer(WorldMap[x, y].Position),
                            Size = new Size(Tools.TileSize, Tools.TileSize)
                        },
                    };

                    RenderWall(screen, tile);
                }
            }
        }

        private static void RenderWall(Graphics screen, Tile tile)
        {
            if (tile.Sprite == null)
            {
                screen.FillRectangle(Brushes.Purple, tile.Bounds);
                return;
            }

            if (tile.SptireType != "roof")
                return;

            Point distanceToPlatyer = new Point()
            {
                X = tile.Position.X - Tools.ScreenCentre.X,
                Y = tile.Position.Y - Tools.ScreenCentre.Y
            };

            tile.Bounds = new Rectangle()
            {
                Location = new Point()
                {
                    X = tile.Position.X - Tools.TileSize / 2 + distanceToPlatyer.X,
                    Y = tile.Position.Y - Tools.TileSize / 2 + distanceToPlatyer.Y
                },
                Size = new Size(2 * Tools.TileSize, 2 * Tools.TileSize)
            };

            Brush wallsColor = Brushes.LightBlue;

            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable)
            {
                Point topRightPosition = new Point(tile.Position.X + Tools.TileSize, tile.Position.Y);
                Point topRightBounds = new Point(tile.Bounds.Right, tile.Bounds.Top);

                Point[] wallCorners = new Point[]
                {
                    tile.Position,
                    tile.Bounds.Location,
                    topRightBounds,
                    topRightPosition
                };

                GraphicsPath wall = new GraphicsPath();
                wall.AddPolygon(wallCorners);
                screen.FillPath(wallsColor, wall);
            }

            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].Passable)
            {
                Point bottomRightPosition = new Point(tile.Position.X + Tools.TileSize, tile.Position.Y + Tools.TileSize);
                Point bottomRightBounds = new Point(tile.Bounds.Right, tile.Bounds.Bottom);
                Point bottomLeftPosition = new Point(tile.Position.X, tile.Position.Y + Tools.TileSize);
                Point bottomLeftBounds = new Point(tile.Bounds.Left, tile.Bounds.Bottom);

                Point[] wallCorners = new Point[]
                {
                    bottomRightPosition,
                    bottomRightBounds,
                    bottomLeftBounds,
                    bottomLeftPosition
                };

                GraphicsPath wall = new GraphicsPath();
                wall.AddPolygon(wallCorners);
                screen.FillPath(wallsColor, wall);
            }

            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].Passable)
            {
                Point topRightPosition = new Point(tile.Position.X + Tools.TileSize, tile.Position.Y);
                Point topRightBounds = new Point(tile.Bounds.Right, tile.Bounds.Top);
                Point bottomRightPosition = new Point(tile.Position.X + Tools.TileSize, tile.Position.Y + Tools.TileSize);
                Point bottomRightBounds = new Point(tile.Bounds.Right, tile.Bounds.Bottom);

                Point[] wallCorners = new Point[]
                {
                    topRightPosition,
                    topRightBounds,
                    bottomRightBounds,
                    bottomRightPosition
                };

                GraphicsPath wall = new GraphicsPath();
                wall.AddPolygon(wallCorners);
                screen.FillPath(wallsColor, wall);
            }

            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].Passable)
            {
                Point bottomLeftPosition = new Point(tile.Position.X, tile.Position.Y + Tools.TileSize);
                Point bottomLeftBounds = new Point(tile.Bounds.Left, tile.Bounds.Bottom);

                Point[] wallCorners = new Point[]
                {
                    bottomLeftPosition,
                    bottomLeftBounds,
                    tile.Bounds.Location,
                    tile.Position
                };

                GraphicsPath wall = new GraphicsPath();
                wall.AddPolygon(wallCorners);
                screen.FillPath(wallsColor, wall);
            }
        }
    }
}
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
using static gta_1.Map;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace gta_1
{
    internal class Map
    {
        public static Tile[,] WorldMap;
        public static bool[,] Pavements;
        public static bool[,] Roads;
        public static Point WorldMapSize;
        private static readonly Random RandomTileRotation = new Random();

        public struct Tile
        {
            public Point Position;
            public Point MapIndex;
            public Bitmap Sprite;
            public string SptireType;
            public bool Passable;
            public Rectangle Bounds;

            public Tile(Point position, Point positionInWorldMap)
            {
                Position = position;
                MapIndex = positionInWorldMap;
                Sprite = null;
                SptireType = string.Empty;
                Passable = true;
                Bounds = new Rectangle(position, new Size(Tools.TileSize, Tools.TileSize));
            }

            public Tile(Tile tile)
            {
                Position = tile.Position;
                MapIndex = tile.MapIndex;
                Sprite = tile.Sprite;
                SptireType = tile.SptireType;
                Passable = tile.Passable;
                Bounds = tile.Bounds;
            }
        }

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

        public static void RenderWorldMap(Graphics screen)
        {
            for (int x = 0; x < WorldMapSize.X; x++)
            {
                for (int y = 0; y < WorldMapSize.Y; y++)
                {
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

            screen.DrawImage(tile.Sprite, tile.Bounds);
            return;
        }

        public static Tile RandomTile()
        {
            Random random = new Random();

            return WorldMap[random.Next(0, WorldMapSize.X), random.Next(0, WorldMapSize.Y)];
        }

        /// <summary>
        /// Set all tile types
        /// </summary>
        private static Tile SetRoofTile(Tile tile)
        {
            tile.SptireType = "roof";

            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].Passable && WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_corner_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].Passable && WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_corner_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].Passable && WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_corner_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].Passable && WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_corner_1;
                return tile;
            }

            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_edge_1;
                return tile;
            }
            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable)
            {
                tile.Sprite = Properties.Resources.roof_edge_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].Passable)
            {
                tile.Sprite = Properties.Resources.roof_edge_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].Passable)
            {
                tile.Sprite = Properties.Resources.roof_edge_1;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }

            switch (RandomTileRotation.Next(0, 4))
            {
                case 0: 
                    tile.Sprite = Properties.Resources.roof_middle;
                    break;
                case 1:
                    tile.Sprite = Properties.Resources.roof_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    tile.Sprite = Properties.Resources.roof_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    tile.Sprite = Properties.Resources.roof_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return tile;
        }

        private static Tile SetPavementTile(Tile tile)
        {
            tile.SptireType = "pavement";

            if (!WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable && WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y - 1].Passable)
            {
                tile.Sprite = Properties.Resources.pavement_edge_end;
                return tile;
            }

            if (!WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].Passable)
            {
                tile.Sprite = Properties.Resources.pavement_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }

            switch (RandomTileRotation.Next(0, 4))
            {
                case 0:
                    tile.Sprite = Properties.Resources.pavement_middle;
                    break;
                case 1:
                    tile.Sprite = Properties.Resources.pavement_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    tile.Sprite = Properties.Resources.pavement_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    tile.Sprite = Properties.Resources.pavement_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return tile;
        }

        private static Tile SetRoadTile(Tile tile)
        {
            tile.SptireType = "road";

            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_corner;
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement" && 
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }

            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.road_edge;
                return tile;
            }

            switch (RandomTileRotation.Next(0, 4))
            {
                case 0:
                    tile.Sprite = Properties.Resources.road_middle;
                    break;
                case 1:
                    tile.Sprite = Properties.Resources.road_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    tile.Sprite = Properties.Resources.road_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    tile.Sprite = Properties.Resources.road_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return tile;
        }

        private static Tile SetGrassTile(Tile tile)
        {
            tile.SptireType = "grass";

            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_corner;
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement" &&
                WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_corner;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }

            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y + 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X, tile.MapIndex.Y - 1].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X + 1, tile.MapIndex.Y].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_edge;
                tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                return tile;
            }
            if (WorldMap[tile.MapIndex.X - 1, tile.MapIndex.Y].SptireType == "pavement")
            {
                tile.Sprite = Properties.Resources.grass_edge;
                return tile;
            }

            switch (RandomTileRotation.Next(0, 4))
            {
                case 0:
                    tile.Sprite = Properties.Resources.grass_middle;
                    break;
                case 1:
                    tile.Sprite = Properties.Resources.grass_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    tile.Sprite = Properties.Resources.grass_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    tile.Sprite = Properties.Resources.grass_middle;
                    tile.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return tile;
        }
    }
}
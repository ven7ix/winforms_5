using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gta_1.Map;

namespace gta_1
{
    public class Tile
    {
        public Point Position;
        public Point MapIndex;
        public Bitmap Sprite;
        public string SptireType;
        public bool Passable;
        public Rectangle Bounds;
        private static readonly Random RandomTileRotation = new Random();

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

        public static Tile RandomTile()
        {
            Random random = new Random();

            return WorldMap[random.Next(0, WorldMapSize.X), random.Next(0, WorldMapSize.Y)];
        }

        /// <summary>
        /// Set all tile types
        /// </summary>
        public static Tile SetRoofTile(Tile tile)
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

        public static Tile SetPavementTile(Tile tile)
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

        public static Tile SetRoadTile(Tile tile)
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

        public static Tile SetGrassTile(Tile tile)
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
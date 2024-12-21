using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    public partial class Game : Form
    {
        public static Entity player;
        readonly List<Entity> entities = new List<Entity>();

        public Game()
        {
            InitializeComponent();

            Tools.Initialize(48, new Point(Screen.Width, Screen.Height));

            Map.LoadMapFromFile();

            Point spawn = new Point()
            {
                X = Tools.ScreenCentre.X + 2 * Tools.TileSize,
                Y = Tools.ScreenCentre.Y - 2 * Tools.TileSize
            };
            player = new Entity(0, false, spawn, new Size(Tools.TileSize, Tools.TileSize), 100, 4);
            Entity car = new Entity(1, true, Map.WorldMap[4, 6].Position, new Size(2 * Tools.TileSize, 2 * Tools.TileSize), 100, 2);
            Entity car2 = new Entity(1, true, Map.WorldMap[10, 6].Position, new Size(2 * Tools.TileSize, 2 * Tools.TileSize), 100, 2);
            //entities.Add(player);
            entities.Add(car);
            entities.Add(car2);
        }

        private void Game_MouseMove(object sender, MouseEventArgs e)
        {
            player.CalculateLookingDirection(e.Location);
            Screen.Invalidate();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                player.Move(new Point(0, 1), entities);
            }
            if (e.KeyCode == Keys.A)
            {
                player.Move(new Point(-1, 0), entities);
            }
            if (e.KeyCode == Keys.S)
            {
                player.Move(new Point(0, -1), entities);
            }
            if (e.KeyCode == Keys.D)
            {
                player.Move(new Point(1, 0), entities);
            }

            if (e.KeyCode == Keys.E)
            {
                foreach (Entity entity in entities)
                    player = entity.Interact(player);
            }

            Screen.Invalidate();
        }

        private void Screen_MouseClick(object sender, MouseEventArgs e)
        {
            Screen.Invalidate();
        }

        private void Screen_Paint(object sender, PaintEventArgs e)
        {
            Graphics screen = e.Graphics;

            Map.RenderWorldMap(screen);

            player.RenderEntity(screen);
            foreach (Entity entity in entities)
                entity.RenderEntity(screen);
        }
    }
}
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
        public static IEntity player;
        public static List<IEntity> entities = new List<IEntity>();

        public Game()
        {
            InitializeComponent();

            Tools.Initialize(48, new Point(Screen.Width, Screen.Height));

            Map.LoadMapFromFile();

            player = new Player(Map.WorldMap[1, 1].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 4);
            entities.Add(player);
            entities.Add(new Vehicle(true, Map.WorldMap[3, 6].Position, new Size(2 * Tools.TileSize, 2 * Tools.TileSize), 100, 2));
            entities.Add(new Vehicle(true, Map.WorldMap[10, 6].Position, new Size(3 * Tools.TileSize, 3 * Tools.TileSize), 100, 2));
            entities.Add(new NPC(true, Map.WorldMap[3, 3].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16));
            entities.Add(new NPC(true, Map.WorldMap[30, 14].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16));
            entities.Add(new NPC(true, Map.WorldMap[15, 17].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16));
            entities.Add(new NPC(true, Map.WorldMap[24, 3].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 16));

            TimerGameLoop.Start();
        }

        private void TimerGameLoop_Tick(object sender, EventArgs e)
        {
            foreach (IEntity entity in entities)
            {
                if (entity is NPC)
                    (entity as NPC).Walk();
            }

            Screen.Invalidate();
        }

        private void Game_MouseMove(object sender, MouseEventArgs e)
        {
            player.CalculateLookingDirection(e.Location);
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                player.Move(new Point(0, 1));
            }
            if (e.KeyCode == Keys.A)
            {
                player.Move(new Point(-1, 0));
            }
            if (e.KeyCode == Keys.S)
            {
                player.Move(new Point(0, -1));
            }
            if (e.KeyCode == Keys.D)
            {
                player.Move(new Point(1, 0));
            }

            if (e.KeyCode == Keys.E)
            {
                foreach (IEntity entity in entities)
                    player = entity.Interact(player);
            }
        }

        private void Screen_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (IEntity entity in entities)
                (player as Player).Hit(entity);

            Screen.Invalidate();
        }

        private void Screen_Paint(object sender, PaintEventArgs e)
        {
            Graphics screen = e.Graphics;

            Map.RenderWorldMap(screen);

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].CheckIfDead())
                {
                    entities.Remove(entities[i--]);
                    continue;
                }

                entities[i].RenderEntity(screen);
            }

            player.RenderEntity(screen);
        }
    }
}
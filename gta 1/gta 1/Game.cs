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
        private readonly HashSet<Keys> PressedMovementKeys = new HashSet<Keys>();

        //Весь рендер происходит относительно игорка (игрок не двигается)
        //А все расчеты происходят относительно карты (карта не двигается)
        public Game()
        {
            InitializeComponent();

            Tools.Initialize(64, new Point(Screen.Width, Screen.Height));

            Map.LoadMapFromFile();

            player = new Player(Map.WorldMap[1, 1].Position, new Size(Tools.TileSize, Tools.TileSize), 100, 8, new Player.Weapon(1, 10));
            entities.Add(player);
            TimerGameLoop.Start();
        }

        private void TimerGameLoop_Tick(object sender, EventArgs e)
        {
            CheckMovementKeys();

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].CheckIfDead())
                    entities.RemoveAt(i--);
            }

            foreach (IEntity entity in entities)
            {
                if (entity is NPC)
                    (entity as NPC).Wonder();
            }

            if (player.CheckIfDead())
                TimerGameLoop.Stop();

            Screen.Invalidate();
        }

        private void Game_MouseMove(object sender, MouseEventArgs e)
        {
            player.CalculateLookingDirection(e.Location);
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.E)
            {
                if (player.IsMoving(player.MovingVector))
                    return;

                foreach (IEntity entity in entities)
                    player = entity.Interact(player);

                if (!entities.Contains(player))
                    entities.Add(player);
            }
            else if (!PressedMovementKeys.Contains(e.KeyCode))
            {
                PressedMovementKeys.Add(e.KeyCode);
            }
        }

        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            PressedMovementKeys.Remove(e.KeyCode);
        }

        private void CheckMovementKeys()
        {
            if (PressedMovementKeys.Contains(Keys.W) && PressedMovementKeys.Contains(Keys.D))
            {
                player.Move(new Point(1, 1));
            }
            else if (PressedMovementKeys.Contains(Keys.W) && PressedMovementKeys.Contains(Keys.A))
            {
                player.Move(new Point(-1, 1));
            }
            else if (PressedMovementKeys.Contains(Keys.S) && PressedMovementKeys.Contains(Keys.D))
            {
                player.Move(new Point(1, -1));
            }
            else if (PressedMovementKeys.Contains(Keys.S) && PressedMovementKeys.Contains(Keys.A))
            {
                player.Move(new Point(-1, -1));
            }
            else if (PressedMovementKeys.Contains(Keys.W))
            {
                player.Move(new Point(0, 1));
            }
            else if (PressedMovementKeys.Contains(Keys.A))
            {
                player.Move(new Point(-1, 0));
            }
            else if (PressedMovementKeys.Contains(Keys.S))
            {
                player.Move(new Point(0, -1));
            }
            else if (PressedMovementKeys.Contains(Keys.D))
            {
                player.Move(new Point(1, 0));
            }
            else
            {
                player.Move(new Point(0, 0));
            }
        }

        private void Screen_MouseClick(object sender, MouseEventArgs e)
        {
            if (player.IsMoving(player.MovingVector))
                return;

            foreach (IEntity entity in entities)
            {
                if (player is Player)
                    (player as Player).Hit(entity);
            }
        }

        private void Screen_Paint(object sender, PaintEventArgs e)
        {
            Graphics screen = e.Graphics;

            Map.RenderTiles(screen);

            foreach (IEntity entity in entities)
                entity.RenderEntity(screen);

            Map.RenderWalls(screen);

            Map.RenderRoofs(screen);
        }

        private void Game_Resize(object sender, EventArgs e)
        {
            Tools.UpdateScreenSize(new Point(Screen.Width, Screen.Height));
        }
    }
}
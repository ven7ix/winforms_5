using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace gta_1
{
    public class Player : IEntity
    {
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }
        public int Speed { get; set; }
        public bool Interactable { get; set; }

        public Point Position { get; set; }
        public Point LookingDirection { get; set; }
        public Rectangle Bounds { get; set; }

        public Player(Point position, Size size, int maximumHP, int speedModifier)
        {
            Interactable = false;

            MaximumHP = maximumHP;
            CurrentHP = maximumHP;
            Speed = Tools.TileSize / speedModifier;

            Position = position;
            Bounds = new Rectangle(position, size);
        }

        public void Move(Point moveDirection)
        {
            Point newPosition = new Point()
            {
                X = Position.X + moveDirection.X * Speed,
                Y = Position.Y - moveDirection.Y * Speed
            };

            if (MoveCollisionCheckObject(newPosition))
                return;

            if (MoveCollisionCheckEntity(newPosition))
                return;

            Position = newPosition;
            Bounds = new Rectangle(newPosition, Bounds.Size);
        }

        public bool MoveCollisionCheckObject(Point newPosition)
        {
            Rectangle newBounds = new Rectangle(newPosition, Bounds.Size);

            for (int x = 0; x < Map.WorldMapSize.X; x++)
            {
                for (int y = 0; y < Map.WorldMapSize.Y; y++)
                {
                    Map.Tile tile = Map.WorldMap[x, y];

                    if (tile.SpriteID != 0)
                    {
                        //проверка входит ли объект в хитбокс игрока (на каждый из 4х углов)
                        if (newBounds.Contains(tile.Position))
                            return true;

                        if (newBounds.Contains(new Point(tile.Position.X + Tools.TileSize - 1, tile.Position.Y)))
                            return true;

                        if (newBounds.Contains(new Point(tile.Position.X, tile.Position.Y + Tools.TileSize - 1)))
                            return true;

                        if (newBounds.Contains(new Point(tile.Position.X + Tools.TileSize - 1, tile.Position.Y + Tools.TileSize - 1)))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool MoveCollisionCheckEntity(Point newPosition)
        {
            foreach (IEntity entity in Game.entities)
            {
                if (entity == this)
                    continue;

                //проверка входит ли хитбокс игрока в сущность (на каждый из 4х углов)
                if (entity.Bounds.Contains(newPosition))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X + Bounds.Width - 1, newPosition.Y)))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X, newPosition.Y + Bounds.Height - 1)))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X + Bounds.Width - 1, newPosition.Y + Bounds.Height - 1)))
                    return true;
            }

            return false;
        }

        public IEntity Interact(IEntity entity)
        {
            return entity;
        }

        public void Hit(IEntity entity)
        {
            Rectangle boundsRelativeToPlayer = new Rectangle()
            {
                Location = new Point()
                {
                    X = entity.Position.X - (Position.X - Tools.ScreenCentre.X),
                    Y = entity.Position.Y - (Position.Y - Tools.ScreenCentre.Y)
                },
                Size = entity.Bounds.Size
            };
            if (!boundsRelativeToPlayer.Contains(LookingDirection))
                return;

            Rectangle distanceReach = new Rectangle()
            {
                Location = new Point()
                {
                    X = Bounds.Location.X - Tools.TileSize,
                    Y = Bounds.Location.Y - Tools.TileSize
                },
                Size = new Size(Bounds.Width + 2 * Tools.TileSize, Bounds.Height + 2 * Tools.TileSize)
            };
            if (!distanceReach.Contains(entity.Bounds))
                return;

            entity.CurrentHP -= 50;
        }

        public void CalculateLookingDirection(Point mousePosition)
        {
            LookingDirection = mousePosition;
        }

        public bool CheckIfDead()
        {
            if (CurrentHP == 0)
            {
                return true;
            }
            return false;
        }

        public void RenderEntity(Graphics screen)
        {
            Point positionRelativeToPlayer = new Point()
            {
                X = Position.X - (Game.player.Position.X - Tools.ScreenSize.X / 2),
                Y = Position.Y - (Game.player.Position.Y - Tools.ScreenSize.Y / 2)
            };

            //Entity
            screen.FillRectangle(Brushes.Black, new Rectangle(positionRelativeToPlayer, Bounds.Size));

            //LookigDirection
            if (Tools.ScreenCentre == positionRelativeToPlayer)
                screen.DrawLine(Pens.Blue, new Point(Tools.ScreenCentre.X + Bounds.Width / 2, Tools.ScreenCentre.Y + Bounds.Height / 2), LookingDirection);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal class NPC : IEntity
    {
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }
        public int Speed { get; set; }
        public bool Interactable { get; set; }

        public Point Position { get; set; }
        public Point LookingDirection { get; set; }
        public Rectangle Bounds { get; set; }
        
        public NPC(bool interactable, Point position, Size size, int maximumHP, int speedModifier)
        {
            Interactable = interactable;

            MaximumHP = maximumHP;
            CurrentHP = maximumHP;
            Speed = Tools.TileSize / speedModifier;

            Position = position;
            Bounds = new Rectangle(position, size);

            Destination = Map.RandomTile().Position;
        }

        public void Move(Point moveDirection)
        {
            Point newPosition = new Point()
            {
                X = Position.X + moveDirection.X * Speed,
                Y = Position.Y - moveDirection.Y * Speed
            };

            if (MoveCollisionCheckObject(newPosition))
            {
                Destination = Map.RandomTile().Position;
                return;
            }
                

            if (MoveCollisionCheckEntity(newPosition))
            {
                Destination = Map.RandomTile().Position;
                return;
            }

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
                        //проверка входит ли стена в хитбокс игрока (на каждый из 4х углов)
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

        Point Destination;
        public void Walk()
        {
            Point moveDirection = new Point(Math.Sign(Destination.X - Position.X), Math.Sign(Destination.Y - Position.Y));

            if (Position == Destination)
                Destination = Map.RandomTile().Position;

            Move(moveDirection);
        }

        public IEntity Interact(IEntity entity)
        {
            if (!Interactable)
                return entity;

            Rectangle boundsRelativeToEntity = new Rectangle()
            {
                Location = new Point()
                {
                    X = Position.X - (entity.Position.X - Tools.ScreenCentre.X),
                    Y = Position.Y - (entity.Position.Y - Tools.ScreenCentre.Y)
                },
                Size = Bounds.Size
            };
            if (!boundsRelativeToEntity.Contains(entity.LookingDirection))
                return entity;

            Rectangle interactableHitbox = new Rectangle()
            {
                Location = new Point()
                {
                    X = Bounds.Location.X - Tools.TileSize,
                    Y = Bounds.Location.Y - Tools.TileSize
                },
                Size = new Size(Bounds.Width + 2 * Tools.TileSize, Bounds.Height + 2 * Tools.TileSize)
            };
            if (!interactableHitbox.Contains(Game.player.Position))
                return entity;

            MessageBox.Show("Hi");

            return entity;
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
            screen.FillRectangle(Brushes.Blue, new Rectangle(positionRelativeToPlayer, Bounds.Size));
        }
    }
}
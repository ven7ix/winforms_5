using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gta_1.Map;

namespace gta_1
{
    public class Entity
    {
        public int Tag { get; set; }
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }
        public int Speed { get; set; }
        public bool Interactable { get; set; }
        public Entity PlayerInside { get; set; }

        public Point Position { get; set; }
        public Point LookingDirection { get; set; }
        public Rectangle Bounds { get; set; }

        public Entity(int tag, bool interactable, Point position, Size size, int maximumHP, int speedModifier)
        {
            Tag = tag;
            Interactable = interactable;

            MaximumHP = maximumHP;
            CurrentHP = maximumHP;
            Speed = Tools.TileSize / speedModifier;

            Position = position;
            Bounds = new Rectangle(position, size);
        }

        public void Move(Point moveDirection, List<Entity> entities)
        {
            Point newPosition = new Point()
            {
                X = Position.X + moveDirection.X * Speed,
                Y = Position.Y - moveDirection.Y * Speed
            };

            if (MoveCollisionCheck(newPosition))
                return;

            if (MoveCollisionCheckEntity(newPosition, entities))
                return;

            Position = newPosition;
            Bounds = new Rectangle(newPosition, Bounds.Size);
        }

        public bool MoveCollisionCheck(Point newPosition)
        {
            for (int x = 0; x < Map.WorldMapSize.X; x++)
            {
                for (int y = 0; y < Map.WorldMapSize.Y; y++)
                {
                    Map.Tile tile = Map.WorldMap[x, y];

                    if (tile.SpriteID != 0)
                    {
                        //проверка на каждый из 4х углов хитбокса
                        if (tile.Bounds.Contains(newPosition))
                            return true;

                        if (tile.Bounds.Contains(new Point(newPosition.X + Bounds.Size.Width - 1, newPosition.Y)))
                            return true;

                        if (tile.Bounds.Contains(new Point(newPosition.X, newPosition.Y + Bounds.Size.Height - 1)))
                            return true;

                        if (tile.Bounds.Contains(new Point(newPosition.X + Bounds.Size.Width - 1, newPosition.Y + Bounds.Size.Height - 1)))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool MoveCollisionCheckEntity(Point newPosition, List<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                if (entity == this)
                    continue;


                if (entity.Bounds.Contains(newPosition))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X + Bounds.Size.Width - 1, newPosition.Y)))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X, newPosition.Y + Bounds.Size.Height - 1)))
                    return true;

                if (entity.Bounds.Contains(new Point(newPosition.X + Bounds.Size.Width - 1, newPosition.Y + Bounds.Size.Height - 1)))
                    return true;
            }

            return false;
        }

        public Entity Interact(Entity entity)
        {
            if (!Interactable)
                return entity;
            
            Point positionRelativeToEntity = new Point()
            {
                X = Position.X - (entity.Position.X - Tools.ScreenSize.X / 2),
                Y = Position.Y - (entity.Position.Y - Tools.ScreenSize.Y / 2)
            };
            Rectangle boundsRelativeToEntity = new Rectangle(positionRelativeToEntity, Bounds.Size);
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

            if (entity.Tag == 1)
            {
                PlayerInside.Position = new Point()
                {
                    X = Position.X - Bounds.Width / 2,
                    Y = Position.Y
                };
                return PlayerInside;
            }

            PlayerInside = entity;
            return this;
        }

        public void CalculateLookingDirection(Point mousePosition)
        {
            LookingDirection = mousePosition;
        }

        public void RenderEntity(Graphics screen)
        {
            Point positionRelativeToPlayer = new Point()
            {
                X = Position.X - (Game.player.Position.X - Tools.ScreenSize.X / 2),
                Y = Position.Y - (Game.player.Position.Y - Tools.ScreenSize.Y / 2)
            };

            //Entity
            if (Tag == 0)
                screen.FillRectangle(Brushes.Black, new Rectangle(positionRelativeToPlayer, Bounds.Size));
            else
                screen.FillRectangle(Brushes.Green, new Rectangle(positionRelativeToPlayer, Bounds.Size));

            //LookigDirection
            if (Tools.ScreenCentre == positionRelativeToPlayer)
                screen.DrawLine(Pens.Blue, new Point(Tools.ScreenCentre.X + Bounds.Width / 2, Tools.ScreenCentre.Y + Bounds.Height / 2), LookingDirection);
        }
    }
}
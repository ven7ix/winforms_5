using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal class Vehicle : IEntity
    {
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }
        public int Speed { get; set; }
        public bool Interactable { get; set; }
        public Player PlayerInside { get; set; }

        public Point MovingVector { get; set; }
        public Point LastMovingVector { get; set; }
        public Point Position { get; set; }
        public Point LookingDirection { get; set; }
        public Rectangle Bounds { get; set; }

        public Bitmap Sprite { get; set; }
        public Bitmap[,] AnimationSprites { get; set; }
        public int Frame { get; set; }
        public int TimeElapsedSinceLastFrame { get; set; }

        public Vehicle(Point position, Size size, int maximumHP, int speedModifier, bool interactable)
        {
            MaximumHP = maximumHP;
            CurrentHP = maximumHP;
            Speed = Tools.TileSize / speedModifier;
            Interactable = interactable;

            Position = position;
            Bounds = new Rectangle(position, size);

            Animation.SetAnimationSprites(this);
        }

        public void Move(Point moveDirection)
        {
            MovingVector = moveDirection;

            if (moveDirection == new Point(0, 0))
                return;

            Point newPosition = new Point()
            {
                X = Position.X + moveDirection.X * Speed,
                Y = Position.Y - moveDirection.Y * Speed
            };

            if (MoveCollisionCheckObject(newPosition))
            {
                if (Math.Abs(moveDirection.X) + Math.Abs(moveDirection.Y) < 2)
                    return;

                newPosition = new Point(Position.X + moveDirection.X * Speed, Position.Y);
                if (MoveCollisionCheckObject(newPosition))
                {
                    newPosition = new Point(Position.X, Position.Y - moveDirection.Y * Speed);
                    if (MoveCollisionCheckObject(newPosition))
                        return;
                }
            }

            if (MoveCollisionCheckEntity(newPosition))
                return;

            Position = newPosition;
            Bounds = new Rectangle(newPosition, Bounds.Size);

            RunOverNPC();
        }

        public bool MoveCollisionCheckObject(Point newPosition)
        {
            Rectangle newBounds = new Rectangle(newPosition, Bounds.Size);

            for (int x = 0; x < Map.WorldMapSize.X; x++)
            {
                for (int y = 0; y < Map.WorldMapSize.Y; y++)
                {
                    Tile tile = Map.WorldMap[x, y];

                    if (!tile.Passable)
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
                if (entity is NPC)
                    continue;

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

        private void RunOverNPC()
        {
            foreach (IEntity entity in Game.entities)
            {
                if (!(entity is NPC))
                    continue;

                if (Bounds.Contains(entity.Position))
                    entity.CurrentHP = 0;

                if (Bounds.Contains(new Point(entity.Position.X + entity.Bounds.Width - 1, entity.Position.Y)))
                    entity.CurrentHP = 0;

                if (Bounds.Contains(new Point(entity.Position.X, entity.Position.Y + entity.Bounds.Height - 1)))
                    entity.CurrentHP = 0;

                if (Bounds.Contains(new Point(entity.Position.X + entity.Bounds.Width - 1, entity.Position.Y + entity.Bounds.Height - 1)))
                    entity.CurrentHP = 0;
            }
        }

        public IEntity Interact(IEntity entity)
        {
            if (!Interactable)
                return entity;

            if (!Tools.GetBoundsRelativeToEntity(this, entity).Contains(entity.LookingDirection))
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

            if (entity == this)
            {
                PlayerInside.Position = new Point()
                {
                    X = Position.X - Tools.TileSize,
                    Y = Position.Y
                };

                if (MoveCollisionCheckObject(PlayerInside.Position))
                    return entity;

                if (MoveCollisionCheckEntity(PlayerInside.Position))
                    return entity;

                return PlayerInside;
            }

            if (entity is Player)
            {
                PlayerInside = new Player(entity as Player);
                entity.CurrentHP = 0;
                return this;
            }

            return entity;
        }

        public void CalculateLookingDirection(Point mousePosition)
        {
            LookingDirection = mousePosition;
        }

        public bool CheckIfDead()
        {
            return CurrentHP == 0;
        }

        public bool IsMoving(Point movingVector)
        {
            return !(movingVector.X == 0 && movingVector.Y == 0);
        }

        public bool IsMovingDiagonally(Point movingVector)
        {
            return Math.Abs(movingVector.X) + Math.Abs(movingVector.Y) == 2;
        }

        public void RenderEntity(Graphics screen)
        {
            //Vehicle
            Animation.AnimationHanler(this, 20);
            screen.DrawImage(Sprite, new Rectangle(Tools.GetPositionRelativeToPlayer(Position), Bounds.Size));
        }
    }
}
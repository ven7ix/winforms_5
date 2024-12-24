using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta_1
{
    internal class Weapon : IEntity
    {
        public int MaximumHP { get; set; }
        public int CurrentHP { get; set; }
        public int Speed { get; set; }
        public bool Interactable { get; set; }
        public int Range { get; set; }
        public int Damage { get; set; }

        public Point MovingVector { get; set; }
        public Point LastMovingVector { get; set; }
        public Point Position { get; set; }
        public Point LookingDirection { get; set; }
        public Rectangle Bounds { get; set; }

        public Bitmap Sprite { get; set; }
        public Bitmap[,] AnimationSprites { get; set; }
        public int Frame { get; set; }
        public int TimeElapsedSinceLastFrame { get; set; }

        public Weapon(Point position, Size size, int range, int damage, bool interactable)
        {
            MaximumHP = -1;
            CurrentHP = -1;
            Speed = 0;
            Range = range;
            Damage = damage;
            Interactable = interactable;

            Position = position;
            LookingDirection = position;
            Bounds = new Rectangle(position, size);
        }

        public void Move(Point moveDirection)
        {
            return;
        }

        public bool MoveCollisionCheckObject(Point newPosition)
        {
            return false;
        }

        public bool MoveCollisionCheckEntity(Point newPosition)
        {
            return false;
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

            if (entity is Player)
            {
                CurrentHP = 0;
                (entity as Player).EquippedWeapon = new Player.Weapon(Range, Damage);
            }

            return entity;
        }

        public void CalculateLookingDirection(Point mousePosition)
        {
            return;
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
            //Weapon
            screen.FillRectangle(Brushes.Brown, new Rectangle(Tools.GetPositionRelativeToPlayer(Position), Bounds.Size));
        }
    }
}
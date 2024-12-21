using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        //speedModifier должен быть делителем TileSize
        public Player(Point position, Size size, int maximumHP, int speedModifier)
        {
            MaximumHP = maximumHP;
            CurrentHP = maximumHP;
            Speed = Tools.TileSize / speedModifier;
            Interactable = false;

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

            if (MoveCollisionCheck(newPosition))
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

        public void CalculateLookingDirection(Point mousePosition)
        {
            LookingDirection = mousePosition;
        }

        public IEntity Interact(IEntity entity)
        {
            return this;
        }

        public void RenderEntity(Graphics screen)
        {
            Point positionRelativeToPlayer = new Point()
            {
                X = Position.X - (Game.player.Position.X - Tools.ScreenSize.X / 2),
                Y = Position.Y - (Game.player.Position.Y - Tools.ScreenSize.Y / 2)
            };
            //Player
            screen.FillRectangle(Brushes.Black, new Rectangle(positionRelativeToPlayer, Bounds.Size));
            //LookigDirection
            if (Tools.ScreenCentre == positionRelativeToPlayer)
                screen.DrawLine(Pens.Blue, new Point(Tools.ScreenCentre.X + Bounds.Size.Width / 2, Tools.ScreenCentre.Y + Bounds.Size.Height / 2), LookingDirection);
        }
    }
}
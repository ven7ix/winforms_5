using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace gta_1
{
    internal class Animation
    {
        private static readonly int FrameDelay = 60;
        private static int AnimationFrames = 0;
        private static readonly Random random = new Random();

        public static void SetAnimationSprites(IEntity entity)
        {
            if (entity is Player)
            {
                entity.Sprite = Properties.Resources.player_1;
                entity.AnimationSprites = new Bitmap[8, 2];

                entity.AnimationSprites[0, 0] = Properties.Resources.player_1;
                entity.AnimationSprites[1, 0] = Properties.Resources.player_2;
                entity.AnimationSprites[2, 0] = Properties.Resources.player_3;
                entity.AnimationSprites[3, 0] = Properties.Resources.player_2;
                entity.AnimationSprites[4, 0] = Properties.Resources.player_1;
                entity.AnimationSprites[5, 0] = Properties.Resources.player_4;
                entity.AnimationSprites[6, 0] = Properties.Resources.player_5;
                entity.AnimationSprites[7, 0] = Properties.Resources.player_4;

                entity.AnimationSprites[0, 1] = Properties.Resources.player_diagonal_1;
                entity.AnimationSprites[1, 1] = Properties.Resources.player_diagonal_2;
                entity.AnimationSprites[2, 1] = Properties.Resources.player_diagonal_3;
                entity.AnimationSprites[3, 1] = Properties.Resources.player_diagonal_2;
                entity.AnimationSprites[4, 1] = Properties.Resources.player_diagonal_1;
                entity.AnimationSprites[5, 1] = Properties.Resources.player_diagonal_4;
                entity.AnimationSprites[6, 1] = Properties.Resources.player_diagonal_5;
                entity.AnimationSprites[7, 1] = Properties.Resources.player_diagonal_4;

                return;
            }
            if (entity is Vehicle)
            {
                entity.AnimationSprites = new Bitmap[1, 2]; //first is frames, second is animation type

                int randomSprite = random.Next(0, 6);

                switch (randomSprite)
                {
                    case 0:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_1;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_1;
                        break;
                    case 1:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_2;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_2;
                        break;
                    case 2:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_3;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_3;
                        break;
                    case 3:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_4;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_4;
                        break;
                    case 4:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_5;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_5;
                        break;
                    case 5:
                        entity.AnimationSprites[0, 0] = Properties.Resources.car_6;
                        entity.AnimationSprites[0, 1] = Properties.Resources.car_diagonal_6;
                        break;
                }
                entity.AnimationSprites[0, 0].RotateFlip(RotateFlipType.Rotate180FlipNone);
                entity.AnimationSprites[0, 1].RotateFlip(RotateFlipType.Rotate180FlipNone);

                entity.Sprite = entity.AnimationSprites[0, 0];

                return;
            }
            if (entity is NPC)
            {
                entity.Sprite = Properties.Resources.player_1;
                entity.AnimationSprites = new Bitmap[8, 2];

                entity.AnimationSprites[0, 0] = Properties.Resources.player_1;
                entity.AnimationSprites[1, 0] = Properties.Resources.player_2;
                entity.AnimationSprites[2, 0] = Properties.Resources.player_3;
                entity.AnimationSprites[3, 0] = Properties.Resources.player_2;
                entity.AnimationSprites[4, 0] = Properties.Resources.player_1;
                entity.AnimationSprites[5, 0] = Properties.Resources.player_4;
                entity.AnimationSprites[6, 0] = Properties.Resources.player_5;
                entity.AnimationSprites[7, 0] = Properties.Resources.player_4;

                entity.AnimationSprites[0, 1] = Properties.Resources.player_diagonal_1;
                entity.AnimationSprites[1, 1] = Properties.Resources.player_diagonal_2;
                entity.AnimationSprites[2, 1] = Properties.Resources.player_diagonal_3;
                entity.AnimationSprites[3, 1] = Properties.Resources.player_diagonal_2;
                entity.AnimationSprites[4, 1] = Properties.Resources.player_diagonal_1;
                entity.AnimationSprites[5, 1] = Properties.Resources.player_diagonal_4;
                entity.AnimationSprites[6, 1] = Properties.Resources.player_diagonal_5;
                entity.AnimationSprites[7, 1] = Properties.Resources.player_diagonal_4;

                return;
            }
        }

        public static void AnimationHanler(IEntity entity, int deltaTime)
        {
            entity.TimeElapsedSinceLastFrame += deltaTime;
            if (entity.TimeElapsedSinceLastFrame < FrameDelay)
                return;

            entity.TimeElapsedSinceLastFrame = 0;
            AnimationFrames = entity.AnimationSprites.GetLength(0);

            if (!entity.IsMoving(entity.MovingVector))
            {
                entity.Sprite = (Bitmap)entity.AnimationSprites[0, 0].Clone();
                if (entity.IsMovingDiagonally(entity.LastMovingVector))
                    entity.Sprite = (Bitmap)entity.AnimationSprites[0, 1].Clone();

                RotateSprite(entity.LastMovingVector, entity);
                return;
            }

            entity.Sprite = (Bitmap)entity.AnimationSprites[entity.Frame % AnimationFrames, 0].Clone();
            if (entity.IsMovingDiagonally(entity.MovingVector))
                entity.Sprite = (Bitmap)entity.AnimationSprites[entity.Frame % AnimationFrames, 1].Clone();

            RotateSprite(entity.MovingVector, entity);

            entity.Frame++;
            entity.Frame %= 8;
            entity.LastMovingVector = entity.MovingVector;
        }

        private static void RotateSprite(Point vector, IEntity entity)
        {
            if (vector.X == 1 && vector.Y == 1)
            {
                //no rotation
            }
            else if (vector.X == 1 && vector.Y == -1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (vector.X == -1 && vector.Y == 1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (vector.X == -1 && vector.Y == -1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (vector.X == 1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (vector.X == -1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (vector.Y == 1)
            {
                //no rotation
            }
            else if (vector.Y == -1)
            {
                entity.Sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
        }
    }
}

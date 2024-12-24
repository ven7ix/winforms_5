using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

namespace gta_1
{
    public interface IEntity
    {
        int MaximumHP { get; set; }
        int CurrentHP { get; set; }
        int Speed { get; set; }
        bool Interactable { get; set; }

        Point MovingVector { get; set; }
        Point LastMovingVector { get; set; }
        Point Position { get; set; }
        Point LookingDirection { get; set; }
        Rectangle Bounds { get; set; }

        Bitmap Sprite { get; set; }
        Bitmap[,] AnimationSprites { get; set; }
        int Frame { get; set; }
        int TimeElapsedSinceLastFrame { get; set; }

        void Move(Point moveDirection);

        bool MoveCollisionCheckObject(Point newPosition);

        bool MoveCollisionCheckEntity(Point newPosition);

        IEntity Interact(IEntity entity);

        void CalculateLookingDirection(Point mousePosition);

        bool CheckIfDead();

        bool IsMoving(Point movingVector);

        bool IsMovingDiagonally(Point movingVector);

        void RenderEntity(Graphics screen);
    }
}

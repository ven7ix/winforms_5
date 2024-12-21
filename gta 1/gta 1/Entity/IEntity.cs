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

        Point Position { get; set; }
        Point LookingDirection { get; set; }
        Rectangle Bounds { get; set; }

        void Move(Point moveDirection);

        bool MoveCollisionCheckObject(Point newPosition);

        bool MoveCollisionCheckEntity(Point newPosition);

        IEntity Interact(IEntity entity);

        void CalculateLookingDirection(Point mousePosition);

        bool CheckIfDead();

        void RenderEntity(Graphics screen);
    }
}

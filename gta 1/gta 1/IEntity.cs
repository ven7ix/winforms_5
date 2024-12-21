using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gta_1
{
    public interface IEntity
    {
        int MaximumHP { get; set; }
        int CurrentHP { get; set; }
        bool Interactable { get; set; }

        Point Position { get; set; }
        Point LookingDirection { get; set; }
        Rectangle Bounds { get; set; }

        void Move(Point moveDirection);
        bool MoveCollisionCheck(Point newPosition);
        void CalculateLookingDirection(Point mousePosition);
        IEntity Interact(IEntity entity);
        void RenderEntity(Graphics screen);
    }
}

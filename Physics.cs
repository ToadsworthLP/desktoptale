using System.Collections.Generic;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class Physics
    {
        public IPhysicsObject PhysicsObjectUnderCursor { get; private set; }
        public bool HasColliderUnderCursorChanged { get; private set; }
        
        private ISet<IPhysicsObject> colliders;
        private InputManager inputManager;

        public Physics(InputManager inputManager)
        {
            this.inputManager = inputManager;
            
            colliders = new HashSet<IPhysicsObject>();
        }

        public void Update()
        {
            IPhysicsObject physicsObject = GetColliderAtPosition(inputManager.PointerPosition.ToVector2());
            if (PhysicsObjectUnderCursor != physicsObject) HasColliderUnderCursorChanged = true;
            PhysicsObjectUnderCursor = physicsObject;
            
            if(inputManager.LeftClickJustPressed) PhysicsObjectUnderCursor?.OnLeftClicked();
            if(inputManager.RightClickJustPressed) PhysicsObjectUnderCursor?.OnRightClicked();

            if (inputManager.LeftClickJustPressed && PhysicsObjectUnderCursor == null)
            {
                MessageBus.Send(new UnfocusAllCharactersMessage());
            }
        }

        public void AddCollider(IPhysicsObject physicsObject)
        {
            colliders.Add(physicsObject);
        }

        public bool RemoveCollider(IPhysicsObject physicsObject)
        {
            return colliders.Remove(physicsObject);
        }

        public IPhysicsObject GetColliderAtPosition(Vector2 position)
        {
            IPhysicsObject result = null;
            float lowestZ = float.MaxValue;
            foreach (IPhysicsObject collider in colliders)
            {
                if (collider.HitBox.Contains(position) && collider.Depth < lowestZ)
                {
                    result = collider;
                    lowestZ = collider.Depth;
                }
            }
            
            return result;
        }
    }
}
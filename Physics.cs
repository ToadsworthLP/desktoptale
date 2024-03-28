using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class Physics
    {
        public ICollider ColliderUnderCursor { get; private set; }
        public bool HasColliderUnderCursorChanged { get; private set; }
        
        private ISet<ICollider> colliders;
        private InputManager inputManager;

        public Physics(InputManager inputManager)
        {
            this.inputManager = inputManager;
            
            colliders = new HashSet<ICollider>();
        }

        public void Update()
        {
            ICollider collider = GetColliderAtPosition(inputManager.PointerPosition.ToVector2());
            if (ColliderUnderCursor != collider) HasColliderUnderCursorChanged = true;
            ColliderUnderCursor = collider;
        }

        public void AddCollider(ICollider collider)
        {
            colliders.Add(collider);
        }

        public bool RemoveCollider(ICollider collider)
        {
            return colliders.Remove(collider);
        }

        public ICollider GetColliderAtPosition(Vector2 position)
        {
            ICollider result = null;
            float highestZ = float.MinValue;
            foreach (ICollider collider in colliders)
            {
                if (collider.HitBox.Contains(position) && collider.Depth > highestZ)
                {
                    result = collider;
                    highestZ = collider.Depth;
                }
            }
            
            return result;
        }
    }
}
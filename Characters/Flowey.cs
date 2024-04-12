using System;
using Desktoptale.States.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Flowey : Character
    {
        public Flowey(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void Initialize()
        {
            base.Initialize();

            IdleState = InitialState;
            WalkState = new WalkState(0, true);
            RunState = new RunState(0, true);
            RandomMovementState = new RandomMovementState(0);
            EnabledAutoOrientation = false;
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite;
            
            Random rng = new Random(GetHashCode());
            if (rng.Next(0, 100) == 0)
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_FloweyNewCut_Idle_Up"),    
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_FloweyNewCut_Idle_Down"),    
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_FloweyNewCut_Idle_Left"),    
                    1
                );
            }
            else
            {
                idleSprite = new OrientedAnimatedSprite(
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Up"),    
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Down"),    
                    contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Left"),
                    1
                );
            }
            
            
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;
            WalkSprite = idleSprite;
            RunSprite = idleSprite;
        }
    }
}
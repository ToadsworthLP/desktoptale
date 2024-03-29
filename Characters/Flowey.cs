﻿using Desktoptale.States.Flowey;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Flowey : Character
    {
        protected override IState<Character> InitialState => new FloweyIdleState();

        public Flowey(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void Initialize()
        {
            base.Initialize();

            IdleState = InitialState;
            EnabledAutoOrientation = false;
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Up"),    
                contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Down"),    
                contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Left"),    
                contentManager.Load<Texture2D>("Included/Flowey/Spr_Flowey_Idle_Right"),    
                1
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;
        }
    }
}
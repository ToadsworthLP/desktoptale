﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Axis : Character
    {
        public Axis(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}

        public override void LoadContent(ContentManager contentManager)
    {
        OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Up"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Down"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Left"),
            7
        );
        idleSprite.Loop = true;
        idleSprite.Framerate = 10;
        IdleSprite = idleSprite;

        OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Up"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Down"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Left"),
            7
        );
        walkSprite.Loop = true;
        walkSprite.Framerate = 10;
        WalkSprite = walkSprite;

        OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Up"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Down"),    
            contentManager.Load<Texture2D>("Included/Axis/Spr_Axis_Idle_Left"),
            7
        );
        runSprite.Loop = true;
        runSprite.Framerate = 10;
        RunSprite = runSprite;
    }
    }
}
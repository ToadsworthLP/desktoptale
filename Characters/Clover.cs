using Desktoptale.States.Clover;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Clover : Character
    {
        public IState<Character> DanceState { get; protected set; }
        public IAnimatedSprite DanceSprite { get; protected set; }
        
        public Clover(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}

        public override void Initialize()
        {
            base.Initialize();

            RandomMovementWaitState = new CloverRandomMovementWaitState();
            DanceState = new CloverDanceState();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
        
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Up"),    
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Down"),    
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Idle_Left"),    
                1
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Up"), 4,
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Down"), 4,
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Walk_Left"), 2
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Up"),    
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Down"),    
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Run_Left"),    
                6
            );
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;

            AnimatedSprite danceSprite = new AnimatedSprite(
                contentManager.Load<Texture2D>("Characters/Clover/Spr_Clover_Dance"),
                6
            );
            danceSprite.Loop = true;
            danceSprite.Framerate = 7;
            DanceSprite = danceSprite;
        }
    }
}
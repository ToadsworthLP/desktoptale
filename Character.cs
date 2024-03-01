using System;
using Desktoptale.Messages;
using Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;

namespace Desktoptale;

public class Character : IGameObject
{
    public float MovementSpeed { get; set; }
    
    private GameWindow window;
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private InputManager inputManager;

    private IAnimatedSprite sprite;
    private Vector2 previousVelocity = Vector2.Zero;
    private Vector2 scale;
    private bool dragging;

    public Character(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager)
    {
        this.spriteBatch = spriteBatch;
        this.inputManager = inputManager;
        this.graphics = graphics;
        this.window = window;
    }

    public void Initialize()
    {
        MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
        
        MovementSpeed = 100;
    }

    public void LoadContent(ContentManager contentManager)
    {
        OrientedAnimatedSprite animatedSprite = new OrientedAnimatedSprite();
        animatedSprite.Loop = true;
        animatedSprite.Framerate = 5;
        animatedSprite.Orientation = Orientation.Down;
        
        LoadOrientedSpriteFrames(contentManager, animatedSprite, Orientation.Up, 
            "Characters/Clover/Spr_Clover_Idle_Up",
            "Characters/Clover/Spr_Clover_Walk_0_Up",
            "Characters/Clover/Spr_Clover_Idle_Up",
            "Characters/Clover/Spr_Clover_Walk_1_Up"
        );
        
        LoadOrientedSpriteFrames(contentManager, animatedSprite, Orientation.Down, 
            "Characters/Clover/Spr_Clover_Idle_Down",
            "Characters/Clover/Spr_Clover_Walk_0_Down",
            "Characters/Clover/Spr_Clover_Idle_Down",
            "Characters/Clover/Spr_Clover_Walk_1_Down"
        );
        
        LoadOrientedSpriteFrames(contentManager, animatedSprite, Orientation.Left, 
            "Characters/Clover/Spr_Clover_Idle_Left",
            "Characters/Clover/Spr_Clover_Walk_Left"
        );
        
        LoadOrientedSpriteFrames(contentManager, animatedSprite, Orientation.Right, 
            "Characters/Clover/Spr_Clover_Idle_Right",
            "Characters/Clover/Spr_Clover_Walk_Right"
        );

        sprite = animatedSprite;
    }
    
    public void Update(GameTime gameTime)
    {
        sprite.Update(gameTime);
        
        Vector2 velocity = inputManager.DirectionalInput * MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        velocity *= MathF.Min(scale.X, scale.Y);

        Orientation? updatedOrientation = GetOrientationFromVelocity(velocity);
        if (updatedOrientation != null && sprite is OrientedAnimatedSprite animatedSprite) animatedSprite.Orientation = updatedOrientation.Value;

        if (previousVelocity != velocity)
        {
            if (velocity.LengthSquared() > float.Epsilon)
            {
                sprite.Play();
                sprite.CurrentFrameIndex = 1;
            }
            else
            {
                sprite.Stop();
            }
        }
        
        Point position = window.Position;
        position.X += (int)Math.Round(velocity.X);
        position.Y += (int)Math.Round(velocity.Y);
        window.Position = position;

        previousVelocity = velocity;
        
        DragCharacter();
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Vector2 center = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
        Vector2 origin = new Vector2(sprite.CurrentFrame.Width / 2f, sprite.CurrentFrame.Height / 2f);
        sprite.Draw(spriteBatch, center, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
        spriteBatch.End();
    }

    private Orientation? GetOrientationFromVelocity(Vector2 input)
    {
        if (input.Y < 0) return Orientation.Up;
        if (input.Y > 0) return Orientation.Down;
        if (input.X < 0) return Orientation.Left;
        if (input.X > 0) return Orientation.Right;

        return null;
    }

    private void DragCharacter()
    {
        if (inputManager.LeftClickJustPressed &&
            graphics.GraphicsDevice.Viewport.Bounds.Contains(inputManager.PointerPosition))
        {
            dragging = true;
        } else if (!inputManager.LeftClickPressed)
        {
            dragging = false;
        }

        if (dragging)
        {
            Point windowCenter = window.Position -
                                 new Point(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            window.Position = windowCenter + inputManager.PointerPosition;
        }
    }

    private void LoadOrientedSpriteFrames(ContentManager contentManager, OrientedAnimatedSprite sprite, params string[] basePaths)
    {
        foreach (string basePath in basePaths)
        {
            sprite.Add(Orientation.Down, contentManager.Load<Texture2D>($"{basePath}_Down"));
            sprite.Add(Orientation.Up, contentManager.Load<Texture2D>($"{basePath}_Up"));
            sprite.Add(Orientation.Left, contentManager.Load<Texture2D>($"{basePath}_Left"));
            sprite.Add(Orientation.Right, contentManager.Load<Texture2D>($"{basePath}_Right"));
        }
    }
    
    private void LoadOrientedSpriteFrames(ContentManager contentManager, OrientedAnimatedSprite sprite, Orientation orientation, params string[] paths)
    {
        foreach (string path in paths)
        {
            sprite.Add(orientation, contentManager.Load<Texture2D>(path));
        }
    }

    private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
    {
        scale = new Vector2(message.ScaleFactor, message.ScaleFactor);
        
        graphics.PreferredBackBufferWidth = (int)(20 * message.ScaleFactor);
        graphics.PreferredBackBufferHeight = (int)(30 * message.ScaleFactor);
        graphics.ApplyChanges();
    }
}
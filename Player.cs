using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer.Animations;

namespace VaniaPlatformer;

public class Player : GameActor {
    
    // Properties 
    private AnimationManager animationManager;
    private Vector2 boundingSize;

    // Constructors
    public Player(Texture2D textureSheet, int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {
        this.animationManager = new AnimationManager();
        this.boundingSize = new Vector2(collisionWidth,collisionHeight);

        Position = new Vector2(startX,startY);
        Velocity = new Vector2(0,0);
        BoundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y, 
            (int)boundingSize.X,
            (int)boundingSize.Y);

        InitializeAnimations(textureSheet);
    }

    // Methods
    private void InitializeAnimations(Texture2D textureSheet) {
        var animations = new List<Animation>();

        Animation idle = new Animation(Common.AnimationIndex.Idle, textureSheet, new Vector2(0,0), new Vector2(48,48), 3, 0.35f, true);

        animations.Add(idle);

        foreach(var a in animations) {
            animationManager.AddAnimation(a.Index,a);
        }

        animationManager.Play(Common.AnimationIndex.Idle);
    }

    public void Draw(SpriteBatch spriteBatch) {
        animationManager.Draw(spriteBatch, BoundingBox, Color.White);
    }

    public override void Update(GameTime gameTime) {
        animationManager.Update(gameTime);
    }

    public override void MoveAndSlide()
    {
        throw new System.NotImplementedException();
    }
}
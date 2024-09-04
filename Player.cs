using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer.Animations;

namespace VaniaPlatformer;

public class Player : Actor {
    
    // Fields
    private string textureSheetId;
    private TextureRef textureRef;
    private Rectangle boundingBox;

    // Properties 
    private AnimationManager animationManager;
    private Vector2 boundingSize;

    // Constructors
    public Player(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {
        
        textureSheetId = "PlayerSheet";
        textureRef = Common.GetTextureRefByID(textureSheetId);
        animationManager = new AnimationManager();
        boundingSize = new Vector2(collisionWidth,collisionHeight);

        Position = new Vector2(startX,startY);
        Velocity = new Vector2(0,0);
        boundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y, 
            (int)boundingSize.X,
            (int)boundingSize.Y);

        Colliders.Add(boundingBox);

        InitializeAnimations(textureRef.LoadTexture());
    }

    // Methods
    private void InitializeAnimations(Texture2D textureSheet) {
        var animations = new List<Animation>();

        Animation idle = new Animation(Common.AnimationIndex.Idle, textureSheet, new Vector2(50,38), new Vector2(50,37), 6, 0.15f, false);

        animations.Add(idle);

        foreach(var a in animations) {
            animationManager.AddAnimation(a.Index,a);
        }

        animationManager.Play(Common.AnimationIndex.Idle);
    }

    public void Draw(SpriteBatch spriteBatch) {
        animationManager.Draw(spriteBatch, boundingBox, Color.White);
    }

    public override void Update(GameTime gameTime) {
        animationManager.Update(gameTime);
    }

    public override void MoveAndSlide()
    {
        throw new System.NotImplementedException();
    }
}
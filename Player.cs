using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.Animations;

namespace VaniaPlatformer;

public class Player : Actor {
    
    // Constants
    private const string ANIM_KEY_IDLE = "Idle";

    // Fields
    private string textureSheetId;
    private Rectangle boundingBox;
    private float moveSpeed;

    // Properties 
    private AnimationManager animationManager;
    private Vector2 boundingSize;

    // Constructors
    public Player(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {
        
        textureSheetId = "textures/AdventurerSheet";
        animationManager = new AnimationManager();
        boundingSize = new Vector2(collisionWidth,collisionHeight);
        moveSpeed = 128.0f;

        Colliders = new List<Rectangle>{ boundingBox };
        Position = new Vector2(startX,startY);
        Velocity = new Vector2(0,0);
        
        UpdateBoundingBox();
        Colliders.Add(boundingBox);

        InitializeAnimations();
    }

    // Methods
    private void InitializeAnimations() {

        Texture2D textureSheet = Globals.Content.Load<Texture2D>(textureSheetId);

        animationManager.AddAnimation(
            ANIM_KEY_IDLE,
            new Animation(
                textureSheet, 
                new Vector2(50,38), 
                new Vector2(50,37), 
                6, 0.05f, false)
        );

        animationManager.Play(ANIM_KEY_IDLE);
    }

    public void Draw(SpriteBatch spriteBatch) {
        animationManager.Draw(spriteBatch, boundingBox, Color.White);
    }

    public override void Update() {
        KeyboardState keyboardState = Keyboard.GetState();

        if(keyboardState.IsKeyDown(Keys.A)) {
            float velocityX = (Velocity.X - moveSpeed) * (float)Globals.DeltaTime;

            Velocity = new Vector2(velocityX, Velocity.Y);
        }

        if(keyboardState.IsKeyDown(Keys.D)) {
            float velocityX = (Velocity.X + moveSpeed) * (float)Globals.DeltaTime;

            Velocity = new Vector2(velocityX, Velocity.Y);
        }

        if((int)Velocity.X == 0) {
            animationManager.Stop();
        }
        else {
            animationManager.Play();
        }

        MoveAndSlide();

        animationManager.Update();
    }

    public override void MoveAndSlide()
    {
        if(Velocity != Vector2.Zero) {
            int proposedX = (int)(Position.X + Velocity.X);
            int proposedY = (int)(Position.Y + Velocity.Y);

            // <Todo: Propose Position and Check for Collisions Here!>

            Position = new Vector2(proposedX, proposedY);

            Velocity = new Vector2(0,0);

            UpdateBoundingBox();
        }
    }

    private void UpdateBoundingBox() {
        boundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y, 
            (int)boundingSize.X,
            (int)boundingSize.Y);
    }
}
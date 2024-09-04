using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.Animations;

namespace VaniaPlatformer;

public class Player : Actor {
    
    // Constants
    private const string ANIM_KEY_IDLE = "Idle";

    // Events
    public event EventHandler<MoveEventArgs> IsMoving;

    // Fields
    private string textureSheetId;
    private Rectangle boundingBox;
    private float moveSpeed;
    private float jumpSpeed;
    private float maxFallSpeed;
    private float gravity;
    List<Rectangle> collisions;
    bool onGround;

    // Properties 
    private AnimationManager animationManager;
    private Vector2 boundingSize;

    // Constructors
    public Player(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {
        
        collisions = new List<Rectangle>();
        textureSheetId = "textures/AdventurerSheet";
        animationManager = new AnimationManager();
        boundingSize = new Vector2(collisionWidth,collisionHeight);
        moveSpeed = 128.0f;
        jumpSpeed = 256.0f;
        onGround = false;

        gravity = 0.35f;
        maxFallSpeed = 128.0f;

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
        spriteBatch.Draw(Globals.DebugTexture, boundingBox, Color.Yellow * 0.5f);
    }

    public override void Update() {
        KeyboardState keyboardState = Keyboard.GetState();

        if(!onGround) {
            // Add Gravity
            float currentFallSpeed = Velocity.Y;

            if(currentFallSpeed < maxFallSpeed) {
                currentFallSpeed += gravity;

                Velocity = new Vector2(Velocity.X, currentFallSpeed);
            }
        }

        if(keyboardState.IsKeyDown(Keys.A)) {
            float velocityX = (Velocity.X - moveSpeed) * (float)Globals.DeltaTime;

            Velocity = new Vector2(velocityX, Velocity.Y);
        }

        if(keyboardState.IsKeyDown(Keys.D)) {
            float velocityX = (Velocity.X + moveSpeed) * (float)Globals.DeltaTime;

            Velocity = new Vector2(velocityX, Velocity.Y);
        }

        if(keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D)) {
            Velocity = new Vector2(0, Velocity.Y);
        }

        if(keyboardState.IsKeyDown(Keys.W) && onGround) {
            float jumpVelocity = -(jumpSpeed * (float)Globals.DeltaTime);

            Velocity = new Vector2(Velocity.X, jumpVelocity);
            
            onGround = false;
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
        if(Velocity.X != 0 || Velocity.Y != 0) {
            int proposedX = (int)(Position.X + Velocity.X);
            int proposedY = (int)(Position.Y + Velocity.Y);
            Vector2 proposedPosition = new Vector2(proposedX, proposedY);

            OnMoving(this, new MoveEventArgs(
                proposedPosition,
                new Rectangle(
                    (int)proposedPosition.X,
                    (int)proposedPosition.Y,
                    boundingBox.Width,
                    boundingBox.Height
                )));
            
            // Iterate through all Collisions and Correct ProposedPosition
            if(collisions.Count > 0) {
                foreach(Rectangle collision in collisions) {
                    if(collision.Height >= collision.Width) {
                        float leftDiff = Math.Abs(collision.Left - boundingBox.Left);
                        float rightDiff = Math.Abs(collision.Right - boundingBox.Right);

                        if(leftDiff <= rightDiff) {
                            // Collision is on our LEFT side
                            float collisionProposedX = proposedPosition.X + collision.Width;

                            proposedPosition = new Vector2(collisionProposedX, proposedPosition.Y);
                        } 
                        else if (rightDiff < leftDiff) {
                            // Collision is on our RIGHT side
                            float collisionProposedX = proposedPosition.X - collision.Width;

                            proposedPosition = new Vector2(collisionProposedX, proposedPosition.Y);
                        }
                    }
                    else {
                        float topDiff = Math.Abs(collision.Top - boundingBox.Top);
                        float bottomDiff = Math.Abs(collision.Bottom - boundingBox.Bottom);

                        if(bottomDiff <= topDiff) {
                            // Collision is BELOW Player
                            float collisionProposedY = proposedPosition.Y - collision.Height;

                            proposedPosition = new Vector2(proposedPosition.X, collisionProposedY);

                            onGround = true;
                        }
                        else if (topDiff < bottomDiff) {
                            // Collision is ABOVE Player
                            float collisionProposedY = proposedPosition.Y + collision.Height;

                            proposedPosition = new Vector2(proposedPosition.X, collisionProposedY);
                        }
                    }
                }

                Velocity = new Vector2(0,0);
                collisions.Clear();
            }

            Position = new Vector2(proposedPosition.X, proposedPosition.Y);

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

    protected void OnMoving(object o, MoveEventArgs args) {
        IsMoving?.Invoke(this, args);
    } 

    public void AddCollision(Rectangle collision) {
        collisions.Add(collision);
    }
}
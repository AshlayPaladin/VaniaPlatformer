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
    public event EventHandler<MoveEventArgs> FinishedMoving;

    // Fields
    private string textureSheetId;
    private Rectangle boundingBox;
    private Rectangle leftBoundingBox;
    private Rectangle rightBoundingBox;
    private Rectangle topBoundingBox;
    private Rectangle bottomBoundingBox;
    private float coyoteTime;
    private float coyoteTimeReset;
    private float baseMoveSpeed;
    private float minimumMoveSpeed;
    private float activeMoveSpeed;
    private float topMoveSpeed;
    private float baseAcceleration;
    private float activeAcceleration;
    private bool isRunning;
    private float jumpSpeed;
    private float maxFallSpeed;
    private float gravity;
    List<Rectangle> collisions;
    bool onGround;

    // Properties 
    private AnimationManager animationManager;
    private Vector2 boundingSize;
    public new Rectangle BoundingBox { get { return boundingBox; } }

    // Constructors
    public Player(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {
        
        collisions = new List<Rectangle>();
        textureSheetId = "textures/AdventurerSheet";
        animationManager = new AnimationManager();
        boundingSize = new Vector2(collisionWidth,collisionHeight);
        baseMoveSpeed = 128.0f;
        minimumMoveSpeed = 16f;
        activeMoveSpeed = 0f;
        topMoveSpeed = baseMoveSpeed;
        baseAcceleration = 8f;
        activeAcceleration = baseAcceleration;
        jumpSpeed = 384.0f;
        onGround = false;
        isRunning = false;

        gravity = 0.35f;
        maxFallSpeed = 128.0f;

        coyoteTimeReset = 0.15f;
        coyoteTime = coyoteTimeReset;

        Colliders = new List<Rectangle>{ boundingBox };
        Position = new Vector2(startX,startY);
        Velocity = new Vector2(0,0);
        
        UpdateBoundingBox();
        Colliders.Add(boundingBox);

        SetOrigin();

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
        //spriteBatch.Draw(Globals.DebugTexture, boundingBox, Color.Yellow * 0.5f);
        //spriteBatch.Draw(Globals.DebugTexture, bottomBoundingBox, Color.Red * 0.5f);
        //spriteBatch.Draw(Globals.DebugTexture, leftBoundingBox, Color.Red * 0.5f);
        //spriteBatch.Draw(Globals.DebugTexture, rightBoundingBox, Color.Red * 0.5f);
        //spriteBatch.Draw(Globals.DebugTexture, topBoundingBox, Color.Red * 0.5f);
    }

    public override void Update() {
        KeyboardState keyboardState = Keyboard.GetState();

        // Always apply Gravity if we are in the air
        if(!onGround) {
            // Add Gravity
            float currentFallSpeed = Velocity.Y;

            if(currentFallSpeed < maxFallSpeed) {
                currentFallSpeed += gravity;

                Velocity = new Vector2(Velocity.X, currentFallSpeed);
            }
        }
        else {
            OnMoving(this, new MoveEventArgs(new Vector2(bottomBoundingBox.X, bottomBoundingBox.Y), Velocity, bottomBoundingBox));

            if(collisions.Count == 0) 
            {
                // Nothing below us, begin Coyote Time countdown before setting OnGround
                if(coyoteTime > 0) {
                    coyoteTime -= Globals.DeltaTime;
                }
                else {
                    onGround = false;
                    coyoteTime = coyoteTimeReset;
                }

                // Begin falling immediately, but still be considered "onGround" so player can still jump
                float currentFallSpeed = Velocity.Y;

                if(currentFallSpeed < maxFallSpeed) {
                    currentFallSpeed += gravity;

                    Velocity = new Vector2(Velocity.X, currentFallSpeed);
                }
            }
            else {
                coyoteTime = coyoteTimeReset;
                collisions.Clear();
            }
        }

        if(keyboardState.IsKeyDown(Keys.LeftShift) && !isRunning) {
            topMoveSpeed *= 2f;
            activeAcceleration *= 1.25f;
            isRunning = true;
        }

        if(keyboardState.IsKeyUp(Keys.LeftShift) && isRunning) {
            topMoveSpeed = baseMoveSpeed;
            activeAcceleration = baseAcceleration;
            isRunning = false;
        }

        // Check for Horizontal Key presses (A & D) and apply Horizontal Velocity, as needed
        if(keyboardState.IsKeyDown(Keys.A)) {
            OnMoving(this, new MoveEventArgs(new Vector2(leftBoundingBox.X, leftBoundingBox.Y), Velocity, leftBoundingBox));

            if(collisions.Count > 0) {
                collisions.Clear();

                Velocity = new Vector2(0, Velocity.Y);
                activeMoveSpeed = 0;
            } 
            else {
                if(activeMoveSpeed < topMoveSpeed) {
                    activeMoveSpeed += activeAcceleration;
                }
                else if (activeMoveSpeed > topMoveSpeed) {
                    activeMoveSpeed = topMoveSpeed;
                }

                if(activeAcceleration < minimumMoveSpeed) {
                    activeAcceleration = minimumMoveSpeed;
                }

                float velocityX = (float)-(Math.Floor((Velocity.X + activeMoveSpeed) * Globals.DeltaTime));

                Velocity = new Vector2(velocityX, Velocity.Y);
            }
        }

        if(keyboardState.IsKeyDown(Keys.D)) {
            // Send out OnMoving event to trigger SolidActors to verify their position in relation to Player
            OnMoving(this, new MoveEventArgs(new Vector2(rightBoundingBox.X, rightBoundingBox.Y), Velocity, rightBoundingBox));

            // If any collisions with our TestRectangle were found, we are against a wall already
            if(collisions.Count > 0) {
                // Clear the collisions, no need to handle them
                // Then set horizontal velocity to 0, we will not move if there is a wall in our way
                collisions.Clear();

                Velocity = new Vector2(0, Velocity.Y);
                activeMoveSpeed = 0;
            } 
            else {
                // If no collisions are detected, we are free to begin moving
                if(activeMoveSpeed < topMoveSpeed) {
                    activeMoveSpeed += activeAcceleration;
                }
                else if (activeMoveSpeed > topMoveSpeed) {
                    activeMoveSpeed = topMoveSpeed;
                }

                if(activeAcceleration < minimumMoveSpeed) {
                    activeAcceleration = minimumMoveSpeed;
                }

                float velocityX = (Velocity.X + activeMoveSpeed) * Globals.DeltaTime;

                Velocity = new Vector2(velocityX, Velocity.Y);
            }
        }

        if(keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D)) {
            Velocity = new Vector2(0, Velocity.Y);
            activeMoveSpeed = 0;
        }

        // Check for Jump Key while On the Ground
        if(keyboardState.IsKeyDown(Keys.W) && onGround) {
            float jumpVelocity = -(jumpSpeed * Globals.DeltaTime);

            Velocity = new Vector2(Velocity.X, jumpVelocity);
            
            onGround = false;
        }

        // End Jump Premature if Jump Key is Released before reaching max jump height
        if(keyboardState.IsKeyUp(Keys.W) && !onGround && Velocity.Y < 0) {
            Velocity = new Vector2(Velocity.X, 0);
        }

        // Stop Animation if there is no horizontal velocity
        if((int)Velocity.X == 0) {
            animationManager.Stop();
        }
        else {
            if(Velocity.X > 0) {
                animationManager.SetSpriteEffects(SpriteEffects.None);
            }
            else{
                animationManager.SetSpriteEffects(SpriteEffects.FlipHorizontally);
            }

            animationManager.Play();
        }

        // Enter into movement method using Position and Velocity set previously
        MoveAndSlide();

        // Fire off FinishedMoving for listeners
        OnFinishedMoving();

        // Update animation based on movement and other conditions
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
                Velocity,
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

                        Velocity = new Vector2(0, Velocity.Y);
                        //activeMoveSpeed = 0;
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

                            onGround = false;
                        }

                        Velocity = new Vector2(Velocity.X, 0);
                    }
                }

                collisions.Clear();
            }

            Position = new Vector2(proposedPosition.X, proposedPosition.Y);

            UpdateBoundingBox();
            Origin = new Vector2(boundingBox.X + (boundingBox.Width / 2), boundingBox.Y + (boundingBox.Height / 2));
        }
    }

    private void UpdateBoundingBox() {
        boundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y, 
            (int)boundingSize.X,
            (int)boundingSize.Y);

        bottomBoundingBox = new Rectangle(
            (int)boundingBox.Left,
            (int)boundingBox.Bottom + 1,
            boundingBox.Width,
            1);

        rightBoundingBox = new Rectangle(
            (int)boundingBox.Right + 1,
            (int)Position.Y,
            1,
            boundingBox.Height);
        
        leftBoundingBox = new Rectangle(
            (int)boundingBox.Left - 1, 
            (int)Position.Y, 
            1, 
            boundingBox.Height);

        topBoundingBox = new Rectangle(
            (int)boundingBox.Left,
            (int)boundingBox.Top - 1,
            (int)boundingBox.Width,
            1
        );
    }

    public void AddCollision(Rectangle collision) {
        collisions.Add(collision);
    }
    
    protected void OnMoving(object o, MoveEventArgs args) {
        IsMoving?.Invoke(this, args);
    } 

    protected void OnFinishedMoving() {
        FinishedMoving?.Invoke(this, new MoveEventArgs(Position, Velocity, BoundingBox));
    }
}
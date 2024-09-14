using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class PlayerEntity : Entity {
    
    // Constants

    // Events
    public event EventHandler HeadBonked;

    // Fields
    private string textureSheetId;

    // Properties

    // Constructors
    public PlayerEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) {

        textureSheetId = "textures/AdventurerSheet";

        AddComponent(
            new TransformComponent(
                new Vector2(startX, startY),
                new Vector2(1, 1)
            )
        );

        AddComponent(
            new SpriteComponent(
                Globals.Content.Load<Texture2D>(textureSheetId)
            )
        );

        AddComponent(
            new AnimationComponent(
                GetComponent<SpriteComponent>(),
                new Rectangle(50, 38, 50, 37),
                6, 0.05f, AnimationComponent.Loop.Reverse
            )
        );

        AddComponent(
            new ColliderComponent(
                new Rectangle(startX, startY, collisionWidth, collisionHeight)
            )
        );

        AddComponent(
            new MoveComponent()
        );

        AddComponent(
            new InputComponent()
        );

        // Connect our ColliderComponent Collided event to the MoveComponent collision correction method
        GetComponent<ColliderComponent>().Collided += GetComponent<MoveComponent>().OnCollision;
    }

    // Methods
    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Yellow * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().BottomCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().LeftCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().RightCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().TopCollider, Color.Red * 0.5f);
    }

    public void Update() {
        MoveComponent movement = GetComponent<MoveComponent>();
        ColliderComponent collider = GetComponent<ColliderComponent>();
        InputComponent input = GetComponent<InputComponent>();

        // Check for Input
        if(input.IsRunKeyDown && !movement.IsRunning) {
            movement.MaxMoveSpeed *= 2f;
            movement.CurrentAcceleration *= 1.25f;
            movement.IsRunning = true;
        }

        if(!input.IsRunKeyDown && movement.IsRunning) {
            movement.MaxMoveSpeed = movement.BaseMoveSpeed;
            movement.CurrentAcceleration = movement.BaseAcceleration;
            movement.IsRunning = false;
        }

        // Check for Horizontal Key presses (A & D) and apply Horizontal Velocity, as needed
        if(input.IsLeftKeyDown) {
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.LeftCollider))
            {
                movement.Velocity = new Vector2(0, movement.Velocity.Y);
                movement.CurrentMoveSpeed = 0;
            }
            else 
            {
                if(movement.CurrentMoveSpeed < movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed += movement.CurrentAcceleration;
                }
                else if (movement.CurrentMoveSpeed > movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed = movement.MaxMoveSpeed;
                }

                if(movement.CurrentAcceleration < movement.MinimumMoveSpeed) {
                    movement.CurrentAcceleration = movement.MinimumMoveSpeed;
                }

                float velocityX = (float)-(Math.Floor((movement.Velocity.X + movement.CurrentMoveSpeed) * Globals.DeltaTime));

                movement.Velocity = new Vector2(velocityX, movement.Velocity.Y);
            }
        }

        if(input.IsRightKeyDown) {

            // If the ColliderSystem returns true, we are against a wall on our right already
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.RightCollider)) {
                // Set horizontal velocity to 0, we will not move if there is a wall in our way
                movement.Velocity = new Vector2(0, movement.Velocity.Y);
                movement.CurrentMoveSpeed = 0;
            } 
            else {
                // If no collisions are detected, we are free to begin moving
                if(movement.CurrentMoveSpeed < movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed += movement.CurrentAcceleration;
                }
                else if (movement.CurrentMoveSpeed > movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed = movement.MaxMoveSpeed;
                }

                if(movement.CurrentAcceleration < movement.MinimumMoveSpeed) {
                    movement.CurrentAcceleration = movement.MinimumMoveSpeed;
                }

                float velocityX = (movement.Velocity.X + movement.CurrentMoveSpeed) * Globals.DeltaTime;

                movement.Velocity = new Vector2(velocityX, movement.Velocity.Y);
            }
        }

        if(!input.IsLeftKeyDown && !input.IsRightKeyDown) {
            movement.Velocity = new Vector2(0, movement.Velocity.Y);
            movement.CurrentMoveSpeed = 0;
        }

        // Check for Jump Key while On the Ground
        if(input.IsJumpKeyDown && movement.OnGround) {
            movement.Jump();
        }

        // End Jump Premature if Jump Key is Released before reaching max jump height
        if(!input.IsJumpKeyDown && !movement.OnGround && movement.Velocity.Y < 0) {
            movement.Velocity = new Vector2(movement.Velocity.X, 0);
        }

        // TODO: Adjust SpriteEffects and AnimationComponent based on Movement

        // Update the AnimationComponent's DestinationRectangle
        GetComponent<AnimationComponent>().DestinationRectangle = GetComponent<ColliderComponent>().Collider;
    }

    protected void OnHeadBonked() {
        HeadBonked?.Invoke(this, null);
    }
}
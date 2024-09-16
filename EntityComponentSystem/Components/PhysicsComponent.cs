using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class PhysicsComponent : Component
{
    // Fields
    private float coyoteTimer;
    private float bounceTimer = 0f;

    // Properties
    public float CoyoteTime = 0.15f;
    public float MaxFallSpeed = 128.0f;
    public float EffectiveGravity = 0.35f;
    public bool OnGround = false;
    public bool IsFlying = false;
    public bool IsBouncing = false;

    // Constructor
    public PhysicsComponent()
    {
        this.coyoteTimer = CoyoteTime;

        PhysicsSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        ColliderComponent collider = Entity.GetComponent<ColliderComponent>();
        MoveComponent movement = Entity.GetComponent<MoveComponent>();

        if(!IsFlying)
        {
            if(!OnGround)
            {
                float currentFallSpeed = movement.Velocity.Y;

                if(currentFallSpeed < MaxFallSpeed)
                {
                    currentFallSpeed += EffectiveGravity;

                    movement.Velocity = new Vector2(movement.Velocity.X, currentFallSpeed);
                }

                if(bounceTimer > 0)
                {
                    bounceTimer -= Globals.DeltaTime;
                }
                else if(bounceTimer <= 0 && IsBouncing == true)
                {
                    bounceTimer = 0;
                    IsBouncing = false;
                }
            }
            else
            {
                if(collider != null)
                {
                    if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.BottomCollider) == null)
                    {
                        // We're set as OnGround, but nothing is under us, Begin CoyoteTime!
                        // Nothing below us, begin Coyote Time countdown before setting OnGround
                        if(coyoteTimer > 0) {
                            coyoteTimer -= Globals.DeltaTime;
                        }
                        else {
                            OnGround = false;
                            coyoteTimer = CoyoteTime;
                        }

                        // Begin falling immediately, but still be considered "onGround" so player can still jump
                        float currentFallSpeed = movement.Velocity.Y;

                        if(currentFallSpeed < MaxFallSpeed) {
                            currentFallSpeed += EffectiveGravity;

                            movement.Velocity = new Vector2(movement.Velocity.X, currentFallSpeed);
                        }
                    }
                    else {
                        coyoteTimer = CoyoteTime;
                    }
                }
            }
        }
    }

        public void Jump() 
    {
        float jumpVelocity = -(Entity.GetComponent<MoveComponent>().JumpSpeed * Globals.DeltaTime);

        Entity.GetComponent<MoveComponent>().Velocity = new Vector2(Entity.GetComponent<MoveComponent>().Velocity.X, jumpVelocity);
        
        OnGround = false;
    }

    public void Bounce()
    {
        IsBouncing = true;
        bounceTimer = 1f;

        Jump();
    }
}
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class RigidBodyComponent : Component
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
    public RigidBodyComponent()
    {
        this.coyoteTimer = CoyoteTime;

        PhysicsSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        ColliderComponent collider = Entity.GetComponent<ColliderComponent>();
        MovementComponent movement = Entity.GetComponent<MovementComponent>();

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
        float jumpVelocity = -(Entity.GetComponent<MovementComponent>().JumpSpeed * Globals.DeltaTime);

        Entity.GetComponent<MovementComponent>().Velocity = new Vector2(Entity.GetComponent<MovementComponent>().Velocity.X, jumpVelocity);
        
        OnGround = false;
    }

    public void Bounce()
    {
        IsBouncing = true;
        bounceTimer = 1f;

        Jump();
    }
}
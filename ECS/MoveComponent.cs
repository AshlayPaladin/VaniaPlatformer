using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class MoveComponent : Component 
{
    // Fields
    private float coyoteTimer;
    private float currentMoveSpeed;
    private float currentAcceleration;

    // Properties
    public Vector2 Velocity = Vector2.Zero;
    public float BaseMoveSpeed = 128.0f;
    public float MinimumMoveSpeed = 16f;
    public float MaxMoveSpeed = 128.0f;
    public float CoyoteTime = 0.15f;
    public float BaseAcceleration = 8f;
    public float JumpSpeed = 384.0f;
    public float MaxFallSpeed = 128.0f;
    public float EffectiveGravity = 0.35f;
    public bool IsRunning = false;
    public bool OnGround = false;

    // Constructor
    public MoveComponent()
    {
        this.coyoteTimer = CoyoteTime;
        this.currentAcceleration = 0f;
        this.currentMoveSpeed = 0f;
    }

    // Methods
    public override void Update()
    {
        // Add Gravity to Falling, if we are in the air
        if(!OnGround)
        {
            float currentFallSpeed = Velocity.Y;

            if(currentFallSpeed < MaxFallSpeed)
            {
                currentFallSpeed += EffectiveGravity;

                Velocity = new Vector2(Velocity.X, currentFallSpeed);
            }
        }
        else
        {
            
        }
    }

    public void Stop() 
    {
        currentMoveSpeed = 0f;
        currentAcceleration = 0f;
    }

    public void Accelerate() 
    {
        if(currentMoveSpeed < MaxMoveSpeed) 
        {
            currentMoveSpeed += currentAcceleration;
        }
    }

    public void Decelerate() 
    {
        if(currentMoveSpeed > 0) 
        {
            currentMoveSpeed /= 1.15f;
        }
    }

    public void Jump() 
    {
        
    }
}
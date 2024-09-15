using System;

namespace VaniaPlatformer.ECS;

public class HealthComponent : Component
{
    // Events
    public event EventHandler Damaged;
    public event EventHandler Killed;

    // Fields
    private float invulnTimer = 0f;

    // Properties
    public int Health { get; private set; }
    public bool Invulnerable { get; private set; }
    public float RecoveryTime { get; private set; }

    // Constructor
    public HealthComponent(int health = 1, bool invulnerable = false, float recoveryTime = 3f)
    {
        Health = health;
        Invulnerable = false;
        RecoveryTime = recoveryTime;
    }

    // Methods
    public override void Update()
    {
        if(invulnTimer > 0)
        {
            invulnTimer -= Globals.DeltaTime;
        }
        else
        {
            Invulnerable = false;
        }
    }

    public void Damage(int damage = 1)
    {
        if(!Invulnerable)
        {
            Health -= damage;

            Invulnerable = true;
            invulnTimer = RecoveryTime;

            if(Health <= 0)
            {
                Killed?.Invoke(this, null);
            }
            else
            {
                Damaged?.Invoke(this, null);
            }
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer;
using VaniaPlatformer.ECS;

public class EnemyEntity : GameActorEntity
{
    // Events
    public event EventHandler Killed;

    // Construction
    public EnemyEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0, string textureAssetId = "") 
        : base(collisionWidth, collisionWidth, startX, startY, textureAssetId)
    {
        AddComponent(
            new HealthComponent(1)
        );

        GetComponent<HealthComponent>().Killed += OnKilled;
        GetComponent<ColliderComponent>().Collided += GetComponent<MoveComponent>().OnCollision;
        GetComponent<ColliderComponent>().Collided += OnCollision;
    }

    // Methods
    public void Draw(SpriteBatch spriteBatch)
    {
        if(GetComponent<HealthComponent>().Health > 0)
        {
            spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Red * 0.5f);
        }
    }

    public void OnKilled(object sender, EventArgs args)
    {
        Destroy();
        Killed?.Invoke(this, null);
    }
}
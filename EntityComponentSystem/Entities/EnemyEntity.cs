using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer;
using VaniaPlatformer.ECS;

public class EnemyEntity : GameActorEntity
{
    // Construction
    public EnemyEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0, string textureAssetId = "") 
        : base(collisionWidth, collisionWidth, startX, startY, textureAssetId)
    {
        GetComponent<ColliderComponent>().Collided += GetComponent<MoveComponent>().OnCollision;
        GetComponent<ColliderComponent>().Collided += OnCollision;
    }

    // Methods
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Red * 0.5f);
    }
}
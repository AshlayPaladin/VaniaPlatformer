using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class SpriteComponent : Component {

    // Properties
    public Texture2D Texture;
    public Vector2 Origin = Vector2.Zero;
    public Rectangle SourceRectangle = Rectangle.Empty;
    public SpriteEffects SpriteEffect = SpriteEffects.None;

    // Constructor
    public SpriteComponent(Texture2D texture)
    {
        this.Texture = texture;
        this.Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

        DrawSystem.Register(this);
    }

    // Methods
    public virtual void Draw(SpriteBatch spriteBatch) 
    {
        AnimationComponent a = Entity.GetComponent<AnimationComponent>();
        TransformComponent t = Entity.GetComponent<TransformComponent>();

        if(a != null) 
        {
            if(Texture != null) 
            {
                Vector2 offset = new Vector2(
                Math.Abs(a.DestinationRectangle.Width - SourceRectangle.Width) / 2,
                Math.Abs(a.DestinationRectangle.Height - SourceRectangle.Height) / 2
                );

                Rectangle destRect = new(
                    (int)(a.DestinationRectangle.X + Origin.X - offset.X),
                    (int)(a.DestinationRectangle.Y + Origin.Y + offset.Y),
                    a.DestinationRectangle.Width,
                    a.DestinationRectangle.Height);

                spriteBatch.Draw(Texture, destRect, SourceRectangle, Color.White, t.Rotation, Origin, SpriteEffect, t.LayerDepth);
            }
        }
        else 
        {
            spriteBatch.Draw(Texture, t.Position + Origin, Color.White);
        }
    }
}
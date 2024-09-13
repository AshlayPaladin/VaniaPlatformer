using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class Sprite {
    // Properties
    public Texture2D Spritesheet { get; protected set; }
    public Rectangle SourceRect { get; protected set; }
    public Vector2 Origin { get; protected set; }
    public float Rotation { get; protected set; }

    // Constructor
    public Sprite(Texture2D spriteSheet, Rectangle sourceRect) 
    {
        this.Spritesheet = spriteSheet;
        this.SourceRect = sourceRect;
        this.Origin = CalculateOrigin();
        this.Rotation = 0f;
    }

    // Methods
    public void Defaults() {
        Rotation = 0f;
    }

    public void Rotate(float angle) {
        this.Rotation = MathHelper.ToRadians(angle);
    }

    public Vector2 CalculateOrigin() {
        return new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, SpriteEffects spriteEffects = SpriteEffects.None) 
    { 
        Draw(spriteBatch, destinationRectangle, Color.White, spriteEffects);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, SpriteEffects spriteEffects = SpriteEffects.None) 
    {
        Vector2 offset = new Vector2(
            Math.Abs(destinationRectangle.Width - SourceRect.Width) / 2,
            Math.Abs(destinationRectangle.Height - SourceRect.Height) / 2
        );

        Rectangle destRect = new(
            (int)(destinationRectangle.X + Origin.X - offset.X), 
            (int)(destinationRectangle.Y + Origin.Y + offset.Y),
            destinationRectangle.Width,
            destinationRectangle.Height);

        spriteBatch.Draw(Spritesheet, destRect, SourceRect, color, Rotation, Origin, spriteEffects, 0);
    }
}
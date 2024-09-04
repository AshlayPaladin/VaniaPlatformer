using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public abstract class Actor {

    // Events
    public event EventHandler HasCollided;
    
    // Properties
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public Rectangle BoundingBox { get; protected set; }

    // Methods
    public abstract void Update(GameTime gameTime);
    public abstract void MoveAndSlide();
    public virtual void OnCollision(EventArgs args) {
        HasCollided?.Invoke(this, args);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public abstract class Actor {

    // Events
    public event EventHandler HasCollided;
    
    // Properties
    public Vector2 Position { get; protected set; }
    public Rectangle BoundingBox { get; protected set; }
    public Vector2 Origin { get; protected set; }
    protected Vector2 Velocity { get; set; }


    // Containers
    protected List<Rectangle> Colliders { get; set; }

    // Methods
    public abstract void Update();
    public abstract void MoveAndSlide();
    public virtual void SetOrigin() {
        Origin = new Vector2(BoundingBox.Width / 2, BoundingBox.Height / 2);
    }
    public virtual void OnCollision(EventArgs args) {
        HasCollided?.Invoke(this, args);
    }
}
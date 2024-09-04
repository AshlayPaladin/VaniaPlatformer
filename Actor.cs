using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public abstract class Actor {

    // Events
    public event EventHandler HasCollided;
    
    // Properties
    public Vector2 Position { get; protected set; }
    protected Vector2 Velocity { get; set; }

    // Containers
    protected List<Rectangle> Colliders { get; set; }

    // Methods
    public abstract void Update();
    public abstract void MoveAndSlide();
    public virtual void OnCollision(EventArgs args) {
        HasCollided?.Invoke(this, args);
    }
}
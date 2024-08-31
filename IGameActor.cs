using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public interface IGameActor {
    // Events
    event EventHandler HasCollided;

    // Properties
    Vector2 Position { get;}
    Vector2 Velocity { get; }
    Rectangle BoundingBox { get; }

    // Methods
    void Update(GameTime gameTime);
    void MoveAndSlide();
}
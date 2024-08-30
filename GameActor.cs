using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public abstract class GameActor : IGameActor {
    
    // Properties
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public Rectangle CollisionRectangle { get; private set; }

    // Methods
    public abstract void Update(GameTime gameTime);
    public abstract void MoveAndSlide();

}
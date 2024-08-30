using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public interface IGameActor {
    Vector2 Position;
    Vector2 Velocity;
    Rectangle CollisionRectangle;

    void Update(GameTime gameTime);
    void MoveAndSlide();
}
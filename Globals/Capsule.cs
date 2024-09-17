using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public class Capsule
{
    // Properties
    public Circle TopCircle;
    public Rectangle Body;
    public Circle BottomCircle;

    // Constructor
    public Capsule(Vector2 position, int width, int height)
    {
        float radius = width / 2f;  // Radius is always half of width. Capsules are exclusively rounded on top and bottom, never on left-right sides
        
        // Height of the Body rectangle
        float bodyHeight = height - 2 * radius;

        // Central Rectangle Body of Capsule
        Rectangle body = new Rectangle(
            (int)(position.X - radius), // X Position (adjusted to center the body)
            (int)(position.Y + radius), // Y Position (starts just below the TopCircle)
            width,
            (int)bodyHeight
        );

        // Top and Bottom Circles of Capsule
        Circle topCircle = new Circle
        (
            new Vector2(position.X, position.Y + radius),   // Circle center just above the body
            radius
        );

        Circle bottomCircle = new Circle
        (
            new Vector2(position.X, position.Y + radius + bodyHeight),  // Circle center just below the body
            radius
        );
    }
}
using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public class Circle
{
    // Properties
    public Vector2 Center;
    public float Radius;

    // Constructor
    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    // Methods
    public bool Intersects(Rectangle rectangle)
    {
        float closestX = Math.Clamp(Center.X, rectangle.Left, rectangle.Right);
        float closestY = Math.Clamp(Center.Y, rectangle.Top, rectangle.Bottom);

        float distanceX = Center.X - closestX;
        float distanceY = Center.Y - closestY;

        return (distanceX * distanceX + distanceY * distanceY) < (Radius * Radius);
    }

    public bool Intersects(Circle other)
    {
        float distanceX = Center.X - other.Center.X;
        float distanceY = Center.Y - other.Center.Y;
        float distanceSquared = distanceX * distanceX + distanceY * distanceY;
        float radiusSum = Radius + other.Radius;

        return distanceSquared < radiusSum * radiusSum;
    }
}
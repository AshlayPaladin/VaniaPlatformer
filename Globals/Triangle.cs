using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public class Triangle
{
    // Properties
    public Vector2[] Vertices;
    public Vector2[] WorldVertices;

    // Constructor
    public Triangle(Vector2 position, Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        Vertices = [ pointA, pointB, pointC ];
        WorldVertices = [ position + pointA, position + pointB, position + pointC ];
    }
}
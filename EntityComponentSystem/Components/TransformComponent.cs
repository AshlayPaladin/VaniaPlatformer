using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class TransformComponent : Component {
    
    // Properties
    public Vector2 Position = Vector2.Zero;
    public Vector2 Scale = Vector2.Zero;
    public float LayerDepth = 0;
    public float Rotation = 0;
    
    // Constructor
    public TransformComponent(Vector2 position, Vector2 scale, float layerDepth = 0, float rotation = 0) 
    {
        Position = position;
        Scale = scale;
        LayerDepth = layerDepth;
        Rotation = rotation;
        
        TransformSystem.Register(this);
    }
}
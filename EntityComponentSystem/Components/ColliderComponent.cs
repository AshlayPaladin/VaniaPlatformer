using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class ColliderComponent : Component {

    // Events
    public event EventHandler<CollisionEventArgs> Collided;

    // Fields
    private Vector2 colliderPosition;
    private Vector2 colliderSize;

    // Properties
    public Rectangle Collider;
    public Rectangle BottomCollider { get; private set; }
    public Rectangle TopCollider { get; private set; }
    public Rectangle LeftCollider { get; private set; }
    public Rectangle RightCollider { get; private set; }

    // Constructor
    public ColliderComponent(Rectangle collider) 
    {
        Collider = collider;
        OnColliderChanged();

        ColliderSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        if(Collider.X != colliderPosition.X || 
            Collider.Y != colliderPosition.Y ||
            Collider.Width != colliderSize.X ||
            Collider.Height != colliderSize.Y)
        {
            OnColliderChanged();
        }
    }

    public void Intersects(ColliderComponent component)
    {
        Rectangle collision = Rectangle.Intersect(Collider, component.Collider);

        if(collision != Rectangle.Empty) 
        {
            this.OnCollision(component, collision);
        }
    }

    public void OnCollision(ColliderComponent component, Rectangle collision) 
    {
        component.Collided?.Invoke(this, new CollisionEventArgs(this, collision));
        Collided?.Invoke(this, new CollisionEventArgs(component, collision));
    }

    public void OnColliderChanged() 
    {
        colliderPosition = new Vector2(Collider.X, Collider.Y);
        colliderSize = new Vector2(Collider.Width, Collider.Height);
        SetSurroundingColliders();
    }

    private void SetSurroundingColliders() {

        BottomCollider = new Rectangle(
            (int)Collider.Left,
            (int)Collider.Bottom,
            Collider.Width,
            1);

        TopCollider = new Rectangle(
            (int)Collider.Left,
            (int)Collider.Top - 1,
            Collider.Width,
            1);
        
        LeftCollider = new Rectangle(
            (int)Collider.Left - 1, 
            (int)Collider.Top + 8, 
            1, 
            Collider.Height - 16);

        RightCollider = new Rectangle(
            (int)Collider.Right,
            (int)Collider.Top + 8,
            1,
            Collider.Height - 16
        );
    }
}
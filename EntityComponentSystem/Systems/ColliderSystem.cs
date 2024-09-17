using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public struct CollisionResponse
{
    // Properties
    public Entity OtherEntity;
    public Vector2 Correction;
    public bool StopX;
    public bool StopY;
}

public class ColliderSystem : BaseSystem<ColliderComponent> 
{
    // Methods
    public static new void Update() 
    {
        // Update all Colliders based on their potential new positions
        foreach(ColliderComponent collider in components) 
        {
            Vector2 position = collider.Entity.GetComponent<TransformComponent>().Position;

            switch(collider.Shape)
            {
                case ColliderComponent.ColliderShape.Box:
                {
                    collider.BoxCollider.X = (int)position.X;
                    collider.BoxCollider.Y = (int)position.Y;
                    break;
                }
                case ColliderComponent.ColliderShape.Circle:
                {
                    collider.CircleCollider.Center = new Vector2((int)position.X, (int)position.Y);
                    break;
                }
                case ColliderComponent.ColliderShape.Triangle:
                {
                    for(int i = 0; i < collider.TriangleCollider.WorldVertices.Length - 1; i++)
                    {
                        collider.TriangleCollider.WorldVertices[i] = 
                            new Vector2(
                                (int)(position.X + collider.TriangleCollider.Vertices[i].X),
                                (int)(position.Y + collider.TriangleCollider.Vertices[i].Y)
                            );
                    }
                    break;
                }
            }
        }

        HandleCollisions();
    }

    public static void HandleCollisions() 
    {
        // Check for any collisions between all ColliderComponents in the system
        for(int i = 0; i < components.Count - 1; i++) {
            for(int j = components.Count - 1; j >= 0; j--) {

                // No need to check colliders before our own index, as that would be iterating items we've already compared
                if(j >= i)
                {
                    // Create some useful in-scope fields
                    ColliderComponent collider = components[i];
                    ColliderComponent otherCollider = components[j];
                    CollisionResponse collisionResponse = CalculateCollisionResponse(collider, otherCollider);
                    collisionResponse.OtherEntity = components[j].Entity;

                    // If our CollisionResponse shows an adjustment is needed AND our Entity is capable of Movement, we will correct our position
                    if(collisionResponse.Correction != Vector2.Zero &&
                        collider.Entity.GetComponent<MovementComponent>() != null) 
                    {
                        // Our Collider is attached to an Entity that moves, so we should correct that Entity's Position
                        TransformComponent transform = collider.Entity.GetComponent<TransformComponent>();

                        if(transform != null)
                        {
                            // Make sure our Transform isn't null, just in case
                            transform.Position += collisionResponse.Correction;

                            if(collisionResponse.StopX) { collider.Entity.GetComponent<MovementComponent>().Velocity.X = 0; }
                            if(collisionResponse.StopY) { collider.Entity.GetComponent<MovementComponent>().Velocity.Y = 0; }
                        }
                    }
                }
            }
        }
    }

    public static CollisionResponse CheckForEntityCollisions<T>() where T : Entity
    {
        // Check for any collisions between all ColliderComponents in the system
        for(int i = 0; i < components.Count - 1; i++) {
            for(int j = components.Count - 1; j >= 0; j--) {

                // No need to check colliders before our own index, as that would be iterating items we've already compared
                if(j >= i)
                {
                    // Create some useful in-scope fields
                    ColliderComponent collider = components[i];
                    ColliderComponent otherCollider = components[j];
                    CollisionResponse collisionResponse = CalculateCollisionResponse(collider, otherCollider);

                    if(collisionResponse.Correction != Vector2.Zero)
                    {
                        collisionResponse.OtherEntity = components[j].Entity;
                        return collisionResponse;
                    }
                }
            }
        }

        return null;
    }

    public static ColliderComponent.CollisionSide DetermineCollisionSide(ColliderComponent colliderA, ColliderComponent colliderB)
    {
        if(colliderA.BoxCollider != Rectangle.Empty && colliderB.BoxCollider != Rectangle.Empty)
        {
            Rectangle bodyA = colliderA.BoxCollider;
            Rectangle bodyB = colliderB.BoxCollider;

            // Calculate which side of colliderA is colliding with colliderB
            float dx = (bodyA.Left + bodyA.Width / 2) - (bodyB.Left + bodyB.Width / 2);
            float dy = (bodyA.Top + bodyA.Height / 2) - (bodyB.Top + bodyB.Height / 2);
            float width = (bodyA.Width + bodyB.Width) / 2;
            float height = (bodyA.Height + bodyB.Height) / 2;

            if (Math.Abs(dx) <= width && Math.Abs(dy) <= height)
            {
                float wy = width * dy;
                float hx = height * dx;

                if (wy > hx)
                {
                    return (wy > -hx) ? ColliderComponent.CollisionSide.Bottom : ColliderComponent.CollisionSide.Left;
                }
                else
                {
                    return (wy > -hx) ? ColliderComponent.CollisionSide.Right : ColliderComponent.CollisionSide.Top;
                }
            }
        }

        return ColliderComponent.CollisionSide.None;
    }

    private static CollisionResponse CalculateCollisionResponse(ColliderComponent component, ColliderComponent otherComponent)
    {
        Vector2 correction = Vector2.Zero;

        switch(component.Shape)
        {
            case ColliderComponent.ColliderShape.Triangle:
            {
                switch(otherComponent.Shape)
                {
                    case ColliderComponent.ColliderShape.Box:
                    {
                        correction = ResolveBoxTriangleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Circle:
                    {
                        correction = ResolveCircleTriangleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Triangle:
                    {
                        correction = ResolveTriangleTriangleCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Capsule:
                    {
                        correction = ResolveTriangleCapsuleCollision(component, otherComponent);
                        break;
                    }
                }

                break;
            }
            case ColliderComponent.ColliderShape.Capsule:
            {
                switch(otherComponent.Shape)
                {
                    case ColliderComponent.ColliderShape.Box:
                    {
                        correction = ResolveBoxCapsuleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Circle:
                    {
                        correction = ResolveCircleCapsuleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Triangle:
                    {
                        correction = ResolveTriangleCapsuleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Capsule:
                    {
                        correction = ResolveCapsuleCapsuleCollision(component, otherComponent);
                        break;
                    }
                }

                break;
            }
            case ColliderComponent.ColliderShape.Circle:
            {
                switch(otherComponent.Shape)
                {
                    case ColliderComponent.ColliderShape.Box:
                    {
                        correction = ResolveBoxCircleCollision(otherComponent, component);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Circle:
                    {
                        correction = ResolveCircleCircleCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Triangle:
                    {
                        correction = ResolveCircleTriangleCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Capsule:
                    {
                        correction = ResolveCircleCapsuleCollision(component, otherComponent);
                        break;
                    }
                }

                break;
            }
            default:    // Box Collisions are the Default
            {
                switch(otherComponent.Shape)
                {
                    case ColliderComponent.ColliderShape.Box:
                    {
                        correction = ResolveBoxBoxCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Circle:
                    {
                        correction = ResolveBoxCircleCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Triangle:
                    {
                        correction = ResolveBoxTriangleCollision(component, otherComponent);
                        break;
                    }
                    case ColliderComponent.ColliderShape.Capsule:
                    {
                        correction = ResolveBoxCapsuleCollision(component, otherComponent);
                        break;
                    }
                }

                break;
            }
        }

        // PLACEHOLDER RESPONSE
        return new CollisionResponse
        {
            Correction = correction,
            StopX = correction.X == 0 ? true : false,
            StopY = correction.Y == 0 ? true : false
        };
    }

    private static Vector2 ResolveBoxBoxCollision(ColliderComponent collider, ColliderComponent otherCollider)
    {
        Rectangle boxA = collider.BoxCollider;
        Rectangle boxB = otherCollider.BoxCollider;

        // Calculate how much overlap there is on both axes
        float overlapX = Math.Min(
            (boxA.X + boxA.Size.X) - boxB.X,  // boxA right to boxB left
            (boxB.X + boxB.Size.X) - boxA.X   // boxB right to boxA left
        );

        float overlapY = Math.Min(
            (boxA.Y + boxA.Size.Y) - boxB.Y,  // boxA bottom to boxB top
            (boxB.Y + boxB.Size.Y) - boxA.Y   // boxB bottom to boxA top
        );

        // Resolve the collision by moving the box along the axis of least penetration
        if (overlapX < overlapY)
        {
            // Move on the X axis
            if (boxA.X < boxB.X)
            {
                // Move boxA left
                return new Vector2(-overlapX, 0);
            }
            else
            {
                // Move boxA right
                return new Vector2(overlapX, 0);
            }
        }
        else
        {
            // Move on the Y axis
            if (boxA.Y < boxB.Y)
            {
                // Move boxA up
                return new Vector2(0, -overlapY);
            }
            else
            {
                // Move boxA down
                return new Vector2(0, overlapY);
            }
        }
    }

    private static Vector2 ResolveBoxCircleCollision(ColliderComponent boxCollider, ColliderComponent circleCollider)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveBoxTriangleCollision(ColliderComponent boxCollider, ColliderComponent triangleCollider)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveBoxCapsuleCollision(ColliderComponent boxCollider, ColliderComponent capsuleCollider)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveCircleCircleCollision(ColliderComponent circleColliderA, ColliderComponent circleColliderB)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveCircleTriangleCollision(ColliderComponent circleCollider, ColliderComponent triangleCollider)
    {
        Circle circle = circleCollider.CircleCollider;
        Triangle triangle = triangleCollider.TriangleCollider;

        // Find the closest point on the triangle to the circle's center
        Vector2 closestPoint = FindClosestPointOnTriangle(circle.Center, triangle);

        // Calculate the distance between the circle's center and the closest point
        float distance = Vector2.Distance(circle.Center, closestPoint);

        // Check if the circle is intersecting with the triangle (penetrating the slope)
        if (distance < circle.Radius)
        {
            // Penetration depth is how far the circle is overlapping with the slope
            float penetrationDepth = circle.Radius - distance;

            // Find the normal of the slope (the direction we need to push the circle out)
            Vector2 slopeNormal = Vector2.Normalize(circle.Center - closestPoint);

            // Move the circle out of the slope along the normal by the penetration depth
            return slopeNormal * penetrationDepth;
        }

        // If no collision, return (0, 0) meaning no adjustment needed
        return Vector2.Zero;
    }

    private static Vector2 ResolveCircleCapsuleCollision(ColliderComponent circleCollider, ColliderComponent capsuleCollider)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveTriangleTriangleCollision(ColliderComponent triangleColliderA, ColliderComponent triangleColliderB)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveTriangleCapsuleCollision(ColliderComponent triangleCollider, ColliderComponent capsuleCollider)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    private static Vector2 ResolveCapsuleCapsuleCollision(ColliderComponent capsuleColliderA, ColliderComponent capsuleColliderB)
    {

        return Vector2.Zero;    // PLACEHOLDER RETURN
    }

    // Helper function to find the closest point on the triangle to the circle's center
    private static Vector2 FindClosestPointOnTriangle(Vector2 point, Triangle triangle)
    {
        // Get the closest point on each of the triangle's edges
        Vector2 closestOnEdge1 = GetClosestPointOnLineSegment(point, triangle.WorldVertices[0], triangle.WorldVertices[1]);
        Vector2 closestOnEdge2 = GetClosestPointOnLineSegment(point, triangle.WorldVertices[1], triangle.WorldVertices[2]);
        Vector2 closestOnEdge3 = GetClosestPointOnLineSegment(point, triangle.WorldVertices[2], triangle.WorldVertices[0]);

        // Determine which point is the closest to the circle's center
        Vector2 closestPoint = closestOnEdge1;
        float minDistance = Vector2.Distance(point, closestOnEdge1);

        float distance2 = Vector2.Distance(point, closestOnEdge2);
        if (distance2 < minDistance)
        {
            closestPoint = closestOnEdge2;
            minDistance = distance2;
        }

        float distance3 = Vector2.Distance(point, closestOnEdge3);
        if (distance3 < minDistance)
        {
            closestPoint = closestOnEdge3;
        }

        return closestPoint;
    }

    // Helper function to get the closest point on a line segment to a given point
    private static Vector2 GetClosestPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        // Vector from start to end of the line segment
        Vector2 line = lineEnd - lineStart;

        // Vector from start of the line segment to the point
        Vector2 pointToStart = point - lineStart;

        // Project the point onto the line, clamping to the segment length
        float t = Vector2.Dot(pointToStart, line) / Vector2.Dot(line, line);
        t = Math.Clamp(t, 0, 1);  // Clamp between 0 and 1 to keep the point on the segment

        // Return the closest point on the segment
        return lineStart + t * line;
    }

    // Function to calculate how far the entity must be moved upwards to be out of the slope
    public static int CalculateVerticalAdjustmentForSlope(Vector2 boxCorner, Vector2 slopeStart, Vector2 slopeEnd)
    {
        // Extract the x and y coordinates
        float boxX = boxCorner.X;
        float boxY = boxCorner.Y;

        // Calculate the slope's Y position at the X of the box corner
        float slopeY = GetYOnSlopeAtX(slopeStart, slopeEnd, boxX);

        // If the box is below the slope, calculate how much it needs to move up
        if (boxY > slopeY)
        {
            // The difference in Y values tells us how much the box is "penetrating" the slope
            return (int)(boxY - slopeY);
        }
        else
        {
            // No adjustment needed, already above or on the slope
            return 0;
        }
    }

    // Function to calculate Y on a slope given an X value
    private static float GetYOnSlopeAtX(Vector2 slopeStart, Vector2 slopeEnd, float x)
    {
        // The slope formula: (y2 - y1) / (x2 - x1) gives us the slope
        float slope = (slopeEnd.Y - slopeStart.Y) / (slopeEnd.X - slopeStart.X);

        // y = slope * (x - x1) + y1
        float yAtX = slope * (x - slopeStart.X) + slopeStart.Y;

        return yAtX;
    }
}
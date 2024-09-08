using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer;

public class Camera2D {

    // Fields
    private Actor targetActor;
    private Rectangle target;
    private Vector2 targetOrigin;
    private Vector2 tiledMapSize;
    private Vector2 position;
    private Vector2 origin;
    private int cameraMargin;
    private Vector2 viewportSize;
    private float moveSpeed;
    private Vector2 cameraVelocity;
    private Matrix translationMatrix;
    
    private Rectangle verticalStillcamZone;
    private Rectangle horizontalStillcamZone;

    // Properties
    public Rectangle Target { get { return target; } protected set { target = value; } }
    public Vector2 TiledMapSize { get { return tiledMapSize;} protected set { tiledMapSize = value;} }
    public Vector2 Position { get { return position; } protected set { position = value; } }
    public Vector2 Origin { get { return origin;} protected set { origin = value; } }
    public Vector2 ViewportSize { get { return viewportSize; } protected set { viewportSize = value; } }
    public float MoveSpeed { get { return moveSpeed; } protected set { moveSpeed = value; } }
    public Rectangle VerticalStillcamZone { get { return verticalStillcamZone;} protected set { verticalStillcamZone = value;} } 
    public Rectangle HorizontalStillcamZone { get { return horizontalStillcamZone; } protected set { horizontalStillcamZone = value; } }
    public Matrix TranslationMatrix { get { return translationMatrix; } protected set { translationMatrix = value; } }

    // Constructor
    public Camera2D(Actor targetActor, Vector2 tiledMapSize) {
        this.targetActor = targetActor;
        this.target = targetActor.BoundingBox;
        this.targetOrigin = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
        this.tiledMapSize = tiledMapSize;
        this.position = Vector2.Zero;
        this.viewportSize = new Vector2(360, 360);
        this.origin = new Vector2(
            (int)position.X + (int)(viewportSize.X / 2),
            (int)position.Y + (int)(viewportSize.Y / 2)
        );
        this.moveSpeed = 96.0f;
        this.cameraVelocity = Vector2.Zero;
        this.cameraMargin = 4;
        GenerateStillcamZones();
        translationMatrix = CalculateTranslation();
    }

    public Camera2D(Actor targetActor, Vector2 tiledMapSize, Vector2 position) {
        this.targetActor = targetActor;
        this.target = targetActor.BoundingBox;
        this.targetOrigin = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
        this.tiledMapSize = tiledMapSize;
        this.position = position;
        this.viewportSize = new Vector2(360, 360);
        this.origin = new Vector2(
            (int)position.X + (int)(viewportSize.X / 2),
            (int)position.Y + (int)(viewportSize.Y / 2)
        );
        this.moveSpeed = 96.0f;
        this.cameraVelocity = Vector2.Zero;
        GenerateStillcamZones();
        this.cameraMargin = 4;
        translationMatrix = CalculateTranslation();
    }

    public Camera2D(Actor targetActor, Vector2 tiledMapSize, Vector2 position, Vector2 viewportSize, float moveSpeed, Rectangle verticalStillcamZone, Rectangle horizontalStillcamZone, int cameraMargin) {
        this.targetActor = targetActor;
        this.target = targetActor.BoundingBox;
        this.targetOrigin = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
        this.tiledMapSize = tiledMapSize;
        this.position = position;
        this.viewportSize = viewportSize;
        this.moveSpeed = moveSpeed;
        this.cameraVelocity = Vector2.Zero;
        this.verticalStillcamZone = verticalStillcamZone;
        this.horizontalStillcamZone = horizontalStillcamZone;
        this.cameraMargin = cameraMargin;
        translationMatrix = CalculateTranslation();
    }

    // Methods    
    public void Update() {

        // Check if Target is outside the Vertical Stillcam Zone on the Left or Right Side
        if (target.Left < verticalStillcamZone.Left) {
            // Begin Moving Left
            int leftDiff = Math.Abs(target.Left - verticalStillcamZone.Left);

            if(leftDiff < cameraMargin) {
                // Snap Camera into Position
                position = new Vector2(position.X - leftDiff, position.Y);
                cameraVelocity = new Vector2(0, cameraVelocity.Y);
                this.origin = new Vector2(
                    (int)position.X + (int)(viewportSize.X / 2),
                    (int)position.Y + (int)(viewportSize.Y / 2)
                );
                GenerateStillcamZones();
            }
            else {
                // Set Camera Velocity
                cameraVelocity = new Vector2(
                    -leftDiff,
                    cameraVelocity.Y
                );
            }
        }
        else if (target.Right > verticalStillcamZone.Right) {
            // Begin Moving Right
            int rightDiff = Math.Abs(target.Right - verticalStillcamZone.Right);

            if(rightDiff < cameraMargin) {
                // Snap Camera into Position
                position = new Vector2(position.X + rightDiff, position.Y);
                cameraVelocity = new Vector2(0, cameraVelocity.Y);
                this.origin = new Vector2(
                    (int)position.X + (int)(viewportSize.X / 2),
                    (int)position.Y + (int)(viewportSize.Y / 2)
                );
                GenerateStillcamZones();
            }
            else {
                // Set Camera Velocity
                cameraVelocity = new Vector2(
                    rightDiff,
                    cameraVelocity.Y
                );
            }
        }

    // Check if Target is outside the Horizontal Stillcam Zone on the Bottom or Top Side
        if (target.Bottom > horizontalStillcamZone.Bottom) {
            // Begin Moving Down
            int bottomDiff = Math.Abs(target.Bottom - horizontalStillcamZone.Bottom);

            if(bottomDiff < cameraMargin) {
                // Snap Camera to Correct Position
                position = new Vector2(position.X, position.Y + bottomDiff);
                cameraVelocity = new Vector2(cameraVelocity.X, 0);
                this.origin = new Vector2(
                    (int)position.X + (int)(viewportSize.X / 2),
                    (int)position.Y + (int)(viewportSize.Y / 2)
                );
                GenerateStillcamZones();
            }
            else {
                // Set Camera Velocity
                cameraVelocity = new Vector2(
                    cameraVelocity.X,
                    bottomDiff
                );
            }
        }
        else if (target.Top < horizontalStillcamZone.Top) {
            // Begin Moving Up
            int topDiff = Math.Abs(target.Top - horizontalStillcamZone.Top);
            if(topDiff < cameraMargin) {
                // Snap Camera to Position
                position = new Vector2(position.X, position.Y - topDiff);
                this.origin = new Vector2(
                    (int)position.X + (int)(viewportSize.X / 2),
                    (int)position.Y + (int)(viewportSize.Y / 2)
                );
                cameraVelocity = new Vector2(cameraVelocity.X, 0);
                GenerateStillcamZones();
            }
            else {
                // Set Camera Velocity
                cameraVelocity = new Vector2(
                    cameraVelocity.X,
                    -topDiff
                );
            }
        }

        // If any Velocity is being applied, update the camera via a Lerp call to keep movement smooth
        if(cameraVelocity.X != 0 || cameraVelocity.Y != 0) {
            position = Globals.Lerp(Position, (Position + cameraVelocity), Globals.DeltaTime);
            this.origin = new Vector2(
                (int)position.X + (int)(viewportSize.X / 2),
                (int)position.Y + (int)(viewportSize.Y / 2)
            );
            GenerateStillcamZones();
            cameraVelocity = Vector2.Zero;
        }

        // Clamp Camera to Camera Limits
        ClampCamera();

        // Update the Transformation Matrix
        translationMatrix = CalculateTranslation();
    }

    public void GenerateStillcamZones() {
        this.verticalStillcamZone = new Rectangle(
            (int)origin.X - (int)(viewportSize.X / 4),
            (int)position.Y,
            (int)viewportSize.X / 2,
            (int)viewportSize.Y);

        this.horizontalStillcamZone = new Rectangle(
            (int)position.X,
            (int)origin.Y - (int)(viewportSize.Y / 4),
            (int)viewportSize.X,
            (int)(viewportSize.Y / 2)
        );
    }

    protected Matrix CalculateTranslation() {
        var dx = (viewportSize.X / 2) - targetOrigin.X;
        dx = Math.Clamp(dx, -tiledMapSize.X + viewportSize.X + (16), 16);
        var dy = (viewportSize.Y / 2) - targetOrigin.Y;
        dy = Math.Clamp(dy, -tiledMapSize.Y + viewportSize.Y + 16, 16);
        return Matrix.CreateTranslation(dx, dy, 0f);
    }

    protected void ClampCamera() {
        // Get updated X and/or Y based on Clamp Limits
        float cameraXClamped = Math.Clamp(position.X, 0, tiledMapSize.X);
        float cameraYClamped = Math.Clamp(position.Y, 0, tiledMapSize.Y);

        if(cameraXClamped != position.X || cameraYClamped != position.Y) {
            // If either value needs to be updated, update the position of the camera and re-generate stillcam zones
            position = new Vector2(cameraXClamped, cameraYClamped);
            GenerateStillcamZones();
            translationMatrix = CalculateTranslation();
        }
    }

    public void OnTargetActorFinishedMoving(object o, MoveEventArgs args) {
        target = args.Boundingbox;
        GenerateStillcamZones();
        targetOrigin = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
        translationMatrix = CalculateTranslation();
    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.DebugTexture, VerticalStillcamZone, Color.DarkGreen * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, horizontalStillcamZone, Color.DarkGreen * 0.5f);
    }
}
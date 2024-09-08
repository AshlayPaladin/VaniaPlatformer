    using Microsoft.Xna.Framework;

    namespace VaniaPlatformer;

    public class Camera2D {

        // Fields
    private Actor target;
    private Vector2 stageSize;
    private float cameraMoveSpeed;

    // Properties
    public Vector2 Position { get; private set; }
    public float Zoom { get; private set; }
    public float Rotation { get; private set; }

    // Width and Height of Viewport window which we need to adjust
    // each time the player resizes the game window
    public int ViewportWidth { get; set; }
    public int ViewportHeight { get; set; }

    // Center of the Viewport, not accounting for scale
    public Vector2 ViewportCenter
    {
        get
        {   
            return new Vector2( ViewportWidth / 2, ViewportHeight / 2 );
        }
    }

        // Transform Matrix to offset everything drawn
        // since the camera Position is where the camera is,
        // we offset everything by the negative of that to simulate
        // a camera moving. We also cast INT to avoid artifacts
    public Matrix TranslationMatrix
    {
        get
        {
            Position = MapClampedPosition( Position );

            return Matrix.CreateTranslation( -(int) Position.X,
                -(int) Position.Y, 0 ) *
                Matrix.CreateRotationZ( Rotation ) *
                Matrix.CreateScale( new Vector3( Zoom, Zoom, 1 ) ) *
                Matrix.CreateTranslation( new Vector3( ViewportCenter, 0 ) );
        }
    }

    public Camera2D(Actor target, int stageWidth, int stageHeight, float cameraMovespeed = 96.0f) {
        this.target = target;
        this.stageSize = new Vector2( stageWidth, stageHeight);
        this.cameraMoveSpeed = cameraMovespeed;
        Zoom = 3.0f;
    }


    // Call this method with negative values to zoom out
    // or positive values to zoom in. It looks at the current zoom
    // and adjusts it by the specified amount. If we were at a 1.0f
    // zoom level and specified -0.5f amount it would leave us with
    // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
    public void AdjustZoom( float amount )
    {
        Zoom += amount;
        if ( Zoom < 0.25f )
        {
            Zoom = 0.25f;
        }
    }

    // Move the camera in an X and Y amount based on the cameraMovement param.
    // if clampToMap is true the camera will try not to pan outside of the
    // bounds of the map.
    public void MoveCamera( Vector2 cameraMovement, bool clampToMap = true )
    {
        Vector2 newPosition = Position + cameraMovement;

        if ( clampToMap )
        {
            Position = MapClampedPosition( newPosition );
        }
        else
        {
            Position = newPosition;
        }
    }

    public Rectangle ViewportWorldBoundry()
    {
        Vector2 viewPortCorner = ScreenToWorld( new Vector2( 0, 0 ) );
        Vector2 viewPortBottomCorner =
            ScreenToWorld( new Vector2( ViewportWidth, ViewportHeight ) );

        return new Rectangle( (int) viewPortCorner.X,
            (int) viewPortCorner.Y,
            (int) ( viewPortBottomCorner.X - viewPortCorner.X ),
            (int) ( viewPortBottomCorner.Y - viewPortCorner.Y ) );
    }

    // Center the camera on specific pixel coordinates
    public void CenterOn( Vector2 position )
    {
        Position = position;
    }

    // Clamp the camera so it never leaves the visible area of the map.
    private Vector2 MapClampedPosition( Vector2 position )
    {
        var cameraMax = new Vector2( stageSize.X -
            ( ViewportWidth / Zoom / 2 ),
            stageSize.Y -
            ( ViewportHeight / Zoom / 2 ) );

        return Vector2.Clamp( position,
            new Vector2( ViewportWidth / Zoom / 2, ViewportHeight / Zoom / 2 ),
            cameraMax );
    }

    public Vector2 WorldToScreen( Vector2 worldPosition )
    {
        return Vector2.Transform( worldPosition, TranslationMatrix );
    }

    public Vector2 ScreenToWorld( Vector2 screenPosition )
    {
        return Vector2.Transform( screenPosition,
            Matrix.Invert( TranslationMatrix ) );
    }

    public void Update() {
        CenterOn(target.Origin);
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class TileMapBackground {

    public enum ParallaxDirection {
        Left,
        Right,
        Up,
        Down
    }

    // Fields
    private Texture2D texture;
    private Vector2 position;
    private Vector2 repeatPosition;

    // Properties
    public bool ParallaxEnabled { get; private set; }
    public bool ParallaxWithPlayer { get; private set; }
    public float ParallaxSpeed { get; private set; }
    public ParallaxDirection Direction { get; private set; }

    // Constructor
    public TileMapBackground(Texture2D texture, Vector2 position, bool parallaxEnabled = false, bool parallaxWithPlayer = false, float parallaxSpeed = 0f, ParallaxDirection parallaxDirection = ParallaxDirection.Left) {
        this.texture = texture;
        this.ParallaxEnabled = parallaxEnabled;
        this.ParallaxWithPlayer = parallaxWithPlayer;
        this.ParallaxSpeed = parallaxSpeed;
        this.Direction = parallaxDirection;
        this.position = position;

        repeatPosition = Vector2.Zero;
    }

    // Methods
    public void Update(float deltaTime) {
        // Update Position using Parallax, if needed
        if(ParallaxEnabled & !ParallaxWithPlayer) {
            // Run Parallax Normally Always (Direction * Speed)
            Vector2 velocity = GetParallaxVelocity();
            position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
            position.X %= texture.Width;
        }

        if(position.X <= 0) {
            repeatPosition = new Vector2(position.X + texture.Width, position.Y);
        }
        else {
            repeatPosition = new Vector2(position.X - texture.Width, position.Y);
        }
    }

    public void Draw(SpriteBatch spriteBatch) { 
        spriteBatch.Draw(texture, position, Color.White);
        spriteBatch.Draw(texture, repeatPosition, Color.White);
    }

    public Vector2 GetParallaxVelocity() {
        Vector2 velocity = new Vector2(0, 0);

        switch(Direction) {
            case ParallaxDirection.Left: {
                float velocityX = (float)Globals.DeltaTime * (-ParallaxSpeed);
                velocity = new Vector2(velocityX, velocity.Y);
                break;
            }
            case ParallaxDirection.Right: {
                float velocityX = (float)Globals.DeltaTime * (ParallaxSpeed);
                velocity = new Vector2(velocityX, velocity.Y);
                break;
            }
            case ParallaxDirection.Up: {
                float velocityY = (float)Globals.DeltaTime * (-ParallaxSpeed);
                velocity = new Vector2(velocity.X, velocityY);
                break;
            }
            case ParallaxDirection.Down: {
                float velocityY = (float)Globals.DeltaTime * (ParallaxSpeed);
                velocity = new Vector2(velocity.X, velocityY);
                break;
            }
        }

        return velocity;
    }

    public void OnPlayerMoved(object o, MoveEventArgs args) {
        // Used for ParallaxWithPlayer
        int playerVelocityX = (int)args.Velocity.X;
        Vector2 velocity = GetParallaxVelocity();

        if(playerVelocityX > 0) {
            // Move Parallax Normally
            position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
            position.X %= texture.Width;
        }
        else if(playerVelocityX < 0) {
            // Move Parallax Backward
            position = new Vector2(position.X - velocity.X, position.Y - velocity.Y);
            position.X %= texture.Width;
        }

        if(position.X <= 0) {
            repeatPosition = new Vector2(position.X + texture.Width, position.Y);
        }
        else {
            repeatPosition = new Vector2(position.X - texture.Width, position.Y);
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer;

public class SolidActor : Actor {

    // Fields
    private Texture2D texture;
    private Rectangle boundingBox;

    // Properties
    public int Width { get; private set;}
    public int Height { get; private set;}
    

    // Constructor
    public SolidActor(Vector2 position, int width, int height) {
        Position = new Vector2(position.X, position.Y);
        Width = width;
        Height = height;

        texture = Globals.Content.Load<Texture2D>("textures/DebugPixel");

        boundingBox = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            Width,
            Height);

        SetOrigin();
        
        Colliders = new List<Rectangle>() { boundingBox };
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public override void MoveAndSlide()
    {
        throw new System.NotImplementedException();
    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(texture, boundingBox, Color.Red * 0.5f);
    }

    public void OnPlayerMoved(object o, MoveEventArgs args) {
        Rectangle playerRect = args.Boundingbox;

        if(playerRect.Intersects(boundingBox)) {
            // Add Collision to Player
            Player player = o as Player;
            player.AddCollision(Rectangle.Intersect(playerRect, boundingBox));
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMapLib;

namespace VaniaPlatformer;

public static class Globals {

    // Constants
    public const Keys LeftKey = Keys.A;
    public const Keys RightKey = Keys.D;
    public const Keys JumpKey = Keys.W;
    public const Keys RunKey = Keys.LeftShift;

    // Properties & Collections
    public static ContentManager Content;
    public static float DeltaTime;
    public static Texture2D DebugTexture;
    public static Tileset ActiveTileset;
    public static Vector2 GraphicsScreenSize;

    // Methods
    public static void InitializeGlobals(ContentManager content) {
        Content = content;
        DebugTexture = Content.Load<Texture2D>("textures/DebugPixel");
    }

    public static void Update(GameTime gameTime) {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public static void SetActiveTileset(TiledMapTileset tiledMapTileset) 
    {
        ActiveTileset = new Tileset(tiledMapTileset.Name, tiledMapTileset.Image, tiledMapTileset.Rows, tiledMapTileset.Columns, tiledMapTileset.FirstGid, tiledMapTileset.ImageWidth,
        tiledMapTileset.ImageHeight, tiledMapTileset.Margin, tiledMapTileset.Spacing, tiledMapTileset.TileWidth, tiledMapTileset.TileHeight, tiledMapTileset.TileCount);
    }

    public static Vector2 Lerp(Vector2 firstPosition, Vector2 secondPosition, float amount) {
        float retX = Lerp(firstPosition.X, secondPosition.X, amount);
        float retY = Lerp(firstPosition.Y, secondPosition.Y, amount);
        return new Vector2(retX, retY);
    }

    public static float Lerp(float initial, float target, float amount) {
        return initial * (1 - amount) + target * amount;
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledMapLib;

namespace VaniaPlatformer;

public static class Globals {

    // Fields


    // Properties & Collections
    public static ContentManager Content;
    public static double DeltaTime;
    public static Texture2D DebugTexture;
    public static Tileset ActiveTileset;

    // Methods
    public static void InitializeGlobals(ContentManager content) {
        Content = content;
        DebugTexture = Content.Load<Texture2D>("textures/DebugPixel");
    }

    public static void Update(GameTime gameTime) {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
    }

    public static void SetActiveTileset(TiledMapTileset tiledMapTileset) 
    {
        ActiveTileset = new Tileset(tiledMapTileset.Name, tiledMapTileset.Image, tiledMapTileset.Rows, tiledMapTileset.Columns, tiledMapTileset.FirstGid, tiledMapTileset.ImageWidth,
        tiledMapTileset.ImageHeight, tiledMapTileset.Margin, tiledMapTileset.Spacing, tiledMapTileset.TileWidth, tiledMapTileset.TileHeight, tiledMapTileset.TileCount);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMapLib;

namespace VaniaPlatformer;

public static class Globals {

    // Events
    public static event EventHandler WindowSizeChanged;

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
    public static Vector2 WindowSize;
    public static List<Vector2> Resolutions;

    // Methods
    public static void InitializeGlobals(ContentManager content) {
        Content = content;
        DebugTexture = Content.Load<Texture2D>("textures/DebugPixel");

        Resolutions = new List<Vector2>() 
        {
            new Vector2(640, 480),
            new Vector2(800, 600),
            new Vector2(1024, 768),
            new Vector2(1280, 800),
            new Vector2(1440, 900),
            new Vector2(1680, 1050),
            new Vector2(1920, 1200),
            new Vector2(1280, 720),
            new Vector2(1366, 768),
            new Vector2(1920, 1080),
            new Vector2(2560, 1440)
        };

        WindowSize = Resolutions[0];
    }

    public static void Update(GameTime gameTime) {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public static void SetActiveTileset(TiledMapTileset tiledMapTileset) 
    {
        ActiveTileset = new Tileset(tiledMapTileset.Name, tiledMapTileset.Image, tiledMapTileset.Rows, tiledMapTileset.Columns, tiledMapTileset.FirstGid, tiledMapTileset.ImageWidth,
        tiledMapTileset.ImageHeight, tiledMapTileset.Margin, tiledMapTileset.Spacing, tiledMapTileset.TileWidth, tiledMapTileset.TileHeight, tiledMapTileset.TileCount);
    }

    public static void DebugNextResolution()
    {
        for(int i = 0; i < Resolutions.Count; i++)
        {
            if(Resolutions.ElementAt(i).Equals(WindowSize))
            {
                if(i == Resolutions.Count - 1)
                {
                    WindowSize = Resolutions[0];
                }
                else
                {
                    WindowSize = Resolutions[i + 1];
                }

                break;
            }
        }

        WindowSizeChanged?.Invoke(null, null);
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
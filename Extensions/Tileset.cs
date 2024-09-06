using TiledMapLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VaniaPlatformer;

public class Tileset : TiledMapTileset {
    
    // Constants
    private const string TILESET_PREFIX = "textures/tilesets/";

    // Fields
    private Texture2D textureAtlas;
    private Dictionary<int, Rectangle> tiles;

    // Properties
    public Texture2D TextureAtlas { get { return textureAtlas; } protected set { textureAtlas = value; } }
    public Dictionary<int,Rectangle> Tiles { get { return tiles;} protected set { tiles = value; } }

    // Constructor
    public Tileset(string name, string image, int rows, int columns, int firstGid, int imageWidth, int imageHeight, int margin, int spacing, int tileWidth, int tileHeight, int tileCount) 
    : base(name, image, rows, columns, firstGid, imageWidth, imageHeight, margin, spacing, tileWidth, tileHeight, tileCount) 
    {
        TextureAtlas = Globals.Content.Load<Texture2D>(TILESET_PREFIX + name);
        Tiles = new Dictionary<int, Rectangle>();

        CreateTiles();
    }

    // Methods
    private void CreateTiles()
    {
        int tilesPerRow = TextureAtlas.Width / TileWidth;
        int totalRows = TextureAtlas.Height / TileWidth;
        int totalTiles = tilesPerRow * totalRows;

        for (int i = 0; i < totalTiles; i++)
        {
            int _tileRow = i / tilesPerRow;
            int _tileColumn = i % tilesPerRow;

            int _tileX = _tileColumn * TileWidth;
            int _tileY = _tileRow * TileWidth;

            Tiles.Add(i + 1, new Rectangle(
                _tileX,
                _tileY,
                TileWidth,
                TileWidth));
        }
    }
}
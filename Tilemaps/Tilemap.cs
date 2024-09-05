using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class Tilemap {
    
    // Fields
    int tilesWide;
    int tilesTall;
    int tileWidth;
    Tileset tileset;
    int[][] tilemap;
    List<int[][]> tilemaps;

    // Properties
    public List<ObjectTile> ObjectTiles;

    // Constructor
    public Tilemap(Tileset tileset, int tilesWide, int tilesTall, int tileWidth, List<int[][]> tilemaps, List<ObjectTile> objectTiles) {
        this.tileset = tileset;
        this.tilesWide = tilesWide;
        this.tilesTall = tilesTall;
        this.tileWidth = tileWidth;
        this.tilemaps = tilemaps;

        ObjectTiles = new List<ObjectTile>(objectTiles);
    }

    // Methods
    public void RenderMap(SpriteBatch spriteBatch) {
        if(tilemaps.Count > 0) {
            foreach(var tilemap in tilemaps) {
                for(int i = 0; i < tilesWide; i++) {
                    for(int j = 0; j < tilesTall; j++) {
                        int tileIndex = tilemap[j][i];

                        if(tileIndex != 0) {
                            int destRectX = (tileWidth * i);
                            int destRectY = (tileWidth * j);
                            Rectangle destRect = new Rectangle(destRectX, destRectY, tileWidth, tileWidth);

                            spriteBatch.Draw(tileset.TextureAtlas, destRect, tileset.Tiles[tileIndex], Color.White);
                        }
                    }
                }
            }
        }
    }
}
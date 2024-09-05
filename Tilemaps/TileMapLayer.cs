using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class TileMapLayer {

    // Field
    int tileWidth;
    int layerWidth;
    int layerHeight;
    int mapTilesWide;
    int mapTilesTall;

    // Properties
    public Tileset Tileset { get; private set; }
    public int[][] TileLayerMap { get; private set; }

    public TileMapLayer(Tileset tileset, int[][] tileLayerMap, int tileWidth, int layerWidth, int layerHeight) {
        this.Tileset = tileset;
        this.TileLayerMap = tileLayerMap;
        this.tileWidth = tileWidth;
        this.layerWidth = layerWidth;
        this.layerHeight = layerHeight;

        mapTilesWide = layerWidth / tileWidth;
        mapTilesTall = layerHeight / tileWidth;
    }

    public void Draw(SpriteBatch spriteBatch) {
        for(int i = 0; i < mapTilesWide; i++) {
            for(int j = 0; j < mapTilesTall; j++) {
                int tileIndex = TileLayerMap[j][i];

                if(tileIndex != 0) {
                    int destRectX = (tileWidth * i);
                    int destRectY = (tileWidth * j);
                    Rectangle destRect = new Rectangle(destRectX, destRectY, tileWidth, tileWidth);

                    spriteBatch.Draw(Tileset.TextureAtlas, destRect, Tileset.Tiles[tileIndex], Color.White);
                }
            }
        }
    }
}
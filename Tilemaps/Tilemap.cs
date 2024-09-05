using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class TileMap {
    
    // Fields
    int tilesWide;
    int tilesTall;
    int tileWidth;
    Tileset tileset;
    List<TileMapLayer> tileMapLayers;
    List<TileMapBackground> tileMapBackgrounds;

    // Properties
    public List<ObjectTile> ObjectTiles;
    public List<TileMapBackground> TileMapBackgrounds { get {return tileMapBackgrounds;} }

    // Constructor
    public TileMap(Tileset tileset, int tilesWide, int tilesTall, int tileWidth, List<TileMapBackground> tileMapBackgrounds, List<TileMapLayer> tileMapLayers, List<ObjectTile> objectTiles) {
        this.tileset = tileset;
        this.tilesWide = tilesWide;
        this.tilesTall = tilesTall;
        this.tileWidth = tileWidth;
        this.tileMapBackgrounds = tileMapBackgrounds;
        this.tileMapLayers = tileMapLayers;

        ObjectTiles = new List<ObjectTile>(objectTiles);
    }

    // Methods
    public void Update() {
        if(tileMapBackgrounds.Count > 0) {
            foreach(var back in tileMapBackgrounds) {
                back.Update((float)Globals.DeltaTime);
            }
        }
    }

    public void RenderMap(SpriteBatch spriteBatch) {
        if(tileMapBackgrounds.Count > 0) {
            // Render all Backgrounds
            foreach(var back in tileMapBackgrounds) {
                back.Draw(spriteBatch);
            }
        }

        if(tileMapLayers.Count > 0) {
            foreach(var layer in tileMapLayers) {
                layer.Draw(spriteBatch);
            }
        }
    }
}
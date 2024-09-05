using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class Tileset {

    // Fields
    private int atlasWidth;
    private int atlasHeight;
    private float testTimerReset;
    private float testTimerCurrent;
    private int testDrawIndex;
    
    // Properties
    public Texture2D TextureAtlas { get; private set;}
    public int TileWidth { get; private set;}
    public Dictionary<int, Rectangle> Tiles { get; private set;}

    public Tileset(Texture2D textureAtlas, int tileWidth) {
        this.TextureAtlas = textureAtlas;
        this.TileWidth = tileWidth;

        this.atlasWidth = textureAtlas.Width;
        this.atlasHeight = textureAtlas.Height;

        Tiles = new Dictionary<int, Rectangle>();
        CutoutTiles();

        testTimerReset = 0.5f;
        testTimerCurrent = testTimerReset;
        testDrawIndex = 0;
    }

    // Methods
    private void CutoutTiles() {
		int tilesPerRow = atlasWidth / TileWidth;
		int totalRows = atlasHeight / TileWidth;
		int totalTiles = tilesPerRow * totalRows;
		
		for(int i = 0; i < totalTiles; i++) {
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

    public void UpdateTest() {
        if(testTimerCurrent > 0) {
            testTimerCurrent -= (float)Globals.DeltaTime;
        }
        else {
            if(testDrawIndex == Tiles.Count) {
                testDrawIndex = 0;
            }
            else {
                testDrawIndex++;
            }

            testTimerCurrent = testTimerReset;
        }
    }

    public void DrawTest(SpriteBatch spriteBatch, Rectangle destinationRectangle) {
        if(testDrawIndex > 0) {
            spriteBatch.Draw(TextureAtlas, destinationRectangle, Tiles[testDrawIndex], Color.White);
        }
    }
}
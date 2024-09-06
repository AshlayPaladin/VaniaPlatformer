using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledMapLib;

namespace VaniaPlatformer;

public static class TiledMapTileLayerExtensions {

    public static void Draw(this TiledMapTileLayer tiledMapTileLayer, SpriteBatch spriteBatch) {
        for (int i = 0; i < tiledMapTileLayer.Width; i++)
		{
			for (int j = 0; j < tiledMapTileLayer.Height; j++)
			{
				int tileIndex = tiledMapTileLayer.TileMap[j][i];

				if (tileIndex != 0)
				{
					int destRectX = (tiledMapTileLayer.TileWidth * i);
					int destRectY = (tiledMapTileLayer.TileWidth * j);
					Rectangle destRect = new Rectangle(destRectX, destRectY, tiledMapTileLayer.TileWidth, tiledMapTileLayer.TileWidth);

					spriteBatch.Draw(Globals.ActiveTileset.TextureAtlas, destRect, Globals.ActiveTileset.Tiles[tileIndex], Color.White);
				}
			}
		}
    }
}
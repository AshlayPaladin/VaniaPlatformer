using Microsoft.Xna.Framework.Graphics;
using TiledMapLib;

namespace VaniaPlatformer;

public static class TiledMapExtensions {

    public static void RenderMap(this TiledMap t, SpriteBatch spriteBatch) {
        foreach(var layer in t.TiledMapTileLayers) {
            layer.Draw(spriteBatch);
        }
    }
}
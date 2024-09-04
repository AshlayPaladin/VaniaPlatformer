using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer;

public static class Globals {

    // Fields


    // Properties & Collections
    public static ContentManager Content;
    public static double DeltaTime;
    public static Texture2D DebugTexture;

    // Methods
    public static void InitializeGlobals(ContentManager content) {
        Content = content;
        DebugTexture = Content.Load<Texture2D>("textures/DebugPixel");
    }

    public static void Update(GameTime gameTime) {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
    }
}
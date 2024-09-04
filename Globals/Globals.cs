using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VaniaPlatformer;

public static class Globals {

    // Fields


    // Properties & Collections
    public static ContentManager Content;
    public static double DeltaTime;

    // Methods
    public static void InitializeGlobals(ContentManager content) {
        Content = content;
    }

    public static void Update(GameTime gameTime) {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
    }
}
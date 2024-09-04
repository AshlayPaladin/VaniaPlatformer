using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace VaniaPlatformer;

public static class Common {
    
    // Enums
    public enum AnimationIndex {
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Swing,
        Throw,
        Cast,
        Hurt,
        Die
    }

    // Properties & Collections
    private static List<TextureRef> textureRefs = new List<TextureRef>();

    public static void InitializeCommon(ContentManager content) {
        InitializeTextureRefLibrary(content);
    }

    private static void InitializeTextureRefLibrary(ContentManager content) {
        TextureRef testTextureRef = new TextureRef("PlayerSheet", "textures/AdventurerSheet", content);

        textureRefs.Add(testTextureRef);
    }

    public static TextureRef GetTextureRefByID(string id) {
        foreach(var textureRef in textureRefs) {
            if(textureRef.ID == id) {
                return textureRef;
            }
        }

        return null;
    }
}
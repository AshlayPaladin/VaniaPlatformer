using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer;

public class TextureRef {

    // Fields
    private ContentManager content;

    // Properties
    public string ID { get; private set; }
    public string AssetPath {get; private set; }    
    public Texture2D Texture { get; private set; }
    public int GameActorsUsing { get; private set; }

    // Constructors
    public TextureRef(string id, string assetPath, ContentManager contentManager) {
        ID = id;
        AssetPath = assetPath;
        content = contentManager;
        Texture = null;
        GameActorsUsing = 0;
    }

    // Methods
    public Texture2D LoadTexture() {
        if (Texture == null) {
            Texture = content.Load<Texture2D>(AssetPath);
        }

        GameActorsUsing++;
        return Texture;
    }

    public void UnloadTexture() {
        if(GameActorsUsing == 1) {
            Texture = null;
        }
        
        GameActorsUsing--;
    }
}
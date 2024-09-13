using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class AnimationManager {
    
    // Fields
    private bool playbackIsPaused;
    private SpriteEffects spriteEffect;

    // Properties
    public Dictionary<string, AnimatedSprite> AnimatedSprites { get; private set; }
    public AnimatedSprite Sprite { get; private set; }

    // Constructors
    public AnimationManager() {
        playbackIsPaused = false;
        spriteEffect = SpriteEffects.None;

        AnimatedSprites = new Dictionary<string, AnimatedSprite>();
        Sprite = null;
    }

    // Methods
    public void AddAnimation(string name, AnimatedSprite animation) {
        if(AnimatedSprites.ContainsKey(name)) {
            AnimatedSprites[name] = animation;
        }
        else {
            AnimatedSprites.Add(name, animation);
        }
    }

    public void Play(string name) {
        if(AnimatedSprites.ContainsKey(name)) {
                Sprite = AnimatedSprites[name];
                return;
            }
    }

    public void Play() {
        Resume();
    }

    public void Resume() {
        if(playbackIsPaused && Sprite != null) {
            playbackIsPaused = false;
        }
    }
    
    public void Stop() {
        playbackIsPaused = true;
    }

    public void Update() {
        if (Sprite != null && !playbackIsPaused) {
            Sprite.Update();
        }
    }

    public void FlipHorizontally() {
        this.spriteEffect = SpriteEffects.FlipHorizontally;
    }

    public void FlipVertically() {
        this.spriteEffect = SpriteEffects.FlipVertically;
    }

    public void SetSpriteEffects(SpriteEffects spriteEffect) {
        this.spriteEffect = spriteEffect;
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color colorMask) {
        Sprite.Draw(spriteBatch, destinationRectangle, colorMask, spriteEffect);
    }
}
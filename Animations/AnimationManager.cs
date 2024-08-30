using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class AnimationManager {
    
    // Fields
    private List<Animation> animations;
    private Animation activeAnimation;
    private float animationTimer;
    private bool playbackIsPaused;

    // Properties


    // Constructors
    public AnimationManager() {
        animations = new List<Animation>();
        activeAnimation = null;
        animationTimer = 0;
        playbackIsPaused = false;
    }

    // Methods
    public void AddAnimation(Common.AnimationIndex index, Animation animation) {
        for(int i = 0; i < animations.Count; i++) {
            if(animations[i].Index == index) {
                animations[i] = animation;
                return;
            }
        }

        animations.Add(animation);
    }

    public void Play(Common.AnimationIndex index) {
        foreach(Animation a in animations) {
            if(a.Index == index) {
                activeAnimation = a;
                animationTimer = a.FrameTime;
                return;
            }
        }
    }

    public void Play() {
        Resume();
    }

    public void Resume() {
        if(playbackIsPaused && activeAnimation != null) {
            playbackIsPaused = false;
        }
    }
    
    public void Stop() {
        playbackIsPaused = true;
    }

    public void Update(GameTime gameTime) {
        if (activeAnimation != null) {
            if(animationTimer > 0) {
                animationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else {
                activeAnimation.NextFrame();
                animationTimer = activeAnimation.FrameTime;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color colorMask) {
        spriteBatch.Draw(activeAnimation.Texture, destinationRectangle, activeAnimation.SourceRectangle, colorMask);
    }
}
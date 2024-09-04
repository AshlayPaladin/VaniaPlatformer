using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class AnimationManager {
    
    // Fields
    private Dictionary<string,Animation> animationDictionary;
    private Animation activeAnimation;
    private float animationTimer;
    private bool playbackIsPaused;

    // Properties


    // Constructors
    public AnimationManager() {
        animationDictionary = new Dictionary<string,Animation>();
        activeAnimation = null;
        animationTimer = 0;
        playbackIsPaused = false;
    }

    // Methods
    public void AddAnimation(string name, Animation animation) {
        if(animationDictionary.ContainsKey(name)) {
            animationDictionary[name] = animation;
        }
        else {
            animationDictionary.Add(name,animation);
        }
    }

    public void Play(string name) {
        if(animationDictionary.ContainsKey(name)) {
                activeAnimation = animationDictionary[name];
                animationTimer = activeAnimation.FrameTime;
                return;
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

    public void Update() {
        if (activeAnimation != null && !playbackIsPaused) {
            if(animationTimer > 0) {
                animationTimer -= (float)Globals.DeltaTime;
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
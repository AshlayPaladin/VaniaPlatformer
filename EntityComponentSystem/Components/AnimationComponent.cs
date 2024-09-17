using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class Animation
{
    // Enum
    public enum Loop {
        None,
        FromBeginning,
        Reverse
    }

    // Properties
    public string Name { get; private set; }
    public Rectangle FirstFrame { get; private set; }
    public int Frames { get; private set; }
    public float FrameTime { get; private set; }
    public Loop LoopType { get; private set; }

    public Animation(string name, Rectangle firstFrame, int frames, float frameTime, Loop loopType = Loop.None)
    {
        this.Name = name;
        this.FirstFrame = firstFrame;
        this.FrameTime = frameTime;
        this.Frames = frames;
        this.LoopType = loopType;
    }
}

public class AnimationComponent : Component
{
    // Properties
    public int FrameIndex;
    public int FinalFrameIndex;
    public float FrameTimer;
    public bool IsReverseAnimating;
    public Vector2 FirstFramePosition;
    public Vector2 FirstFrameSize;
    public Rectangle DestinationRectangle = Rectangle.Empty;
    public List<Animation> Animations;
    public Animation Animation;
    public bool Paused;

    // Constructors
    public AnimationComponent()
    {

        Animations = new List<Animation>();

        Animation = null;
        FrameIndex = 0;
        FinalFrameIndex = 0;
        FrameTimer = 0f;
        FirstFramePosition = Vector2.Zero;
        FirstFrameSize = Vector2.Zero;
        Paused = false;

        Paused = false;
        AnimationSystem.Register(this);
    }

    // Methods
    public void Play(Animation animation) 
    {
        // Swap out Current Animation with New One
        if(Animation != animation)
        {
            Animation = animation;

            FrameIndex = 0;
            FinalFrameIndex = animation.Frames - 1;
            FrameTimer = animation.FrameTime;
            FirstFramePosition = new Vector2(animation.FirstFrame.X, animation.FirstFrame.Y);
            FirstFrameSize = new Vector2(animation.FirstFrame.Width, animation.FirstFrame.Height);

            Entity.GetComponent<SpriteComponent>().SourceRectangle = new Rectangle(
                    (int)(FirstFramePosition.X + (FirstFrameSize.X * FrameIndex)),
                    (int)FirstFramePosition.Y,
                    (int)FirstFrameSize.X,
                    (int)FirstFrameSize.Y);

            Entity.GetComponent<SpriteComponent>().Origin = new Vector2(
                animation.FirstFrame.Width / 2,
                animation.FirstFrame.Height / 2
            );

            Paused = false;
        }
    }

    public void Pause()
    {
        // Pause Animation
        Paused = true;
    }

    public void Resume()
    {
        // Resume Animation
        Paused = false;
    }
}
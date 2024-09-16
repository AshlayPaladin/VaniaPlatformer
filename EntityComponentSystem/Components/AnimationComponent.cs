using System;
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
    public Guid ID { get; private set; }
    public Rectangle FirstFrame { get; private set; }
    public int Frames { get; private set; }
    public float FrameTime { get; private set; }
    public Loop LoopType { get; private set; }

    public Animation(Rectangle firstFrame, int frames, float frameTime, Loop loopType = Loop.None)
    {
        this.ID = Guid.NewGuid();
        this.FirstFrame = firstFrame;
        this.FrameTime = frameTime;
        this.Frames = frames;
        this.LoopType = loopType;
    }
}

public class AnimationComponent : Component
{
    // Enums
    

    // Fields
    private int frameIndex;
    private int finalFrameIndex;
    private float frameTimer;
    private bool isReverseAnimating;
    private Vector2 firstFramePosition;
    private Vector2 firstFrameSize;

    // Properties
    public Rectangle DestinationRectangle = Rectangle.Empty;
    public Animation Animation;
    public bool Paused { get; private set; }

    // Constructors
    public AnimationComponent(Animation animation = null)
    {

        if(animation != null)
        {
            this.Animation = animation;
            Play(Animation);
        }
        else
        {
            Animation = null;
            this.frameIndex = 0;
            this.finalFrameIndex = 0;
            this.frameTimer = 0f;
            this.firstFramePosition = Vector2.Zero;
            this.firstFrameSize = Vector2.Zero;
            Pause();
        }

        Paused = false;
        AnimationSystem.Register(this);
    }

    // Methods
    public override void Update() 
    {
        if(!Paused)
        {
            if(frameTimer > 0) {
                frameTimer -= Globals.DeltaTime;
            }
            else {
                switch(Animation.LoopType) {
                    case Animation.Loop.FromBeginning:
                    {
                        if(frameIndex == finalFrameIndex) {
                            frameIndex = 0;
                            frameTimer = Animation.FrameTime;
                        }
                        else {
                            frameIndex++;
                            frameTimer = Animation.FrameTime;
                        }
                        break;
                    }
                    case Animation.Loop.Reverse:
                    {
                        if(frameIndex == finalFrameIndex && !isReverseAnimating) {
                            isReverseAnimating = !isReverseAnimating;
                            frameIndex--;
                            frameTimer = Animation.FrameTime;
                        }
                        else if(frameIndex == 0 && isReverseAnimating) {
                            isReverseAnimating = !isReverseAnimating;
                            frameIndex++;
                            frameTimer = Animation.FrameTime;
                        }
                        else {
                            if(isReverseAnimating) {
                                frameIndex--;
                            }
                            else {
                                frameIndex++;
                            }

                            frameTimer = Animation.FrameTime;
                        }
                        break;
                    }
                }

                Entity.GetComponent<SpriteComponent>().SourceRectangle = new Rectangle(
                    (int)(firstFramePosition.X + (firstFrameSize.X * frameIndex)),
                    (int)firstFramePosition.Y,
                    (int)firstFrameSize.X,
                    (int)firstFrameSize.Y);

            }
        }
    }

    public void Play(Animation animation) 
    {
        // Swap out Current Animation with New One
        if(Animation != animation)
        {
            Animation = animation;

            this.frameIndex = 0;
            this.finalFrameIndex = animation.Frames - 1;
            this.frameTimer = animation.FrameTime;
            this.firstFramePosition = new Vector2(animation.FirstFrame.X, animation.FirstFrame.Y);
            this.firstFrameSize = new Vector2(animation.FirstFrame.Width, animation.FirstFrame.Height);

            Entity.GetComponent<SpriteComponent>().SourceRectangle = new Rectangle(
                    (int)(firstFramePosition.X + (firstFrameSize.X * frameIndex)),
                    (int)firstFramePosition.Y,
                    (int)firstFrameSize.X,
                    (int)firstFrameSize.Y);

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
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class AnimationComponent : Component
{
    // Enums
    public enum Loop {
        None,
        FromBeginning,
        Reverse
    }

    // Fields
    private int frameIndex;
    private int finalFrameIndex;
    private float frameTimer;
    private bool isReverseAnimating;
    private Vector2 firstFramePosition;
    private Vector2 firstFrameSize;

    // Properties
    public Rectangle Frame = Rectangle.Empty;
    public Rectangle DestinationRectangle = Rectangle.Empty;
    public int Frames = 0;
    public float FrameTime = 0;
    public Loop LoopType = Loop.None;

    // Constructors
    public AnimationComponent(Rectangle firstFrame, int frames, float frameTime, Loop loopType = Loop.None)
    {
        this.Frame = firstFrame;
        this.Frames = frames;
        this.FrameTime = frameTime;
        this.LoopType = loopType;

        this.frameIndex = 0;
        this.finalFrameIndex = Frames - 1;
        this.frameTimer = FrameTime;
        this.firstFramePosition = new Vector2(Frame.X, Frame.Y);
        this.firstFrameSize = new Vector2(Frame.Width, Frame.Height);

        Entity.GetComponent<SpriteComponent>().Origin = new Vector2(
            Frame.Width / 2,
            Frame.Height / 2
        );

        AnimationSystem.Register(this);
    }

    // Methods
    public override void Update() 
    {
        if(frameTimer > 0) {
            frameTimer -= Globals.DeltaTime;
        }
        else {
            switch(LoopType) {
                case Loop.FromBeginning:
                {
                    if(frameIndex == finalFrameIndex) {
                        frameIndex = 0;
                    }
                    else {
                        frameIndex++;
                        frameTimer = FrameTime;
                    }
                    break;
                }
                case Loop.Reverse:
                {
                    if(frameIndex == finalFrameIndex && !isReverseAnimating) {
                        isReverseAnimating = !isReverseAnimating;
                        frameIndex--;
                        frameTimer = FrameTime;
                    }
                    else if(frameIndex == 0 && isReverseAnimating) {
                        isReverseAnimating = !isReverseAnimating;
                        frameIndex++;
                        frameTimer = FrameTime;
                    }
                    else {
                        if(isReverseAnimating) {
                            frameIndex--;
                        }
                        else {
                            frameIndex++;
                        }

                        frameTimer = FrameTime;
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
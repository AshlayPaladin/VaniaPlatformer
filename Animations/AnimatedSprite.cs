using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class AnimatedSprite : Sprite {

    // Enum
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
    private float rotationRadians;

    // Properties
    public int Frames { get; protected set; }
    public float FrameTime { get; protected set; }
    public bool Looping { get; protected set; }
    public Loop LoopType { get; protected set; } 

    // Constructor
    /// <summary>
    /// Creates a new AnimatedSprite
    /// </summary>
    /// <param name="spritesheet">Texture2D to pull all frames from. Frames should be arranged horizontally in a row</param>
    /// <param name="sourceRect">Rectangle of Frame 0</param>
    /// <param name="frames">Number of Frames in entire animation</param>
    /// <param name="frameTime">Time between frames (in seconds)</param>
    /// <param name="loopType">How to handle reaching the end of the animation</param>
    public AnimatedSprite(Texture2D spritesheet, Rectangle sourceRect, int frames, float frameTime, Loop loopType = AnimatedSprite.Loop.None) : base(spritesheet, sourceRect)
    {
        this.Spritesheet = spritesheet;
        this.SourceRect = sourceRect;
        this.Frames = frames;
        this.FrameTime = frameTime;
        this.LoopType = loopType;

        this.firstFramePosition = new Vector2(sourceRect.X, sourceRect.Y);
        this.firstFrameSize = new Vector2(sourceRect.Width, sourceRect.Height);
        this.frameIndex = 0;
        this.finalFrameIndex = Frames - 1;
        this.frameTimer = FrameTime;
        this.isReverseAnimating = false;
        rotationRadians = MathHelper.ToRadians(0);
    }

    // Methods
    public void Update() {
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

            SourceRect = new Rectangle(
                (int)(firstFramePosition.X + (firstFrameSize.X * frameIndex)),
                (int)firstFramePosition.Y,
                (int)firstFrameSize.X,
                (int)firstFrameSize.Y);
        }
    }
}
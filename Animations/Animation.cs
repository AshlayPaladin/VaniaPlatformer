using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Animations;

public class Animation {

    // Fields
    private Texture2D texture;
    private Vector2 framePosition;
    private Vector2 frameSize;
    private int frameCount;
    private int frameIndex;
    private float frameTime;
    private bool reverseOnEnd;
    private bool isReverseAnimating;

    // Properties
    public Texture2D Texture {
        get { return texture; }
    }
    public Vector2 FramePosition {
        get { return framePosition; }
    }
    public Vector2 FrameSize {
        get { return frameSize; }
    }
    public int FrameCount {
        get { return frameCount; }
    }
    public float FrameTime {
        get { return frameTime; }
    }
    public bool ReverseOnEnd {
        get { return reverseOnEnd; }
    }
    public Rectangle SourceRectangle { get; private set; }


    // Constructor
    public Animation(Texture2D texture, Vector2 framePosition, Vector2 frameSize, int frameCount, float frameTime = 30.0f, bool reverseOnEnd = false) {
        this.texture = texture;
        this.framePosition = framePosition;
        this.frameSize = frameSize;
        this.frameCount = frameCount;
        this.frameTime = frameTime;
        this.reverseOnEnd = reverseOnEnd;

        frameIndex = 0;
        isReverseAnimating = false;

        UpdateSourceRectangle();
    }

    // Methods
    public void NextFrame() {
        int lastFrame = FrameCount - 1;

        if(frameIndex == lastFrame && !isReverseAnimating) {
            if(ReverseOnEnd) {
                // Begin Reverse Animation
                isReverseAnimating = true;
                frameIndex--;
            }
            else {
                frameIndex = 0;
            }
        }
        else if (frameIndex == 0 && isReverseAnimating) {
            // Begin Forward Animation Again if done reversing
            isReverseAnimating = false;
            frameIndex++;
        }
        else {
            if(isReverseAnimating) {
                frameIndex--;
            }
            else{
                frameIndex++;
            }
        }

        UpdateSourceRectangle();
    }

    private void UpdateSourceRectangle() {
        int posX = (int)framePosition.X + ((int)frameSize.X * frameIndex);
        int posY = (int)framePosition.Y;
        int width = (int)frameSize.X;
        int height = (int)frameSize.Y;

        SourceRectangle = new Rectangle(posX, posY, width, height);
    }
}
using VaniaPlatformer;
using VaniaPlatformer.ECS;
using Microsoft.Xna.Framework;
using System.Linq;

public class AnimationSystem : BaseSystem<AnimationComponent> 
{
    // Methods
    public static new void Update() 
    {
        foreach(AnimationComponent c in components)
        {
            if(!c.Paused)
            {
                if(c.FrameTimer > 0) {
                    c.FrameTimer -= Globals.DeltaTime;
                }
                else {
                    switch(c.Animation.LoopType) {
                        case Animation.Loop.FromBeginning:
                        {
                            if(c.FrameIndex == c.FinalFrameIndex) {
                                c.FrameIndex = 0;
                                c.FrameTimer = c.Animation.FrameTime;
                            }
                            else {
                                c.FrameIndex++;
                                c.FrameTimer = c.Animation.FrameTime;
                            }
                            break;
                        }
                        case Animation.Loop.Reverse:
                        {
                            if(c.FrameIndex == c.FinalFrameIndex && !c.IsReverseAnimating) {
                                c.IsReverseAnimating = !c.IsReverseAnimating;
                                c.FrameIndex--;
                                c.FrameTimer = c.Animation.FrameTime;
                            }
                            else if(c.FrameIndex == 0 && c.IsReverseAnimating) {
                                c.IsReverseAnimating = !c.IsReverseAnimating;
                                c.FrameIndex++;
                                c.FrameTimer = c.Animation.FrameTime;
                            }
                            else {
                                if(c.IsReverseAnimating) {
                                    c.FrameIndex--;
                                }
                                else {
                                    c.FrameIndex++;
                                }

                                c.FrameTimer = c.Animation.FrameTime;
                            }
                            break;
                        }
                    }

                    c.Entity.GetComponent<SpriteComponent>().SourceRectangle = new Rectangle(
                        (int)(c.FirstFramePosition.X + (c.FirstFrameSize.X * c.FrameIndex)),
                        (int)c.FirstFramePosition.Y,
                        (int)c.FirstFrameSize.X,
                        (int)c.FirstFrameSize.Y);

                }
            }
        }
    }

    public static void Play(Entity entity, string animationName)
    {
        var animationComponent = entity.GetComponent<AnimationComponent>();

        if(animationComponent != null && animationComponent.Animation.Name != animationName) {}
        {
            // Swap out Current Animation with New One
            if(animationComponent.Animation.Name != animationName)
            {
                animationComponent.Animation = animationComponent.Animations.Where(x => x.Name == animationName).Select(x => x).FirstOrDefault();

                animationComponent.FrameIndex = 0;
                animationComponent.FinalFrameIndex = animationComponent.Animation.Frames - 1;
                animationComponent.FrameTimer = animationComponent.Animation.FrameTime;
                animationComponent.FirstFramePosition = new Vector2(animationComponent.Animation.FirstFrame.X, animationComponent.Animation.FirstFrame.Y);
                animationComponent.FirstFrameSize = new Vector2(animationComponent.Animation.FirstFrame.Width, animationComponent.Animation.FirstFrame.Height);

                entity.GetComponent<SpriteComponent>().SourceRectangle = new Rectangle(
                        (int)(animationComponent.FirstFramePosition.X + (animationComponent.FirstFrameSize.X * animationComponent.FrameIndex)),
                        (int)animationComponent.FirstFramePosition.Y,
                        (int)animationComponent.FirstFrameSize.X,
                        (int)animationComponent.FirstFrameSize.Y);

                entity.GetComponent<SpriteComponent>().Origin = new Vector2(
                    animationComponent.Animation.FirstFrame.Width / 2,
                    animationComponent.Animation.FirstFrame.Height / 2
                );

                animationComponent.Paused = false;
            }
        }
    }
}
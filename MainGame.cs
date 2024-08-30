using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.Animations;

namespace VaniaPlatformer;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private AnimationManager _animManager = new AnimationManager();
    private Rectangle _testRect = new Rectangle(100, 100, 48, 48);

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        Texture2D charSheet = this.Content.Load<Texture2D>("textures/Tralia");
        Animation animation = new Animation(Common.AnimationIndex.Idle, charSheet, new Vector2(0,0), new Vector2(48,48), 3, 0.35f, true);
        _animManager.AddAnimation(animation.Index, animation);
        _animManager.Play(Common.AnimationIndex.Idle);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _animManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _animManager.Draw(_spriteBatch, _testRect, Color.White);
        base.Draw(gameTime);

        _spriteBatch.End();
    }
}

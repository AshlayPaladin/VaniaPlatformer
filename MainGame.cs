using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VaniaPlatformer;

public class MainGame : Game
{

    // Properties
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _testPlayer;
    private List<SolidActor> solidActors = new List<SolidActor>();

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        Globals.InitializeGlobals(Content);

        _testPlayer = new Player(50, 37, 500, 150);

        solidActors.Add( new SolidActor(new Vector2(0, 400), 1280, 32) );
        solidActors.Add( new SolidActor(new Vector2(0, 0), 32, 720) );
        solidActors.Add( new SolidActor(new Vector2(1280 - 32, 0), 32, 720) );

        foreach(var solidActor in solidActors) {
            _testPlayer.IsMoving += solidActor.OnPlayerMoved;
        }

        

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

    }

    protected override void Update(GameTime gameTime)
    {
        Globals.Update(gameTime);

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _testPlayer.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _testPlayer.Draw(_spriteBatch);

        foreach(var solid in solidActors) {
            solid.Draw(_spriteBatch);
        }

        base.Draw(gameTime);

        _spriteBatch.End();
    }
}

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.Tilemaps;

namespace VaniaPlatformer;

public class MainGame : Game
{

    // Properties
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _testPlayer;
    private List<SolidActor> solidActors = new List<SolidActor>();

    private Tileset _testTiles;
    private Tilemap _testTilemap;
    private int[][] _tilemap;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 320;
        _graphics.PreferredBackBufferHeight = 320;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        Globals.InitializeGlobals(Content);

        //_testPlayer = new Player(50, 37, 500, 150);

        //solidActors.Add( new SolidActor(new Vector2(0, 400), 1280, 32) );
        //solidActors.Add( new SolidActor(new Vector2(0, 0), 32, 720) );
        //solidActors.Add( new SolidActor(new Vector2(1280 - 32, 0), 32, 720) );

        

        _testTiles = new Tileset(Content.Load<Texture2D>("textures/debugTileset"), 32);
        _tilemap = new int[5][];

        _tilemap[0] = new int[] {1, 1, 2, 5, 3};
        _tilemap[1] = new int[] {4, 4, 4, 4, 4};
        _tilemap[2] = new int[] {10, 11, 8, 9, 10};
        _tilemap[3] = new int[] {16, 0, 0, 0, 5};
        _tilemap[4] = new int[] {0, 1, 14, 11, 0};

        List<ObjectTile> objectTiles = new List<ObjectTile>() {
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 0), new Vector2(320, 32), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 0), new Vector2(32, 320), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(288, 0), new Vector2(32, 320), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 288), new Vector2(320, 32), new List<string>()),
            new ObjectTile(ObjectTile.TileType.PlayerStart, new Vector2(100, 100), new Vector2(32, 64), new List<string>())
        };

        _testTilemap = new Tilemap(_testTiles, 5, 5, 32, _tilemap, objectTiles);

        foreach(var o in _testTilemap.ObjectTiles) {
            if(o.Type == ObjectTile.TileType.PlayerStart) {
                _testPlayer = new Player(50, 37, (int)o.TilePosition.X, (int)o.TilePosition.Y);
            }
            else if (o.Type == ObjectTile.TileType.SolidActor) {
                SolidActor newSolid = new SolidActor(o.TilePosition, (int)o.TileSize.X, (int)o.TileSize.Y);
                solidActors.Add(newSolid);
            }
        }

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

        _testTilemap.RenderMap(_spriteBatch);

        base.Draw(gameTime);

        _spriteBatch.End();
    }
}

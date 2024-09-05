﻿using System.Collections.Generic;
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
    private int[][] _tilemap2;
    private List<int[][]> _testTilemaps = new List<int[][]>();
    private bool[] debugEnabled;

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

        debugEnabled = new bool[] 
        {
            false    // [0] show SolidActor collision masks
        }; 
        
        _testTiles = new Tileset(Content.Load<Texture2D>("textures/TestTiles"), 32);
        
        _tilemap = new int[10][];
        _tilemap[0] = new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        _tilemap[1] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[2] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[3] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[4] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[5] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[6] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[7] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[8] = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 1};
        _tilemap[9] = new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

        _tilemap2 = new int[10][];
        _tilemap2[0] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[1] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[2] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[3] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[4] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[5] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[6] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[7] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[8] = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        _tilemap2[9] = new int[] {0, 2, 2, 2, 2, 2, 2, 2, 2, 0};

        _testTilemaps.Add(_tilemap);
        _testTilemaps.Add(_tilemap2);

        List<ObjectTile> objectTiles = new List<ObjectTile>() {
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 0), new Vector2(320, 32), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 0), new Vector2(32, 320), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(288, 0), new Vector2(32, 320), new List<string>()),
            new ObjectTile(ObjectTile.TileType.SolidActor, new Vector2(0, 288), new Vector2(320, 32), new List<string>()),
            new ObjectTile(ObjectTile.TileType.PlayerStart, new Vector2(100, 100), new Vector2(32, 64), new List<string>())
        };

        _testTilemap = new Tilemap(_testTiles, 10, 10, 32, _testTilemaps, objectTiles);

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

        _testTilemap.RenderMap(_spriteBatch);

        _testPlayer.Draw(_spriteBatch);

        if(debugEnabled[0]) {
            foreach(var solid in solidActors) {
                solid.Draw(_spriteBatch);
            }
        }

        base.Draw(gameTime);

        _spriteBatch.End();
    }
}

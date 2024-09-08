using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMapLib;
using Newtonsoft.Json;
using System.IO;

namespace VaniaPlatformer;

public class MainGame : Game
{

    // Properties
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _testPlayer;
    private List<SolidActor> solidActors = new List<SolidActor>();

    private TiledMap _testTilemap;
    private Camera2D _camera;

    // Debug Fields
    private bool[] debugEnabled;
    private bool tildePressed;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        Globals.InitializeGlobals(Content);

        debugEnabled = new bool[] 
        {
            false    // [0] show SolidActor collision masks
        }; 

        tildePressed = false;

        // TODO: Initialize our Test TiledMap here
        string tiledMapJson = File.ReadAllText("../../../Content/tiledMaps/debugStage.json");
        _testTilemap = JsonConvert.DeserializeObject<TiledMap>(tiledMapJson);

        _graphics.PreferredBackBufferWidth = _testTilemap.Width * _testTilemap.TileWidth;
        _graphics.PreferredBackBufferHeight = _testTilemap.Height * _testTilemap.TileWidth;
        _graphics.ApplyChanges();

        Globals.SetActiveTileset(_testTilemap.TiledMapTilesets[0]);

        if(_testTilemap.TiledMapObjectLayers.Count > 0) {
            foreach(var layer in _testTilemap.TiledMapObjectLayers) {
                if(layer.Name == "SolidsLayer") 
                {
                    foreach(var o in layer.TiledObjects) 
                    {
                        SolidActor newSolidActor = new SolidActor(new Vector2(o.X, o.Y), o.Width, o.Height);
                        solidActors.Add(newSolidActor);
                    }

                    continue;
                }

                if(layer.Name == "ActorLayer") 
                {
                    foreach(var o in layer.TiledObjects) 
                    {
                        switch(o.Type)
                        {
                            case "PlayerSpawn" : 
                            {
                                _testPlayer = new Player(o.Width, o.Height, o.X, o.Y);
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }

                    continue;
                }
            }
        }

        if(solidActors.Count > 0) {
            foreach(var solidActor in solidActors) {
                _testPlayer.IsMoving += solidActor.OnPlayerMoved;
            }
        }
        
        _camera = new Camera2D(_testPlayer, new Vector2(_testTilemap.Width * _testTilemap.TileWidth, _testTilemap.Height * _testTilemap.TileWidth), Vector2.Zero);
        _testPlayer.FinishedMoving += _camera.OnTargetActorFinishedMoving;

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

        if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) && 
        Keyboard.GetState().IsKeyDown(Keys.LeftShift) && 
        Keyboard.GetState().IsKeyDown(Keys.OemTilde) &&
        !tildePressed) {
            debugEnabled[0] = !debugEnabled[0];
            tildePressed = true;
        }

        if(Keyboard.GetState().IsKeyUp(Keys.OemTilde) && tildePressed) {
            tildePressed = false;
        }

        _testPlayer.Update();
        _camera.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(transformMatrix: _camera.TranslationMatrix);

        _testTilemap.RenderMap(_spriteBatch);
        _testPlayer.Draw(_spriteBatch);

        if(debugEnabled[0] && solidActors.Count > 0) {
            foreach(var solid in solidActors) {
                solid.Draw(_spriteBatch);
            }
        }

        _camera.Draw(_spriteBatch);

        base.Draw(gameTime);

        _spriteBatch.End();
    }
}

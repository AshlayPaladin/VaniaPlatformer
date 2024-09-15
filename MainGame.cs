using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMapLib;
using Newtonsoft.Json;
using System.IO;
using System;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class MainGame : Game
{

    // Properties
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PlayerEntity _testPlayer;
    private List<Entity> entities = new List<Entity>();
    private List<GameActorEntity> actorEntities = new List<GameActorEntity>();

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
        Globals.GraphicsScreenSize = new Vector2(640, 320);

        debugEnabled = new bool[] 
        {
            false    // [0] show SolidActor collision masks
        }; 

        tildePressed = false;

        // TODO: Initialize our Test TiledMap here
        string tiledMapJson = File.ReadAllText("../../../Content/tiledMaps/debugStage2.json");
        _testTilemap = JsonConvert.DeserializeObject<TiledMap>(tiledMapJson);

        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        Globals.SetActiveTileset(_testTilemap.TiledMapTilesets[0]);

        if(_testTilemap.TiledMapObjectLayers.Count > 0) {
            foreach(var layer in _testTilemap.TiledMapObjectLayers) {
                if(layer.Name == "SolidsLayer") 
                {
                    foreach(var o in layer.TiledObjects) 
                    {
                        switch(o.Type)
                        {
                            case "OneWaySolidActor" :
                            {
                                entities.Add(new OneWaySolidEntity(new Vector2(o.X, o.Y), o.Width, o.Height));
                                break;
                            }
                            default:
                            {
                                entities.Add(new SolidEntity(new Vector2(o.X, o.Y), o.Width, o.Height));
                                break;
                            }
                        }
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
                                _testPlayer = new PlayerEntity(32, 48, o.X, o.Y, "textures/AdventurerSheet");
                                _testPlayer.PlayerKilled += OnPlayerDeath;
                                break;
                            }
                            case "ActorSpawn" :
                            {
                                EnemyEntity enemy = new EnemyEntity(o.Width, o.Height, o.X, o.Y, "textures/TestEnemy");
                                enemy.Killed += OnEntityDeath;
                                actorEntities.Add(enemy);
                                break;
                            }
                            case "MovingPlatform" :
                            {
                                actorEntities.Add(new MovingPlatformEntity(o.Width, o.Height, o.X, o.Y, "textures/MovingPlatform"));
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
        
        _camera = new Camera2D(_testPlayer, _testTilemap.Width * _testTilemap.TileWidth, _testTilemap.Height * _testTilemap.TileHeight);
        _camera.SetViewportSize(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);

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
        
        InputSystem.Update();
        TransformSystem.Update();
        MoveSystem.Update();
        ColliderSystem.Update();
        AnimationSystem.Update();
        HealthSystem.Update();

        _testPlayer.Update();
        _camera.Update();

        foreach(var entity in actorEntities) 
        {
            entity.Update();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(transformMatrix: _camera.TranslationMatrix);

        _testTilemap.RenderMap(_spriteBatch);
        _testPlayer.Draw(_spriteBatch);

        foreach(var actorEntity in actorEntities)
        {
            if(actorEntity.GetType() == typeof(EnemyEntity))
            {
                EnemyEntity enemy = actorEntity as EnemyEntity;
                enemy.Draw(_spriteBatch);
            }
            
        }

        if(debugEnabled[0] && entities.Count > 0) {
            foreach(var solid in entities) {

                if(solid.GetType() == typeof(SolidEntity)){
                    SolidEntity solidEntity = solid as SolidEntity;
                    solidEntity.Draw(_spriteBatch);
                }
                
            }
        }

        //_camera.Draw(_spriteBatch);
        DrawSystem.Draw(_spriteBatch);

        base.Draw(gameTime);

        _spriteBatch.End();
    }


    protected void OnPlayerDeath(object sender, EventArgs args)
    {
        Exit();
    }

    protected void OnEntityDeath(object sender, EventArgs args)
    {
        GameActorEntity entity = sender as GameActorEntity;

        for(int i = actorEntities.Count - 1; i >= 0; i--)
        {
            var actor = actorEntities[i];
            if(actor.ID == entity.ID)
            {
                actorEntities.RemoveAt(i);
                break;
            }
        }
    }
}

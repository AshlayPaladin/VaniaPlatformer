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

    // Constants
    private const int VIRTUAL_WINDOW_WIDTH = 640;
    private const int VIRTUAL_WINDOW_HEIGHT = 320;  

    // Fields
    private Rectangle viewport;

    // Properties
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PlayerEntity _testPlayer;
    private List<Entity> entities = new List<Entity>();
    private List<GameActorEntity> actorEntities = new List<GameActorEntity>();

    private TiledMap _testTilemap;
    private Camera2D _camera;

    private RenderTarget2D renderTarget2D;

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
        Globals.WindowSizeChanged += OnWindowSizeChanged;

        debugEnabled = new bool[] 
        {
            false    // [0] show SolidActor collision masks
        }; 

        tildePressed = false;

        // TODO: Initialize our Test TiledMap here
        string tiledMapJson = File.ReadAllText("../../../Content/tiledMaps/debugStage2.json");
        _testTilemap = JsonConvert.DeserializeObject<TiledMap>(tiledMapJson);

        _graphics.PreferredBackBufferWidth = (int)Globals.WindowSize.X;
        _graphics.PreferredBackBufferHeight = (int)Globals.WindowSize.Y;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.ApplyChanges();

        renderTarget2D = new RenderTarget2D(
            GraphicsDevice,
            VIRTUAL_WINDOW_WIDTH,
            VIRTUAL_WINDOW_HEIGHT,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        CalculateViewport();

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
        _camera.SetViewportSize((int)VIRTUAL_WINDOW_WIDTH, (int)VIRTUAL_WINDOW_HEIGHT);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        Globals.Update(gameTime);

        if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) && 
        Keyboard.GetState().IsKeyDown(Keys.LeftShift) && 
        Keyboard.GetState().IsKeyDown(Keys.OemTilde) &&
        !tildePressed) {
            //debugEnabled[0] = !debugEnabled[0];
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            tildePressed = true;
        }

        if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) && 
        Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && 
        Keyboard.GetState().IsKeyDown(Keys.OemTilde) &&
        !tildePressed) {
            Globals.DebugNextResolution();
            tildePressed = true;
        }

        if(Keyboard.GetState().IsKeyUp(Keys.OemTilde) && tildePressed) {
            tildePressed = false;
        }
        
        InputSystem.Update();
        TransformSystem.Update();
        PhysicsSystem.Update();
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
        DrawSceneToTexture(renderTarget2D);

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _spriteBatch.Draw(renderTarget2D, new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height), Color.White);

        base.Draw(gameTime);

        _spriteBatch.End();
    }

    private void DrawSceneToTexture(RenderTarget2D renderTarget)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);
        //GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

        _spriteBatch.Begin(samplerState:SamplerState.PointClamp, transformMatrix: _camera.TranslationMatrix);

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
        
        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
    }

    protected void CalculateViewport()
    {
        float scaleX = (float)Globals.WindowSize.X / (float)VIRTUAL_WINDOW_WIDTH;
        float scaleY = (float)Globals.WindowSize.Y / (float)VIRTUAL_WINDOW_HEIGHT;

        float viewportScale = scaleX < scaleY ? scaleX : scaleY;

        int viewportWidth = (int)(VIRTUAL_WINDOW_WIDTH * viewportScale);
        int viewportHeight = (int)(VIRTUAL_WINDOW_HEIGHT * viewportScale);

        int offsetX = (int)Globals.WindowSize.X / 2 - viewportWidth / 2;
        int offsetY = (int)Globals.WindowSize.Y / 2 - viewportHeight / 2;

        viewport = new Rectangle(offsetX, offsetY, viewportWidth, viewportHeight);
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

    protected void OnWindowSizeChanged(object sender, EventArgs args)
    {
        _graphics.PreferredBackBufferWidth = (int)Globals.WindowSize.X;
        _graphics.PreferredBackBufferHeight = (int)Globals.WindowSize.Y;
        _graphics.ApplyChanges();

        CalculateViewport();
    }
}

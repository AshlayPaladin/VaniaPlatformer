using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.Tilemaps;

public class ObjectTile {
    
    // Enums
    public enum TileType {
        ActorStart,
        PlayerStart,
        SolidActor
    }

    // Fields
    private List<string> customProperies;

    // Properties
    public Vector2 TilePosition { get; private set; }
    public Vector2 TileSize { get; private set; }
    public TileType Type { get; private set; }

    // Constrcutor
    public ObjectTile(TileType type, Vector2 tilePosition, Vector2 tileSize, List<string> customProperties) {
        this.Type = type;
        this.TilePosition = tilePosition;
        this.TileSize = tileSize;
        this.customProperies = new List<string>(customProperties);
    }

    private void InstantiateObjects() {
        
        switch(Type) {
            case TileType.ActorStart: {
                /*
                    customProperties[0] should give ActorName (e.g. "Goomba" for GoombaEnemyActor)
                    Then, using TilePosition, create a new Actor of the correct type at TilePosition
                */
                break;
            }
            case TileType.PlayerStart: {
                Player newPlayer = new Player(50, 37, (int)TilePosition.X, (int)TilePosition.Y);
                break;
            }
            case TileType.SolidActor: {
                SolidActor newSolid = new SolidActor(TilePosition, (int)TileSize.X, (int)TileSize.Y);
                break;
            }
        }
    }
}
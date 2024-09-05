using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public class MoveEventArgs : EventArgs {
    public Vector2 ProposedPosition;
    public Vector2 Velocity;
    public Rectangle Boundingbox;

    public MoveEventArgs(Vector2 pos, Vector2 velocity, Rectangle boundingbox) {
        ProposedPosition = pos;
        Velocity = velocity;
        Boundingbox = boundingbox;
    }
}
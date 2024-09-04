using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer;

public class MoveEventArgs : EventArgs {
    public Vector2 ProposedPosition;
    public Rectangle Boundingbox;

    public MoveEventArgs(Vector2 pos, Rectangle boundingbox) {
        ProposedPosition = pos;
        Boundingbox = boundingbox;
    }
}
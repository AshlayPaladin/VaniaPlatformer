using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace VaniaPlatformer.ECS;

public class InputComponent : Component 
{
    // Properties
    public bool IsLeftKeyDown { get; private set; }
    public bool IsRightKeyDown { get; private set;}
    public bool IsJumpKeyDown { get; private set; }
    public bool IsRunKeyDown { get; private set; }

    // Constructor
    public InputComponent(params Keys[] keys)
    {
        InputSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        KeyboardState keyboardState = Keyboard.GetState();

        IsLeftKeyDown = keyboardState.IsKeyDown(Globals.LeftKey);
        IsRightKeyDown = keyboardState.IsKeyDown(Globals.RightKey);
        IsJumpKeyDown = keyboardState.IsKeyDown(Globals.JumpKey);
        IsRunKeyDown = keyboardState.IsKeyDown(Globals.RunKey);
    }
}
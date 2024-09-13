using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;


namespace VaniaPlatformer;

public static class InputManager {

    // Properties
    public static InputKey Left { get; private set; }
    public static InputKey Right { get; private set; }
    public static InputKey Up { get; private set; }
    public static InputKey Down { get; private set; }
    public static InputKey Run { get; private set; }
    public static InputKey Jump { get; private set; }
    public static InputKey Accept { get; private set; }
    public static InputKey Cancel { get; private set; }

    // Collections
    public static List<InputKey> AllKeys { get; private set; }

    public static void InitializeDefaults() {
        Left = new InputKey(Keys.A);
        Right = new InputKey(Keys.D);
        Up = new InputKey(Keys.W);
        Down = new InputKey(Keys.S);
        Run = new InputKey(Keys.LeftShift);
        Jump = new InputKey(Keys.Space);
        Accept = new InputKey(Keys.Space);
        Cancel = new InputKey(Keys.Escape);

        AllKeys = new List<InputKey> 
        {
            Left,
            Right,
            Up,
            Down,
            Run,
            Jump,
            Accept,
            Cancel
        };
    }

    public static void Update() {
        KeyboardState keyboardState = Keyboard.GetState();

        foreach(InputKey key in AllKeys) {

            int keyIndex = AllKeys.IndexOf(key);

            if(keyboardState.IsKeyDown(key.Key)) {
                if(key.State == InputKey.KeyState.Pressed) {
                    // Key has already flagged its Pressed event
                    AllKeys[keyIndex].ChangeState(InputKey.KeyState.Held);
                }
                else {
                    // Key was JUST pressed
                    AllKeys[keyIndex].ChangeState(InputKey.KeyState.Pressed);
                }
            }
            else if (keyboardState.IsKeyUp(key.Key)) {

                if(key.State == InputKey.KeyState.Held || key.State == InputKey.KeyState.Pressed) {
                    // Key was JUST released
                    AllKeys[keyIndex].ChangeState(InputKey.KeyState.Released);
                }
                else {
                    AllKeys[keyIndex].ChangeState(InputKey.KeyState.None);
                }
            }
        }
    }
}
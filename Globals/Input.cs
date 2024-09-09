using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace VaniaPlatformer;

public class Input {

    // Enum
    public enum KeyState {
        None,
        Pressed,
        Held,
        Released
    }

    // Properties
    public Keys Left;
    public Keys Right;
    public Keys Up;
    public Keys Down;
    public Keys Run;
    public Keys Jump;
    public Keys Accept;
    public Keys Cancel;

    // Collections
    public Dictionary<Keys, KeyState> KeyStates;

    public Input() {
        Left = Keys.Left;
        Right = Keys.Right;
        Up = Keys.Up;
        Down = Keys.Down;
        Run = Keys.LeftShift;
        Jump = Keys.Space;
        Accept = Keys.Enter;
        Cancel = Keys.Escape;

        KeyStates = new Dictionary<Keys, KeyState>
        {
            { Left, KeyState.None },
            { Right, KeyState.None },
            { Up, KeyState.None },
            { Down, KeyState.None },
            { Run, KeyState.None },
            { Jump, KeyState.None },
            { Accept, KeyState.None },
            { Cancel, KeyState.None }
        };
    }

    public void Update() {
        KeyboardState keyboardState = Keyboard.GetState();

        foreach(KeyValuePair<Keys, KeyState> entry in KeyStates) {
            Keys key = entry.Key;

            if(keyboardState.IsKeyDown(key)) {
                if(entry.Value == KeyState.Pressed) {
                    // Key has already flagged its Pressed event
                    KeyStates[key] = KeyState.Held;
                }
                else {
                    // Key was JUST pressed
                    KeyStates[key] = KeyState.Pressed;
                }
            }
            else if (keyboardState.IsKeyUp(key)) {

                if(entry.Value == KeyState.Held || entry.Value == KeyState.Pressed) {
                    // Key was JUST released
                    KeyStates[key] = KeyState.Released;
                }
                else {
                    KeyStates[key] = KeyState.None;
                }
            }
        }
    }
}
using System;
using Microsoft.Xna.Framework.Input;

namespace VaniaPlatformer;

public class InputKey {

    // Events
    public event EventHandler KeyPressed;
    public event EventHandler KeyReleased;
    public event EventHandler KeyHeld;

    // Enum
    public enum KeyState 
    {
        None,
        Pressed,
        Held,
        Released
    }

    // Properties
    public Keys Key { get; private set; }
    public KeyState State { get; private set; }

    // Constructor
    public InputKey(Keys key) {
        this.Key = key;
        this.State = KeyState.None;
    }

    // Methods
    public void ChangeState(KeyState state) {
        this.State = state;

        if(this.State == KeyState.Pressed) {
            OnKeyPressed();
        }
        else if(this.State == KeyState.Released) {
            OnKeyReleased();
        }
        else if(this.State == KeyState.Held) {

        }
    }

    public void OnKeyPressed() {
        KeyPressed?.Invoke(this, null);
    }

    public void OnKeyReleased() {
        KeyReleased?.Invoke(this, null);
    }

    public void OnKeyHeld() {
        KeyHeld?.Invoke(this, null);
    }
}
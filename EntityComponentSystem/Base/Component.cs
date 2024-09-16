
using System;

namespace VaniaPlatformer.ECS;

public class Component {

    // Events
    public event EventHandler ComponentDestroyed;

    // Properties
    public Entity Entity;
    public bool Enabled = true;
    
    // Methods
    public virtual void Update() { }

    public virtual void Enable() 
    {
        Enabled = true;
    }

    public virtual void Disable() 
    {
        Enabled = false;
    }

    public virtual void Destroy()
    {
        ComponentDestroyed?.Invoke(this, EventArgs.Empty);
    }
}

using System;
using System.Collections.Generic;

namespace VaniaPlatformer.ECS;

public class Entity 
{
    // Properties
    public Guid ID { get; private set; }
    public List<Component> Components { get; private set; }

    // Constructor
    public Entity() 
    {
        this.ID = Guid.NewGuid();
        this.Components = new List<Component>();
    }

    // Methods
    public void AddComponent(Component component) 
    {
        component.Entity = this;
        Components.Add(component);
    }

    public T GetComponent<T>() where T : Component
    {
        foreach(Component component in Components) {
            if(component.GetType() == typeof(T))
            {
                return (T)component;
            }
        }

        return null;
    }

    public virtual void Destroy()
    {
        foreach(var component in Components) 
        {
            component.Destroy();
        }
    }
}
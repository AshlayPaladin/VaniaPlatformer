
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace VaniaPlatformer.ECS;

public class BaseSystem<T> where T : Component {

    // Fields
    protected static List<T> components = new List<T>();

    // Methods
    public static void Register(T component) 
    {
        components.Add(component);
        component.ComponentDestroyed += OnComponentDestroyed;
    }

    public static void Update()
    {
        for(int i = components.Count-1; i >= 0; i--)
        {
            if(components[i].Enabled)
            {
                components[i].Update();
            }
        }
    }

    public static void Deregister(T component)
    {
        components.Remove(component);
    }

    protected static void OnComponentDestroyed(object sender, EventArgs e)
    {
        T component = (T)sender;
        Deregister(component);
        component = null;
    }
}
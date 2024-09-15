
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
    }

    public static void Update()
    {
        foreach(T component in components)
        {
            component.Update();
        }
    }

    public static void Deregister(T component)
    {
        components.Remove(component);
    }
}
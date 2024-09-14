
using System;

namespace VaniaPlatformer;

public class ComponentDependencyNotConfiguredException : Exception 
{
    public ComponentDependencyNotConfiguredException() { }

    public ComponentDependencyNotConfiguredException(string message) : base(message) { }

    public ComponentDependencyNotConfiguredException(string message, Exception innerException) : base(message, innerException) { }
}
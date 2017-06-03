# Extend Jobbr
Jobbr comes with a comprehensive plugin system, based on the `JobbrBuilder`. It's used by all the plugins available on https://github.com/JobbrIO and can also be leveraged to implement custom additions to the server.

## Own Components
The simples way to create your own component is to immplement the `IJobbrComponent` interface and register the type to the `JobbrBuilder`. The interface is available in [Jobbr.ComponentModel.Registration](https://github.com/jobbrIO/jobbr-cm-registration). You don't need a dependency to Jobbr and thus should only have dependencies to Component Models.

**Sample component**

```c#

public class DemoComponent : IJobbrComponent
{
    public void Dispose()
    {
    }

    public void Start()
    {
    }

    public void Stop()
    {
    }
}

```

### Registration ###
To register the component, you'll need to register the type to the Jobbr internal DI, which is available through the JobbrBuilder.

```c#
// Add the type to the DI
builder.Register<IJobbrComponent>(typeof(DemoComponent));
```

Usually this code is wrapped into an extension method for the IJobbrBuilder, like in the following sample.

**DemoComponentExtension.cs**

```c#

public static class DemoComponentExtension {

    public static void AddDemoComponent(this IJobbrBuilder builder) {
        
        // Add the type to the DI
        builder.Register<IJobbrComponent>(typeof(DemoComponent));
    }
}

```

This allows to easily add your component to any Jobbr that offers a `IJobbrBuilder`.

**Usage**

```c#

var builder = new JobbrBuilder();

builder.AddDemoComponent();

```

### Dependencies


### Settings


### Validators


### Lifetime


## Specific Replacements
Jobbr defines a couple of interfaces that belong to the core functionality. These interfaces are specified in the corresponding component model repositories and packages.


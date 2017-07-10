# Extend Jobbr
Jobbr comes with a comprehensive plugin system, based on the `JobbrBuilder`. It's used by all the plugins available on https://github.com/JobbrIO and can also be leveraged to implement custom additions to the server.

## Additional Custom Functionality
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
var server = builder.Create();
```
> **Note**: Registered types and instances are registered in a **Singleton-Scope**. Calling `builder.Create()` multiple times can lead into unexpected behavior.

#### Lifetime
There a three important as state in the interface `IJobbrComponent`.

* **Activation**: The component is created when `builder.Create()` is called
* **Start()**: Called when the Jobbr Server is started
* **Stop()**: Called when the Jobbr Server gets started
* **Dispose()**: Called when the instance of JobbrServer gets Disposed

### Dependencies
If you like to base on existing services from component implementations or own dependencies you need to specify those in the constructor of your component. 

**Sample**
```c#
public class DemoComponent : IJobbrComponent
{
    public void DemoComponent(IQueryService queryService) {
        this.queryService = queryService;
    }

    public void Start() {
        Console.WriteLine($"There are {this.queryService.GetJobRuns().Count} jobruns in the system");
    }

    /* ... */
}
```

Alltough is possible to inject any available type, (also internal classes) you should only use those services that are defined in Component Model Packages.

### Configuration
In most cases you'll need to register a settings object to jobbr while setting up and retrieving it in your component. For a storage implementation this might be a configuration how to connect to the database.

You'll need a settings class like the one below

**DemoComponentConfiguration.cs**
```c#
public class DemoComponentConfiguration {

    public string DatabaseConnection { get; set; }
}
```

An instance of this class needs to be registed to the builder and the DemoComponent's Constructor needs to be adjusted so that this specific instance is passed to the constructor.

**Registering an instance**
```c#
var configuration = new DemoComponentConfiguration()
{
    DatabaseConnection = "foo"
};

builder.Add<DemoComponentConfiguration>(configuration));
```

This is usually done by providing a fluent syntax in the IJobbrBuilder extension from above.

**DemoComponentExtension.cs**

```c#
public static class DemoComponentExtension {

    public static void AddDemoComponent(this IJobbrBuilder builder, Action<ForkedExecutionConfiguration> config) {
        
        // Create a new instance of default configuration and pass it to the caller
        var configuration = new DemoComponentConfiguration()
        {
            DatabaseConnection "foo"
        };

        config?.Invoke(configuration);

        // Register config instance to DI
        builder.Add<DemoComponentConfiguration>(configuration));

        // Add the type to the DI
        builder.Register<IJobbrComponent>(typeof(DemoComponent));
    }
}
```

The consumer of the extension then gets a really easy interface to configure your component.

**Usage Sample**
```c#
var builder = new JobbrBuilder();

builder.AddDemoComponent(c => {
    c.DatabaseConnection = "something different";
});
```

#### Configuration Validators
You might wan't to validate the configuration that is passed to your Component before the component is actually activated. This can be done by registering Validators for a given configuration type.

The interface `IConfugurationValidator` from `Jobbr.ComponentModel.Registration`.

```c#
public interface IConfigurationValidator
{
    Type ConfigurationType { get; set; }

    bool Validate(object configuration);
}
``` 

**Registration** for the imaginary implementation named `ConfigurationValiator`.
```c#
// As type
builder.Register<IConfigurationValidator>(typeof(ConfigurationValidator));

// Or as instance
builder.Add<IConfigurationValidator>(new ConfigurationValidato()));

```
Your validator is called for each instance of the specified `ConfigurationType`. It's expected that you either return false in the validation process or throw an exception.

If one validator fails (or returns false), the whole startup process is stopped and the server is in an unrecoverable Error-State.


## Replace Core Functionality
Jobbr defines a couple of interfaces that belong to the core functionality. These interfaces are specified in the corresponding component model repositories and packages.

* **Execution**: Contract between an executor and the Jobbr-Server, fullfilled by both the server and a component.
* **JobStorage**: Defines interfaces for storage access
* **ArtefactStore**: Defines interfaces to store and retrieve job related artefacts

These packages are build upon the component infrastructure introduced above. The only different is that you'll need to implement specific interfaces from these packages instead of the generic `IJobbrComponent`.

There are additional component models that define functionality that is available to all components provided by the server implementation
* **Registration**: Boostrapping services, Registration, Validation, Components
* **Management**: Manage the server, Query Jobs, etc.

Plase also see [all Component Models on GitHub](https://github.com/jobbrIO?q=cm)
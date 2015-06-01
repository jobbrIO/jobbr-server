# Jobbr
Jobbr is a .NET JobServer. Unless other JobServer-Frameworks Jobbr explicitly solves the following problems
* Isolation of Jobs on process-level (JobRunner)
* Restful API to manage and trigger Jobs and watch the execution state
* Artefacts-store for both job parameters and job results (RestApi)
* Embeddable in your own C# application (JobServer and Runner)
* DI-Resolution in Runner for your Jobs
* Progress tracking via stdout (`Console.WriteLine()`)
* **NO** Dependency to the Jobbr-Assemblies needed from your Jobs
* **NO** Dependency to any existing Logging-Framework

# QuickStart
There is a demo-solution with a ready to run application.

## Installation
Simply create a database (or use any existing) by executing the CreateSchemaAndTables.sql-File, located in source\Jobbr.Server.Dapper. The JobStorageProvider is Dapper-based and has been tested against SQL Server 2012.

## Hosting a JobbrServer
To host a JobbrServer simply define a Storage Provider for Jobs and JobArtefacts and initialize the JobServer:

```c#
var jobStorageProvider = new DapperStorageProvider(@"YourConnectionString");
var artefactStorageProvider = new FileSystemArtefactsStorageProvider("C:\jobdata");

var config = new DefaultJobbrConfiguration
{
    JobStorageProvider = jobStorageProvider,
    ArtefactStorageProvider = artefactStorageProvider,
    JobRunnerExeResolver = () => @"..\..\..\Demo.JobRunner\bin\Debug\Demo.JobRunner.exe",
    IsRuntimeWaitingForDebugger = true, // The runtime waits 10s for an debugger
};

using (var jobbrServer = new JobbrServer(config))
{
    jobbrServer.Start();

    Console.WriteLine("JobServer has started . Press enter to quit")
    Console.ReadLine();

    Console.WriteLine("Shutting down. Please wait...");
    jobbrServer.Stop();
}
```

The JobbrServer has an embedded OWIN-Selfhost for WebApi, so please add the corresponding NuGet-Package to the project where the JobbrServer is included.

	PM> Install-Package Microsoft.Owin.Host.HttpListener


## Hosting a Runner
Hosting a runner in the separate Runner-Executable is even easier.


```c#
public static void Main(string[] args)
{
    var jobbrRuntime = new JobbrRuntime(typeof(MyJobs.MinimalJob).Assembly);
    jobbrRuntime.Run(args);
}
```

## Make your Jobs Jobbr compatible
Good news! All your C#-Code is compatible with jobbr as long as the CLR-type can be instantiated and has at least a `public Run()`- Method.

If you want to log, just log with the framework of your choice, thus there is no dependency to any framework you can use log4net, nLog or any other framework.
  
You have long running jobs an would like to show the progress somewhere in your application? Simply drop a service-message to the `Console`. Sample

```c#
var progress = (double)(i + 1) / iterations * 100;

Console.WriteLine("##jobbr[progress percent='{0:0.00}']", progress);
``` 
## Define Jobs
Adding Jobs can be done via Database, RestAPI (see below) or via the FluentApi. A job consists at least of an ``UniqueName`` and a ``Type`` (which is basically the CLR-Type of the JobClass). To register Jobs on Startup, implement your own ``JobbrContiguration`` and override the ``OnRepositoryCreating``-Method.

> **Note**
> - There is no functionality to remove existing jobs
> - All in Fluent API unspecified triggers automatically get deactivated

**Sample:**
```c#
public class MyJobbrConfiguration : DefaultJobbrConfiguration
{
    public MyJobbrConfiguration()
    {
        /* ... */
    }

    public override void OnRepositoryCreating(RepositoryBuilder repositoryBuilder)
    {
        base.OnRepositoryCreating(repositoryBuilder);

        repositoryBuilder.Define("MinimalJobId", "Demo.MyJobs.MinimalJob")
            .WithTrigger(new DateTime(2015, 3, 20, 12, 00, 00))
            .WithTrigger("* 15 * * *");
    }
}

``` 

## Rest API
The JobbrServer exposes a Rest-API to define Jobs, Triggers and watch the status for running Jobs. Please see the section WebAPI for a complete reference

### Available Jobs
Take the following Endpoint

	GET http://localhost/jobbr/api/jobs

### Trigger a Job to run (JobRun)
A job can be triggered by using the following Endpoint (JobId or UniqueId is required)

	POST http://localhost/jobbr/api/jobs/{JobId}/trigger
	POST http://localhost/jobbr/api/jobs/{UniqueId}/trigger

There are 3 different modes 

#### 1. Instant

	{
        "triggerType": "instant",
        "isActive": true,
        "userId": 12,
		"parameters": { "Param1": "test", "Param2" : 42 }
	}

#### 2. Scheduled

	{
        "triggerType": "scheduled",
		"startDateTimeUtc": "2015-03-12 11:00"
        "isActive": true,
		"userName": "test"
		"parameters": { "Param1": "test", "Param2" : 42 }
	}

#### 3. Recurring

	{
        "triggerType": "recurring",
		"startDateTimeUtc": "2015-03-12 11:00
		"endDateTimeUtc": "2015-03-19 18:00"
		"definition: "* 15 * * *",
	    "isActive": true,
		"userName": "test"
		"parameters": { "Param1": "test", "Param2" : 42 }
	}

A definition is a cron definition as specified here: http://en.wikipedia.org/wiki/Cron 

Please note that
* DateTime Values are always UTC
* UserId, UserName or UserDisplayName are optional
* Parameters are an object


### List JobRuns By User
A jobrun is triggered by a trigger. To get jobruns for a specific used, it required to provide at least a UserId or UserName for the trigger.

	GET http://localhost/jobbr/api/jobruns/?userId=1234

Or 

	GET http://localhost/jobbr/api/jobruns/?userName=name

### Watch the Run-Status
To get a detailed view for a jobrun you have to know the JobRunId

	GET http://localhost/jobbr/api/jobruns/{JobRunId}

Sample Response

	{
		"jobId": 7,
		"triggerId": 446,
		"jobRunId": 446,
		"uniqueId": "95e9e93e-062c-4b00-8708-df5ca1270c2e",
		"instanceParameter": {
			"Param1": "test",
			"Param2": 42
		},
		"jobName": "ThirdJob",
		"jobTitle": "This a sample Job",
		"state": "Completed",
		"progress": 100,
		"plannedStartUtc": "2015-03-11T11:23:15.74",
		"auctualStartUtc": "2015-03-11T11:23:16.52",
		"auctualEndUtc": "2015-03-11T11:23:34.48"
	}

### Getting Artefacts
If there are any artefacts for a specific run, they are available under.

	GET http://localhost/jobbr/api/jobruns/446/artefacts/{filename}

#Features

## Logging
Jobbr uses the LobLog library to detect your Logging-Framework of the Hosting Process. When using Jobbr, you don't introduce a new dependency to an existing Logging-Framework. See https://github.com/damianh/LibLog for details.

## Parameters
Write about jobParameters and runParameters. Serialization to concrete types, etc.

## Dependency Injection (Runtime)
Provide your own Dependency Resolver which activates the clr-type of your JobRun-Class by implementing the `IJobbrDependencyResolver`-Interface. If no JobbrDependencyResover is provided, a generic resolver is used, but a parameterless constructor is then required.

### RuntimeContext
Sometimes it's necessary to retreive the Jobbr `RuntimeContext` which is currently providing the 
`UserId` and `UserName`. See RuntimeContext.cs for available information.

If you decide to use the RuntimeContext, you will have to implement a DependencyResolver which also supports registration of instances by the Jobbr Runtime (i.e the Context) by implementing the `IJobbrDependencyRegistrator`-Interface. See the Demo with the SpecifiedUser-Job.

The JobbrRuntime will then register a instance of `RuntimeContex` after start but before the actual Job will be activated. Depending on your Depencency Container, this instance can then be injected to the Job-Instance.
How-ever, you should not reference the JobbrRuntime from your Jobs-Assembly, but this can be handled with an additional indirection. See the Damo with the SpeciciedUser-Job for details.


## Artefacts
Everything you store in the Working-Directory is automatically collected and pushed to the Jobbr-Server 

## Service Messages
At the moment, only progress is supported.

## Restful API Reference

### Get all Jobs
Take the following Endpoint

	GET http://localhost/jobbr/api/jobs

With a sample return value

	[
		{
			"id": 1,
			"uniquename": "MyJob1",
			"title": "My First Job",
			"type": "MinimalJob",
			"parameters": "{ \"param1\" : \"test\" }",
			"createdDateTimeUtc": "2015-03-04T17:40:00"
		},
		{
			"id": 3,
			"uniquename": "MyJob2",
			"title": "Second Job",
			"type": "ParameterizedlJob",
			"parameters": "{ \"param1\" : \"test\" }",
			"createdDateTimeUtc": "2015-03-10T00:00:00"
		},
		{
			"id": 7,
			"uniquename": "MyJob3",
			"title": "Third Job",
			"type": "ProgressJob",
			"createdDateTimeUtc": "2015-03-10T00:00:00"
		}
	]

### Add Job
Only the ``UniqueName`` and ``Type`` are required.

	POST http://localhost/jobbr/api/jobs

	{
		"uniquename": "MyJob1",
		"title": "My First Job",
		"type": "MinimalJob",
		"parameters": "{ \"param1\" : \"test\" }",
		"createdDateTimeUtc": "2015-03-04T17:40:00"
	}

### More
...

# Credits
 
## Based On
* CommandLineParser
* Dapper
* LibLog
* Microsoft ASP.NET WebAPI
* Microsoft OWIN
* nrcrontab
* Newtonsoft JSON.NET
* Ninject

## Authors
* Michael Schnyder
* Peter Gfader
* Mark Odermatt

# License
This software cannot be licenced.

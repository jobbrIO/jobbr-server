# Jobbr
Jobbr is a .NET JobServer. Unless other JobServer-Frameworks Jobbr explicitly solves the following problems
* Isolation of Jobs on process-level (JobRunner)
* Restful API to trigger Jobs and watch the execition state
* Artefacts-store for both job parameters and job results
* Embeddable in your Environment (JobServer and Runner)
* DI-Resolution in Server and Runner for your Jobs
* No Dependency to the Jobbr-Assemblies needed from your Jobs

# QuickStart
There is a demo-solution with a ready to run application.

## Installation
Simply create a database (or use any existing) by executing the CreateSchemaAndTables.sql-File, located in source\Jobbr.Server.Dapper. The JobStorageProvider is Dapper-based and has been tested agains SQL Server 2012.

## Hosting a JobbrServer
To Host a JobbrServer simply define a Storage Provider for Jobs and JobArtefacts and initialize the JobServer:

    var jobStorageProvider = new DapperStorageProvider(@"YourConnectionString");
	var artefactStorageProvider = new FileSystemArtefactsStorageProvider("C:\jobdata");

    var config = new DefaultJobbrConfiguration
         {
             JobStorageProvider = jobStorageProvider,
             ArtefactStorageProvider = artefactStorageProvider,
             JobRunnerExeResolver = () => @"..\..\..\Demo.JobRunner\bin\Debug\Demo.JobRunner.exe",
             BeChatty = true, // Verbose output on the RunnerExecutable
         };

    using (var jobbrServer = new JobbrServer(config))
    {
        jobbrServer.Start();

        Console.WriteLine("JobServer has started . Press enter to quit")
        Console.ReadLine();

        Console.WriteLine("Shutting down. Please wait...");
        jobbrServer.Stop();
    }


## Hosting a Runner
Hosting a runner in the separate Runner-Executable is even easier.

    public static void Main(string[] args)
    {
	    var jobbrRuntime = new JobbrRuntime(typeof(MyJobs.MinimalJob).Assembly);
	    jobbrRuntime.Run(args);
	}

## API
The JobbrServer exposes a RestFul-Api to define Jobs, Triggers and watch the status for running Jobs. Please see the section WebAPI for a complete reference

### Available Jobs
Take the following Endpoint

	GET http://localhost/jobbr/api/jobs

### Trigger a Job to run
A job can be triggered in three different modes using the following Endpoint:

	POST http://localhost/jobbr/api/jobs/{JobId}/trigger

Please note that
* DateTime Values are always UTC
* UserId, UserName or UserDisplayName

Instant

	{
        "triggerType": "instant",
        "isActive": true,
        "userId": 12,
		"parameters": { "Param1": "test", "Param2" : 42 }
	}

Scheduled

	{
        "triggerType": "scheduled",
		"dateTime": "2015-03-12 11:00"
        "isActive": true,
		"userName": "test"
		"parameters": { "Param1": "test", "Param2" : 42 }
	}

Recurring

### List Jobs By User


### Watch the Run-Status

### Getting Artefacts



## Features Explained

# Logging
Jobbr uses the LobLog library to detect your Logging-Framework of the Hosting Process. When using Jobbr, you don't introduce a new dependency to an existing Logging-Framework. See https://github.com/damianh/LibLog for details

# RestFul Api Reference

## Jobs
### Get all Jobs
Take the following Endpoint

	GET http://localhost/jobbr/api/jobs

With a sample return value

	[
		{
			"id": 1,
			"name": "My First Job",
			"type": "MinimalJob",
			"parameters": "{ \"param1\" : \"test\" }",
			"createdDateTimeUtc": "2015-03-04T17:40:00"
		},
		{
			"id": 3,
			"name": "Second Job",
			"type": "ParameterizedlJob",
			"parameters": "{ \"param1\" : \"test\" }",
			"createdDateTimeUtc": "2015-03-10T00:00:00"
		},
		{
			"id": 7,
			"name": "Thir Job",
			"type": "ProgressJob",
			"createdDateTimeUtc": "2015-03-10T00:00:00"
		}
	]

# Credits
 
## Based On
* nrcron
* ASP.NET WebAPI (OWIN)
* Dapper
* CommandLineParser
* JSON.NET
* LibLog

## Authors
* Michael Schnyder

# Licence
This software cannot be licenced.

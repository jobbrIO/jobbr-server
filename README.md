# Jobbr [![Build status](https://img.shields.io/appveyor/ci/jobbr/jobbr-server/develop.svg)](https://ci.appveyor.com/project/Jobbr/jobbr-server)
Jobbr is a .NET job server. It is designed to reduce artifical complexity when a job server is needed in a project. Jobbr tries to get out of your way as much as possible so that you can focus on business logic. Job implementations have no dependency to Jobbr which makes it easy to integrate Jobbr in any .NET project.

## Main Features

* Isolation of Jobs on process-level
* REST API to manage and trigger Jobs and watch the execution state
* Provides a typed client to consume the REST API
* Persists created files from running jobs in an artefact store.
* Supports CRON expressions for recurring triggers
* Embeddable in your own C# application (JobServer and Runner)
* Easily testable
* IOC for your jobs
* Extendable (jobstorage providers, artefact storage providers, execution etc)
* Progress tracking via stdout (`Console.WriteLine()`)

## QuickStart
The best way to get started is to check out the [demo repo](https://github.com/jobbrIO/jobbr-demo) to see a running example of Jobbr.

## Implementing a job
Good news! All your C#-Code is compatible with jobbr as long as the CLR-type can be instantiated and has at least a public `Run()`- Method.

```c#
public class SampleJob
{
    public void Run()
    {
        const int iterations = 15;

        for (var i = 0; i < iterations; i++)
        {
            Thread.Sleep(500);

            Console.WriteLine("##jobbr[progress percent='{0:0.00}']", (double)(i + 1) / iterations * 100); // optional: report progress
        }
    }
}
```

## Configuring job with triggers
To define jobs use the `AddJobs` extension method:
```c#
jobbrBuilder.AddJobs(repo =>
{
    // define one job with two triggers
    repo.Define("SampleJob", "Quickstart.SampleJob")
        .WithTrigger(DateTime.UtcNow.AddHours(2)) /* run once in two hours */
        .WithTrigger("* * * * *"); /* run every minute */
});

``` 

If you need to add triggers at runtime head over to the [REST API component](https://github.com/jobbrIO/jobbr-webapi) which also includes a typed client. The REST API is optional and has to be plugged in if needed.

## Persistence
There are two different storages: 

- `Storage`: stores jobs, triggers and jobruns
- `ArtefactStorage`: stores files created by running jobs

By default Jobbr runs in memory, thus all data is lost when Jobbr is restarted. To keep the data configure one of the storage providers:

### Storage
- [MsSql Storage Provider](https://github.com/jobbrIO/jobbr-server-mssql) to store the data in MS SQL Server
- [RavenDb Storage Provider](https://github.com/jobbrIO/jobbr-server-ravendb) to store the data in [RavenDB](http://ravendb.net)

### ArtefactStorage
- [Filesystem Storage Provider](https://github.com/jobbrIO/jobbr-artefactstorage-filesystem) to store the data in a folder
- [RavenFS Storage Provider](https://github.com/jobbrIO/jobbr-artefactstorage-ravenfs) to store the data in [RavenDB](http://ravendb.net)

## Logging
Jobbr uses the LibLog library to detect your Logging-Framework of the Hosting Process. When using Jobbr, you don't introduce a new dependency to an existing Logging-Framework. See https://github.com/damianh/LibLog for details.

# License
This software is licenced under GPLv3. See [LICENSE](LICENSE), please see the related licences of 3rd party libraries below.

## Acknowledgements

Jobbr Server is based on following great open source projects:
* [AutoMapper](https://github.com/AutoMapper/AutoMapper]) [(MIT)](https://github.com/AutoMapper/AutoMapper/blob/master/LICENSE.txt)
* [LibLog](https://github.com/damianh/LibLog) [(MIT)](https://github.com/damianh/LibLog/blob/master/licence.txt)
* [NCrontab](https://github.com/atifaziz/NCrontab) [(Apache-2.0)](https://github.com/atifaziz/NCrontab/blob/master/COPYING.txt)
* [Newtonsoft Json.NET](https://github.com/JamesNK/Newtonsoft.Json) [(MIT)](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
* [Ninject](https://github.com/ninject/Ninject) [(Apache-2.0)](https://github.com/ninject/ninject/blob/master/LICENSE.txt)
* [TinyMessenger](https://github.com/grumpydev/TinyMessenger/blob/master/licence.txt) [(Ms-PL)](https://github.com/grumpydev/TinyMessenger/blob/master/licence.txt)

## Credits
This application was built by the following awesome developers:
* Michael Schnyder
* Oliver Zürcher
* Peter Gfader
* Mark Odermatt
* [Steven Giesel](https://github.com/linkdotnet)
* David Fiebig

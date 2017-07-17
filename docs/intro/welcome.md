# Welcome
This is the documentation for the .NET JobServer Jobbr. Feel free to contribute by editing this page or head to one of your repositories on [GitHub](https://github.com/JobbrIO).

## Why another JobServer?
You might argue that there are already a couple of .NET JobServers out there. And you are right. [Quartz.NET](https://www.quartz-scheduler.net/) for instance, is a very popular C# port of a very popular open source Java job scheduling framework [Quartz](http://www.quartz-scheduler.org/). Another well known player is [Hangfire](https://www.hangfire.io/), an easy way to perform background processing in .NET and .NET Core applications. You could also use your favorite MessageBus or server-less framework to execute your code on a regular basis, and for sure there are a lot of stand-alone job server products that are able to help you there.

For those that want to compare Quartz.NET, Hangfire and Jobbr to each other, here are some notes.

**Quartz.NET** is a mature *Scheduling Framerwork* which delegates the execution of business logic into classes that need to follow the exact contract defined by the Frameworks interfaces. It's a scheduler and therefore does not care about collecting the files produced by your jobs or offers a restful API to steer the jobs and their execution.

**Hangfire.io** is primarly a *task offloader* built for large ASP.NET Web farms. It allows you to create background tasks triggered by some interactions on a web application. The programming model clearly focuses on one-time shots. Like Quartzs.NET, Hangfire does not provide a pluggable architecture where you just can add a restful API.

**Our goal** is to provide a library that allows you to host your own JobServer where ever you want and without any compromises regarding dependencies and versions, logger-abstractions, storage implementations, extendability and stability. At the bare-minimum a Jobbr jobserver, it's execution engine and storage can be run in-memory. 

Besides of that we've focused on clear responsibilities and unique features like

* Embeddable in your own C# application (job server and executor)
* Isolation of Jobs on process-level
* REST API and typed client to manage and trigger Jobs and watch the execution state
* Persists created files from jobruns in an artefact store.
* Supports CRON expressions for recurring triggers
* Easy testable supported by in-memory stores
* No additional dependencies or version issues for your jobs
* DI Container ready for your jobs
* [Extendable](../dev/extend.html) (jobstorage providers, artefact storage providers, execution etc)
* Progress tracking via stdout (Console.WriteLine())


## Demo
Have a look at the [Jobbr Demo repository](https://github.com/jobbrIO/jobbr-demo).

## Team
Jobbr was handcrafter by the following talented software engineers @ Zühlke Engineering AG, Switzerland.

**Michael Schnyder**
Lead Software Architect @ [Zühlke](https://www.zuehlke.com/ch/de/), creator of [http://www.jobbr.io](Jobbr) and .NET Trainer. Focused on .NET and modern web applications.

**Oliver Zürcher**
@olibanjoli

**Mark Odermatt**
@mo85

**Peter Gfader**
@peitor

# Zühlke Portal Solutions
Jobbr is part of the Zühlke Portal Framework. Portal solutions connect existing data, systems and services to a collaborative ecosystem and give users a common digital home. This makes the same data accessible and usable for highly varied customer and user groups.

If you want to get more information in how creating added value at the customer interface, please head to [https://www.zuehlke.com/ch/en/markettrends/digital-transformation/portals/](https://www.zuehlke.com/ch/en/markettrends/digital-transformation/portals/)
# Organisation
## Core Team
The core team consist of
* Michael Schnyder
* Oliver Zürcher

## Source Code
This is the main repository for the Jobbr project and contains the core functionality of a JobServer itself. These are

* A Scheduler
* JobRepository Builder
* Abstractions for Storage Provides
* Abstractions for Job Execution
* Interfaces to register additional components

Please see all the other repositories for additional features like
* Storage
* Execution
* Web Management

These projects follow their own release cycle and might also publish packages on NuGet. Hereby a short overview of current related projects

## Related Projects

### [MSSQL Storage Provider](../../jobbr-storage-mssql) (jobbr-storage-mssql)
Allows to store the defined Jobs and their state on a MSSQL-based database. Please see the project for details.

### [Forked Execution](../../jobbr-execution-forked) (jobbr-execution-forked)
Enables jobs to be run in a separate process to improve stability. Contains both additions for the server to start additional processes and components to execute jobruns in a stand alone console app as a separate process

### [InMemory Execution](../../jobbr-execution-inmemory) (jobbr-execution-inmemory)
Contains a simple InMemory Executor that let's jobs run within the same process as the JobServer is running.

### [Rest Web API](../../jobbr-webapi) (jobbr-webapi)
Contains sources for both the webapi-plugin attached to the Jobbr-Server as also a fully typed client to access the rest-based apis with .NET

### [Runtime](../../jobbr-runtime) (jobbr-runtime)
The runtime is actively managing the process of starting and monitoring jobs. There will be different execution models (forked, in-memory) which both provide the exact same runtime. This is more like an "internal" project without its own release cycle.

### [Demo](../../jobbr-demo) (jobbr-demo)
Samples to play around with different configurations and for reference purposes
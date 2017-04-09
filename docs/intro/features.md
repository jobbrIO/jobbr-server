# Features

## Management

### Rest API

### Trigger Types

### Non-Invasive

### Fully typed client

## Job Execution

### Parameterization
Both Job & Trigger (JobRun)

### Forked Execution
Isolation of Jobs on process-level (JobRunner)

### Progress Reporting

### Output Collection

### DI for Jobs
Optional

### Logging

## Persistence

### RavenFS Store

### Filesystem Store

### MSSQL JobStore

### RavenDB JobStore

## Extendability
We think its important to define clear responsibilities for such an important thing like a JobServer. By having this in mind, we where able to cut the core components into different extensions to that you can choose which features you need and which complexity you need to add. Furthermore this makes the development more stable and preditcable. You will notice that by less frequent version updates for the server itself, but evolving feature componenents.
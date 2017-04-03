## MN 2017-01-16 "Architecture / Packages"

**Topic**: How should we split the different assemblies and packages. Is it required to release features independent from others etc

### Participants
* Oliver Zuercher (@olibanjoli)
* Michael Schnyder (@MichaelSchnyder)

### Open Questions
* Shall we go in the direction of having a glue-package for each abstraction type (storage, exection, management)
* If Glue: Is there also a model-repository that covers the essential datatypes or is it backed in each of the abstractions and mapped from the core model owned by the JobbrServer?
* If Glue: Releaseplan and management of the glue-libraries 
* If !Glue: One Repo and Version fits it all?

### Decision
Based on the current information and advantages and drawbacks the decision was made to introduce abstraction packages that contain the interface specifications for each extension possibility. The Isolation overweighted the disadwantage of having additional NuGet-Packges for the sole abstraction purpose.

----------

The whole project is split across different packages which work as glue code between server & features and packages that contain the actual feature.

This is cause by the following drivers that should support the scenarios below

### Drivers
#### Component based architecture
While every JobServer will contain one instance of the JobbrServer, you might not install any RestAPI or WebDashboad component. You should also be able to store your JobInformations and Artefacts into different storage types.

#### Independent Component Release Cycle
Components should have their own release cycle and feature set. Each Optional / Extended Feature should be able to release packages on their own by basing in well defined interfaces that the server supplies. As an example: A Web-dashboard could be updated more frequent than the core server component. 

#### SemVer2.0
All configurable components should follow the SemVer2.0 principle. As an example, the Storage Provider is SemVer2.0 compilant if major changes (you need to adjust your code or DB manually) must only happen from version 1.x to 2.0.

#### Reduce Server Version Dependency
Internal changes to the server shall be isolated and should not have any impact to additional components (i.e. storage providers). The server does not own a specific interface, the server _implements_ a given interface specification.

##### Case 1: Optional Features (like Web UI / Rest API) should not trigger new server version
We would like to isolate functional changes to the related packages and repositories. A change to the storage layer should not cause changes to the components optionally using this library.

##### Case 2: Change Propagation
.NET Framework always refers to a exact defined version of a compiled assembly. We would like to isolate the changes related only to any component as low as possible. Given that, a change to the Storage Functionality in MSSQL should not cause a new version of the Raven Storage adapter or Web API.

### Approaches
#### Same Repository
If the WebApi would reside in the the same repository as the server lives, they would be coupled also by their version. 

**The Good**
- It's very easy to update all packages to the same all-compatible version
- Bugfixes could be done on release branches

**The Bad**
- When WebApi introduces a new feature, also the server gets bumped (because they reside on the same repo).
- The version has a very broad context and could also be dumped by components that are not in used in project

#### Direct Dependency to Server
Both WebAPI and Server reside in separate repositories, where as the WebAPI references the concrete sever project for all the interfaces and therefore also compiles to a specific version of the server. 

**The Good**
- If the storage layer updates (due to support of the new MSSQL Server) causes no impact / update on other components.

**The Bad**
- If the Specification to the storage layer updates, this causes a change to the server and a new assembly version. That will raise the need to a new WebAPI Version even if the WebAPI did not change. Assembly redirects would do the trick for the WebAPI for this case, if the WebAPI does not use the Interface to the storage layer. But that needs to be done in every project which contains Server + Storage Layer + WebAPI combinations

#### Interface Packages
Extension Points must be specified with interfaces and reside on separate repositories. Each repository has its own version which follows its own internal release cycle. Every change to this interface repositories is breaking if the MSIL code changes. A server, which offers extension possibility does this by explicitly referencing and thus implementing a specific version of the given extension-set. (i.e. The server references the StorageAdapterExtension-2.0.0 which contains the specification implemented by a specific storage provider).

**The Good**
- Changes to the specifications of extension of type A do not propagate to other extensions (type B) even if the server implementing/providing interfaces from A needs a new version. The Host (Jobbr Server) and dependent packages are loosely coupled by an in-between package.

**The Bad**
- Added complexity, see below

![Package Organisation](assets/mn-2017-01-16-package-organisation.png)

### Scenarios
1. Release a new Storage Provider which supports MSSQL2016
This is a non-breaking change for the user of the coresponding MSSQL Adapter. The server functionality shall be the same.

### Additional Resources
http://stackoverflow.com/questions/1456785/a-definitive-guide-to-api-breaking-changes-in-net

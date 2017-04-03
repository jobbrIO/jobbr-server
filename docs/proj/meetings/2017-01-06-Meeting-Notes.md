## MN 2017-01-06 "Planning"

**Topic**: How should we proceed with the .NET JobServer Jobbr regarding a self manageable project with can be evolved by individuals rather than the main contributors 

### Participants
* Oliver Zuercher (@olibanjoli)
* Michael Schnyder (@MichaelSchnyder)

-------------
### Short Term 
	
* Version & Release Strategy?? (1PD)
  * Define the way how we uprelease the packages
  * WHich Packages share the same version?
		
* Build Scrips (1PD)
  * GitFlow & GitVersion
  * Reasonable Analyzers and fixes --> StyleCop and FxCop Analyzers V2 (Roslyn)
  * .NET UP 4.6.2, C# 6.0
  * Nuget deps Update


  * DB Migration strategy???
  --> keep current solution release notes with sql script

* Small Features (5 PD)
  * Check for RunnerExe on Start (Improve exception message)
  * Delete Jobfolder when empty after (temp and workdir)
  * Set temp dir in jobrun process
  * check all the warn log messages for missing features
  * downgrade InfoLogs to Debug (too verbose)
  * Bug with filenames with special chars when uploading to remote ExcutorController
  * API Features + Client nachziehen (2 PD)
    * Get 100 runs by date
    * paging
    * Get Type UserName / UserId
      * GetByState
    * (- Swagger)
  * Maintenance Mode / Graceful shutdown before redeployment  + API Route/Controller, Doc, Client 

* Landing Page / Docs (Total 5)
  * jobbr.io Landing Page (1)
  * ReadTheDocs / GH Pages Setup (1)
    * include swagger defs
  * Playground on azure with swagger (2PD?)

### OOS
	
* Arch (8 PD?)
  * IJobService (4PD)
    * "IJobManagementService" <-> API
    * "IStateChange" <- updates state from jobrun <-- JobRunContext / Execution Controller
* Chattier / More Specific Storage Provider Interface to avoid Race Conditions (1PD)
* Separation of API for RemoteExec and JobbrAdmin and group RemoteEx in separate namespace/asm (2-3PD)
	
### V1																													   
	* Improve Testability (21 PD ???)
	  - Testing the Scheduler 
	  - Manipulate Time

	  
### Outlook / Long Term

	Small Features
	- Retention policy for artefacts & jobruns 
	- Create subdirectories for jobruns??? Strategy???
	- Classname is validated on JobServer start
	
	 
	Extended Features 
	* Let the JobRunner run as specific user account

	Upcoming
	* Client API Authentication
	* Remote Execution Authentication
	* Web Dashboard as Package (seperate Package that consumes the JobbrService[s])
	 - Same Host
	 - Separate Host 
	 - Current JobRuns / Start 
	 - Crud Edit Triggers
	
	* Business Result vs. "Technical Logging"
	  - Businesslog vs. Runtime Technical log 
	  - Concrete result of job / JobResponse
		- Service Message to set the business state
		- Return an object which is 
		    - stored in db after jobrun
			- serialized to the directory and available in api
		- Client awaits result of jobrun 
	  
	* Long running Job / keep Job running 

	* Run Jobs on remote machine
	  - Similar to scale out with multiple servers with runners 
	
	* Performance
	  - It takes long to find concrete type by the string provided
	  
	
	* Scaling
	  - Multiple Servers with Runners with one JobServer 
	  - Multiple JobServer with multiple local runners each
	  - Multiple All
	 
	* HA
	  - Work with messages against DB Server 
	  
	  
	  
	Long Term
	* Queuing
	* Highproio Queue / vs. Low Prio Queue 
	 

Status & Versions
*****************

Please see the following badges for the project including all extensions

Main Packages
#####################

The following table shows the current versions of the main Jobbr Project and related feature packages.

+----------------------------------------------+-----------------------------------------------------+-----------------------------------------------------+
| Component                                    | Build                                               | NuGet                                               |
+==============================================+=====================================================+=====================================================+
| **Jobbr.Server**                             | djdjdjdj                                            | djdjdjdj                                            |
+----------------------------------------------+-----------------------------------------------------+-----------------------------------------------------+
| `FileSystem Artefact Provider`_              | | |artefact-fs-badge-build-master|_                 | djdjdjdj                                            |
|                                              | | |artefact-fs-badge-build-develop|_                | djdjdjdj                                            |
+----------------------------------------------+-----------------------------------------------------+-----------------------------------------------------+

|                 Project                                             |                           Build                          |                           NuGet                              |
| :-----------------------------------------                          | :------------------------------------------------------- | :----------------------------------------------------------- |
| **Jobbr.Server**                                                    | -                         | -                        |
| [**FileSystem Artefact Provider**][artefact-fs-link-repo]           | [![AppVeyor][artefact-fs-badge-build-master]][artefact-fs-link-build] [![AppVeyor][artefact-fs-badge-build-develop]][artefact-fs-link-build] | [![NuGet][artefact-fs-badge-nuget]][artefact-fs-link-nuget] [![NuGet][artefact-fs-badge-nuget-pre]][artefact-fs-link-nuget]
| RavenFS Artefact Provider | | |
| [**MS SQL Storage Provider**][mssql-link-repo]                       | [![AppVeyor][mssql-badge-build-master]][mssql-link-build] [![AppVeyor][mssql-badge-build-develop]][mssql-link-build]                                             | [![NuGet][mssql-badge-nuget]][mssql-link-nuget] [![NuGet][mssql-badge-nuget-pre]][mssql-link-nuget]      | 
| RavenDB Storage Provider | | |
| [**Forked Execution**][execution-forked-link-repo] | [![AppVeyor][execution-forked-badge-build-master]][execution-forked-link-build] [![AppVeyor][execution-forked-badge-build-develop]][execution-forked-link-build] | [![NuGet][execution-forked-server-badge-nuget]][execution-forked-server-link-nuget] [![NuGet][execution-forked-server-badge-nuget-pre]][execution-forked-server-link-nuget] <br/> [![NuGet][execution-forked-console-badge-nuget]][execution-forked-console-link-nuget] [![NuGet][execution-forked-console-badge-nuget-pre]][execution-forked-console-link-nuget]          |
| [**Web API Extension**][webapi-link-repo]                           | [![AppVeyor][webapi-badge-build-master]][webapi-link-build] [![AppVeyor][webapi-badge-build-develop]][webapi-link-build]          | [![NuGet][webapi-badge-nuget]][webapi-link-nuget] [![NuGet][webapi-badge-nuget-pre]][webapi-link-nuget]  |

### Component Models
|                 Project                  |                           Stable                         |                           Pre-release                        |
| :--------------------------------------- | :------------------------------------------------------: | :----------------------------------------------------------: |
| **Jobbr.ComponentModel.Registration**    |   -                                                      | [![NuGet][cm-registration-pre-badge]][cm-registration]       |
| **Jobbr.ComponentModel.Execution**       |   -                                                      | [![NuGet][cm-execution-pre-badge]][cm-execution]             |
| **Jobbr.ComponentModel.Management**      |   -                                                      | [![NuGet][cm-management-pre-badge]][cm-management]           |
| **Jobbr.ComponentModel.JobStorage**      |   -                                                      | [![NuGet][cm-jobstorage-pre-badge]][cm-jobstorage]           |
| **Jobbr.ComponentModel.ArtefactStorage** |   -                                                      | [![NuGet][cm-artefactstorage-pre-badge]][cm-artefactstorage] |


## Issue Lists
The issues are split across the different repositories where they belong to. How-ever, the list if issues can still be aggregated for the while organisation:
* [Open issues](https://github.com/issues?q=is%3Aopen+is%3Aissue+user%3AjobbrIO)
* [Open issues for 1.0](https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+milestone%3A1.0+)
* [List of open issues without milestone](https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+no%3Amilestone+)

.. _FileSystem Artefact Provider:    https://github.com/JobbrIO/jobbr-artefactstorage-filesystem
.. _artefact-fs-link-repo:             https://github.com/JobbrIO/jobbr-artefactstorage-filesystem
.. _artefact-fs-badge-build-develop:   https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-filesystem
.. _artefact-fs-badge-build-master:    https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-filesystem
.. _artefact-fs-link-nuget:            https://www.nuget.org/packages/Jobbr.ArtefactStorage.FileSystem

.. |artefact-fs-badge-build-develop|  image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-filesystem/develop.svg?label=develop
.. |artefact-fs-badge-build-master|   image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-filesystem/master.svg?label=master
.. |artefact-fs-badge-nuget|          image::  https://img.shields.io/nuget/v/Jobbr.ArtefactStorage.FileSystem.svg?label=NuGet%20stable
.. |artefact-fs-badge-nuget-pre|      image::  https://img.shields.io/nuget/vpre/Jobbr.ArtefactStorage.FileSystem.svg?label=NuGet%20pre

[mssql-link-repo]:             https://github.com/JobbrIO/jobbr-storage-mssql         
[mssql-link-build]:            https://ci.appveyor.com/project/Jobbr/jobbr-storage-mssql         
[mssql-link-nuget]:            https://www.nuget.org/packages/Jobbr.Server.MsSql
[mssql-badge-build-develop]:   https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-mssql/develop.svg?label=develop
[mssql-badge-build-master]:    https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-mssql/master.svg?label=master
[mssql-badge-nuget]:           https://img.shields.io/nuget/v/Jobbr.Server.MsSql.svg?label=NuGet%20stable
[mssql-badge-nuget-pre]:       https://img.shields.io/nuget/vpre/Jobbr.Server.MsSql.svg?label=NuGet%20pre

[execution-forked-link-repo]:                   https://github.com/JobbrIO/jobbr-execution-forked         
[execution-forked-link-build]:                  https://ci.appveyor.com/project/Jobbr/jobbr-execution-forked         
[execution-forked-server-link-nuget]:           https://www.nuget.org/packages/Jobbr.Execution.Forked
[execution-forked-console-link-nuget]:          https://www.nuget.org/packages/Jobbr.Runtime.Console
[execution-forked-badge-build-develop]:         https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-forked/develop.svg?label=develop
[execution-forked-badge-build-master]:          https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-forked/master.svg?label=master
[execution-forked-server-badge-nuget]:          https://img.shields.io/nuget/v/Jobbr.Execution.Forked.svg?label=NuGet%20stable%20%28Extension%29
[execution-forked-server-badge-nuget-pre]:      https://img.shields.io/nuget/vpre/Jobbr.Execution.Forked.svg?label=NuGet%20pre%20%28Extension%29
[execution-forked-console-badge-nuget]:         https://img.shields.io/nuget/v/Jobbr.Runtime.Console.svg?label=NuGet%20stable%20%28Runtime%29
[execution-forked-console-badge-nuget-pre]:     https://img.shields.io/nuget/vpre/Jobbr.Runtime.Console.svg?label=NuGet%20pre%20%28Runtime%29

[webapi-link-repo]:             https://github.com/JobbrIO/jobbr-webapi         
[webapi-link-build]:            https://ci.appveyor.com/project/Jobbr/jobbr-webapi         
[webapi-link-nuget]:            https://www.nuget.org/packages/Jobbr.Server.WebAPI
[webapi-badge-build-develop]:   https://img.shields.io/appveyor/ci/Jobbr/jobbr-webapi/develop.svg?label=develop
[webapi-badge-build-master]:    https://img.shields.io/appveyor/ci/Jobbr/jobbr-webapil/master.svg?label=master
[webapi-badge-nuget]:           https://img.shields.io/nuget/v/Jobbr.Server.WebAPI.svg?label=NuGet%20stable
[webapi-badge-nuget-pre]:       https://img.shields.io/nuget/vpre/Jobbr.Server.WebAPI.svg?label=NuGet%20pre
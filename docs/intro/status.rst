Status & Versions
*****************

Please see the following badges for the project including all extensions

Server
==========
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| Name                                                                     | Build                                       | Version                                    |
+==========================================================================+=============================================+============================================+
| `Jobbr Server`_                                                          | | |server-badge-build-master|_              | | |server-badge-nuget|_                    |
|                                                                          | | |server-badge-build-develop|_             | | |server-badge-nuget-pre|_                |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+


Extensions
==========

The following table shows the current versions of the main Jobbr Project and related feature packages.

.. ===================================================
   NOTE: Please see the replacements after the table!
   ===================================================

+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| Extension                                                                | Build                                       | Version                                    |
+==========================================================================+=============================================+============================================+
| `MS SQL Storage Provider`_                                               | | |storage-mssql-badge-build-master|_       | | |storage-mssql-badge-nuget|_             |
|                                                                          | | |storage-mssql-badge-build-develop|_      | | |storage-mssql-badge-nuget-pre|_         |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `FileSystem Artefact Provider`_                                          | | |artefact-fs-badge-build-master|_         | | |artefact-fs-badge-nuget|_               |
|                                                                          | | |artefact-fs-badge-build-develop|_        | | |artefact-fs-badge-nuget-pre|_           |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `RavenDB Storage Provider`_                                              | | |storage-ravendb-badge-build-master|_     | | |storage-ravendb-badge-nuget|_           |
|                                                                          | | |storage-ravendb-badge-build-develop|_    | | |storage-ravendb-badge-nuget-pre|_       |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `RavenFS Artefact Provider`_                                             | | |artefact-ravenfs-badge-build-master|_    | | |artefact-ravenfs-badge-nuget|_          |
|                                                                          | | |artefact-ravenfs-badge-build-develop|_   | | |artefact-ravenfs-badge-nuget-pre|_      |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `Forked Process Execution`_                                              | | |execution-forked-gen-build-master|_      | | Server Extension:                        |
|                                                                          | | |execution-forked-gen-build-develop|_     | | |execution-forked-ext-badge-nuget|_      |
|                                                                          |                                             | | |execution-forked-ext-badge-nuget-pre|_  |
|                                                                          |                                             | | Runtime (for Console):                   |
|                                                                          |                                             | | |execution-forked-rt-badge-nuget|_       |
|                                                                          |                                             | | |execution-forked-rt-badge-nuget-pre|_   |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `InProcess Process Execution`_                                           | | |execution-inproc-build-master|           | | |execution-inproc-badge-nuget|           |
|                                                                          | | |execution-inproc-build-develop|          | | |execution-inproc-badge-nuget-pre|       |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `Restful Web API (+ Client)`_                                            | | |webapi-gen-build-master|_                | | Server Extension:                        |
|                                                                          | | |webapi-gen-build-develop|_               | | |webapi-ext-badge-nuget|_                |
|                                                                          |                                             | | |webapi-ext-badge-nuget-pre|_            |
|                                                                          |                                             | | Fully typed Rest-Client:                 |
|                                                                          |                                             | | |webapi-client-badge-nuget|_             |
|                                                                          |                                             | | |webapi-client-badge-nuget-pre|_         |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+



.. Images and Targets for the elements above

.. _Jobbr Server:                           https://github.com/JobbrIO/jobbr-server
.. _server-badge-build-master:              https://ci.appveyor.com/project/Jobbr/jobbr-server/branch/master
.. |server-badge-build-master|              image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-server/master.svg?label=master%20
.. _server-badge-build-develop:             https://ci.appveyor.com/project/Jobbr/jobbr-server/branch/develop
.. |server-badge-build-develop|             image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-server/develop.svg?label=develop
.. _server-badge-nuget:                     https://www.nuget.org/packages/Jobbr.Server
.. |server-badge-nuget|                     image::  https://img.shields.io/nuget/v/Jobbr.Server.svg?label=stable
.. _server-badge-nuget-pre:                 https://www.nuget.org/packages/Jobbr.Server
.. |server-badge-nuget-pre|                 image::  https://img.shields.io/nuget/vpre/Jobbr.Server.svg?label=pre%20%20%20%20

.. _MS SQL Storage Provider:                https://github.com/JobbrIO/jobbr-storage-mssql
.. _storage-mssql-badge-build-master:       https://ci.appveyor.com/project/Jobbr/jobbr-storage-mssql/branch/master
.. |storage-mssql-badge-build-master|       image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-mssql/master.svg?label=master%20
.. _storage-mssql-badge-build-develop:      https://ci.appveyor.com/project/Jobbr/jobbr-storage-mssql/branch/develop
.. |storage-mssql-badge-build-develop|      image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-mssql/develop.svg?label=develop
.. _storage-mssql-badge-nuget:              https://www.nuget.org/packages/Jobbr.Storage.MsSql
.. |storage-mssql-badge-nuget|              image::  https://img.shields.io/nuget/v/Jobbr.Storage.MsSql.svg?label=stable
.. _storage-mssql-badge-nuget-pre:          https://www.nuget.org/packages/Jobbr.Storage.MsSql
.. |storage-mssql-badge-nuget-pre|          image::  https://img.shields.io/nuget/vpre/Jobbr.Storage.MsSql.svg?label=pre%20%20%20%20

.. _FileSystem Artefact Provider:           https://github.com/JobbrIO/jobbr-artefactstorage-filesystem
.. _artefact-fs-badge-build-master:         https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-filesystem/branch/master
.. |artefact-fs-badge-build-master|         image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-filesystem/master.svg?label=master%20
.. _artefact-fs-badge-build-develop:        https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-filesystem/branch/develop
.. |artefact-fs-badge-build-develop|        image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-filesystem/develop.svg?label=develop
.. _artefact-fs-badge-nuget:                https://www.nuget.org/packages/Jobbr.ArtefactStorage.FileSystem
.. |artefact-fs-badge-nuget|                image::  https://img.shields.io/nuget/v/Jobbr.ArtefactStorage.FileSystem.svg?label=stable
.. _artefact-fs-badge-nuget-pre:            https://www.nuget.org/packages/Jobbr.ArtefactStorage.FileSystem
.. |artefact-fs-badge-nuget-pre|            image::  https://img.shields.io/nuget/vpre/Jobbr.ArtefactStorage.FileSystem.svg?label=pre%20%20%20%20

.. _RavenDB Storage Provider:               https://github.com/JobbrIO/jobbr-storage-ravendb
.. _storage-ravendb-badge-build-master:     https://ci.appveyor.com/project/Jobbr/jobbr-storage-ravendb/branch/master
.. |storage-ravendb-badge-build-master|     image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-ravendb/master.svg?label=master%20
.. _storage-ravendb-badge-build-develop:    https://ci.appveyor.com/project/Jobbr/jobbr-storage-ravendb/branch/develop
.. |storage-ravendb-badge-build-develop|    image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-storage-ravendb/develop.svg?label=develop
.. _storage-ravendb-badge-nuget:            https://www.nuget.org/packages/Jobbr.Storage.RavenDb
.. |storage-ravendb-badge-nuget|            image::  https://img.shields.io/nuget/v/Jobbr.Storage.RavenDb.svg?label=stable
.. _storage-ravendb-badge-nuget-pre:        https://www.nuget.org/packages/Jobbr.Storage.RavenDb
.. |storage-ravendb-badge-nuget-pre|        image::  https://img.shields.io/nuget/vpre/Jobbr.Storage.RavenDb.svg?label=pre%20%20%20%20

.. _RavenFS Artefact Provider:              https://github.com/JobbrIO/jobbr-artefactstorage-ravenfs
.. _artefact-ravenfs-badge-build-master:    https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-ravenfs/branch/master
.. |artefact-ravenfs-badge-build-master|    image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-ravenfs/master.svg?label=master%20
.. _artefact-ravenfs-badge-build-develop:   https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-ravenfs/branch/develop
.. |artefact-ravenfs-badge-build-develop|   image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-ravenfs/develop.svg?label=develop
.. _artefact-ravenfs-badge-nuget:           https://www.nuget.org/packages/Jobbr.ArtefactStorage.RavenFS
.. |artefact-ravenfs-badge-nuget|           image::  https://img.shields.io/nuget/v/Jobbr.ArtefactStorage.RavenFS.svg?label=stable
.. _artefact-ravenfs-badge-nuget-pre:       https://www.nuget.org/packages/Jobbr.ArtefactStorage.RavenFS
.. |artefact-ravenfs-badge-nuget-pre|       image::  https://img.shields.io/nuget/vpre/Jobbr.ArtefactStorage.RavenFS.svg?label=pre%20%20%20%20


.. _Forked Process Execution:               https://github.com/JobbrIO/jobbr-execution-forked 
.. _execution-forked-gen-build-master:      https://ci.appveyor.com/project/Jobbr/jobbr-execution-forked/branch/master   
.. |execution-forked-gen-build-master|      image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-forked/master.svg?label=master%20
.. _execution-forked-gen-build-develop:     https://ci.appveyor.com/project/Jobbr/jobbr-execution-forked/branch/develop
.. |execution-forked-gen-build-develop|     image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-forked/develop.svg?label=develop
.. _execution-forked-ext-badge-nuget:       https://www.nuget.org/packages/Jobbr.Execution.Forked
.. |execution-forked-ext-badge-nuget|       image::  https://img.shields.io/nuget/v/Jobbr.Execution.Forked.svg?label=stable
.. _execution-forked-ext-badge-nuget-pre:   https://www.nuget.org/packages/Jobbr.Execution.Forked
.. |execution-forked-ext-badge-nuget-pre|   image::  https://img.shields.io/nuget/vpre/Jobbr.Execution.Forked.svg?label=pre%20%20%20%20
.. _execution-forked-rt-badge-nuget:        https://www.nuget.org/packages/Jobbr.Runtime.ForkedExecution
.. |execution-forked-rt-badge-nuget|        image::  https://img.shields.io/nuget/v/Jobbr.Runtime.ForkedExecution.svg?label=stable
.. _execution-forked-rt-badge-nuget-pre:    https://www.nuget.org/packages/Jobbr.Runtime.ForkedExecution
.. |execution-forked-rt-badge-nuget-pre|    image::  https://img.shields.io/nuget/vpre/Jobbr.Runtime.ForkedExecution.svg?label=pre%20%20%20%20

.. _InProcess Process Execution:            https://github.com/JobbrIO/jobbr-execution-inproc 
.. _execution-inproc-build-master:          https://ci.appveyor.com/project/Jobbr/jobbr-execution-inproc/branch/master   
.. |execution-inproc-build-master|          image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-inproc/master.svg?label=master%20
.. _execution-inproc-build-develop:         https://ci.appveyor.com/project/Jobbr/jobbr-execution-inproc/branch/develop
.. |execution-inproc-build-develop|         image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-execution-inproc/develop.svg?label=develop
.. _execution-inproc-badge-nuget:           https://www.nuget.org/packages/Jobbr.Execution.InProc
.. |execution-inproc-badge-nuget|           image::  https://img.shields.io/nuget/v/Jobbr.Execution.InProc.svg?label=stable
.. _execution-inproc-badge-nuget-pre:       https://www.nuget.org/packages/Jobbr.Execution.InProc
.. |execution-inproc-badge-nuget-pre|       image::  https://img.shields.io/nuget/vpre/Jobbr.Execution.InProc.svg?label=pre%20%20%20%20


.. _Restful Web API (+ Client):             https://github.com/JobbrIO/jobbr-webapi 
.. _webapi-gen-build-master:                https://ci.appveyor.com/project/Jobbr/jobbr-webapi/branch/master   
.. |webapi-gen-build-master|                image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-webapi/master.svg?label=master%20
.. _webapi-gen-build-develop:               https://ci.appveyor.com/project/Jobbr/jobbr-webapi/branch/develop
.. |webapi-gen-build-develop|               image::  https://img.shields.io/appveyor/ci/Jobbr/jobbr-webapi/develop.svg?label=develop
.. _webapi-ext-badge-nuget:                 https://www.nuget.org/packages/Jobbr.Server.Webapi
.. |webapi-ext-badge-nuget|                 image::  https://img.shields.io/nuget/v/Jobbr.Server.WebAPI.svg?label=stable
.. _webapi-ext-badge-nuget-pre:             https://www.nuget.org/packages/Jobbr.Server.WebAPI
.. |webapi-ext-badge-nuget-pre|             image::  https://img.shields.io/nuget/vpre/Jobbr.Server.WebAPI.svg?label=pre%20%20%20%20
.. _webapi-client-badge-nuget:              https://www.nuget.org/packages/Jobbr.Client
.. |webapi-client-badge-nuget|              image::  https://img.shields.io/nuget/v/Jobbr.Client.svg?label=stable
.. _webapi-client-badge-nuget-pre:          https://www.nuget.org/packages/Jobbr.Client
.. |webapi-client-badge-nuget-pre|          image::  https://img.shields.io/nuget/vpre/Jobbr.Client.svg?label=pre%20%20%20%20

Component Models
----------------

The following packages define the contracts between Jobbr and extensions

+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| Component Model                                                          | Build                                       | Version                                    |
+==========================================================================+=============================================+============================================+
| `Jobbr Registration Component Model`_                                    | | |cm-registration-badge-build-master|_     | | |cm-registration-badge-nuget|_           |
|                                                                          | | |cm-registration-badge-build-develop|_    | | |cm-registration-badge-nuget-pre|_       |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `Execution Component Model`_                                             | | |cm-execution-badge-build-master|_        | | |cm-execution-badge-nuget|_              |
|                                                                          | | |cm-execution-badge-build-develop|_       | | |cm-execution-badge-nuget-pre|_          |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `Management Component Model`_                                            | | |cm-management-badge-build-master|_       | | |cm-management-badge-nuget|_             |
|                                                                          | | |cm-management-badge-build-develop|_      | | |cm-management-badge-nuget-pre|_         |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `JobStorage Component Model`_                                            | | |cm-jobstorage-badge-build-master|_       | | |cm-jobstorage-badge-nuget|_             |
|                                                                          | | |cm-jobstorage-badge-build-develop|_      | | |cm-jobstorage-badge-nuget-pre|_         |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+
| `Artefact Storage Component Model`_                                      | | |cm-artefactstorage-badge-build-master|_  | | |cm-artefactstorage-badge-nuget|_        |
|                                                                          | | |cm-artefactstorage-badge-build-develop|  | | |cm-artefactstorage-badge-nuget-pre|_    |
+--------------------------------------------------------------------------+---------------------------------------------+--------------------------------------------+


Roadmap
=======

The issues are split across the different repositories where they belong to. How-ever, the list if issues can still be aggregated for the whole organisation:

* `Open issues planned for 1.0`_
* `Open issues planned for 1.1`_
* `Open issues planned for vNext`_
* `Unplanned Issues`_
* `All open issues`_ (for all milestones)

.. _Open issues planned for 1.0:        https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+milestone%3A1.0+
.. _Open issues planned for 1.1:        https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+milestone%3A1.1+
.. _Open issues planned for vNext:      https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+milestone%3AvNext+
.. _Unplanned Issues:                   https://github.com/issues?utf8=%E2%9C%93&q=is%3Aopen+is%3Aissue+user%3AjobbrIO+no%3Amilestone+
.. _All open issues:                    https://github.com/issues?q=is%3Aopen+is%3Aissue+user%3AjobbrIO
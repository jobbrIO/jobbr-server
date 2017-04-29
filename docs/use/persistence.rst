Persistence
===========

There are two different categories of information that needs to be stored

* **JobStorage**: Job Information, Triggers, and Runs
* **Artefacts**: Files created during Run by the job

.. Note::
    You can combine different technologies to persist job related information (and triggers, runs, etc.) and Artefacts created b the jobruns.

The following implementations provide adapters for one or two of these categories

============  ===================================  ================================================
Technology    JobStorage                           Artefact storage
============  ===================================  ================================================
MSSQL Server  Yes, with `Jobbr.Storage.MsSQL`_     N / A
FileSystem    N/A                                  Yes, with `Jobbr.ArtefactStorage.FileSystem`_
Raven         Yes, with `Jobbr.Storage.RavenDB`_   Yes, with `Jobbr.ArtefactStorage.RavenFS`_
============  ===================================  ================================================

.. _Jobbr.Storage.MsSQL: https://github.com/jobbrIO/jobbr-storage-mssql
.. _Jobbr.ArtefactStorage.FileSystem: https://github.com/jobbrIO/jobbr-artefactstorage-filesystem
.. _Jobbr.Storage.RavenDB: https://github.com/jobbrIO/jobbr-storage-ravendb
.. _Jobbr.ArtefactStorage.RavenFS: https://github.com/jobbrIO/jobbr-artefactstorage-ravenfs

.. Note::
    **InMemory**: Without any extensions, the jobserver will fall back to InMemory versions shipped with with the `Jobbr.Server`-Package. 
    The InMemory implementations are a perfect fit for testing purposes but we don't recomment them for production scenarions. 
    Using them will write `WARN` log messages in your log.

MsSQL Server
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Filesystem
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


RavenDB / RavenFS
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
In contrast to the MsSQL Server and the Filestore implementation, Raven can be used to both store Job Informations and Artefacts.


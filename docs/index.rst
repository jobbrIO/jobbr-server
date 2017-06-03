Home of the Jobbr Documentation
===============================

Thank you for your interest on Jobbr, the .NET JobServer. Please head to the 
GitHub Organisation page on https://github.com/JobbrIO to find the main repo and extensions.

We created a super small set of articles to start with where you can learn how Jobbr will help you implementing background jobs more efficently 
and why we decided it's time for an(other) embeddable JobServer in C#.

.. toctree::
   :maxdepth: 1

   intro/welcome
   intro/features
   intro/architecture
   intro/status

If you consider using Jobbr in your .NET Project please read the following capters which cover the API that is provided for you as a developer.
It also contains suggestions how to deal with certain requirements. 

  
.. toctree::
   :maxdepth: 2
   :caption: Library Documentation

   use/first-start
   use/createJob
   use/triggers
   use/execution
   use/restApi
   use/persistence
   use/recommondations

It might happen that some single feature is not covered by the current version. If you plan to extend Jobbr 
its important to know where and what the architectural decisions where in the past so that you can understand and align with them.

This section also convers how we release and how version bumps are made.

.. toctree::
   :maxdepth: 2
   :caption: Developer Documentation

   dev/contribution
   dev/setup
   dev/extend
   dev/knowledgebase

.. toctree::
   :maxdepth: 2
   :caption: Project

   proj/project

.. toctree::
   :maxdepth: 1
   :glob:
   :caption: Meeting Notes

   proj/meetings/*

# ServiceFabricGuidance
patterns &amp; practices Service Fabric Guidance

patterns &practices team is working on Service Fabric guidance. It will highlight the benefit of microservices architecture and its own feature set in two steps.
 
-          Step 1: Move existing code base from Cloud Service. We’ll move TailSpin survey application. (See description at bottom)
-          Step 2: Optimize TailSpin to Service Fabric.
-          
 
TailSpin application is a multi-tenant survey application in which tenants can create/analyze surveys while public users can fill surveys. Here’s the scenario behind this application. It runs on Web/Worker role with SQL DB and Azure storage (see attached architecture diagram). It’s a canonical application that most of Azure developers have built 5 years ago. We’ll use this app to mitigate our development effort as well as to illustrate the migration path from Cloud Services to the latest compute options.
 

# Migrate an Azure Cloud Services application to Azure Service Fabric
Microsoft patterns &amp; practices 

https://docs.microsoft.com/azure/architecture/service-fabric

patterns &practices team is working on Service Fabric guidance. It will highlight the benefit of microservices architecture and its own feature set in two steps.
 
1. Move existing code base from Cloud Service. 
2. Optimize TailSpin to Service Fabric.
 
We’ll move TailSpin survey application which is a multi-tenant survey application in which tenants can create/analyze surveys while public users can fill surveys. Here’s the scenario behind this application. It runs on Web/Worker role with SQL DB and Azure storage (see attached architecture diagram). It’s a canonical application that most of Azure developers have built 4 years ago. We’ll use this app to illustrate the migration path from Cloud Services to Service Fabric.
 
 ---
 
 This project has adopted the Microsoft Open Source Code of Conduct. For more information see the Code of Conduct FAQ or contact opencode@microsoft.com with any additional questions or comments.

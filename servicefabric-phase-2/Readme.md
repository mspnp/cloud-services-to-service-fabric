# ServiceFabricGuidance
## patterns &amp; practices Service Fabric Guidance Phase 2

Tailspin Surveys Application is decomposed into microservices architecture. 

It highlights the benefits of microservices deployed in Microsoft Azure Service Fabric. 

Each microservice lives in it's own application project. Below is a brief description of the code layout:

|   Project                         |   Description                                                             |
|   ------------------------------  |---------------------------------------------------------------------------|
|   Tailspin                        | Admin and public web front ends with shared library and tests.            |
|   Tailspin.SurveyManagementService| Survey management service that allows users to create and publish surveys.|
|   Tailspin.SurveyAnswerService    | Survey answer service that allows users to select and answer surveys.     |
|   Tailspin.SurveyAnalysisService  | Survey analysis service that allows users to analyze survey responses.    |
 

 

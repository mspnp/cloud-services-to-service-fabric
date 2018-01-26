# ServiceFabricGuidance
## patterns &amp; practices Service Fabric Guidance Phase 2

Tailspin Surveys Application is decomposed into smaller services. 

Below is a brief description of the projects in the solution:

|   Project                         |   Description                                                             |
|   ------------------------------  |---------------------------------------------------------------------------|
|   Tailspin.Web                    | Admin web front end that provides management of surveys.                  |
|   Tailspin.Web.Survey.Public      | Public web front end that allows users to fill out surveys.               |
|   Tailspin.SurveyManagementService| Service that allows users to create and publish surveys.                  |
|   Tailspin.SurveyAnswerService    | Service that retrieves survey answers and analysis.                       |
|   Tailspin.SurveyAnalysisService  | Service that merges survey responses with analysis summaries.             |
|   Tailspin.SurveyResponseService  | Service that saves responses to blob storage and merges with summaries.   |

 

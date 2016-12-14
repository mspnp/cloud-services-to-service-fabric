//===============================================================================
// Microsoft patterns & practices
// Windows Azure Architecture Guide
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://wag.codeplex.com/license)
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.Win32.Dialogs;

using ArtOfTest.WebAii.Silverlight;
using ArtOfTest.WebAii.Silverlight.UI;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace TailspinFunctionalTests
{
    /// <summary>
    /// Summary description for WebAiiVSUnitTest1
    /// </summary>
    [TestClass]
    public class TailspinTests : BaseTest
    {
        ResourceManager resManager;
        private string baseUrl;
        private string tagnameA;
        private string tagnameStrong;
        private string tagnameListItem;
        private string tagNameH2;
        private string tagNameH1;
        private string textcontent;
        private string innerText;
        private string mySurveyLink;
        private string tailSpinPageTitle;
        private string forwardSlash;
        private string tagnameTD;
        private string tagnameSpan;
        private string mandatoryQuestion;
        private string mandatoryTitle;
        private string mandatoryAnswer;
        private string thankyou;
        private string finishsurveyText;
        private string newSurvey;
        private string current;
        private string newsurveyUrl;
        private string createButton;
        private string finishButton;
        private string titleText;
        private string questionText;
        private string addToSurveyButton;
        private string addQuestionLink;
        private string newQuestion;
        private string selectQuestionType;
        private string quesTypeSimpleText;
        private string quesTypeMultipleChoice;
        private string quesTypeRating;
        private string question1;
        private string value;
        private string answer1;
        private string answer2;
        private string answer3;
        private string answer4;
        private string star1;
        private string star2;
        private string star3;
        private string star4;
        private string star5;
        private string signout;
        private string enterCheck;
        private string tagNameH3;
        private string noSurveys;
        private string fabrikam;
        private string mary;
        private string mySurveyLinkFabrikam;
        private string newsurveyUrlFabrikam;
        private string simulatedIssuerPageTitleFabrikam;
        private string simulatedIssuerUrlFabrikam;
        private string surveyAlreadyPresent1;
        private string surveyAlreadyPresent2;
        private string analyze;


        #region [Setup / TearDown]

        private TestContext testContextInstance = null;
        /// <summary>
        ///Gets or sets the VS test context which provides
        ///information about and functionality for the
        ///current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {

        }



        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            #region WebAii Initialization

            // Initializes WebAii manager to be used by the test case.
            // If a WebAii configuration section exists, settings will be
            // loaded from it. Otherwise, will create a default settings
            // object with system defaults.
            //
            // Note: We are passing in a delegate to the VisualStudio
            // testContext.WriteLine() method in addition to the Visual Studio
            // TestLogs directory as our log location. This way any logging
            // done from WebAii (i.e. Manager.Log.WriteLine()) is
            // automatically logged to the VisualStudio test log and
            // the WebAii log file is placed in the same location as VS logs.
            //
            // If you do not care about unifying the log, then you can simply
            // initialize the test by calling Initialize() with no parameters;
            // that will cause the log location to be picked up from the config
            // file if it exists or will use the default system settings (C:\WebAiiLog\)
            // You can also use Initialize(LogLocation) to set a specific log
            // location for this test.

            // Pass in 'true' to recycle the browser between test methods
            Initialize(false, this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            /*

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.DefaultBrowser = BrowserType.FireFox;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

            */

            // Set the current test method. This is needed for WebAii to discover
            // its custom TestAttributes set on methods and classes.
            // This method should always exist in [TestInitialize()] method.
            SetTestMethod(this, (string)TestContext.Properties["TestName"]);
            GetResourceStrings();
            #endregion

            //
            // Place any additional initialization here
            //

        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

            //
            // Place any additional cleanup here
            //

            #region WebAii CleanUp

            // Shuts down WebAii manager and closes all browsers currently running
            // after each test. This call is ignored if recycleBrowser is set
            this.CleanUp();

            #endregion
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            // This will shut down all browsers if
            // recycleBrowser is turned on. Else
            // will do nothing.
            ShutDown();
        }

        #endregion

        #region -- GETRESOURCESTRINGS --
        public void GetResourceStrings()
        {
            resManager = new ResourceManager("TailspinFunctionalTests.Properties.TailspinResource", Assembly.GetExecutingAssembly());
            baseUrl = resManager.GetString("baseUrl");
            tagnameA = resManager.GetString("tagnameA");
            tagnameStrong = resManager.GetString("tagnameStrong");
            tagnameListItem = resManager.GetString("tagnameListItem");
            tagNameH2 = resManager.GetString("tagNameH2");
            tagNameH1 = resManager.GetString("tagNameH1");
            tagNameH3 = resManager.GetString("tagNameH3");
            textcontent = resManager.GetString("textcontent");
            innerText = resManager.GetString("innerText");
            mySurveyLink = resManager.GetString("mySurveyLink");
            tailSpinPageTitle = resManager.GetString("tailSpinPageTitle");
            forwardSlash = resManager.GetString("forwardSlash");
            tagnameTD = resManager.GetString("tagnameTD");
            tagnameSpan = resManager.GetString("tagnameSpan");
            mandatoryQuestion = resManager.GetString("mandatoryQuestion");
            mandatoryTitle = resManager.GetString("mandatoryTitle");
            mandatoryAnswer = resManager.GetString("mandatoryAnswer");
            thankyou = resManager.GetString("thankyou");
            finishsurveyText = resManager.GetString("finishsurveyText");
            newSurvey = resManager.GetString("newSurvey");
            current = resManager.GetString("current");
            newsurveyUrl = resManager.GetString("newsurveyUrl");
            createButton = resManager.GetString("createButton");
            finishButton = resManager.GetString("finishButton");
            titleText = resManager.GetString("titleText");
            questionText = resManager.GetString("questionText");
            addToSurveyButton = resManager.GetString("addToSurveyButton");
            addQuestionLink = resManager.GetString("addQuestionLink");
            newQuestion = resManager.GetString("newQuestion");
            selectQuestionType = resManager.GetString("selectQuestionType");
            quesTypeSimpleText = resManager.GetString("quesTypeSimpleText");
            quesTypeMultipleChoice = resManager.GetString("quesTypeMultipleChoice");
            quesTypeRating = resManager.GetString("quesTypeRating");
            question1 = resManager.GetString("question1");
            value = resManager.GetString("value");
            answer1 = resManager.GetString("answer1");
            answer2 = resManager.GetString("answer2");
            answer3 = resManager.GetString("answer3");
            answer4 = resManager.GetString("answer4");
            star1 = resManager.GetString("star1");
            star2 = resManager.GetString("star2");
            star3 = resManager.GetString("star3");
            star4 = resManager.GetString("star4");
            star5 = resManager.GetString("star5");
            signout = resManager.GetString("signout");
            enterCheck = resManager.GetString("enterCheck");
            noSurveys = resManager.GetString("noSurveys");
            fabrikam = resManager.GetString("fabrikam");
            mary = resManager.GetString("mary");
            mySurveyLinkFabrikam = resManager.GetString("mySurveyLinkFabrikam");
            newsurveyUrlFabrikam = resManager.GetString("newsurveyUrlFabrikam");
            simulatedIssuerPageTitleFabrikam = resManager.GetString("simulatedIssuerPageTitleFabrikam");
            simulatedIssuerUrlFabrikam = resManager.GetString("simulatedIssuerUrlFabrikam");
            surveyAlreadyPresent1 = resManager.GetString("surveyAlreadyPresent1");
            surveyAlreadyPresent2 = resManager.GetString("surveyAlreadyPresent2");
            analyze = resManager.GetString("analyze");
        }
        #endregion

        #region -- ADATUMTESTS --
        /// <summary>
        /// Enables user to login
        /// </summary>
        public void LoginAdatum()
        {

            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo(baseUrl);
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(tailSpinPageTitle, StringComparison.OrdinalIgnoreCase));

            //HtmlAnchor a = ActiveBrowser.Find.bye( href="/survey/fabrikam"
            // Click on adatum
            HtmlAnchorFieldByExpression(tagnameA, "href=" + resManager.GetString("adatum")).Click();

            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Adatum Issuer Page
            Assert.IsTrue(ActiveBrowser.Url.Contains(resManager.GetString("simulatedIssuerUrl")));
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(resManager.GetString("simulatedIssuerPageTitle"), StringComparison.OrdinalIgnoreCase));

            // To Continue with login
            HtmlInputRadioButtonField(resManager.GetString("loginJohndoe")).Check(true, true);
            HtmlInputSubmitField(resManager.GetString("continueButton")).Click();
            Wait.CheckInterval = 3000;
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
        }

        /// <summary>
        /// To check if SIGNOUT link is displayed
        /// </summary>
        [TestMethod]
        public void CheckSignOutLinkOnLoginAdatum()
        {

            LoginAdatum();

            //Check for signed in name and SignOut  link
            Assert.AreEqual(resManager.GetString("johnDoe"), Find.ByExpression(tagnameStrong, textcontent + resManager.GetString("johnDoe"))
                .InnerText);
            HtmlAnchorFieldByExpression(tagnameA, textcontent + signout);
        }

        /// <summary>
        /// To check if the default tab is 'My Surveys'
        /// </summary>
        [TestMethod]
        public void CheckDefaultTabMySurveysInAdatum()
        {
            string mySurveys = resManager.GetString("mySurveys");
            string myAccount = resManager.GetString("myAccount");

            LoginAdatum();

            //Tabs displayed
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + newSurvey));
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + mySurveys));
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + myAccount));

            //Default selected tab is 'My Surveys'
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + mySurveys).CssClassAttributeValue, current, true);
        }

        /// <summary>
        /// To check creation of new survey
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyInAdatum()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click on NewSurvey tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Enter title and click on Create button
            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
            HtmlInputSubmitField(createButton).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
        }


        /// <summary>
        /// To check if TITLE is a mandatory field while creating a survey
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyTitleMandatoryInAdatum()
        {
            LoginAdatum();

            // Click on NewSurvey tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Click on Create button without entering any field for Title          
            HtmlInputSubmitField(createButton).Click();
            // Check for mandatory field
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryTitle));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
        }


        /// <summary>
        /// To check functionality of FINISH button when no question provided
        /// </summary>
        //[TestMethod]
        //public void ClickFinishWithoutQuestionsInAdatum()
        //{    
        //    Random random = new Random();
        //    string strRandom = random.Next().ToString();
        //    strRandom = "Test" + strRandom;

        //    LoginAdatum();

        //    // Click on NewSurvey tab
        //    HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
        //    Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

        //    // Enter title and click on Create button
        //    HtmlInputTextField(titleText).Text = strRandom;
        //    HtmlInputSubmitField(createButton).Click(); 

        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
        //    Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
        //    HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
        //    Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));

        //    // Click on  FINISH button
        //    HtmlInputSubmitField(finishButton).Click();
        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
        //    Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        //}

        /// <summary>
        /// To check functionality of FINISH button when no answer provided
        /// </summary>
        [TestMethod]
        public void ClickFinishWithoutAnswerInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));

            Wait.CheckInterval = 3000;

            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Click on FINISH button
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryAnswer));
        }


        /// <summary>
        /// To check creation of survey with questions
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyWithQuestionsInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));
        }

        /// <summary>
        /// To check creation of questions with SimpleText TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeSimpleTextInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

        }

        /// <summary>
        /// To check creation of questions with MultipleChoice TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeMultipleChoiceInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;


            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            Find.ById<HtmlTextArea>(resManager.GetString("answerTextArea")).Text = answer1 + Environment.NewLine
                + answer2 + Environment.NewLine
                + answer3 + Environment.NewLine
                + answer4;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if answers are present as RADIOBUTTONS
            // Check if answers are present as RADIOBUTTONS
            var ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck);
            if (ele == null)
            {
                ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1);
            }
            var ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck);
            if (ele1 == null)
            {
                ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2);
            }
            var ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck);
            if (ele2 == null)
            {
                ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            var ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4 + enterCheck);
            if (ele3 == null)
            {
                ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            Assert.IsNotNull(ele);
            Assert.IsNotNull(ele1);
            Assert.IsNotNull(ele2);
            Assert.IsNotNull(ele3);
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer4));

        }


        /// <summary>
        /// To check creation of questions with RATING TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeRatingInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeRating);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if rating stars present
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star1));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star2));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star3));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star4));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star5));
        }

        /// <summary>
        /// To check functionality of CANCEL link
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionCancelledInAdatum()
        {
            string cancelLink = resManager.GetString("cancelLink");

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlAnchorFieldByExpression(tagnameA, textcontent + cancelLink).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));

            // The question should be present in newsurvey page
            Assert.IsNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
        }

        /// <summary>
        /// To check if QUESTION is a mandatory field
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionMandatoryInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Click on AddTosurvey button without entering question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Check for mandatory field
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryQuestion));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion + forwardSlash + resManager.GetString("add")));
        }

        /// <summary>
        /// To add answer to the already created SimpleText question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForSimpleTextAndFinishInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Enter answer and click on FINISH
            Find.ById<HtmlTextArea>(resManager.GetString("answerSimpleTextFinish")).Text = "Answer for" + strRandom + question1;
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To add answer to the already created MultipleChoice question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForMultipleChoiceAndFinishInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            Find.ById<HtmlTextArea>(resManager.GetString("answerMultippleChoiceFinish")).Text = answer1 + Environment.NewLine
                + answer2 + Environment.NewLine
                + answer3 + Environment.NewLine
                + answer4;

            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();
  
            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

          
            // Goto survey and check if Question present
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if answers are present as RADIOBUTTONS
            var ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck);
            if (ele == null)
            {
                ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1);
            }
            var ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck);
            if (ele1 == null)
            {
                ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2);
            }
            var ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck);
            if (ele2 == null)
            {
                ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            var ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4 + enterCheck);
            if (ele3 == null)
            {
                ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            Assert.IsNotNull(ele);
            Assert.IsNotNull(ele1);
            Assert.IsNotNull(ele2);
            Assert.IsNotNull(ele3);
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer4));

            // Select an answer
            ele3.Click();
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To add answer to the already created RATINGS question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForRatingAndFinishInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeRating);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if rating stars present
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star1));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star2));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star3));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star4));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star5));

            // Select a rating
            Find.ByExpression<HtmlInputRadioButton>(value + star4).Click();
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To check functionality of DESIGN column
        /// </summary>
        //[TestMethod]
        public void ClickDesignInAdatum()
        {

        }

        /// <summary>
        /// To check functionality of ANALYZE column
        /// </summary>
        [TestMethod]
        public void ClickAnalyzeInAdatum()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
            HtmlInputSubmitField(createButton).Click();
            string analyzeUrl = forwardSlash + mySurveyLink + forwardSlash + strRandom + forwardSlash + analyze;

            // Click on analyze
            HtmlAnchorFieldByExpression(tagnameA, resManager.GetString("href") + analyzeUrl).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(analyzeUrl.ToLower()));
        }

        /// <summary>
        /// To check deletion of survey
        /// </summary>
        [TestMethod]
        public void DeleteSurveyInAdatum()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            HtmlInputSubmitField(createButton).Click();

            // delete the created survey
            HtmlAnchorFieldByExpression(tagnameA, resManager.GetString("deleteLink1") + strRandom + resManager.GetString("deleteLink2")).Click();
            Assert.IsNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
        }

        [TestMethod]
        public void NoSurveysInAdatum()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Delete any existing surveys
            while (HtmlAnchorFieldByExpression(tagnameA, textcontent + "Delete") != null)
            {
                HtmlAnchorFieldByExpression(tagnameA, textcontent + "Delete").Click();
            }

            Assert.IsNotNull(Find.ByExpression(tagNameH3, textcontent + noSurveys));
        }

        [TestMethod]
        public void CreateSurveyWithAlreadyExistingTitleInAdatum()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add same Title and click on Create button
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            HtmlInputSubmitField(createButton).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + surveyAlreadyPresent1 + strRandom + surveyAlreadyPresent2));

        }

        /// <summary>
        /// To check creation of questions with MultipleChoice TYPE needs answer field to be filled
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeMultipleChoiceWithNoAnswerInAdatum()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;


            LoginAdatum();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrl));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice' and click on 'Add to Survey' without entering answer
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            HtmlInputSubmitField(addToSurveyButton).Click();
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + resManager.GetString("mandatoryAnswerMultipleChoice")));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLink + forwardSlash + newQuestion + forwardSlash + resManager.GetString("add")));

        }


        /// <summary>
        /// To check functionality of SIGNOUT 
        /// </summary>
        [TestMethod]
        public void SignOutInAdatum()
        {

            LoginAdatum();

            //SignOut
            HtmlAnchorFieldByExpression(tagnameA, textcontent + signout).Click();
            Assert.IsTrue(ActiveBrowser.Url.Contains(resManager.GetString("tailSpinSimulatedIssuerUrl")));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + resManager.GetString("backToTailspinText")).Click();
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(tailSpinPageTitle, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region -- fabrikamTESTS --
        /// <summary>
        /// Enables user to login
        /// </summary>
        public void Loginfabrikam()
        {

            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo(baseUrl);
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(tailSpinPageTitle, StringComparison.OrdinalIgnoreCase));

            // Click on fabrikam
            HtmlAnchorFieldByExpression(tagnameA, "href=" + resManager.GetString("fabrikam")).Click();

            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // fabrikam Issuer Page
            Assert.IsTrue(ActiveBrowser.Url.Contains(resManager.GetString("simulatedIssuerUrlFabrikam")));
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(resManager.GetString("simulatedIssuerPageTitleFabrikam"), StringComparison.OrdinalIgnoreCase));

            // To Continue with login
            HtmlInputRadioButtonField(resManager.GetString("loginJohndoe")).Check(true, true);
            HtmlInputSubmitField(resManager.GetString("continueButton")).Click();
            Wait.CheckInterval = 3000;
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
        }

        /// <summary>
        /// To check if SIGNOUT link is displayed
        /// </summary>
        [TestMethod]
        public void CheckSignOutLinkOnLoginfabrikam()
        {

            Loginfabrikam();

            //Check for signed in name and SignOut  link
            Assert.AreEqual(resManager.GetString("mary"), Find.ByExpression(tagnameStrong, textcontent + resManager.GetString("mary"))
                .InnerText);
            HtmlAnchorFieldByExpression(tagnameA, textcontent + signout);
        }

        /// <summary>
        /// To check if the default tab is 'My Surveys'
        /// </summary>
        [TestMethod]
        public void CheckDefaultTabMySurveysInfabrikam()
        {
            string mySurveys = resManager.GetString("mySurveys");
            string myAccount = resManager.GetString("myAccount");

            Loginfabrikam();

            //Tabs displayed
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + newSurvey));
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + mySurveys));
            Assert.IsNotNull(Find.ByExpression(tagnameListItem, innerText + myAccount));

            //Default selected tab is 'My Surveys'
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + mySurveys).CssClassAttributeValue, current, true);
        }

        /// <summary>
        /// To check creation of new survey
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyInfabrikam()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click on NewSurvey tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Enter title and click on Create button
            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
            HtmlInputSubmitField(createButton).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
        }


        /// <summary>
        /// To check if TITLE is a mandatory field while creating a survey
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyTitleMandatoryInfabrikam()
        {
            Loginfabrikam();

            // Click on NewSurvey tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Click on Create button without entering any field for Title          
            HtmlInputSubmitField(createButton).Click();
            // Check for mandatory field
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryTitle));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
        }


        ///// <summary>
        ///// To check functionality of FINISH button when no question provided
        ///// </summary>
        //[TestMethod]
        //public void ClickFinishWithoutQuestionsInfabrikam()
        //{
        //    Random random = new Random();
        //    string strRandom = random.Next().ToString();
        //    strRandom = "Test" + strRandom;

        //    Loginfabrikam();

        //    // Click on NewSurvey tab
        //    HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
        //    Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

        //    // Enter title and click on Create button
        //    HtmlInputTextField(titleText).Text = strRandom;
        //    HtmlInputSubmitField(createButton).Click();

        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
        //    Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
        //    HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
        //    Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));

        //    // Click on  FINISH button
        //    HtmlInputSubmitField(finishButton).Click();
        //    Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
        //    Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        //}

        /// <summary>
        /// To check functionality of FINISH button when no answer provided
        /// </summary>
        [TestMethod]
        public void ClickFinishWithoutAnswerInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));

            Wait.CheckInterval = 3000;

            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Open up Survey.Public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();
            //Assert.IsTrue(ActiveBrowser.PageTitle.Equals(tailSpinPageTitle, StringComparison.OrdinalIgnoreCase));

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Click on FINISH button
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryAnswer));
        }


        /// <summary>
        /// To check creation of survey with questions
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyWithQuestionsInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));
        }

        /// <summary>
        /// To check creation of questions with SimpleText TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeSimpleTextInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

        }

        /// <summary>
        /// To check creation of questions with MultipleChoice TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeMultipleChoiceInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;


            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            Find.ById<HtmlTextArea>(resManager.GetString("answerTextArea")).Text = answer1 + Environment.NewLine
                + answer2 + Environment.NewLine
                + answer3 + Environment.NewLine
                + answer4;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if answers are present as RADIOBUTTONS
            // Check if answers are present as RADIOBUTTONS
            var ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck);
            if (ele == null)
            {
                ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1);
            }
            var ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck);
            if (ele1 == null)
            {
                ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2);
            }
            var ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck);
            if (ele2 == null)
            {
                ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            var ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4 + enterCheck);
            if (ele3 == null)
            {
                ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            Assert.IsNotNull(ele);
            Assert.IsNotNull(ele1);
            Assert.IsNotNull(ele2);
            Assert.IsNotNull(ele3);
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer4));

            // Select an answer
            ele3.Click();
        }


        /// <summary>
        /// To check creation of questions with RATING TYPE 
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeRatingInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeRating);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if rating stars present
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star1));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star2));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star3));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star4));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star5));
        }

        /// <summary>
        /// To check functionality of CANCEL link
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionCancelledInfabrikam()
        {
            string cancelLink = resManager.GetString("cancelLink");

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlAnchorFieldByExpression(tagnameA, textcontent + cancelLink).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));

            // The question should be present in newsurvey page
            Assert.IsNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
        }

        /// <summary>
        /// To check if QUESTION is a mandatory field
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionMandatoryInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Click on AddTosurvey button without entering question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Check for mandatory field
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + mandatoryQuestion));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion + forwardSlash + resManager.GetString("add")));
        }

        /// <summary>
        /// To add answer to the already created SimpleText question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForSimpleTextAndFinishInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Simple Text'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeSimpleText);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Enter answer and click on FINISH
            Find.ById<HtmlTextArea>(resManager.GetString("answerSimpleTextFinish")).Text = "Answer for" + strRandom + question1;
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To add answer to the already created MultipleChoice question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForMultipleChoiceAndFinishInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            Find.ById<HtmlTextArea>(resManager.GetString("answerMultippleChoiceFinish")).Text = answer1 + Environment.NewLine
                + answer2 + Environment.NewLine
                + answer3 + Environment.NewLine
                + answer4;

            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if answers are present as RADIOBUTTONS
            var ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck);
            if (ele == null)
            {
                ele = Find.ByExpression<HtmlInputRadioButton>(value + answer1);
            }
            var ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck);
            if (ele1 == null)
            {
                ele1 = Find.ByExpression<HtmlInputRadioButton>(value + answer2);
            }
            var ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck);
            if (ele2 == null)
            {
                ele2 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            var ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4 + enterCheck);
            if (ele3 == null)
            {
                ele3 = Find.ByExpression<HtmlInputRadioButton>(value + answer4);
            }
            Assert.IsNotNull(ele);
            Assert.IsNotNull(ele1);
            Assert.IsNotNull(ele2);
            Assert.IsNotNull(ele3);
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer1 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer2 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer3 + enterCheck));
            //Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + answer4));

            // Select an answer
            ele3.Click();
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To add answer to the already created RATINGS question
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyAddAnswerForRatingAndFinishInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice'
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeRating);
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            //Open up public site
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            ActiveBrowser.NavigateTo("http://127.0.0.1:81");
            while (HtmlAnchorFieldById(resManager.GetString("overridelink")) != null)
                HtmlAnchorFieldById(resManager.GetString("overridelink")).Click();

            // Click on survey
            //Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom).Click();

            // Goto survey and check if Question present
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower()));
            Assert.IsNotNull(Find.ByExpression(tagNameH1, textcontent + strRandom));
            Assert.IsNotNull(HtmlDivFieldByExpression(textcontent + strRandom + question1));

            // Check if rating stars present
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star1));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star2));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star3));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star4));
            Assert.IsNotNull(Find.ByExpression<HtmlInputRadioButton>(value + star5));

            // Select a rating
            Find.ByExpression<HtmlInputRadioButton>(value + star4).Click();
            HtmlInputSubmitField(finishButton).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + strRandom.ToLower() + forwardSlash + thankyou));
            Assert.IsNotNull(Find.ByExpression(tagNameH2, textcontent + finishsurveyText));
        }

        /// <summary>
        /// To check functionality of DESIGN column
        /// </summary>
        //[TestMethod]
        public void ClickDesignInfabrikam()
        {

        }

        /// <summary>
        /// To check functionality of ANALYZE column
        /// </summary>
        [TestMethod]
        public void ClickAnalyzeInfabrikam()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
            HtmlInputSubmitField(createButton).Click();

            string analyzeUrl = forwardSlash + mySurveyLinkFabrikam + forwardSlash + strRandom + forwardSlash + analyze;

            // Click on analyze
            HtmlAnchorFieldByExpression(tagnameA, resManager.GetString("href") + analyzeUrl).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(analyzeUrl.ToLower()));
        }

        /// <summary>
        /// To check deletion of survey
        /// </summary>
        [TestMethod]
        public void DeleteSurveyInfabrikam()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));
            HtmlInputSubmitField(createButton).Click();

            // delete the created survey
            HtmlAnchorFieldByExpression(tagnameA, resManager.GetString("deleteLinkFabrikam") + strRandom + resManager.GetString("deleteLink2")).Click();
            Assert.IsNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
        }

        [TestMethod]
        public void NoSurveysInfabrikam()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Delete any existing surveys
            while (HtmlAnchorFieldByExpression(tagnameA, textcontent + "Delete") != null)
            {
                HtmlAnchorFieldByExpression(tagnameA, textcontent + "Delete").Click();
            }

            Assert.IsNotNull(Find.ByExpression(tagNameH3, textcontent + noSurveys));
        }

        [TestMethod]
        public void CreateSurveyWithAlreadyExistingTitleInfabrikam()
        {
            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;

            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();

            // Redirected to NewSurvey page
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Wait.CheckInterval = 3000;
            // The question should be present in newsurvey page
            Assert.IsNotNull(Find.ByExpression(tagnameTD, textcontent + strRandom + question1));

            // Click on Create button
            HtmlInputSubmitField(createButton).Click();

            // Click on survey
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam));
            Assert.IsNotNull(HtmlAnchorFieldByExpression(tagnameA, textcontent + strRandom));

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add same Title and click on Create button
            HtmlInputTextField(titleText).Text = strRandom;
            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;
            HtmlInputSubmitField(addToSurveyButton).Click();
            HtmlInputSubmitField(createButton).Click();

            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + surveyAlreadyPresent1 + strRandom + surveyAlreadyPresent2));

        }

        /// <summary>
        /// To check creation of questions with MultipleChoice TYPE needs answer field to be filled
        /// </summary>
        [TestMethod]
        public void CreateNewSurveyQuestionTypeMultipleChoiceWithNoAnswerInfabrikam()
        {

            Random random = new Random();
            string strRandom = random.Next().ToString();
            strRandom = "Test" + strRandom;


            Loginfabrikam();

            // Click 'New Survey' tab
            HtmlAnchorFieldByExpression(tagnameA, textcontent + newSurvey).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(newsurveyUrlFabrikam));
            Assert.AreEqual(Find.ByExpression(tagnameListItem, innerText + newSurvey).CssClassAttributeValue, current, true);

            // Add Title
            HtmlInputTextField(titleText).Text = strRandom;

            // Add Question
            HtmlAnchorFieldByExpression(tagnameA, textcontent + addQuestionLink).Click();
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion));
            HtmlInputTextField(questionText).Text = strRandom + question1;

            // select QuestionType as 'Multiple Choice' and click on 'Add to Survey' without entering answer
            Find.ById<HtmlSelect>(selectQuestionType).SelectByValue(quesTypeMultipleChoice);
            HtmlInputSubmitField(addToSurveyButton).Click();
            Assert.IsNotNull(Find.ByExpression(tagnameSpan, textcontent + resManager.GetString("mandatoryAnswerMultipleChoice")));
            Assert.IsTrue(ActiveBrowser.Url.EndsWith(mySurveyLinkFabrikam + forwardSlash + newQuestion + forwardSlash + resManager.GetString("add")));

        }


        /// <summary>
        /// To check functionality of SIGNOUT 
        /// </summary>
        [TestMethod]
        public void SignOutInfabrikam()
        {

            Loginfabrikam();

            //SignOut
            HtmlAnchorFieldByExpression(tagnameA, textcontent + signout).Click();
            Assert.IsTrue(ActiveBrowser.Url.Contains(resManager.GetString("tailSpinSimulatedIssuerUrl")));
            HtmlAnchorFieldByExpression(tagnameA, textcontent + resManager.GetString("backToTailspinText")).Click();
            Assert.IsTrue(ActiveBrowser.PageTitle.Equals(tailSpinPageTitle, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        public HtmlAnchor HtmlAnchorFieldById(string id)
        {
            return Find.ById<HtmlAnchor>(id);
        }

        public HtmlAnchor HtmlAnchorFieldByExpression(params string[] expression)
        {
            return Find.ByExpression<HtmlAnchor>(expression);
        }

        public HtmlDiv HtmlDivFieldByExpression(params string[] expression)
        {
            return Find.ByExpression<HtmlDiv>(expression);
        }

        public HtmlInputRadioButton HtmlInputRadioButtonField(string id)
        {
            return Find.ById<HtmlInputRadioButton>(id);
        }

        public HtmlInputSubmit HtmlInputSubmitField(string id)
        {
            return Find.ById<HtmlInputSubmit>(id);
        }

        public HtmlInputText HtmlInputTextField(string id)
        {
            return Find.ById<HtmlInputText>(id);
        }

    }
}

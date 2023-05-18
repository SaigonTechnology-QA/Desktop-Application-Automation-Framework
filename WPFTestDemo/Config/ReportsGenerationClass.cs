using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FlaUI.Core.Capturing;

namespace WPFTestDemo.Config
{
    [SetUpFixture]
    public abstract class ReportsGenerationClass
    {
        protected ExtentReports _extent;
        protected ExtentTest _test;

        [OneTimeSetUp]
        protected void Setup()
        {
            var path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = path.Substring(0, path.LastIndexOf("bin"));
            var projectPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(projectPath.ToString() + "Reports");
            var reportPath = projectPath + "Reports\\ExtentReport.html";
            var htmlReporter = new ExtentHtmlReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Environment", GlobalVariable.ENVIRONMENT);
            _extent.AddSystemInfo("UserName", GlobalVariable.USER);
        }
        [OneTimeTearDown]
        protected void TearDown()
        {
            _extent.Flush();
        }
        [SetUp]
        public void BeforeTest()
        {
        }
        [TearDown]
        public void AfterTest()
        {
        }

        public static void CaptureWindow(string screenshotName)
        {
            var pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = pth.Substring(0, pth.LastIndexOf("bin"));
            var reportPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(reportPath + "Reports\\" + "Screenshots");
            var finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "Reports\\Screenshots\\" + screenshotName;
            var localpath = new Uri(finalpth).LocalPath;
            Capture.Screen().ToFile(localpath);
        }
    }
}
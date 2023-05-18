using Allure.Net.Commons;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA2;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using WPFTestDemo.Config;
using WPFTestDemo.Test;

namespace WPFTestDemo
{
    public abstract class TestBase : ReportsGenerationClass
    {
        /// <summary>
        /// A method that runs once before any tests in the assembly.
        /// </summary>
        [OneTimeSetUp]
        public static void AssemblyInit()
        {
        }

        /// <summary>
        /// Gets or sets the test context for the current test.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets the automation type to use for the tests.
        /// </summary>
        protected AutomationType AutomationType { get; }

        /// <summary>
        /// Gets or sets the main window of the application under test.
        /// </summary>
        protected Window window { get; private set; }

        /// <summary>
        /// Gets the path of the directory for the screenshots.
        /// </summary>
        protected string ScreenshotDir { get; }
        /// <summary>
        /// Gets the automation class instance for the tests.
        /// </summary>
        protected AutomationBase Automation { get; }
        /// <summary>
        /// Gets or sets the application under test.
        /// </summary>
        protected Application App { get; private set; }

        #region Win32 API
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int show);

        private const int MAXLENGTH_FILENAME = 255;
        #endregion

        /// <summary>
        /// Initializes a new instance of the TestBase class with UIA3 automation type.
        /// </summary>
        protected TestBase()
        : this(AutomationType.UIA3)
        {

        }

        /// <summary>
        /// Initializes a new instance of the TestBase class with a specified automation type.
        /// </summary>
        /// <param name="automationType">The automation type to use.</param>
        protected TestBase(AutomationType automationType)
        {
            AutomationType = automationType;

            ScreenshotDir = GlobalVariable.SCREENSHOT_PATH;

            Automation = GetAutomation(automationType);
        }

        /// <summary>
        /// A method that runs before each test and starts the application (if not started).
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            App = StartApplication();
            // try to bring the main window to foregroud
            // no known reliable way for now.
            // This is needed when we reuse the same application, some keystrokes
            // are sent directly to foreground window 
            // (If we get rid of those keystrokes, we can get rid of this)
            MaximineWindows();
            SetForegroundWindow(App.MainWindowHandle);
            App.WaitWhileMainHandleIsMissing(TimeSpan.FromMinutes(3)); // 3 minutes timeout opening app
        }

        /// <summary>
        /// Maximizes the main window of the application.
        /// </summary>
        public void MaximineWindows()
        {
            window.Patterns.Window.Pattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Maximized);
        }

        /// <summary>
        /// Minimizes the main window of the application.
        /// </summary>
        public void MinimizeWindows()
        {
            window.Patterns.Window.Pattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Minimized);
        }

        /// <summary>
        /// Restores the main window of the application to its normal size and position.
        /// </summary>
        public void NormalWindows()
        {
            window.Patterns.Window.Pattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Normal);
        }

        /// <summary>
        /// Closes the application after each test.
        /// </summary>
        protected void CloseApplication()
        {
            Automation.Dispose();
            App.Kill();
            App.Close();
            App.Dispose();
            Thread.Sleep(TimeSpan.FromSeconds(3));
            App = null;
        }

        /// <summary>
        /// A method that runs after each test and takes a screenshot and posts an execution result to Zephyr based on the test outcome.
        /// </summary>
        [TearDown]
        public void TestCleanup()
        {
            try
            {
                //If test didn't pass, then close the app such that new one will open for next test
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    if(GlobalVariable.SCREENSHOT_ENABLE)
                        TakeScreenshot(NUnit.Framework.TestContext.CurrentContext.Test.Name);

                    Zephyr.PostTestRunStatus(TestContext.CurrentContext.Test.Name, "Pass", $"{TestContext.CurrentContext.Test.Name} - Passed by automation");
                    
                    CloseApplication();
                }
                else
                {
                    if (GlobalVariable.SCREENSHOT_ENABLE)
                        TakeScreenshot(NUnit.Framework.TestContext.CurrentContext.Test.Name);

                    Zephyr.PostTestRunStatus(TestContext.CurrentContext.Test.Name, "Fail", $"{TestContext.CurrentContext.Test.Name} - Failed by automation");
                    
                    CloseApplication();
                }
            }
            catch
            {
                // Handle the exception or log it
            }
        }

        /// <summary>
        /// Method which starts the custom application to test
        /// </summary>
        protected Application StartApplication()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            bool flag = false;
            Process[] processlist = Process.GetProcessesByName(GlobalVariable.APP_NAME);
            foreach (Process p in processlist)
            {
                App = Application.Attach(p.Id);
                flag = true;
            }
            if (flag == false)
            {
                App = Application.Launch(GlobalVariable.APP_PATH);
            }

            window = App.GetMainWindow(Automation);
            Assert.IsNotNull(window, "Cannot start application UI");
            return App;
        }

        protected void RestartApp()
        {
            CloseApplication();
            TestInitialize();
        }

        /// <summary>
        /// Gets an instance of the automation class based on the automation type.
        /// </summary>
        /// <param name="automationType">The automation type to use.</param>
        /// <returns>An instance of the automation class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the automation type is not valid.</exception>
        private AutomationBase GetAutomation(AutomationType automationType)
        {
            switch (automationType)
            {
                case AutomationType.UIA2:
                    return new UIA2Automation();
                case AutomationType.UIA3:
                    return new UIA3Automation();
                default:
                    throw new ArgumentOutOfRangeException(nameof(automationType), automationType, null);
            }
        }

        /// <summary>
        /// Takes a screenshot of the screen and saves it to a file.
        /// </summary>
        /// <param name="screenshotName">The name of the screenshot file.</param>
        public static void TakeScreenshot(string screenshotName)
        {
            var imagename = GetOnlyTextAndNumber(screenshotName) + ".png";
            var imagePath = Path.Combine(GlobalVariable.SCREENSHOT_PATH, imagename);
            try
            {
                Directory.CreateDirectory(GlobalVariable.SCREENSHOT_PATH);
                FlaUI.Core.Capturing.Capture.Screen().ToFile(imagePath);
                AllureLifecycle.Instance.AddAttachment(imagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save screenshot to directory: {0}, filename: {1}, Ex: {2}", "", imagePath, ex);
            }
        }

        /// <summary>
        /// Gets a string that contains only text and numbers by replacing any other characters with underscores.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A string that contains only text and numbers.</returns>
        public static string GetOnlyTextAndNumber(string input)
        {
            string result = Regex.Replace(input, "[^a-zA-Z0-9]+", "_");
            if (result.Length > MAXLENGTH_FILENAME) //max length file name = 256 (windows)
            {
                result = result.Substring(0, MAXLENGTH_FILENAME);
            }
            return result;
        }

        /// <summary>
        /// Switches to the top window of the application.
        /// </summary>
        protected void SwitchToTopWindow()
        {
            Window topWindow = App.GetAllTopLevelWindows(Automation)[0];
            if (topWindow == null)
                return;
            else
            {
                this.window = topWindow;
            }
        }

    }
}
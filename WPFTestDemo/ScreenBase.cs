using FlaUI.Core.AutomationElements;
using NUnit.Allure.Attributes;
using System;
using System.Diagnostics;
using System.Threading;

namespace WPFTestDemo
{
    /// <summary>
    /// An abstract class that represents a screen in the application under test.
    /// </summary>
    public abstract class ScreenBase
    {
        private AutomationElement element;

        /// <summary>
        /// Initializes a new instance of the ScreenBase class with a given element.
        /// </summary>
        /// <param name="element">The element that represents the screen.</param>
        public ScreenBase(AutomationElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// Finds an element by its automation id.
        /// </summary>
        /// <param name="text">The automation id of the element.</param>
        /// <returns>The element or null if not found.</returns>
        public AutomationElement FindByAutomationId(string text)
        {
            return RetryUntilNotNull(() => element.FindFirstDescendant(cf => cf.ByAutomationId(text)));
        }

        /// <summary>
        /// Finds an element by its name.
        /// </summary>
        /// <param name="text">The name of the element.</param>
        /// <returns>The element or null if not found.</returns>
        public AutomationElement FindByName(string text)
        {
            return RetryUntilNotNull(() => element.FindFirstDescendant(cf => cf.ByName(text)));
        }

        /// <summary>
        /// Finds an element by its value.
        /// </summary>
        /// <param name="text">The value of the element.</param>
        /// <returns>The element or null if not found.</returns>
        public AutomationElement FindByValue(string text)
        {
            return RetryUntilNotNull(() => element.FindFirstDescendant(cf => cf.ByValue(text)));
        }

        /// <summary>
        /// Finds an element by its control type.
        /// </summary>
        /// <param name="text">The control type of the element.</param>
        /// <returns>The element or null if not found.</returns>
        public AutomationElement FindByControlType(FlaUI.Core.Definitions.ControlType text)
        {
            return RetryUntilNotNull(() => element.FindFirstDescendant(cf => cf.ByControlType(text)));
        }

        /// <summary>
        /// Finds an element by its XPath expression.
        /// </summary>
        /// <param name="text">The XPath expression of the element.</param>
        /// <returns>The element or null if not found.</returns>
        public AutomationElement FindByXPath(string text)
        {
            return RetryUntilNotNull(() => element.FindFirstByXPath(text));
        }

        /// <summary>
        /// Retries a function until it returns a non-null value or times out.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="func">The function to retry.</param>
        /// <param name="timeout">The timeout in seconds.</param>
        /// <returns>The non-null value or null if timed out.</returns>
        public T RetryUntilNotNull<T>(System.Func<T> func, int timeout = 50) where T : class
        {
            T result = null;
            DateTime start = DateTime.UtcNow;

            while (result == null)
            {
                result = func();
                if (DateTime.UtcNow.Subtract(start).TotalSeconds > timeout) break;
                if (result == null) System.Threading.Thread.Sleep(300);
            }

            return result;
        }

        /// <summary>
        /// Maximizes the window of the application under test.
        /// </summary>
        public void MaximizeWindow()
        {
            var maxElement = FindByXPath("//TitleBar/Button[@Name='Maximize']");
            if (maxElement.IsAvailable)
            {
                maxElement.Click();
            }
        }

        /// <summary>
        /// Closes the window of the application under test.
        /// </summary>
        [AllureStep("Close current window")]
        public void CloseWindow()
        {
            var absolutePath = GlobalVariable.APP_PATH;
            string fileName = absolutePath.Substring(absolutePath.LastIndexOf('\\') + 1);

            foreach (var process in Process.GetProcessesByName(fileName))
            {
                process.Kill();
            }
        }

        public bool WaitForElementVisible(AutomationElement element, int timeoutSeconds)
        {
            int startTime = 0;

            while (startTime < timeoutSeconds && !element.IsAvailable)
            {
                startTime++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            return element.IsAvailable;
        }

        public bool WaitForElementNotVisible(AutomationElement element, int timeoutSeconds)
        {
            int startTime = 0;

            while (startTime < timeoutSeconds && element.IsAvailable)
            {
                startTime++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            return !element.IsAvailable;
        }
    }
}

using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace WPFTestDemo.Page
{
    public class MainScreen : ScreenBase
    {
        public MainScreen(AutomationElement element) : base(element) { }

        public Button AddProduct => FindByName("Add Product").AsButton();
        public CheckBox Grouping => FindByAutomationId("Grouping").AsCheckBox();
        public CheckBox Filtering => FindByAutomationId("Filtering").AsCheckBox();
        public CheckBox Sorting => FindByAutomationId("Sorting").AsCheckBox();
        public TextBox ItemName (string name) => FindByName(name).AsTextBox();
        public TextBox DescriptionDTKey => FindByAutomationId("DescriptionDTKey").AsTextBox();

        [AllureStep("Navigate to add product screen")]
        public void navigateToAddProductScreen()
        {
            //step in reportportal.io
            ReportPortal.Shared.Context.Current.Log.Trace("Navigate to add product screen");

            AddProduct.Click();
        }

        [AllureStep("Verify new product is matched with expected name")]
        public void verifyTextBlock(string name, string expectedName, int timeOut)
        {
            //step in reportportal.io
            ReportPortal.Shared.Context.Current.Log.Trace("Verify new product is matched with expected name");

            TextBox expected = FindByName(name).AsTextBox();
            WaitForElementVisible(expected, timeOut);
            Assert.AreEqual(expected.Name.ToString(), expectedName);

            // Assert that the item is added successfully
            Assert.True(isItemPresent(name, expectedName));
        }

        [AllureStep("Verify new product name is present on screen")]
        public bool isItemPresent(string name)
        {
            //step in reportportal.io
            ReportPortal.Shared.Context.Current.Log.Trace("Verify new product name is present on screen");

            TextBox expected = FindByName(name).AsTextBox();
            return WaitForElementVisible(expected, 30);
        }

        [AllureStep("Click on product name")]
        public void clickOnProductName()
        {
            //step in reportportal.io
            ReportPortal.Shared.Context.Current.Log.Trace("Click on product name");

            ItemName("Digital camera - good condition").Click();
        }

        [AllureStep("Navigate to the desired item")]
        public void navigateToTheDesiredItem(string desc)
        {
            //step in reportportal.io
            ReportPortal.Shared.Context.Current.Log.Trace("Navigate to the desired item");

            while (!DescriptionDTKey.Name.ToString().Equals(desc))
            {
                System.Windows.Forms.SendKeys.SendWait("{DOWN}");
            }
        }

    }
}

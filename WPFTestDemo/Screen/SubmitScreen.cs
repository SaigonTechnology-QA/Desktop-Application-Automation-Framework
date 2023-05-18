using FlaUI.Core.AutomationElements;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using System.Threading;

namespace WPFTestDemo.Screen
{
    public class SubmitScreen : ScreenBase
    {
        public SubmitScreen(AutomationElement element) : base(element) { }

        public Button Submit => FindByName("Submit").AsButton();
        public TextBox Description => FindByAutomationId("DescriptionEntryForm").AsTextBox();
        public TextBox Price => FindByAutomationId("StartPriceEntryForm").AsTextBox();
        public TextBox Date => FindByAutomationId("StartDateEntryForm").AsTextBox();
        public TextBox ErrorTextBlock => FindByAutomationId("ErrorTextBlock").AsTextBox();

        [AllureStep("Enter description, price and date for new product")]
        public void enterItemInformation(string desc, string price, string date)
        {
            Description.Text = desc;
            Thread.Sleep(300); // 300 milliseconds = 0.3 second
            Price.Text = price;
            Thread.Sleep(300);
            Date.Text = date;
            Thread.Sleep(300);
        }

        [AllureStep("Click submit new product")]
        public void submitItem()
        {
            Submit.Click();
        }

        [AllureStep("Verify new product information is matched with input data")]
        public void verifyTextBlock(string expectedName, int timeOut)
        {
            WaitForElementVisible(ErrorTextBlock, timeOut);
            Assert.AreEqual(ErrorTextBlock.Name.ToString(), expectedName);
        }
    }
}

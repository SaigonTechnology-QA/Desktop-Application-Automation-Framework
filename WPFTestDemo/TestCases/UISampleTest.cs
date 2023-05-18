using DesktopAppAuto.TestData;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WPFTestDemo.Page;
using WPFTestDemo.Screen;
namespace WPFTestDemo.Test
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Add New Product")]
    [AllureFeature("Add New Product")]
    [AllureEpic("DesktopAppAuto")]
    public class UISampleTest : TestBase
    {
        // A test method that verifies that the user should be able to add a new item to the app with valid values
        [Category("Sanity"), Category("Regression")]
        [TestCase("a", "0", "a", Description = "Verify User Should Be Able To Add New Item To App with Price = 0")]
        [TestCase("b", "12", "b", Description = "Verify User Should Be Able To Add New Item To App with Price > 0")]
        [Order(1)]
        [AllureStory("Add Product")]
        [AllureTag("Regression")]
        public void POS_VerifyUserShouldBeAbleToAddNewItemToApp_DES_T1(string desc, string price, string expectedName)
        {
            // Create a main screen object
            MainScreen mainScreen = new MainScreen(window);

            // Click on the add product button
            mainScreen.navigateToAddProductScreen();

            // Switch to the top window
            SwitchToTopWindow();

            // Create a submit screen object
            SubmitScreen submitScreen = new SubmitScreen(window);

            // Get the current date and format it as M/d/yyyy
            string date = DateTime.Now.ToString(@"M/d/yyyy");

            // Enter the item information
            submitScreen.enterItemInformation(desc, price, date);

            // Submit the item
            submitScreen.submitItem();

            // Close the window
            submitScreen.CloseWindow();

            // Click on the item name
            mainScreen.clickOnProductName();

            // Navigate to the desired item description
            mainScreen.navigateToTheDesiredItem(desc);
            
            // Verify that the expected text block is displayed
            mainScreen.verifyTextBlock(desc, expectedName, 5);
        }

        
        //Test data for NEG test case
        public static IEnumerable<string[]> DDTProduct()
        {
            // Define the path to the CSV file
            var filePath = Path.Combine(@"TestData", "DataList.csv");
            return DDTHelper.DDTGetData(filePath, new[] { "desc", "price", "date", "expectedName" });
        }

        // A test method that verifies that the user cannot add a new item to the app with wrong values
        [Test(Description = "Add New Item To App With InValid Value"), Category("Regression")]
        [TestCaseSource(nameof(DDTProduct))]
        [Order(2)]
        [AllureStory("Add Product")]
        [AllureTag("Smoke")]
        public void NEG_VerifyUserCannotAddNewItemToAppWithWrongValues_DES_T2(string desc, string price, string date, string expectedName)
        {
            // Create a main screen object
            MainScreen mainScreen = new MainScreen(window);

            // Click on the add product button
            mainScreen.navigateToAddProductScreen();

            // Switch to the top window
            SwitchToTopWindow();

            // Create a submit screen object
            SubmitScreen submitScreen = new SubmitScreen(window);

            // Enter the item information
            submitScreen.enterItemInformation(desc, price, date);

            // Submit the item
            submitScreen.submitItem();

            // Verify that the expected text block is displayed
            submitScreen.verifyTextBlock(expectedName, 5);

            // Close the window
            submitScreen.CloseWindow();
        }
    }
}

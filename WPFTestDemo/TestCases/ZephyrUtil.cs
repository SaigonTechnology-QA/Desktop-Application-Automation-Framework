using NUnit.Framework;
using RestSharp;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WPFTestDemo.Test
{
    public class Zephyr
    {
        static RestClient client;

        /// <summary>
        /// Gets or sets the REST client for the Zephyr API.
        /// </summary>
        public static RestClient Client { get => client; set => client=value; }

        /// <summary>
        /// Initializes a new instance of the Zephyr class and sets the default header for the client.
        /// </summary>
        public Zephyr()
        {
            Client = new RestClient("https://api.zephyrscale.smartbear.com/v2/");
            // Set the authorization header value.
            Client.AddDefaultHeader("Authorization", GlobalVariable.ZEPHYR_TOKEN);
        }

        /// <summary>
        /// Gets the test case name from the Zephyr API by its key.
        /// </summary>
        /// <param name="testCaseName">The test case name that contains the key.</param>
        public void GetTestCaseName(string testCaseName)
        {
            string key = ExtractZephyrCode(testCaseName);
            var request = new RestRequest("/testcases/" + key, Method.Get);
            var response = Client.Execute(request);

            // Assert the response status code is OK (200)
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            Console.WriteLine(response.Content);
        }

        //Extract zephyr key from test name
        /// <summary>
        /// Extracts the zephyr key from the test name using a regular expression.
        /// </summary>
        /// <param name="input">The test name that contains the key.</param>
        /// <returns>The zephyr key or null if not found.</returns>
        public static string ExtractZephyrCode(string input)
        {
            var pattern = GlobalVariable.ZEPHYR_PROJECT + "_T\\d+";

            // Try to find a match in the text
            var match = Regex.Match(input, pattern);

            // If a match is found, return it
            if (match.Success)
            {
                return match.Value.Replace("_", "-");
            }

            // Otherwise, log a warning and return null
            else
            {
                Trace.TraceWarning("This test case NOT contains zephyr id");
                return null;
            }
        }

        /// <summary>
        /// Posts the test run status to the Zephyr API using the test name and status.
        /// </summary>
        /// <param name="testName">The test name that contains the key.</param>
        /// <param name="status">The test status to post.</param>
        /// <param name="comment">An optional comment to add to the test execution.</param>
        public static void PostTestRunStatus(string testName, string status, string comment = "")
        {
            if(!GlobalVariable.ZEPHYR_ENABLE)
            {
                return;
            }    

            Client = new RestClient("https://api.zephyrscale.smartbear.com/v2/");

            // Set the authorization header value.
            Client.AddDefaultHeader("Authorization", GlobalVariable.ZEPHYR_TOKEN);

            //handle test case without ID name
            if (!testName.Contains(GlobalVariable.ZEPHYR_PROJECT + "_"))
            {
                return;
            }

            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                status = "Pass";
            }
            else if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                status = "Fail";
            }

            string key = ExtractZephyrCode(testName);
            var request = new RestRequest("/testexecutions", Method.Post);
            request.AddJsonBody(new
            {
                projectKey = GlobalVariable.ZEPHYR_PROJECT,
                testCaseKey = key,
                testCycleKey = GlobalVariable.ZEPHYR_CYCLE,
                statusName = status,
                comment = comment
            });

            var response = Client.Execute(request);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Created);
        }
    }
}
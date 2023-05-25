using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.ProjectModel;
using Newtonsoft.Json.Linq;
using WPFTestDemo.Test;

namespace WPFTestDemo
{
    public class GlobalVariable
    {
        public static string APP_PATH { get; set; }
        public static string SCREENSHOT_PATH { get; set; }
        public static string APP_NAME { get; set; }
        public static bool SCREENSHOT_ENABLE { get; set; }

        //Integrate with Zephyr
        public static bool ZEPHYR_ENABLE { get; set; }
        public static string ZEPHYR_URL { get; set; }
        public static string ZEPHYR_TOKEN { get; set; }
        public static string ZEPHYR_CYCLE { get; set; }
        public static string ZEPHYR_PROJECT { get; set; }


        //Integrate Allure
        public static string ENVIRONMENT = "qa";
        public static string USER = "qa";

        public static JObject ReadConfig(string path)
        {
            string json = File.ReadAllText(path);
            JObject config = JObject.Parse(json);
            return config;
        }

        static GlobalVariable()
        {
            string envPath = Environment.GetEnvironmentVariable("APP_PATH");

            if (envPath == null)
                envPath = @"Config\dev-jenkins.config.json";

            JObject dict = ReadConfig(envPath);

            APP_PATH = dict["APP_PATH"].ToString();

            APP_NAME = dict["APP_NAME"].ToString();

            SCREENSHOT_PATH = dict["SCREENSHOT_PATH"].ToString();

            SCREENSHOT_ENABLE = Boolean.Parse(dict["SCREENSHOT_ENABLE"].ToString());

            ZEPHYR_ENABLE = Boolean.Parse(dict["ZEPHYR_ENABLE"].ToString());

            ZEPHYR_URL = dict["ZEPHYR_URL"].ToString();

            ZEPHYR_TOKEN = Zephyr.DecryptString(dict["ZEPHYR_TOKEN"].ToString()); //decode base64

            if (Environment.GetEnvironmentVariable("ZEPHYR_CYCLE") == null)
                ZEPHYR_CYCLE = dict["ZEPHYR_CYCLE"].ToString();
            else
                ZEPHYR_CYCLE = Environment.GetEnvironmentVariable("ZEPHYR_CYCLE");

            ZEPHYR_PROJECT = dict["ZEPHYR_PROJECT"].ToString();
        }
    }
}

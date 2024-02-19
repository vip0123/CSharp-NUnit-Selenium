using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;

namespace NUnitSelenium
{
    [TestFixture("Chrome", "92.0", "Windows 10", "1920x1080")]
    [TestFixture("MicrosoftEdge", "103.0", "Windows 10", "1920x1080")]
    [Parallelizable(ParallelScope.Children)]
    public class NUnitSeleniumSample
    {
        public static string LT_USERNAME = "vip0123";
        public static string LT_ACCESS_KEY = "3ayiYrouNUDmkP7pJxYVy6MaQ1NEogBUZ2noN8LUe9QDqhH3ul";
        public static bool tunnel = Boolean.Parse(Environment.GetEnvironmentVariable("LT_TUNNEL") == null ? "false" : Environment.GetEnvironmentVariable("LT_TUNNEL"));
        public static string build = Environment.GetEnvironmentVariable("LT_BUILD") == null ? "your build name" : Environment.GetEnvironmentVariable("LT_BUILD");
        public static string seleniumUri = "https://hub.lambdatest.com/wd/hub";


        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private String browser;
        private String version;
        private String os;
        private String resolution;

        public NUnitSeleniumSample(String browser, String version, String os, String resolution)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
            this.resolution = resolution;
        }

        [SetUp]
        public void Init()
        {

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("browserName", browser);
            capabilities.SetCapability("version", version);
            capabilities.SetCapability("platform", os);
            capabilities.SetCapability("resolution", resolution);

            //Requires a named tunnel.
            if (tunnel)
            {
                capabilities.SetCapability("tunnel", tunnel);
            }
            if (build != null)
            {
                capabilities.SetCapability("build", build);
            }

            capabilities.SetCapability("user", LT_USERNAME);
            capabilities.SetCapability("accessKey", LT_ACCESS_KEY);

            capabilities.SetCapability("name",
            String.Format("{0}:{1}",
            TestContext.CurrentContext.Test.ClassName,
            TestContext.CurrentContext.Test.MethodName));
            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), capabilities, TimeSpan.FromSeconds(600));
            Console.Out.WriteLine(driver);
        }

        [Test]
        public void Todotest()
        {
            {
                Console.WriteLine("Navigating to todos app.");
                driver.Value.Navigate().GoToUrl("https://lambdatest.github.io/sample-todo-app/");

                driver.Value.FindElement(By.Name("li4")).Click();
                Console.WriteLine("Clicking Checkbox");
                driver.Value.FindElement(By.Name("li5")).Click();


                // If both clicks worked, then te following List should have length 2
                IList<IWebElement> elems = driver.Value.FindElements(By.ClassName("done-true"));
                // so we'll assert that this is correct.
                Assert.AreEqual(2, elems.Count);

                Console.WriteLine("Entering Text");
                driver.Value.FindElement(By.Id("sampletodotext")).SendKeys("Yey, Let's add it to list");
                driver.Value.FindElement(By.Id("addbutton")).Click();


                // lets also assert that the new todo we added is in the list
                string spanText = driver.Value.FindElement(By.XPath("/html/body/div/div/div/ul/li[6]/span")).Text;
                Assert.AreEqual("Yey, Let's add it to list", spanText);

            }
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to LambdaTest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {

                // Terminates the remote webdriver session
                driver.Value.Quit();
            }
        }
    }
}
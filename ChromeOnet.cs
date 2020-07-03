using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace WebDriver_Demo
{
    class WebDriver_Demo
    {
        String test_url = "https://www.google.com";

        IWebDriver driver;


        [SetUp]
        public void start_Browser()
        {
            Console.WriteLine("SetUp");
            // Local Selenium WebDriver
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void test_search()
        {
            Console.WriteLine("Test");
            driver.Url = test_url;

            System.Threading.Thread.Sleep(1000);

            IWebElement searchText = driver.FindElement(By.CssSelector("[name = 'q']"));

            searchText.SendKeys("www.onet.pl\n");

            System.Threading.Thread.Sleep(1000);


            IWebElement siteButton = driver.FindElement(By.CssSelector("a[href='https://www.onet.pl/']"));

            siteButton.Click();

            System.Threading.Thread.Sleep(1000);



            /* Zapis do pliku + ile jest wystąpień checkWord */
            string checkWord = "w";

            IWebElement body = driver.FindElement(By.TagName("body"));
            String result = driver.FindElement(By.TagName("body")).Text;

            int count = Regex.Matches(result, checkWord).Count;
            result = result +"\n"+ count+"\n";

            //Folder location
            var dir = @"C:\Users\ulatw\Textfile" + DateTime.Now.ToShortDateString();

           // If the folder doesn't exist, create it
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //Creates a file copiedtext.txt with all the contents on the page.
             File.AppendAllText(Path.Combine(dir, "Copiedtext.txt"), result);


            System.Threading.Thread.Sleep(2000);

            Console.WriteLine("Test Passed");
        }

        [TearDown]
        public void close_Browser()
        {
            Console.WriteLine("TearDown");
            driver.Quit();
        }
    }
}

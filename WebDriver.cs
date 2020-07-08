using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public class WebDriver
    {
        private IWebDriver driver;

        private String currentMainSearchPage;

        private List<String> websitesList = new List<String>();

        private List<WebSiteInfo> websitesInfoList = new List<WebSiteInfo>();

        private void startBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }


        private void closeBrowser()
        {
            driver.Quit();
        }

        /* wyszukiwanie zadanej wartości string w googlu */
        private void googleSearch(String searchText)
        {
            String test_url = "https://www.google.com";

            driver.Url = test_url;
            //System.Threading.Thread.Sleep(200);

            IWebElement search = driver.FindElement(By.CssSelector("[name = 'q']"));
            searchText = searchText + "\n";
            search.SendKeys(searchText);

            //zapamiętanie strony wyszukania w currentMainSearchPage
            this.currentMainSearchPage = driver.Url;

           // System.Threading.Thread.Sleep(500);

        }

        /*wyszukuje elementy na stronie (głównej) wyszukiwania, które są z klasy r i mają @href, wpisuje adres strony na websitesList */
        /* tutaj można dodać te podstrony reklamowe... (albo po prostu dać allSubsites) */
        private void findSitesFromMainSearch()
        {

           // ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[@class='r']//*[contains(@href, 'https://www.viperprint.pl/')]"));
            ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[@class='r']//*[contains(@href, 'http')]"));

            foreach (IWebElement webDriver in siteButtons)
            {
                string result = webDriver.GetAttribute("href");

                //bez webcache i related -> można zmienić
                if (Regex.Matches(result, "webcache.googleusercontent.com").Count == 0 && Regex.Matches(result, "www.google.com/search.q=related").Count == 0)
                {
                    if (this.websitesList.Contains(result) == false)
                        this.websitesList.Add(result);
                }
            }
        }


        /* findSitesFromMainSearch + toString + dodatkowo: inicjalizacja i deinicjalizacja 
          zwraca listę typu string z opisami stron wyszukiwanymi z głównej strony wyszukania 
          to jest metoda raczej testowa z początku - teraz jest już raczej nie potrzebna
             */
        public List<String> allSitesFromMainSearchText()
        {
            startBrowser();
            googleSearch("drukarnia");

            /*wyszukiwanie wszystkich linków ze strony wyszukiwania*/
            ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[@class='r']//*[contains(@href, 'http')]"));
            List<String> textSiteButtons = new List<String>();
            foreach (IWebElement webDriver in siteButtons)
            {
                string result = webDriver.GetAttribute("href");
                
                if( Regex.Matches(result, "webcache.googleusercontent.com").Count == 0 && Regex.Matches(result, "www.google.com/search.q=related").Count == 0)
                    Console.WriteLine("początek " + result + " koniec " + siteButtons.Count());
                textSiteButtons.Add(webDriver.Text);
            }

            Console.WriteLine("\nzamknięcie przeglądarki\n");
            closeBrowser();

            return textSiteButtons;
        }

        /* znajduje wszystkie podstrony danej strony i wrzuca je do listy webSitesList 
            Uwaga: trzeba wywalić wszystkie linki do plików, a nie do stron
             */
        private void allSubpages()
        {
            ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[contains(@href, 'http')]"));
            foreach (IWebElement webDriver in siteButtons)
            {
                try
                {
                    string result = webDriver.GetAttribute("href");

                    //sprawdzenie, czy ten adres jest juz na liscie
                    if (this.websitesList.Contains(result) == false)
                        this.websitesList.Add(result);
                }
                catch { }

            }
            //Console.WriteLine("After all Subpages: ");
            //this.websitesList.ForEach(w => Console.WriteLine("Website: " + w));

        }

        /*
         *zwraca string calego tekstu strony
         * uwaga: moze da sie to przetwarzanie i wyszukiwanie robić jakoś bardziej elegancko
         */
        private string siteBodyToString()
        {
            try
            {
                IWebElement body = driver.FindElement(By.TagName("body"));
                String bodyString = driver.FindElement(By.TagName("body")).Text;
                return bodyString;
            }
            catch { return ""; }
        }

        /*
         * wyszukiwanie podciągów będących mailami
         */
        private List<string> mailFromSite (string website)
        {
            Regex regex = new Regex(@"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)");
            var res = regex.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            //res.ForEach(w => Console.WriteLine("Here: " + w));
            //Console.WriteLine("Hi! ");
            return res;
        }

        /*
        * wyszukiwanie podciągów będących telefonami
        * uwaga: nie działa
         */
        private List<string> phoneFromSite(string website)
        {
            Regex regex = new Regex(@"\(?\+?(?:[0-9]\)? ?-?\(?){6,14}[0-9]");
            var res = regex.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            //res.ForEach(w => Console.WriteLine("Here: " + w));
            //Console.WriteLine("Hi! ");
            return res;
        }

        /*
         * zmienia podany adres url na adres dtrony głównej
         */
        private string transformToMainsite(string webUrl)
        {
            int count = Regex.Matches(webUrl, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+(/?)$").Count;
            if (count == 1)  return webUrl;
            else
            {
                return Regex.Match(webUrl, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+(/?)").Value;
            }

        }


        /* operacje po wejsciu na strone z listy*/
        private void checkWebsite(string checkWord)
        {
            /*
             * ( moze: sprawdz, czy jest wpisany wyraz )
             * sprawdz, czy jest mail/telefon: utwórz z nazwy strony strone główną (nie będącą podstroną)
             *                         sprawdz, czy juz istnieje w liscie el. z mailami i czy ma maile
             *                         jeśli ma jakieś nie istniejące jeszcze maile, to wpisz
             * znajdz podstrony i wrzuc na liste
             */
            string siteToString = siteBodyToString();

            int count = Regex.Matches(siteToString, checkWord).Count;
            if (count == 0) return;

            List<string> mailList = mailFromSite(siteToString);
            List<string> phoneList = phoneFromSite(siteToString);
            WebSiteInfo webInfo = new WebSiteInfo();
            //sprawdzenie, czy jest to podstrona i utworzenie z niej strony głównej
            string url = transformToMainsite(driver.Url);

            //sprawdzenie, czy już jest ten adres w websitesInfoList
            int checkIfExists = websitesInfoList.FindIndex(web => web.Url.Equals(url));
            if ( checkIfExists==-1)
            {
                webInfo.Url = url;
            }
            else
            {
                webInfo = websitesInfoList[checkIfExists];
            }

            //wyszukanie maili
            foreach (string mail in mailList)
            {
                webInfo.addToMailList(mail);
            }

            //wyszukanie telefonów
            foreach (string phone in phoneList)
            {
                webInfo.addToPhoneList(phone);
            }

            if (checkIfExists == -1) this.websitesInfoList.Add(webInfo);

            allSubpages();
        }


        private void printWebsitesInfoList()
        {
            Console.WriteLine("WebsitesInfoList:  ");
            foreach (WebSiteInfo web in this.websitesInfoList)
            {
                Console.WriteLine("Website: " + web.Url);
                web.printMailList();
                web.printPhoneList();
            }       
        }

        /*inicjalizacja działania webDrivera - taki jakby mój wewnętrzny main jak na razie */
        public void runWebDriver()
        {
            startBrowser();
            googleSearch("drukarnia");

            findSitesFromMainSearch();
           // this.websitesList.ForEach(w => Console.WriteLine("Website: " + w));
            //allSubpages();

            //próba wejścia w pierwszy link
            //string url = this.websitesList[0];
            //driver.Url = url;
            //mailFromSite(siteBodyToString());
            //allSubpages();

            for (int i=0;i< this.websitesList.Count;i++)
            {
                string web = this.websitesList[i];
                driver.Url = web;
                checkWebsite("druk");
                Console.WriteLine("-------------------------------------------");
                printWebsitesInfoList();
            }

            System.Threading.Thread.Sleep(200);
            printWebsitesInfoList();

            driver.Url = this.currentMainSearchPage;

            System.Threading.Thread.Sleep(200);

            closeBrowser();
        }

    }
}

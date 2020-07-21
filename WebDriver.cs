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

        /*lista obiektów przechowywujących nazwę strony oraz odczytane z niej informacje*/
        private List<WebSiteInfo> websitesInfoList = new List<WebSiteInfo>();

        DbCommunicate dbCommunicate = new DbCommunicate();

        private void startBrowser()
        {
            var chromeOptions = new ChromeOptions();
            //ta opcja raczej nie dziala
            chromeOptions.AddArguments("--download_restrictions=3");
            //chromeOptions.AddArguments("--incognito");
            driver = new ChromeDriver(chromeOptions);
           // driver = new ChromeDriver();
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

            IWebElement search = driver.FindElement(By.CssSelector("[name = 'q']"));
            searchText = searchText + "\n";
            search.SendKeys(searchText);
            
        }

        /* wyszukuje adresy dla stron "google/search?q "*/
        /* wyszukuje elementy na stronie (głównej) wyszukiwania, które są z klasy center_col i mają @href, wpisuje adres strony na websitesList */
        private void findSitesFromMainSearch()
        {
            //ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[@class='r']//*[contains(@href, 'drukarniaonline')]"));
            ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//*[@id='center_col']//*[contains(@href, 'http')] | //*[@id='center_col']//*[contains(@href, 'search?q')]"));

            foreach (IWebElement webDriver in siteButtons)
            {
                string result = webDriver.GetAttribute("href");

                //if (Regex.Matches(result, "webcache.googleusercontent.com").Count == 0 && Regex.Matches(result, "www.google.com/search.q=related").Count == 0)
                if (Regex.Matches(result, "webcache.googleusercontent.com").Count == 0)
                {
                    dbCommunicate.AddWebsiteListInfo(result);
                }
            }
        }

        /* znajduje wszystkie podstrony danej strony zawarte w części body i wrzuca je do listy webSitesList 
            (nie zapamiętuje stron kończących się na .<ciąg a-z v A-Z> )
        */
        private void allSubpages()
        {
            ReadOnlyCollection<IWebElement> siteButtons = driver.FindElements(By.XPath("//body//*[contains(@href, 'http')]"));
            foreach (IWebElement webDriver in siteButtons)
            {
                try
                {
                    List<string> urlCheckList_exist = new List<string>() {"kontakt","contact","adres","address" };
                    List<string> urlCheckList_notexist = new List<string>() { "facebook", "youtube", "twitter", "instagram", "goo.gl/maps" };

                    string result = webDriver.GetAttribute("href");

                    int countUrlCheck_exist = 0;
                    int countUrlCheck_notexist = 0;

                    foreach (string w in urlCheckList_exist) { if (Regex.Matches(result, w).Count > 0) countUrlCheck_exist = 1; }
                    foreach (string w in urlCheckList_notexist) { if (Regex.Matches(result, w).Count > 0) countUrlCheck_notexist = 1; }

                    int googleUrlCheck = 0;
                    if (Regex.Matches(result, "google.com/search?q").Count > 0) googleUrlCheck = 1;

                    //sprawdzenie, czy adres kończy się na .<ciąg a-z v A-Z>
                    int count = Regex.Matches(result, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+[\.]([a-z]{0,}[A-Z]{0,})$").Count;

                    //sprawdzenie, czy ten adres jest juz na liscie, czy mam wyraz z list urlCheckList (exist i notexist)
                    if ( count == 0 && countUrlCheck_notexist == 0 && (countUrlCheck_exist == 1 || googleUrlCheck == 1 || transformToMainsite(result) != transformToMainsite(driver.Url)))
                    {
                        dbCommunicate.AddWebsiteListInfo(result);
                    }
                }
                catch { }

            }

        }

        /*
         *zwraca caly tekst strony w jednym string'u
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
            return res;
        }

        /*
        * wyszukiwanie podciągów będących adresami
        */
        private List<string> addressFromSite(string website)
        {
            Regex regex1 = new Regex(@"(?:(ul|al)(\.)?)( )+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ ?)+([\d\-\/A-z ])+([ \,\n\|])+(?:[0-9]{2})[-]([0-9]{3})( )+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ ?)+");
            //Regex regex2 = new Regex(@"(?:[0-9]{2})[-]([0-9]{3})( )+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ ?)+([ ,.\n])+((ul|al)(\.)?)( )+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ ?)+\d+");
            //Regex regex2 = new Regex(@"(([0-9]{2})[-]([0-9]{3})( ))+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ *)+([ \,\.\n\|])*((ul|al)(\.)?)( )*([A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]+ ?)+[\d\-\/A-z ]+");

            //Regex regex1 = new Regex(@"((ul|al|pl)(\.)?)( )+((\"")?([A-ZŻŹĆĄŚĘŁÓŃ]?\d*){1}( ?)[A-zżźćńółęąśZŻŹĆĄŚĘŁÓŃ""\-\.]+ ?)+([\d\-\/A-z ])+([ \,\n\|])*(?:[0-9]{2})[-]([0-9]{3})( )+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[A-zżźćńółęąśZŻŹĆĄŚĘŁÓŃ]+ ?)+");
            Regex regex2 = new Regex(@"(([0-9]{2})[-]([0-9]{3})( ))+([A-ZŻŹĆĄŚĘŁÓŃ]{1}[A-zżźćńółęąśZŻŹĆĄŚĘŁÓŃ]+ *)+([ \,\.\n\|])*((ul|al)(\.)?)( )*(([A-ZŻŹĆĄŚĘŁÓŃ]?\d*){1}( ?)[A-zżźćńółęąśZŻŹĆĄŚĘŁÓŃ]+ ?)+[\d\-\/A-z ]+");

           var res1 = regex1.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            var res2 = regex2.Matches(website).Cast<Match>().Select(match => match.Value).ToList();

            return res1.Concat(res2).ToList();
            //return res1;
        }

        /*
        * wyszukiwanie podciągów będących telefonami
        * 
         */
        private List<string> phoneFromSite(string website)
        {
            //to jest wyrazenie regularne, ktore na pewno znajduje wszystkie numery telefonów, ale troche smieci tez
            //Regex regex = new Regex(@"\(?\+?(?:[0-9]\)? ?-?\(?){6,14}[0-9]");
            //List<string> res = regex.Matches(website).Cast<Match>().Select(match => match.Value).ToList();

            //wyrazenia regularne oparte na mozliwych kombinacjach pogrupowania cyfr (z uwzglednieniem '+', '(', ')')
            //nie dziala jednokrotnosc kazdej czesci wyrazenia
            Regex regex1 = new Regex(@"(\(?)(\+?[0-9]{2})(\)?)( |-)([0-9]{3})( |-)([0-9]{3})( |-)([0-9]{3})");
            Regex regex2 = new Regex(@"([0-9]{3})( |-)([0-9]{3})( |-)([0-9]{3})");
            Regex regex3 = new Regex(@"(\(?)(\+?[0-9]{2})(\)?)( |-)(\(?)([0-9]{2})(\)?)( |-)([0-9]{3})( |-)([0-9]{3})( |-)([0-9]{3})");
            Regex regex4 = new Regex(@"(\(?)(\+?[0-9]{2})(\)?)( |-)([0-9]{3})( |-)([0-9]{2})( |-)([0-9]{2})( |-)([0-9]{2})");
            Regex regex5 = new Regex(@"([0-9]{3})( |-)([0-9]{2})( |-)([0-9]{2})");
            Regex regex6 = new Regex(@"(\(?)(\+?[0-9]{2})(\)?)( |-)([0-9]{3})( |-)([0-9]{2})( |-)([0-9]{2})");
            Regex regex7 = new Regex(@"(\(?)(\+?[0-9]{2})(\)?)( |-)([0-9]{3})( |-)([0-9]{4})");
            Regex regex8 = new Regex(@"(\(? ?)(\+?[0-9]{2})( ?\)?)( |-)(\(? ?)(\+?[0-9]{2})( ?\)?)( |-)+([0-9]{3})( |-)([0-9]{2})");


            List<string> res1 = regex1.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res2 = regex2.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res3 = regex3.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res4 = regex4.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res5 = regex5.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res6 = regex6.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res7 = regex7.Matches(website).Cast<Match>().Select(match => match.Value).ToList();
            List<string> res8 = regex8.Matches(website).Cast<Match>().Select(match => match.Value).ToList();

            List<string> res = res1.Concat(res2).Concat(res3).Concat(res4).Concat(res5).Concat(res6).Concat(res7).Concat(res8).ToList();

            //unifikacja numerow
            for (int i = 0; i < res.Count; i++)
            {
               res[i]= res[i].Replace("-", " ").Replace("(", "").Replace(")", "").Replace("+", "");
            }

            //sprawdzenie powtarzalnosci numerow
            for(int i =0; i< res.Count; i++ )
            {
                for (int j = 0; j < res.Count; j++)
                {
                    if(res[i] != res[j])
                    {
                        if (res[j].Contains(res[i]) == true)
                            res[i] = "000";
                    }
                }
            }
            res.RemoveAll(nr => nr=="000");

            return res;
        }

        /*
         * zmienia podany adres url na adres strony głównej (jesli byla podana podstrona)
         */
        private string transformToMainsite(string webUrl)
        {
            //Console.WriteLine("Url: "+webUrl);
            if(Regex.Matches(webUrl, "https").Count  == 1)
            {
                //zmiana https na http
                webUrl = webUrl.Remove(4,1);
                //Console.WriteLine("   s: " + webUrl);
            }

            if (Regex.Matches(webUrl, "www").Count == 1)
            {
                //usuniecie www
                webUrl = webUrl.Remove(7, 4);
                //Console.WriteLine("   www: " + webUrl);
            }

            int count = Regex.Matches(webUrl, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+(/?)$").Count;
            if (count == 1)  return webUrl;
            else
            {
                return Regex.Match(webUrl, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+(/?)").Value;
            }

        }


        private void checkWebsite(string checkWord)
        {
            string siteToString = siteBodyToString();

            //sprawdzenie obecnosci checkWord
            int count = Regex.Matches(siteToString, checkWord).Count;
            if (count == 0) return;

            //sprawdzenie, czy jest to strona wyszukiwania w googl'u
            if (Regex.Matches(driver.Url, "google.com/search?").Count != 0 || Regex.Matches(driver.Url, "google.pl/search?").Count != 0 || Regex.Matches(driver.Url, "google.pl/search?").Count != 0)
            {
                findSitesFromMainSearch();
                return;
            }

            //utworzenie listy maili, telefonow i adresów
            List<string> mailList = mailFromSite(siteToString);
            List<string> phoneList = phoneFromSite(siteToString);
            List<string> addressList = addressFromSite(siteToString);

            WebSiteInfo webInfo = new WebSiteInfo();

            //sprawdzenie, czy jest to podstrona i utworzenie z niej strony głównej
            string url = transformToMainsite(driver.Url);

            //sprawdzenie, czy już jest ten adres w websitesInfoList
            int checkIfExists = websitesInfoList.FindIndex(web => web.Url.Equals(url));
            if ( checkIfExists==-1)
            {
                webInfo.Url = url;
                this.websitesInfoList.Add(webInfo);
            }
            else
            {
                webInfo = websitesInfoList[checkIfExists];
            }

            //dodanie maili
            foreach (string mail in mailList)
            {
                webInfo.addToMailList(mail);
            }
            //dodanie adresów
            foreach (string address in addressList)
            {
                webInfo.addToAddressList(address);

            }
            //dodanie telefonów
            foreach (string phone in phoneList)
            {
                webInfo.addToPhoneList(phone);
            }

            //wyszukanie podstron
            allSubpages();
        }


        private void printWebsitesInfoList()
        {
            Console.WriteLine("WebsitesInfoList:  ");
            this.websitesInfoList.Sort((x,y) => x.Url.CompareTo(y.Url));
            foreach (WebSiteInfo web in this.websitesInfoList)
            {
                Console.WriteLine("Website: " + web.Url);
                web.printMailList();
                web.printPhoneList();
                web.printAddressList();
            }       
        }

/*****************************************************************************/


        public void runWebDriver( int websitesToVisit, int urlBeginId)
        {
            int webId = -1;

            //petla po kolejnych nazwach stron na websitesList
            for (int i = 1; i <= websitesToVisit; i++)
            {
                string webUrl = "";
                dbCommunicate.GetWebsiteList(urlBeginId + i, ref webUrl, ref webId);
   
                if (webUrl == "") break;
                try
                {
                    driver.Url = webUrl;

                    checkWebsite("druk");

                    Console.WriteLine("-------------------------------------------");
                    printWebsitesInfoList();
                    Console.WriteLine("-------------------------------------------");
                }
                catch { }
            }
            
            printWebsitesInfoList();

            if (webId != -1)
            {
                dbCommunicate.AddWebsiteListPointer(webId);
            }
        }

 /*****************************************************************************/


        public void runProgram()
        {
            //liczba przetwarzanych stron 
            int websitesToVisit = 1000;

            int urlBeginId= -1;
            bool isEmpty = dbCommunicate.IsDbEmpty(ref urlBeginId);

            startBrowser();

            //baza danych jest pusta
            if (isEmpty)
            {
                googleSearch("drukarnia");
                //poczatkowe wprowadzenie na liste tylko glownych stron wyszukanych
                findSitesFromMainSearch();
            }

            runWebDriver( websitesToVisit, urlBeginId);
            dbCommunicate.AddToDatabaseWebInfoList(this.websitesInfoList);
            closeBrowser();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class WebSiteInfo
    {
        private  String url;
        private  List<String> mailList = new List<String>();
        private List<String> phoneList = new List<String>();

        public string Url   // property
        {
            get { return url; }   // get method
            set { url = value; }  // set method
        }

        public void addToMailList( string value)
        {
            if (this.mailList.Contains(value) == false)
                this.mailList.Add(value);
        }

        public void addToPhoneList(string value)
        {
            if (this.phoneList.Contains(value) == false)
                this.phoneList.Add(value);
        }

        public void printMailList()
        {
            this.mailList.ForEach(w => Console.WriteLine("  " + w));
        }

        public void printPhoneList()
        {
            this.phoneList.ForEach(w => Console.WriteLine("  " + w));
        }
    }
}

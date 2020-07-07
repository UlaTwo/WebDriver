using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class WebSiteInfo
    {
        private  string url;
        private  List<string> mailList = new List<String>();

        public string Url   // property
        {
            get { return url; }   // get method
            set { url = value; }  // set method
        }

        public void addToList( string value)
        {
            if (this.mailList.Contains(value) == false)
                this.mailList.Add(value);
        }
    }
}

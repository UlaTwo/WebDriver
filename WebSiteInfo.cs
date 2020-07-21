using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    //klasa zawierajaca nazwe strony i informacje znalezione na danej stronie
    public class WebSiteInfo
    {
        private  String url;
        private  List<String> mailList = new List<String>();
        private List<String> phoneList = new List<String>();
        private List<String> addressList = new List<String>();
       // private string city;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        //public string City
        //{
        //    get { return city; }
        //    set { city = value; }
        //}

        public void addToMailList( string value)
        {
            if (this.mailList.Contains(value) == false)
                this.mailList.Add(value);
        }

        public void addToAddressList(string value)
        {
            if (this.addressList.Contains(value) == false)
                this.addressList.Add(value);
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

        public void printAddressList()
        {
            this.addressList.ForEach(w => Console.WriteLine("  " + w));
        }

        public void addMailListToDatabase(Database db, int webId)
        {
            foreach (string mail in mailList)
            {
                if (db.Mail.Count(m => m.Name == mail) == 0)
                {
                    Mail m = new Mail { Name = mail };
                    m.WebsiteId = webId;
                    db.Mail.Add(m);
                    db.SaveChanges();
                }
            }
        }

        public void addAddressListToDatabase(Database db, int webId)
        {
            foreach (string address in addressList)
            {
                if (db.Address.Count(m => m.Name == address) == 0)
                {
                    Address a = new Address { Name = address };
                    a.WebsiteId = webId;
                    db.Address.Add(a);
                    db.SaveChanges();
                }
            }
        }

        public void addTelephoneListToDatabase(Database db, int webId)
        {
            foreach (string tel in phoneList)
            {
                if (db.Telephone.Count(m => m.Number == tel) == 0)
                {
                    Telephone t = new Telephone { Number = tel };
                    t.WebsiteId = webId;
                    db.Telephone.Add(t);
                    db.SaveChanges();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class DbCommunicate
    {
        private Database db = new Database();

        public void PrintDatabase(System.Windows.Forms.TextBox textBox1)
        {
            textBox1.Clear();

            var webL = from w in db.Website select w;
            if (webL.Count() == 0) { textBox1.AppendText("  Baza danych jest pusta  "); }
            foreach (var item in webL)
            {
                textBox1.AppendText(item.Url);
                textBox1.AppendText(Environment.NewLine);

                var webM = from w in db.Mail where w.WebsiteId == item.WebsiteId select w;
                foreach (var i in webM)
                {
                    textBox1.AppendText("    "+i.Name);
                    textBox1.AppendText(Environment.NewLine);
                }

                var webT = from w in db.Telephone where w.WebsiteId == item.WebsiteId select w;
                foreach (var i in webT)
                {
                    textBox1.AppendText("    " + i.Number);
                    textBox1.AppendText(Environment.NewLine);
                }

                var webA = from w in db.Address where w.WebsiteId == item.WebsiteId select w;
                foreach (var i in webA)
                {
                    textBox1.AppendText("    " + i.Name);
                    textBox1.AppendText(Environment.NewLine);
                }
                textBox1.AppendText(Environment.NewLine);
            }
        }

        public void resetDatabase(System.Windows.Forms.TextBox textBox1)
        {
            textBox1.Clear();

            db.Database.ExecuteSqlCommand("delete from [WebsiteListPointer]");

            db.Database.ExecuteSqlCommand("delete from [WebsiteList]");
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([WebsiteList], RESEED, -1)");

            db.Database.ExecuteSqlCommand("delete from [Website]");
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Website], RESEED, -1)");

            db.Database.ExecuteSqlCommand("delete from [Mail]");
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Mail], RESEED, -1)");

            db.Database.ExecuteSqlCommand("delete from [Address]");
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Address], RESEED, -1)");

            db.Database.ExecuteSqlCommand("delete from [Telephone]");
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Telephone], RESEED, -1)");

            textBox1.AppendText(Environment.NewLine);
            textBox1.AppendText("Reset bazy danych. ");
            textBox1.AppendText(Environment.NewLine);
        }

        private void addToDatabaseWebInfo(WebSiteInfo WebInfo)
        {
            //powinno być sprawdzenie, czy tego juz tam nie ma
            if (db.Website.Count(t => t.Url == WebInfo.Url) == 0)
            {
                Website web_el = new Website { Url = WebInfo.Url };
                db.Website.Add(web_el);
                db.SaveChanges();

                web_el = db.Website.First(a => a.Url == WebInfo.Url);

                WebInfo.addMailListToDatabase(db, web_el.WebsiteId);

                WebInfo.addAddressListToDatabase(db, web_el.WebsiteId);

                WebInfo.addTelephoneListToDatabase(db, web_el.WebsiteId);

            }
            else
            {
                Website web_el = new Website();
                web_el = db.Website.First(a => a.Url == WebInfo.Url);

                WebInfo.addMailListToDatabase(db, web_el.WebsiteId);
                WebInfo.addAddressListToDatabase(db, web_el.WebsiteId);
                WebInfo.addTelephoneListToDatabase(db, web_el.WebsiteId);
            }
        }

        public void AddToDatabaseWebInfoList(List<WebSiteInfo> websitesInfoList )
        {
            websitesInfoList.ForEach(wIL => addToDatabaseWebInfo(wIL));
        }

        public void AddWebsiteListInfo(string result)
        {
            if (db.WebsiteList.Count(t => t.Url == result) == 0)
            {
                var web = new WebsiteList { Url = result };
                db.WebsiteList.Add(web);
                db.SaveChanges();
            }
        }

        public bool IsDbEmpty(ref int id)
        {
            id = 0;
            var webP = db.WebsiteListPointer.OrderByDescending(u => u.WebsiteListId).Take(1).ToList();
            try { id = webP[0].WebsiteListId; }
            catch
            {
                id = -1;
                return true;
            }

            return false;
        }

        public void AddWebsiteListPointer(int id)
        {
            db.Database.ExecuteSqlCommand("delete from [WebsiteListPointer]");
            WebsiteListPointer webp = new WebsiteListPointer() { WebsiteListId = id };
            db.WebsiteListPointer.Add(webp);
            db.SaveChanges();
        }

        public void GetWebsiteList(int id, ref string  webUrl,ref int webId)
        {
            WebsiteList web_el = new WebsiteList();
            web_el = db.WebsiteList.FirstOrDefault(w => w.WebsiteListId == id);
            if (web_el == null)
            {
                webUrl = "";
                webId = -1;
            }
            else
            {
                webUrl = web_el.Url;
                webId = web_el.WebsiteListId;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            WebDriver web = new WebDriver();

            /*wypisuje tekst i link podstrony */
            //textBox1.AppendText(web.WebElementToString(5));

            /*obsługa listy nazw stron*/
            //List<String> textSites = new List<String>();
            //textSites = web.allSitesFromMainSearchText();
            //foreach (String text in textSites)
            //{
            //    textBox1.AppendText('\n'+ text + '\n');
            //}

            web.runWebDriver();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

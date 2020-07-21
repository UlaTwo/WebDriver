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
            DbCommunicate db = new DbCommunicate();

            //uruchomienie webDrivera
            db.resetDatabase(textBox1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            WebDriver web = new WebDriver();

            //uruchomienie webDrivera
            web.runProgram();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DbCommunicate db = new DbCommunicate();
            db.PrintDatabase(textBox1);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

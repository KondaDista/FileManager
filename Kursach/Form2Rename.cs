using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    public partial class Form2Rename : Form
    {        
        private Form1 f1 ;
        private string type;

        public Form2Rename(Form1 form, string typef1)
        {
            InitializeComponent();
            f1 = form;
            type = typef1;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            if (type == "Folder")
            {
                string name = textBox1.Text;
                string pathFolder = Path.Combine(f1.path, name);
                Directory.CreateDirectory(pathFolder);
                Close();
            }
            if (type == "doc")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".doc");
                new FileInfo(pathFile).Create().Close();
                Close();
            }
            if (type == "xlsx")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".xlsx");
                new FileInfo(pathFile).Create().Close();
                Close();
            }
            if (type == "TXT")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".txt");
                new FileInfo(pathFile).Create().Close();
                Close();
            }
        }
    }
}

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
        private DirectoryInfo info;

        public Form2Rename(Form1 form, string typef1, DirectoryInfo info)
        {
            InitializeComponent();
            f1 = form;
            type = typef1;
            this.info = info;
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

                StreamWriter write = new StreamWriter(f1.logfile, true);
                write.WriteLine("[Create]" + "-Созданна директория-" + "[" + name + "]" + "[" + pathFolder + "]" + "[" + DateTime.Now.ToString() + "]");
                write.Close();
            }
            else if (type == "doc")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".doc");
                new FileInfo(pathFile).Create().Close();
                Close();

                StreamWriter write = new StreamWriter(f1.logfile, true);
                write.WriteLine("[Create]" + "-Создан файл-" + "[" + name + "]" + "[" + pathFile + "]" + "[" + DateTime.Now.ToString() + "]");
                write.Close();
            }
            else if (type == "xlsx")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".xlsx");
                new FileInfo(pathFile).Create().Close();
                Close();

                StreamWriter write = new StreamWriter(f1.logfile, true);
                write.WriteLine("[Create]" + "-Создан файл-" + "[" + name + "]" + "[" + pathFile + "]" + "[" + DateTime.Now.ToString() + "]");
                write.Close();
            }
            else if (type == "TXT")
            {
                string name = textBox1.Text;
                string pathFile = Path.Combine(f1.path, name + ".txt");
                new FileInfo(pathFile).Create().Close();
                Close();

                StreamWriter write = new StreamWriter(f1.logfile, true);
                write.WriteLine("[Create]" + "-Создан файл-" + "[" + name + "]" + "[" + pathFile + "]" + "[" + DateTime.Now.ToString() + "]");
                write.Close();
            }
            else if (type == "Rename")
            {
                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Move(info.FullName, info.Parent.FullName + "\\" + textBox1.Text);
                    Close();

                    StreamWriter write = new StreamWriter(f1.logfile, true);
                    write.WriteLine("[Rename]" + "-Переименованна директория-" + "[" + info.Name + "]" + " переименован в >> " + "[" + textBox1.Text + "]" + "[" + DateTime.Now.ToString() + "]");
                    write.Close();
                }
                else
                {
                    string ext = info.Name.Split('.')[1];
                    File.Move(info.FullName, info.Parent.FullName + "\\" + textBox1.Text + "." + ext);
                    Close();

                    StreamWriter write = new StreamWriter(f1.logfile, true);
                    write.WriteLine("[Rename]" + "-Переименованна директория-" + "[" + info.Name + "]" + " переименован в >> " + "[" + textBox1.Text + "." + ext + "]" + "[" + DateTime.Now.ToString() + "]");
                    write.Close();
                }
            }
        }
    }
}

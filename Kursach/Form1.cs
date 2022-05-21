using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    public partial class Form1 : Form
    {
        public string path;
        private int cut = 0;
        DriveInfo[] drivers;
       // string[] main = { "System", "Корзина", "Документы", "Изображения" };

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Form1()
        {
            InitializeComponent();
            button1.Text = "Обзор";
            button1.Click += new EventHandler(button1_Click);
            button2.Text = "<<";
            button2.Click += new EventHandler(button2_Click);
            Start();
        }

        private void Start()
        {
            listView1.Items.Clear();
            textBox1.Text = null;
            path = null;
            string[] LogicalDrives = Environment.GetLogicalDrives();
            foreach (string s in LogicalDrives)
            {
                listView1.Items.Add(s, 1);
            }
            drivers = DriveInfo.GetDrives();
            GetItems(path, listView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            //folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
                textBox1.Text = path;
                GetItems(path, listView1);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (path != null)
            {
                string[] s = path.Split(new char[] { '\\' });

                if (textBox1.Text.Length <= 3)
                {
                    path = null;
                    textBox1.Text = path;

                    listView1.Items.Clear();
                    string[] LogicalDrives = Environment.GetLogicalDrives();
                    foreach (string st in LogicalDrives)
                    {
                        listView1.Items.Add(st, 1);
                    }

                }
                else if (s.Length > 2)
                {
                    path = path.Remove(path.Length - s[s.Length - 1].Length - 1, s[s.Length - 1].Length + 1);
                    textBox1.Text = path;
                    GetItems(path, listView1);
                }
                else if (s.Length > 1)
                {
                    path = path.Remove(path.Length - s[s.Length - 1].Length, s[s.Length - 1].Length);
                    textBox1.Text = path;
                    GetItems(path, listView1);
                }
            }
        }



        private void GetItems(string path, ListView listView)
        {
            if (Directory.Exists(path))
            {
                string[] folders = Directory.GetDirectories(path);
                string[] files =  Directory.GetFiles(path);     
                //FileInfo[] files = new DirectoryInfo(path).GetFiles();

                listView1.Items.Clear();

                foreach (string f in folders)
                {
                    if ((new DirectoryInfo(f).Attributes & FileAttributes.Hidden) == 0)
                    {
                        string[] ss = f.Split(new char[] { '\\' });
                        // listView1.Items.Add(ss[ss.Length - 1], ss[ss.Length - 1] + Environment.NewLine);

                        DirectoryInfo theFolder = new DirectoryInfo(f);
                        ListViewItem lvItem = new ListViewItem(theFolder.Name);

                        long size = DirSize(theFolder, 163530033582);
                        int i;
                        string sizeSt = "";

                        if (size != 0)
                        {
                            for (i = 0; size > 1024; i++)
                            {
                                size /= 1024;
                            }

                            if (i == 0)
                            {
                                sizeSt = size.ToString() + " b";
                            }
                            else if (i == 1)
                            {
                                sizeSt = size.ToString() + " Kb";
                            }
                            else if (i == 2)
                            {
                                sizeSt = size.ToString() + " Mb";
                            }
                            else if (i == 3)
                            {
                                sizeSt = size.ToString() + " Gb";
                            }
                            
                            //sizeSt = size.ToString() + " b";
                        }
                        else
                        {
                            sizeSt = "";
                        }

                        lvItem.SubItems.Add(sizeSt);
                        lvItem.SubItems.Add(theFolder.LastWriteTime.ToShortDateString());
                        lvItem.SubItems.Add(theFolder.LastWriteTime.ToShortTimeString());
                        listView1.Items.Add(lvItem);
                    }
                }
                foreach(string f in files)
                {
                    if ((new DirectoryInfo(f).Attributes & FileAttributes.Hidden) == 0)
                    {
                        string[] ss = f.Split(new char[] { '\\' });           

                        FileInfo theFile = new FileInfo(f);
                        ListViewItem lvItem = new ListViewItem(theFile.Name);

                        long size = theFile.Length;
                        int i;
                        string sizeSt = "";

                        for (i = 0; size > 1024; i++)
                        {
                            size /= 1024;
                        }
                        if (i == 0)
                        {
                            sizeSt = size.ToString() + " b";
                        }
                        else if (i == 1)
                        {
                            sizeSt = size.ToString() + " Kb";
                        }
                        else if (i == 2)
                        {
                            sizeSt = size.ToString() + " Mb";
                        }
                        else if (i == 3)
                        {
                            sizeSt = size.ToString() + " Gb";
                        }

                        lvItem.SubItems.Add(sizeSt);
                        lvItem.SubItems.Add(theFile.LastWriteTime.ToShortDateString());
                        lvItem.SubItems.Add(theFile.LastWriteTime.ToShortTimeString());
                        listView1.Items.Add(lvItem);
                    }
                }
            }
        }
        public long DirSize(DirectoryInfo d, long aLimit = 0)
        {
            long Size = 0;
            string[] ss = path.Split(new char[] { '\\' });
            textBox2.Text = ss[0];

            if (d.Name != "Program Files" && d.Name != "Program Files (x86)" && d.Name != "Users" && d.Name != "Windows" && ss[0] != @"C:")
            {
                // Add file sizes.
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Size += fi.Length;
                    if (aLimit > 0 && Size > aLimit)
                        return Size;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    Size += DirSize(di, aLimit);
                    if (aLimit > 0 && Size > aLimit)
                        return Size;
                }
                return (Size);
            }
            else
            {
                return (Size);
            }            
        }  



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue == (char)Keys.Enter)
            {
                if (!textBox1.Text.Contains("System"))
                {
                    path = textBox1.Text;
                    GetItems(path, null);
                }
            }

            if (listView1.SelectedItems.Count != 0)
            {
                if(Directory.Exists(Path.Combine(path, listView1.FocusedItem.Text)))
                {
                    if (e.Control && e.KeyCode == Keys.C)
                    {
                        CopyToolStripMenuItem_Click(sender, e);
                    }
                    else if (e.Control && e.KeyCode == Keys.X)
                    {
                        CutToolStripMenuItem_Click(sender, e);
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        deleteToolStripMenuItem_Click(sender, e);
                    }
                    else if (e.Control && e.KeyCode == Keys.V)
                    {
                        InsertToolStripMenuItem_Click(sender, e);
                    }
                }
                else
                {
                    if (e.Control && e.KeyCode == Keys.C)
                    {
                        CopyToolStripMenuItem_Click(sender, e);
                    }
                    else if (e.Control && e.KeyCode == Keys.X)
                    {
                        CutToolStripMenuItem_Click(sender, e);
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        deleteToolStripMenuItem_Click(sender, e);
                    }
                }
            }
            else
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    insertToolStripMenuItem1_Click(sender, e);
                }
            }
            
        }


        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string f = listView1.SelectedItems[0].Text;

                if (path != null && listView1.FocusedItem.Text != "System")
                {
                    if (Directory.Exists(path + "\\" + f))
                    {
                        //папка
                        string[] s = path.Split(new char[] { '\\' });
                        if (s.Length > 2 || s.Length == 2 && s[s.Length - 1].Length > 0)
                        {
                            path += "\\";
                        }
                        path += f;
                        textBox1.Text = path;
                        GetItems(path, listView1);
                    }
                    else
                    {
                        //файл
                        string ff = path;
                        string[] s = ff.Split(new char[] { '\\' });
                        if (s.Length > 2 || s.Length == 2 && s[s.Length - 1].Length > 0)
                        {
                            ff += "\\";
                        }
                        ff += f;
                        Process.Start(ff);
                    }
                }
                else if(listView1.FocusedItem.Text != "System")
                {
                    path += listView1.SelectedItems[0].Text;
                    textBox1.Text = path;
                    GetItems(path, listView1);
                }
            }
        }
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count != 0 && listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    if(path == null)
                    {
                        foreach(var dr in drivers)
                        {
                            if (dr.Name == listView1.FocusedItem.Text)
                            {
                                CopyToolStripMenuItem.Visible = false;
                                InsertToolStripMenuItem.Visible = false;
                                CutToolStripMenuItem.Visible = false;
                                deleteToolStripMenuItem.Visible = false;
                                clearToolStripMenuItem.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        if (listView1.FocusedItem.Text == "System")
                        {
                            OpenToolStripMenuItem.Visible = false;
                            CopyToolStripMenuItem.Visible = false;
                            InsertToolStripMenuItem.Visible = false;
                            CutToolStripMenuItem.Visible = false;
                            deleteToolStripMenuItem.Visible = false;
                            clearToolStripMenuItem.Visible = false;
                        }
                        else if (listView1.FocusedItem.Text == "Документы")
                        {
                            OpenToolStripMenuItem.Visible = true;
                            CopyToolStripMenuItem.Visible = false;
                            InsertToolStripMenuItem.Visible = false;
                            CutToolStripMenuItem.Visible = false;
                            deleteToolStripMenuItem.Visible = false;
                            clearToolStripMenuItem.Visible = false;
                        }
                        else if (listView1.FocusedItem.Text == "Изображения")
                        {
                            OpenToolStripMenuItem.Visible = true;
                            CopyToolStripMenuItem.Visible = false;
                            InsertToolStripMenuItem.Visible = false;
                            CutToolStripMenuItem.Visible = false;
                            deleteToolStripMenuItem.Visible = false;
                            clearToolStripMenuItem.Visible = false;
                        }
                        else if (listView1.FocusedItem.Text == "Корзина")
                        {
                            OpenToolStripMenuItem.Visible = true;
                            CopyToolStripMenuItem.Visible = false;
                            InsertToolStripMenuItem.Visible = false;
                            CutToolStripMenuItem.Visible = false;
                            deleteToolStripMenuItem.Visible = false;
                            clearToolStripMenuItem.Visible = true;
                        }
                        else
                        {
                            OpenToolStripMenuItem.Visible = true;
                            CopyToolStripMenuItem.Visible = true;
                            InsertToolStripMenuItem.Visible = true;
                            CutToolStripMenuItem.Visible = true;
                            deleteToolStripMenuItem.Visible = true;
                            clearToolStripMenuItem.Visible = false;
                        }
                    }

                    if(cut == 1)
                    {
                        CopyToolStripMenuItem.Visible = false;
                        CutToolStripMenuItem.Visible = false;
                    }
                    contextMenuStrip1.Show(Cursor.Position);
                }
                else
                {
                    contextMenuStrip2.Show(Cursor.Position);

                }
            }
        }



        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                string f = listView1.SelectedItems[0].Text;

                if (path != null)
                {
                    if (Directory.Exists(path + "\\" + f))
                    {
                        //папка
                        string[] s = path.Split(new char[] { '\\' });
                        if (s.Length > 2 || s.Length == 2 && s[s.Length - 1].Length > 0)
                        {
                            path += "\\";
                        }
                        path += f;
                        textBox1.Text = path;
                        GetItems(path, listView1);
                    }
                    else
                    {
                        //файл
                        string ff = path;
                        string[] s = ff.Split(new char[] { '\\' });
                        if (s.Length > 2 || s.Length == 2 && s[s.Length - 1].Length > 0)
                        {
                            ff += "\\";
                        }
                        ff += f;
                        Process.Start(ff);
                    }
                }
                else
                {
                    path += listView1.SelectedItems[0].Text;
                    textBox1.Text = path;
                    GetItems(path, listView1);
                }
            }
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.SelectedItems[0].Text;

            Clipboard.SetText(path + '\\' + f);

        }
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        private void InsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathFile = Clipboard.GetText();
            string[] ss = pathFile.Split(new char[] { '\\' });
            string name = ss[ss.Length - 1];


            if (!Directory.Exists(pathFile + '\\' + listView1.FocusedItem.Text + "\\" + name) && !File.Exists(pathFile + '\\' + listView1.FocusedItem.Text + "\\" + name))
            {

                if (Directory.Exists(pathFile) || Directory.Exists(@"D:\Курсач\Корзина" + '\\' + name))
                {
                    if (cut == 1)
                    {
                        Directory.Move(@"D:\Курсач\Корзина" + '\\' + name, path + '\\' + listView1.FocusedItem.Text + "\\" + name);
                    }
                    else
                    {
                        Directory.CreateDirectory(path + '\\' + listView1.FocusedItem.Text + '\\' + name);
                        CopyDirectory(pathFile, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);
                    }
                }
                else
                {
                    if (cut == 1)
                    {
                        File.Copy(@"D:\Курсач\Корзина" + '\\' + name, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);
                        File.Delete(@"D:\Курсач\Корзина" + '\\' + name);
                    }
                    else
                    {
                        File.Copy(pathFile, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);
                    }
                }
                cut = 0;
            }

            GetItems(path, listView1);
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.FocusedItem.Text;
            string MoveFolder = @"D:\Курсач\Корзина";

            if (Directory.Exists(path + "\\" + f))
            {
                string pathFolder = Path.Combine(path, f);

                if (Directory.Exists(MoveFolder + '\\' + f))
                {
                    MessageBox.Show("Такой файл уже существует", "Внимание!", MessageBoxButtons.OK);
                }
                else
                {
                    Directory.Move(pathFolder, MoveFolder + "\\" + f);
                    Clipboard.SetText(path + '\\' + f);
                    cut = 1;
                }
            }
            else 
            {
                string pathFile = Path.Combine(path, f);
                FileInfo fileInf = new FileInfo(pathFile);

                if (fileInf.Exists)
                {                                    
                    string fileName = Path.GetFileName(pathFile);
                    string destFile = Path.Combine(MoveFolder, fileName);
                    string sourceFile = Path.Combine(MoveFolder, pathFile);

                    if (File.Exists(MoveFolder + '\\' + f))
                    {
                        MessageBox.Show("Такой файл уже существует", "Внимание!", MessageBoxButtons.OK);
                    }
                    else
                    {
                        File.Move(sourceFile, destFile);
                        Clipboard.SetText(path + '\\' + f);
                        cut = 1;
                    }

                }
            }

            GetItems(path, listView1);
        }
        private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {         
            string f = listView1.SelectedItems[0].Text;
            string pathF = path + "\\" + f;

            if (Directory.Exists(pathF))
            {
                MessageBox.Show(
                           "Имя каталога: " + new DirectoryInfo(pathF).Name + "\n" +
                           "Полное имя каталога: " + new DirectoryInfo(pathF).FullName + "\n" +
                           "Корневой каталог: " + new DirectoryInfo(pathF).Root + "\n" +
                           "Родительский каталог: " + new DirectoryInfo(pathF).Parent + "\n" +
                           "Время создания: " + new DirectoryInfo(pathF).CreationTime + "\n" +
                           "Время последнего использования: " + new DirectoryInfo(pathF).LastAccessTime + "\n" +
                           "Время последнего изменения: " + new DirectoryInfo(pathF).LastWriteTime);
                

            }
            else if (File.Exists(pathF))
            {
                MessageBox.Show(
                           "Имя файла: " + new FileInfo(pathF).Name + "\n" +
                           "Полное имя файла: " + new FileInfo(pathF).FullName + "\n" +
                           "Родительский каталог: " + new FileInfo(pathF).DirectoryName + "\n" +
                           "Размер файла: " + new FileInfo(pathF).Length + "\n" +
                           "Расширение файла: " + new FileInfo(pathF).Extension);
                
            }
            else
            {
                foreach (var dr in drivers)
                {
                    textBox2.Text = dr.Name;
                    if (dr.Name == listView1.FocusedItem.Text)
                    {
                        double TotalSize = Math.Round(dr.TotalSize / Math.Pow(2, 30), 2);
                        double TotalFreeSpace = Math.Round(dr.TotalFreeSpace / Math.Pow(2, 30), 2);
                        MessageBox.Show("Имя диска: " + dr.Name +
                            "\n" + "Тип диска: " + dr.DriveType +
                            "\n" + "Метка тома: " + dr.VolumeLabel +
                            "\n" + "Файловая система: " + dr.DriveFormat +
                            "\n" + "Объём памяти: " + TotalSize +
                            "\n" + "Объём свободной памяти: " + TotalFreeSpace +
                            "\n" + "Объём занятой памяти: " + Math.Round(TotalSize - TotalFreeSpace, 2));
                        break;
                    }
                }             
            }
        }     
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.FocusedItem.Text;
            string MoveFolder = @"D:\Курсач\Корзина";

            if (Directory.Exists(path + "\\" + f))
            {
                string pathFolder = Path.Combine(path, f);
                Directory.Move(pathFolder, MoveFolder + "\\" + f);
            }
            else 
            {
                string pathFile = Path.Combine(path, f);
                FileInfo fileInf = new FileInfo(pathFile);

                if (fileInf.Exists)
                {
                    string fileName = Path.GetFileName(pathFile);
                    string destFile = Path.Combine(MoveFolder, fileName);
                    string sourceFile = Path.Combine(MoveFolder, pathFile);

                    if (File.Exists(MoveFolder + '\\' + f))
                    {
                        MessageBox.Show("Такой файл уже существует", "Внимание!", MessageBoxButtons.OK);
                    }
                    else
                    {
                        File.Move(sourceFile, destFile);
                    }

                }
            }

            GetItems(path, listView1);
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string MoveFolder = @"D:\Курсач\Корзина";

            if (MessageBox.Show("Вы уверенны, что хотите отчистить корзину?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var fold in new DirectoryInfo(MoveFolder).GetDirectories())
                {
                    fold.Delete(true);
                }
                foreach (var file in new DirectoryInfo(MoveFolder).GetFiles())
                {
                    file.Delete();
                }
            }
        }      


        
        
        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string type = "Folder";
            Form2Rename renameDialog = new Form2Rename(this, type);
            renameDialog.ShowDialog(this);

            GetItems(path, listView1);
        }
        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string type = "doc";
            Form2Rename renameDialog = new Form2Rename(this, type);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void exelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string type = "xlsx";
            Form2Rename renameDialog = new Form2Rename(this, type);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void notepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string type = "TXT";
            Form2Rename renameDialog = new Form2Rename(this, type);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void insertToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string pathFile = Clipboard.GetText();
            string[] ss = pathFile.Split(new char[] { '\\' });
            string name = ss[ss.Length - 1];

            if (!Directory.Exists(pathFile + "\\" + name) && !File.Exists(pathFile + "\\" + name))
            {
                if (Directory.Exists(pathFile) || Directory.Exists(@"D:\Курсач\Корзина" + '\\' + name))
                {
                    if (cut == 1)
                    {
                        Directory.Move(@"D:\Курсач\Корзина" + '\\' + name, path  + "\\" + name);
                    }
                    else
                    {
                        Directory.CreateDirectory(path  + '\\' + name);
                        CopyDirectory(pathFile, path  + '\\' + name, true);
                    }
                }
                else
                {
                    if (cut == 1)
                    {
                        File.Copy(@"D:\Курсач\Корзина" + '\\' + name, path + '\\' + name, true);
                        File.Delete(@"D:\Курсач\Корзина" + '\\' + name);
                    }
                    else
                    {
                        File.Copy(pathFile, path + '\\' + name, true);
                    }
                }
                cut = 0;
            }
            GetItems(path, listView1);
        }


        private void controlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("control");
        }
        private void resourceMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Taskmgr.exe");
        }
        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("msinfo32");
        }
        private void commandLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("cmd");
        }

        private void aboutTheProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("О программе");
        }
        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if(m.Msg == 0x0219)
            {
                switch ((int)m.WParam)
                {
                    case 0x8000:
                        if (textBox1.Text == "")
                        {
                            Start();
                        }
                        break;
                    case 0x8004:
                        if (textBox1.Text != "")
                        {
                            bool check = false;
                            foreach (DriveInfo dr in DriveInfo.GetDrives())
                            {
                                if (dr.Name == new DirectoryInfo(textBox1.Text).Root.ToString())
                                {
                                    check = true;
                                    break;
                                }

                            }
                            if (!check)
                             {
                                  Start();
                             }
                        }
                        else
                        {
                            Start();
                        }
                        break;
                }
            }
        }

        
    }
}

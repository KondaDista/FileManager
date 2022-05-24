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
using System.IO.MemoryMappedFiles;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

namespace Kursach
{
    public partial class Form1 : Form
    {
        public string path;
        public string localpath;
        public string[] discs;
        public string dragitem;
        private int cut = 0;
        DriveInfo[] drivers;
        public string logfile = "logfile.txt";

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
            discs = Environment.GetLogicalDrives();
            GetItems(path, listView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;

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

                StreamWriter write = new StreamWriter(logfile, true);
                write.WriteLine("[Back]" + "-Возврат к предыдущей директории -" + "[" + s[s.Length - 1] + "]" + ">>>" + "[" + path + "]" + "[" + DateTime.Now.ToString() + "]");
                write.Close();
            }
        }



        private void GetItems(string path, ListView listView)
        {
            if (discs.Contains(path) && new DriveInfo(path).DriveType == DriveType.Fixed)
            {
                listView1.Items.Clear();

                DirectoryInfo info = new DirectoryInfo(path);

                foreach (var i in info.GetDirectories())
                {
                    if (i.Name.Equals("FileManager"))
                    {
                        listView1.Items.Add(i.Name, 1);
                    }
                }
            }
            else {
                if (Directory.Exists(path))
                {
                    string[] folders = Directory.GetDirectories(path);
                    string[] files = Directory.GetFiles(path);

                    listView1.Items.Clear();

                    foreach (string f in folders)
                    {
                        if ((new DirectoryInfo(f).Attributes & FileAttributes.Hidden) == 0)
                        {
                            string[] ss = f.Split(new char[] { '\\' });

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
                    foreach (string f in files)
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
            if (e.KeyValue == (char)Keys.Enter)
            {
                if (!textBox1.Text.Contains("System"))
                {
                    path = textBox1.Text;
                    GetItems(path, null);
                }
            }

            if (listView1.SelectedItems.Count != 0)
            {
                if (Directory.Exists(Path.Combine(path, listView1.FocusedItem.Text)))
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
                    else if (e.Control && e.KeyCode == Keys.R)
                    {
                        renameToolStripMenuItem_Click(sender, e);
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
                    else if (e.Control && e.KeyCode == Keys.R)
                    {
                        renameToolStripMenuItem_Click(sender, e);
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

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Open]" + "-Переход к директории-" + "[" + f + "]" + "[" + path + '\\' + f + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
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

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Open]" + "-Открытие файла-" + "[" + ff + "]" + "[" + path + '\\' + f + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                }
                else if (listView1.FocusedItem.Text != "System")
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
                    if (path == null)
                    {
                        foreach (var dr in drivers)
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

                    if (cut == 1)
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

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Open]" + "-Переход к директории-" + "[" + f + "]" + "[" + path + '\\' + f + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
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

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Open]" + "-Открытие файла-" + "[" + ff + "]" + "[" + path + '\\' + f + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
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

            StreamWriter write = new StreamWriter(logfile, true);
            write.WriteLine("[Copy]" + "-Обьект скопирован-" + "[" + f + "]" + "[" + path + '\\' + f + "]" + "[" + DateTime.Now.ToString() + "]");
            write.Close();

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
                        Directory.Move(@"D:\FileManager\Корзина" + '\\' + name, path + '\\' + listView1.FocusedItem.Text + "\\" + name);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Вырезанная директоия вставлена-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    else
                    {
                        Directory.CreateDirectory(path + '\\' + listView1.FocusedItem.Text + '\\' + name);
                        CopyDirectory(pathFile, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Скопированная директория вставлена-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                }
                else
                {
                    if (cut == 1)
                    {
                        File.Copy(@"D:\FileManager\Корзина" + '\\' + name, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);
                        File.Delete(@"D:\FileManager\Корзина" + '\\' + name);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Вырезанный файл вставлен-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    else
                    {
                        File.Copy(pathFile, path + '\\' + listView1.FocusedItem.Text + '\\' + name, true);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Скопированный файл вставлен-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                }
                cut = 0;
            }
            else
            {
                MessageBox.Show("Такая директория или файл уже существует", "Внимание!", MessageBoxButtons.OK);
            }

            GetItems(path, listView1);
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.FocusedItem.Text;
            string MoveFolder = @"D:\FileManager\Корзина";

            if (Directory.Exists(path + "\\" + f))
            {
                string pathFolder = Path.Combine(path, f);

                if (Directory.Exists(MoveFolder + '\\' + f))
                {
                    MessageBox.Show("Такая директория уже существует", "Внимание!", MessageBoxButtons.OK);
                }
                else
                {
                    Directory.Move(pathFolder, MoveFolder + "\\" + f);
                    Clipboard.SetText(path + '\\' + f);
                    cut = 1;

                    StreamWriter write = new StreamWriter(logfile, true);
                    write.WriteLine("[Cut]" + "-Директоия вырезана-" + "[" + new DirectoryInfo(pathFolder).Name + "]" + "[" + new DirectoryInfo(pathFolder).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                    write.Close();
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

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Cut]" + "-Файл вырезан-" + "[" + new DirectoryInfo(pathFile).Name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
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
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.SelectedItems[0].Text;
            string pathF = path + "\\" + f;
            string type = "Rename";

            DirectoryInfo info = new DirectoryInfo(pathF);
            Form2Rename renameDialog = new Form2Rename(this, type, info);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string f = listView1.FocusedItem.Text;
            string MoveFolder = @"D:\FileManager\Корзина";

            if (Directory.Exists(path + "\\" + f))
            {
                string pathFolder = Path.Combine(path, f);

                if (Directory.Exists(MoveFolder + '\\' + f))
                {
                    MessageBox.Show("Такая директория уже существует", "Внимание!", MessageBoxButtons.OK);
                }
                else
                {
                    if (MessageBox.Show("Удолить безвозвратно?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Directory.Delete(pathFolder);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Delete]" + "-Директоия удоленна безвозвратно-" + "[" + new DirectoryInfo(pathFolder).Name + "]" + "[" + new DirectoryInfo(pathFolder).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    else
                    {
                        Directory.Move(pathFolder, MoveFolder + "\\" + f);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Delete]" + "-Директоия перемещенна в корзину-" + "[" + new DirectoryInfo(pathFolder).Name + "]" + "[" + new DirectoryInfo(pathFolder).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
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
                        if (MessageBox.Show("Удолить безвозвратно?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Delete(pathFile);

                            StreamWriter write = new StreamWriter(logfile, true);
                            write.WriteLine("[Delete]" + "-Файл удолен безвозвратно-" + "[" + new FileInfo(pathFile).Name + "]" + "[" + new FileInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                            write.Close();
                        }
                        else
                        {
                            File.Move(sourceFile, destFile);

                            StreamWriter write = new StreamWriter(logfile, true);
                            write.WriteLine("[Delete]" + "-Файл перемещён в корзину-" + "[" + new FileInfo(pathFile).Name + "]" + "[" + new FileInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                            write.Close();
                        }
                    }

                }
            }

            GetItems(path, listView1);
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string MoveFolder = @"D:\FileManager\Корзина";

            if (MessageBox.Show("Вы уверенны, что хотите отчистить корзину?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var fold in new DirectoryInfo(MoveFolder).GetDirectories())
                {
                    fold.Delete(true);

                    StreamWriter write = new StreamWriter(logfile, true);
                    write.WriteLine("[Clear]" + "-Директоия удолена безвозвратно из корзины-" + "[" + fold.Name + "]" + "[" + fold.FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                    write.Close();
                }
                foreach (var file in new DirectoryInfo(MoveFolder).GetFiles())
                {
                    file.Delete();

                    StreamWriter write = new StreamWriter(logfile, true);
                    write.WriteLine("[Clear]" + "-Файл удолен безвозвратно из корзины-" + "[" + file.Name + "]" + "[" + file.FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                    write.Close();

                }
            }
        }




        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathF = path;
            string type = "Folder";

            DirectoryInfo info = new DirectoryInfo(pathF);
            Form2Rename renameDialog = new Form2Rename(this, type, info);
            renameDialog.ShowDialog(this);

            GetItems(path, listView1);
        }
        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathF = path;
            string type = "doc";

            DirectoryInfo info = new DirectoryInfo(pathF);
            Form2Rename renameDialog = new Form2Rename(this, type, info);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void exelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathF = path;
            string type = "xlsx";

            DirectoryInfo info = new DirectoryInfo(pathF);
            Form2Rename renameDialog = new Form2Rename(this, type, info);
            renameDialog.ShowDialog(this);
            GetItems(path, listView1);
        }
        private void notepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathF = path;
            string type = "TXT";

            DirectoryInfo info = new DirectoryInfo(pathF);
            Form2Rename renameDialog = new Form2Rename(this, type, info);
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
                if (Directory.Exists(pathFile) || Directory.Exists(@"D:\FileManager\Корзина" + '\\' + name))
                {
                    if (cut == 1)
                    {
                        Directory.Move(@"D:\FileManager\Корзина" + '\\' + name, path + "\\" + name);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Вырезанная директоия вставлена-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    else
                    {
                        Directory.CreateDirectory(path + '\\' + name);
                        CopyDirectory(pathFile, path + '\\' + name, true);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Скопированная директория вставлена-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                }
                else
                {
                    if (cut == 1)
                    {
                        File.Copy(@"D:\FileManager\Корзина" + '\\' + name, path + '\\' + name, true);
                        File.Delete(@"D:\FileManager\Корзина" + '\\' + name);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Вырезанный файл вставлен-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    else
                    {
                        File.Copy(pathFile, path + '\\' + name, true);

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[Insert]" + "-Скопированный файл вставлен-" + "[" + name + "]" + "[" + new DirectoryInfo(pathFile).FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                }
                cut = 0;
            }
            else
            {
                MessageBox.Show("Такая директория или файл уже существует", "Внимание!", MessageBoxButtons.OK);
            }
            GetItems(path, listView1);
        }


        private void controlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("control");

            StreamWriter write = new StreamWriter(logfile, true);
            write.WriteLine("[Utilities]" + "-Запущенна утилита-" + "[СontrolPanel]" + "[" + DateTime.Now.ToString() + "]");
            write.Close();
        }
        private void resourceMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Taskmgr.exe");

            StreamWriter write = new StreamWriter(logfile, true);
            write.WriteLine("[Utilities]" + "-Запущенна утилита-" + "[ResourceMonitoring]" + "[" + DateTime.Now.ToString() + "]");
            write.Close();
        }
        private void systemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("msinfo32");

            StreamWriter write = new StreamWriter(logfile, true);
            write.WriteLine("[Utilities]" + "-Запущенна утилита-" + "[SystemInformation]" + "[" + DateTime.Now.ToString() + "]");
            write.Close();
        }
        private void commandLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("cmd");

            StreamWriter write = new StreamWriter(logfile, true);
            write.WriteLine("[Utilities]" + "-Запущенна утилита-" + "[CommandLine]" + "[" + DateTime.Now.ToString() + "]");
            write.Close();
        }

        private void aboutTheProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Операционные системы и оболочки\nЯзык программирования: C#\nКонцевич Даниил Дмитриевич\nРПИС-03");
        }
        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("readme.txt");
        }



        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0219)
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



        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem itemdrag = listView1.FocusedItem;

            if (discs.Contains(itemdrag.Text) || discs.Contains(path))
            {
                localpath = path + itemdrag.Text;
            }
            else
            {
                localpath = path + "\\" + itemdrag.Text;
            }

            if (localpath.Length == 3)
            {
                if (new DriveInfo(localpath).DriveType == DriveType.Fixed)
                {
                    MessageBox.Show("No access!");
                }
                else
                {
                    MessageBox.Show("No access!");
                }
            }
            else if (new DriveInfo(localpath.Substring(0, 3)).DriveType == DriveType.Fixed && localpath.Contains("System"))
            {
                MessageBox.Show("No access!");
            }
            else if (itemdrag.Text == "Корзина")
            {
                MessageBox.Show("No access!");
            }

            else if (localpath.Split('\\').Length == 2 && localpath.Split('\\')[1].Equals("FileManager"))
            {
                MessageBox.Show("No access!");
            }
            else
            {
                dragitem = localpath;
                listView1.DoDragDrop(itemdrag, DragDropEffects.Move);
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];

            if (discs.Contains(item.Text) || discs.Contains(path))
            {
                localpath = path + item.Text;
            }
            else
            {
                localpath = path + "\\" + item.Text;
            }

            DirectoryInfo targetinfo = new DirectoryInfo(localpath);
            DirectoryInfo dragedinfo = new DirectoryInfo(dragitem);
            if ((targetinfo.Attributes & FileAttributes.Directory) != 0)
            {
                if ((dragedinfo.Attributes & FileAttributes.Directory) != 0)
                {
                    try
                    {
                        Directory.Move(dragedinfo.FullName, Path.Combine(targetinfo.FullName, dragedinfo.Name));

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[DragAndDrop]" + "-Директория перемещена-" + "[" + dragedinfo.Name + "]" + "[" + dragedinfo.FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
                else
                {
                    try
                    {
                        File.Move(dragedinfo.FullName, Path.Combine(targetinfo.FullName, dragedinfo.Name));

                        StreamWriter write = new StreamWriter(logfile, true);
                        write.WriteLine("[DragAndDrop]" + "-Файл перемещена-" + "[" + dragedinfo.Name + "]" + "[" + dragedinfo.FullName + "]" + "[" + DateTime.Now.ToString() + "]");
                        write.Close();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }

            //listView1.Clear();
            GetItems(path, listView1);
        }
        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem dragover = listView1.HitTest(listView1.PointToClient(new Point(e.X, e.Y))).Item;

            if (dragover != null)
            {
                dragover.Selected = true;
            }
            e.Effect = DragDropEffects.Move;

        }


        private string FindPath(string name)
        {
            foreach (var disc in DriveInfo.GetDrives())
            {
                foreach (var manag in Directory.GetDirectories(disc.Name, "FileManager"))
                {
                    foreach (var dirs in new DirectoryInfo(Path.Combine(disc.Name, manag)).GetDirectories(name, SearchOption.AllDirectories))
                    {
                        return dirs.FullName;
                    }
                }
            }

            return null;
        }


        private void logFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("logfile.txt");
        }
        private void windowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(FindPath("CmdWindows"), "bin\\Debug\\CmdWindows"));
        }
        private void linuxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(FindPath("CmdLinux"), "bin\\Debug\\CmdLinux"));
        }



        private void memoryMappedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Name
            string name = "";
            foreach (var p in Process.GetProcessesByName("explorer"))
            {
                name = GetProcessOwner(p.Id);
            }
            var usernameParts = name.Split('\\');
            name = usernameParts[usernameParts.Length - 1];
            #endregion

            #region CP
            Timer_Tick();
            #endregion

            #region Memory
            MEMORYSTATUS memStatus = new MEMORYSTATUS();
            GlobalMemoryStatus(ref memStatus);
            string dwAvail = memStatus.dwAvailPageFile.ToString();
            long dwInt = long.Parse(dwAvail) / 1024;
            #endregion   

            char[] message = ("Имя пользователя: " + name + "\n"+ "Количество свободных байтов файла подкачки: " + dwInt + " Kb" + "\n" + "использованное время ЦП: "+ CP + "\n" ).ToCharArray();
            //Размер введенного сообщения
            int size = message.Length;

            //Создание участка разделяемой памяти
            //Первый параметр - название участка, 
            //второй - длина участка памяти в байтах: тип char  занимает 2 байта 
            //плюс четыре байта для одного объекта типа Integer
            MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("MemoryFile", size * 2 + 4);

            //Создаем объект для записи в разделяемый участок памяти
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * 2 + 4))
            {
                //запись в разделяемую память
                //запись размера с нулевого байта в разделяемой памяти
                writer.Write(0, size);
                //запись сообщения с четвертого байта в разделяемой памяти
                writer.WriteArray(4, message, 0, message.Length);
            }

            textBox2.Text = "Сообщение записано в разделяемую память";

            Process.Start(Path.Combine(FindPath("Memory"), "bin\\Debug\\Memory"));
            
        }

        #region Name
        public static string GetProcessOwner(int processId)
        {
            var query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectCollection processList;

            using (var searcher = new ManagementObjectSearcher(query))
            {
                processList = searcher.Get();
            }

            foreach (var mo in processList.OfType<ManagementObject>())
            {
                object[] argList = { string.Empty, string.Empty };
                var returnVal = Convert.ToInt32(mo.InvokeMethod("GetOwner", argList));

                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
        #endregion

        #region ЦП
        private PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");

        private string CP;
        public void Timer_Tick()
        {
            CP = this.theCPUCounter.NextValue().ToString() + "%     " + this.theMemCounter.NextValue().ToString() + "MB";
        }
        #endregion

        #region Работа с Памятью

        //апи функция для просмотра памяти
        [DllImport("kernel32.dll")]
        //для выделения памяти
        public static extern IntPtr GlobalAlloc(int con, int size);
        [DllImport("kernel32.dll")]
        //для освобождения
        public static extern int GlobalFree(IntPtr start);
        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;               //Размер структуры, в байтах
            public UInt32 dwMemoryLoad;           //процент занятой памяти
            public UInt32 dwTotalPhys;            //общее кол-во физической(оперативной) памяти
            public UInt32 dwAvailPhys;            //свободное кол-во физической(оперативной) памяти
            public UInt32 dwTotalPageFile;        //предел памяти для системы или текущего процесса
            public UInt32 dwAvailPageFile;        //Максимальный объем памяти,который текущий процесс может передать в байтах.
            public UInt32 dwTotalVirtual;         //общее количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailVirtual;         //свободное количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailExtendedVirtual; //Зарезервировано. Постоянно 0.
        }
        #endregion

        


    }
}

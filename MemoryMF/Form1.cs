﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryMF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Start();
        }

        public void Start()
        {
            //Массив для сообщения из общей памяти
            char[] message;

            //Размер введенного сообщения
            int size;

            //Получение существующего участка разделяемой памяти
            //Параметр - название участка
            MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("MemoryFile");
            //Сначала считываем размер сообщения, чтобы создать массив данного размера
            //Integer занимает 4 байта, начинается с первого байта, поэтому передаем цифры 0 и 4
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 4, MemoryMappedFileAccess.Read))
            {
                size = reader.ReadInt32(0);
            }

            //Считываем сообщение, используя полученный выше размер
            //Сообщение - это строка или массив объектов char, каждый из которых занимает два байта
            //Поэтому вторым параметром передаем число символов умножив на из размер в байтах плюс
            //А первый параметр - смещение - 4 байта, которое занимает размер сообщения
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(4, size * 2, MemoryMappedFileAccess.Read))
            {
                //Массив символов сообщения
                message = new char[size];
                reader.ReadArray<char>(0, message, 0, size);
            }

            textBox1.Text = "Получено сообщение";
            textBox2.Text = "Messege: " + message.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }
    }
}
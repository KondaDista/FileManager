using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memory
{
    class Program
    {
        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");
        private static string CP;
        private static bool stopProcessing = true;

        public static string logfileMemory = "logfileMemory.txt";

        public static async Task Timer_TickAsync()
        {
            if (stopProcessing)
            {
                CP = theCPUCounter.NextValue().ToString() + "%    " + theMemCounter.NextValue().ToString() + "MB";
                Console.WriteLine(CP + "   Для остоновки программы нажмите любую клавишу");
                StreamWriter write = new StreamWriter(logfileMemory, true);
                write.WriteLine(CP);
                write.Close();

                await Task.Delay(1000);
                Timer_TickAsync();
            }
        }

        static void Main(string[] args)
        {
            //Массив для сообщения из общей памяти
            char[] message;
            int size;

            //Получение существующего участка разделяемой памяти
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
            Console.WriteLine("Получено сообщение :");
            Console.WriteLine(message);

            StreamWriter write = new StreamWriter(logfileMemory, true);
            write.WriteLine(message);
            write.Close();

            Console.WriteLine("Динамический список использованного времени ЦП:");
            Timer_TickAsync();

            Console.ReadLine();
            stopProcessing = false;
            Wait();

            void Wait()
            {
                Console.WriteLine("Для продоложения вывода нажмите Y, для выхода из программы и сохранении информации нажмите - N");
                stopProcessing = Console.ReadKey(true).Key == ConsoleKey.Y;

                if (stopProcessing)
                {
                    Timer_TickAsync();
                    Console.ReadLine();
                    stopProcessing = false;
                    Wait();
                }
                else
                {
                    Console.WriteLine("Для продоложения введите имя файла");
                    string name = Console.ReadLine();
                    File.Copy(@"E:\FileManager\System\KursachPre\Kursach\bin\Debug\logfileMemory.txt", @"E:\FileManager\System\KursachPre\Kursach\bin\Debug" + '\\' + name +".txt", true);

                }
            }
        }     

    }
}

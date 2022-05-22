using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;

namespace CmdWindows
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Чтобы получить информацию о коммандах введите: /help \n");
                string command = Console.ReadLine();
                string[] arguments = command.Split(' ');
                string sessionname;
                switch (arguments[0])
                {
                    case "tasklist":

                        if (arguments.Length == 1)
                        {
                            foreach (var VARIABLE in Process.GetProcesses())
                            {
                                if (VARIABLE.SessionId == 0)
                                {
                                    sessionname = "Services";
                                }
                                else
                                {
                                    sessionname = "Console";
                                }
                                //cmd
                                Console.WriteLine(VARIABLE.ProcessName + " " + VARIABLE.Id + " " + VARIABLE.SessionId + " " + sessionname + " " + VARIABLE.WorkingSet64 / 1024 + " KB");
                            }
                        }
                        else
                        {
                            if (arguments[2].Equals("\"USERNAME") && arguments[3].Equals("eq"))
                            {
                                string user = arguments[4].Remove(arguments[4].Length - 1, 1);
                                foreach (var VARIABLE in Process.GetProcesses())
                                {
                                    if (VARIABLE.SessionId == 0)
                                    {
                                        sessionname = "Services";
                                    }
                                    else
                                    {
                                        sessionname = "Console";
                                    }

                                    if (user.Equals(GetProcessOwner(VARIABLE.Id)))
                                        Console.WriteLine(GetProcessOwner(VARIABLE.Id) + " " + VARIABLE.ProcessName + " " + VARIABLE.Id + " " + VARIABLE.SessionId + " " + sessionname + " " + VARIABLE.WorkingSet64 / 1024 + " KB");
                                }
                            }
                            else if (arguments[2].Equals("\"PID") && arguments[3].Equals("eq"))
                            {
                                int pid = Int32.Parse(arguments[4].Remove(arguments[4].Length - 1, 1));
                                Process process = Process.GetProcessById(pid);

                                if (process.SessionId == 0)
                                {
                                    sessionname = "Services";
                                }
                                else
                                {
                                    sessionname = "Console";
                                }

                                Console.WriteLine(process.ProcessName + " " + process.Id + " " + process.SessionId + " " + sessionname + " " + process.WorkingSet64 / 1024 + " KB");
                            }
                            else if (arguments[2].Equals("\"IMAGENAME") && arguments[3].Equals("eq"))
                            {
                                string name = arguments[4].Remove(arguments[4].Length - 1, 1);
                                foreach (var proc in Process.GetProcessesByName(name))
                                {
                                    if (proc.SessionId == 0)
                                    {
                                        sessionname = "Services";
                                    }
                                    else
                                    {
                                        sessionname = "Console";
                                    }

                                    Console.WriteLine(proc.ProcessName + " " + proc.Id + " " + proc.SessionId + " " + sessionname + " " + proc.WorkingSet64 / 1024 + " KB");
                                }
                            }
                            else if (arguments[2].Equals("\"MEMUSAGE") && arguments[3].Equals("gt"))
                            {
                                int size = Int32.Parse(arguments[4].Remove(arguments[4].Length - 1, 1));
                                foreach (var VARIABLE in Process.GetProcesses())
                                {
                                    if (VARIABLE.SessionId == 0)
                                    {
                                        sessionname = "Services";
                                    }
                                    else
                                    {
                                        sessionname = "Console";
                                    }

                                    if (VARIABLE.WorkingSet64 / 1024 >= size)
                                        Console.WriteLine(VARIABLE.ProcessName + " " + VARIABLE.Id + " " + VARIABLE.SessionId + " " + sessionname + " " + VARIABLE.WorkingSet64 / 1024 + " KB");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong command!");
                            }
                        }

                        break;
                    case "taskkill":
                        if (arguments[1].Equals("/im"))
                        {
                            string klname = arguments[2];
                            try
                            {
                                foreach (var VARIABLE in Process.GetProcessesByName(klname))
                                {
                                    VARIABLE.Kill();
                                    Console.WriteLine("Процессы успешно приостановлены");
                                }
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                            }
                        }
                        else if (arguments[1].Equals("/pid"))
                        {
                            int kid = Int32.Parse(arguments[2]);
                            try
                            {
                                Process.GetProcessById(kid).Kill();
                                Console.WriteLine("Процесс успешно приостановлен");
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong command!");
                        }

                        break;
                    case "devcon":
                        switch (arguments[1])
                        {
                            case "desktopMonitor":
                                DesktopMonitor();
                                break;
                            case "soundDevice":
                                SoundDevice();
                                break;
                            case "keyboard":
                                Keyboard();
                                break;
                            case "pointingDevice":
                                PointingDevices();
                                break;
                            default:
                                Console.WriteLine("Wrong command!");
                                break;
                        }

                        break;
                    case "/help":
                        Console.WriteLine(
                            "Все комманды: \n" +
                            "tasklist - выводит все имеющиеся процессы\n" +
                            "tasklist /fi \"USERNAME\" eq ?\" - выводит все процессы вписанного пользователя\n" +
                            "tasklist /fi \"PID eq ?\" - выводит процесс с вписанным ID \n" +
                            "tasklist /fi \"IMAGENAME eq ?\" - выводит все процессы с вписанным именем \n" +
                            "tasklist /fi \"MEMUSAGE gt ?\" - выводит все процессы,у которых память в КБ больше\\равна указанной \n" +
                            "taskkill /im ? - приостанавлиет все процессы с вписанным именем\n" +
                            "taskkill /pid ? - приостанавливает процесс с вписанным ID\n" +
                            "devcon desktopMonitor - выводит информацию о подключенных мониторах\n" +
                            "devcon soundDevice - выводит информацию о подключенных аудио-девайсах(устройствах)\n" +
                            "devcon keyboard - выводит информацию о подключенных клавиатурах\n" +
                            "devcon pointingDevice - выводит информацию о подключенных USB девайсах"
                        );
                        break;
                    default:
                        Console.WriteLine("Wrong command!");
                        break;
                }
            }
        }

        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    return argList[0];
                }
            }

            return "NO OWNER";
        }

        static void DesktopMonitor()
        {
            ManagementObjectSearcher win32Monitor = new ManagementObjectSearcher("select * from Win32_DesktopMonitor");
            foreach (ManagementObject obj in win32Monitor.Get())
            {
                Console.WriteLine(obj.ToString());
            }
        }
        static void SoundDevice()
        {
            ManagementObjectSearcher Win32_SoundDevice =
                new ManagementObjectSearcher("select * from Win32_SoundDevice");
            foreach (ManagementObject obj in Win32_SoundDevice.Get())
            {
                Console.WriteLine(obj.ToString());
            }
        }
        static void Keyboard()
        {
            ManagementObjectSearcher Win32_Keyboard = new ManagementObjectSearcher("select * from Win32_Keyboard");
            foreach (ManagementObject obj in Win32_Keyboard.Get())
            {
                Console.WriteLine(obj.ToString());
            }
        }
        static void PointingDevices()
        {
            ManagementObjectSearcher Win32_PointingDevice =
                new ManagementObjectSearcher("select * from Win32_PointingDevice");
            foreach (ManagementObject obj in Win32_PointingDevice.Get())
            {
                Console.WriteLine(obj.ToString());
            }
        }
    }
}
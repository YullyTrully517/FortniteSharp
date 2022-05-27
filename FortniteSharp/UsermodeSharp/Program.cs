using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using KernelMemorySharp;

namespace UsermodeSharp
{
    class Program
    {
        /*
         Credits
         Driver - Shalzuth 
         Usermode / Everything else - Yully
        */
        [DllImport("kernel32")] static extern IntPtr OpenProcess(Int32 dwDesiredAccess, Boolean bInheritHandle, Int32 dwProcessId);
        [DllImport("kernel32")] static extern Boolean ReadProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, Byte[] lpBuffer, Int32 dwSize, ref Int32 lpNumberOfBytesRead);
        [DllImport("kernel32")] static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")] static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32")] static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32")] static extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);
        [DllImport("kernel32")] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32")] static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        public static bool KernelConsole { get; set; } = false;
        public static bool IsMapped { get; set; } = false;
        static unsafe void Main(string[] args)
        {

            //INFO
            // This driver is detected and it will get you banned (this is a POC)

            string app = "FortniteClient-Win64-Shipping";

            Console.Title = "UsermodeSharp | Fortnite";

            if (!IsMapped)
            {
                if (!MemoryDriver.LoadDriver(@"MemoryDriver.sys"))
                {
                    Util.ConsoleError("Failed to load driver, press any key to exit");
                    Process.GetCurrentProcess().Kill();
                }
                else { Console.WriteLine(); Console.ReadKey(); }
            } else { Util.ConsoleSuccess("Driver already loaded!"); }

            Console.WriteLine();

            if (KernelConsole)
            {
                MemoryDriver.ShowConsole();
            }

            try
            {
                var process = Process.GetProcessesByName(app)[0];
                MemoryDriver._ProcessId = process.Id;
                MemoryDriver._BaseAddress = MemoryDriver.GetModule("kernel32.dll");
            }
            catch { Util.ConsoleError($"Process Not Found: {app[0]}"); }
     

            Console.WriteLine($"Fortnite PID:  {MemoryDriver.ProcessId}");
            Console.WriteLine($"Fortnite Base Address:  {MemoryDriver._BaseAddress}");

            Console.WriteLine();

            Util.ConsoleSuccess("Press Any Key To Return UWorld...");

            Console.ReadKey();
            

            Offsets.GWorld = MemoryDriver.ReadProcessMemory<Int32>(MemoryDriver.BaseAddress + 0xc6b5d08);

            Console.WriteLine();

            Console.WriteLine($"Fortnite UWorld -> {Offsets.GWorld}");

          
            Console.ReadLine();
        }
    }
}


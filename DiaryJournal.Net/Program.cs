using System.Diagnostics;

namespace DiaryJournal.Net
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            Process Proc = Process.GetCurrentProcess();
            long AffinityMask = (long)Proc.ProcessorAffinity;
            AffinityMask &= 0x000F; // use only any of the first 4 available processors
            Proc.ProcessorAffinity = (IntPtr)AffinityMask;

            ProcessThread Thread = Proc.Threads[0];
            AffinityMask = 0x0002; // use only the second processor, despite availability
            Thread.ProcessorAffinity = (IntPtr)AffinityMask;
            */

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmJournal());
        }

    }
}
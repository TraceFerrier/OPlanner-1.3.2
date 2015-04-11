using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;

namespace PlannerNameSpace
{
    public static class Tracer
    {
        static bool OffSwitch = true;
        public static void InitializeTraceFile()
        {
            if (!OffSwitch)
            {
                string traceFilePath = GetTraceFilePath();
                File.Delete(traceFilePath);
            }
        }

        public static void WriteTrace(string trace)
        {
            if (!OffSwitch)
            {
                try
                {
                    string traceFilePath = GetTraceFilePath();
                    using (StreamWriter traceFile = File.AppendText(traceFilePath))
                    {
                        traceFile.WriteLine(trace);
                    }
                }
                catch (Exception e)
                {
                    Globals.ApplicationManager.HandleException(e);
                }
            }
        }

        public static string GetTraceFilePath()
        {
            string rootFolder = ApplicationManager.GetOPlannerFolder();
            return Path.Combine(rootFolder, "tracer.txt");
        }
    }
}

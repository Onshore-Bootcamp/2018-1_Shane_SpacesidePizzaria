using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace SpaceSidePizzariaDAL
{
    public static class Logger
    {
        private static readonly string LogPath = ConfigurationManager.AppSettings["dalLogPath"];

        public static void Log(string level, string source, string target, string message, string stackTrace = null)
        {
            string timeStamp = DateTime.Now.ToString();
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(LogPath, true);
                writer.WriteLine("[{0}] - {1} - {2} - {3} - {4}", timeStamp, level, source, target, message);
                if (stackTrace != null)
                {
                    writer.WriteLine(stackTrace);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public static void LogExceptionNoRepeats(Exception exception)
        {
            if (!exception.Data.Contains("Logged"))
            {
                // If this exception has not been "Logged" key OR if the "Logged" key is false
                Log("Fatal", exception.Source, exception.TargetSite.Name, exception.Message, exception.StackTrace);
                exception.Data["Logged"] = true;
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Preprost zapis v log datoteko (logs\app-LLLLMMDD.log poleg .exe).
    /// V pravem projektu bi uporabili NLog/log4net/Serilog; tu je namen pokazati princip
    /// brez dodatnih NuGet paketov.
    /// </summary>
    public static class AppLogger
    {
        // INTERVJU: log se kliče iz več niti hkrati (UI nit, niti iz poola, globalni handler).
        // Brez zaklepanja bi se dve pisanji v isto datoteko lahko prepletli ali vrgli IOException
        // ("datoteko uporablja drug proces"). lock zagotovi, da piše naenkrat samo ena nit.
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Mapa z log datotekami.
        /// </summary>
        public static string LogDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"); }
        }

        /// <summary>
        /// Pot do današnje log datoteke (nova datoteka vsak dan).
        /// </summary>
        public static string CurrentLogFile
        {
            get { return Path.Combine(LogDirectory, "app-" + DateTime.Now.ToString("yyyyMMdd") + ".log"); }
        }

        /// <summary>
        /// Zapiše informativno sporočilo.
        /// </summary>
        public static void Info(string message)
        {
            Write("INFO ", message, null);
        }

        /// <summary>
        /// Zapiše napako s celotnim opisom izjeme (tip, sporočilo, stack trace, notranje izjeme).
        /// </summary>
        public static void Error(string message, Exception exception)
        {
            Write("ERROR", message, exception);
        }

        private static void Write(string level, string message, Exception exception)
        {
            var line = new StringBuilder();
            line.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append(" [").Append(level).Append("] ")
                // Id niti v logu pomaga pri razhroščevanju večnitnih napak.
                .Append("[nit ").Append(Thread.CurrentThread.ManagedThreadId).Append("] ")
                .AppendLine(message);

            if (exception != null)
            {
                // ToString() izjeme vsebuje tudi stack trace in vse InnerException - Message ne.
                line.AppendLine(exception.ToString());
            }

            try
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(CurrentLogFile, line.ToString(), Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                // Logiranje ne sme podreti aplikacije (npr. mapa samo za branje).
                // Napako namerno prezremo - druge možnosti za zapis nimamo.
            }
        }
    }
}

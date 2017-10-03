using System;
using System.Text;
using NLog;
using RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture;

namespace RestFixture.Net.FitNesseTestServer.HostConsole
{
    class Program
    {
        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();
        private static NLog.Logger MESSAGEONLYLOG = LogManager.GetLogger("MessageOnlyLogger");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MESSAGEONLYLOG.Debug(" ");
            MESSAGEONLYLOG.Debug(new string('*', 110));
            LOG.Debug("Application Starting");
            MESSAGEONLYLOG.Debug(new string('*', 110));

            HttpServerFixture f = new HttpServerFixture();
            LOG.Debug("Setting up Resources database...");
            f.resetResourcesDatabase();
            LOG.Debug("Resources database set up.");
            f.startServer("8765");

            Console.WriteLine();
            Console.WriteLine("Press R to reset the resources database or [ESCAPE] to stop the server...");
            // Any dummy key will do, as long as it isn't one we're interested in.
            ConsoleKeyInfo cki = new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false);
            while (cki.Key != ConsoleKey.Escape)
            {
                cki = Console.ReadKey(true);
                DisplayKeyPressed(cki);

                if (cki.Key == ConsoleKey.R)
                {
                    LOG.Debug("Resetting Resources database...");
                    f.resetResourcesDatabase();
                    LOG.Debug("Resources database reset.");
                }
            }
            Console.WriteLine();

            f.stopServer();

            MESSAGEONLYLOG.Debug(new string('-', 110));
            LOG.Debug("Application Closing");
            MESSAGEONLYLOG.Debug(new string('-', 110));
        }

        private static void DisplayKeyPressed(ConsoleKeyInfo cki)
        {
            string keyDescription = "";
            if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
            {
                keyDescription += "CTRL+";
            }
            if ((cki.Modifiers & ConsoleModifiers.Alt) != 0)
            {
                keyDescription += "ALT+";
            }
            if ((cki.Modifiers & ConsoleModifiers.Shift) != 0)
            {
                keyDescription += "SHIFT+";
            }
            keyDescription += cki.Key;
            Console.WriteLine(keyDescription);
        }
    }
}

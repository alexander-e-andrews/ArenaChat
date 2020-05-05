using System;
using System.Threading.Tasks;
using System.Windows;
using System.Net.WebSockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace chatwindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public KeyboardListener KListener = new KeyboardListener();

        private string username = "";
        void App_Startup(object sender, StartupEventArgs e)
        {
            configLoader.LoadYaml();
            //KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);

            // Application is running
            // Process command line args
            //Sleep just a tad to make sure user has opened mtga and it has had time to start logging
            Thread.Sleep(3000);

            CloseOnMTGClose();

            logParser.OpenLogFile();
            Console.WriteLine("Before get username");
            username = logParser.GetUsername();
            Console.WriteLine("After get username");
            //Now we get out username.

            Loop();
            
        }

        public async Task Loop()
        {
            Console.WriteLine("Here");
            //If we want to have a global chat, connect to the server here.

            //Instead we will wait until an enemy has been selected, and then we will open up our chat window
            string oppoenent = logParser.GetOpponent();
            Console.WriteLine(oppoenent);
            //We have an opponent, lets try to connect to the server
            bool connectionSuccesful= await internetManager.connectAsync(username, oppoenent);
            Console.WriteLine("JJ Wenworth");

            Console.WriteLine(connectionSuccesful);
            //If we connect launch window
            if (connectionSuccesful)
            {
                Console.WriteLine("Succesfully connected");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Console.WriteLine("No connection, wait for next match");
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
            internetManager.disconnect();
            Console.WriteLine("Shutdown");
        }

        private async Task CloseOnMTGClose()
        {
            do
            {
                await Task.Delay(250);
                
                Process[] mtgSearch = Process.GetProcessesByName("MTGA");
                try
                {
                    if (mtgSearch == null || mtgSearch.Length == 0)
                    {
                        Console.WriteLine("Closed becasue of not running magic isntance");
                        //Cnat get to exit normally anymore
                        Environment.Exit(0);
                        await Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            Console.WriteLine("Before Shutdown");
                            try
                            {
                                Current.Shutdown();
                            }catch(Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            Console.WriteLine("After shutdown");
                        });

                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (true);
        }
    }
}

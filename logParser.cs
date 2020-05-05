using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace chatwindow
{
    class logParser
    {
        private const string isLoginLine = "[Accounts - Login] Logged in successfully.";
        private const string usernameGetter = @" (\w+)#\d+";

        private const string matchConnected = "[UnityCrossThreadLogger]==> Event.MatchCreated";
        private const string opponenetGetter = "\"opponentScreenName\":\"(\\w+)\"";

        private const string gameEnded = "[UnityCrossThreadLogger]";
        private const string gameEnded2 = "MatchGameRoomStateType_MatchCompleted";

        private static FileStream fileStreamy;
        private static StreamReader streamy;
        public static void OpenLogFile()
        {
            //Discover the newest log file
            var directory = new DirectoryInfo(configLoader.logLocation);
            var newestLog = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First().FullName;

            fileStreamy = new FileStream(newestLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            streamy = new StreamReader(fileStreamy);
        }

        public static string GetUsername()
        {
            Regex reg = new Regex(usernameGetter);
            int count = 0;
            // read from file
            string line = "";
            do
            {
                //Console.WriteLine("After Sleep");
                //THere is maybe a another character for us to read
                //string line = streamy.ReadLine();
                line = streamy.ReadLine();
                while (line != null && line != "")
                {
                   
                        if (line.Contains(isLoginLine))
                        {
                            Match matchy = reg.Match(line);

                            Group user = matchy.Groups[1];
                            string username = user.Value;
                            return username;

                        }
                    
                    //Console.WriteLine(line + count);
                    count++;
                    line = streamy.ReadLine();
                }
                Thread.Sleep(250);
            } while (true);
        }

        public static string GetOpponent()
        {
            //Figure out what our opponents name is
            Regex reg = new Regex(opponenetGetter);
            // read from file
            string line = "";
            streamy = new StreamReader(fileStreamy);
            do
            {
                //Console.WriteLine("After Sleep");
                //THere is maybe a another character for us to read
                line = streamy.ReadLine();
                
                while (line != null && line != "") 
                {
                    //Console.WriteLine(line);
                    
                    if (line.Contains("opponentScreenName"))
                    {
                        Match matchy = reg.Match(line);

                        Group user = matchy.Groups[1];
                        string username = user.Value;

                        Console.WriteLine("Opponent is:");
                        return username;

                    }
                    line = streamy.ReadLine();
                    //Console.WriteLine(line + count);
                }
                Thread.Sleep(250);
            } while (true);
        }

        public static async Task GameEnded()
        {
            string line = "";
            do
            {
                Thread.Sleep(250);
                //Console.WriteLine("After Sleep");
                //THere is maybe a another character for us to read
                //string line = streamy.ReadLine();
                line = streamy.ReadLine();
                while (line != null && line != "")
                {

                    if (line.Contains(gameEnded2))
                    {
                        //The game has ended

                    }
                    line = streamy.ReadLine();
                    //Console.WriteLine(line + count);
                }
            } while (true);
        }
    }
}

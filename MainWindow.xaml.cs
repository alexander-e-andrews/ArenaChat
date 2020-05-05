using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chatwindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        ClientWebSocket websocks;

        System.Timers.Timer disappearTime;

        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
            ((App)Application.Current).KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);

            cts = new CancellationTokenSource();
            //Receive(websocks, listBox);
            internetManager.Receive(listBox, cts.Token);
            disappearTime = new System.Timers.Timer();
            disappearTime.Interval = configLoader.timeOut * 1000;

            disappearTime.Elapsed += new System.Timers.ElapsedEventHandler(hideListBox);

            disappearTime.AutoReset = false;
            disappearTime.Enabled = false;
        }

        void Window_Closed(object sender, EventArgs e)
        {
            
            internetManager.disconnect();
            cts.Cancel();
            Console.WriteLine("FUUUUCK");
            ((App)Application.Current).Loop();
        }

            // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
            static readonly HttpClient client = new HttpClient();

        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            //Console.WriteLine(args.Key.ToString());
            if (ApplicationIsActivated())
            {
                if (args.Key.ToString() == "Return")
                {
                    Console.WriteLine(fallback);
                    bool result = SetForegroundWindow(fallback);

                    if (result != true)
                    {
                        Console.WriteLine("Returning forground not working");
                    }
                    else
                    {
                        Console.WriteLine("Froground possible returned");
                    }

                    string message = textBox.Text;
                    if (message == "/close")
                    {
                        message = "";
                        this.Close();
                    }
                    else
                    {
                        internetManager.Send(message);
                        textBox.Text = "";
                        disappearTime.Start();
                    }
                }
                Console.WriteLine(args.Key.ToString());
            }
            else {
                if (args.Key.ToString() == "T")
                {
                    disappearTime.Stop();
                    listBox.Visibility = Visibility.Visible;
                    CenterOfAttention();
                    textBox.Focus();
                }
            }

            //Console.WriteLine(args.ToString()); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
        }


        static IntPtr fallback;
        public static void CenterOfAttention()
        {
            var activatedHandle = GetForegroundWindow();

            Console.WriteLine(activatedHandle);

            fallback = activatedHandle;       // No window is currently activated

            var proc = Process.GetCurrentProcess().MainWindowHandle;

            bool result = SetForegroundWindow(proc);

            if (result != true)
            {
                Console.WriteLine("Couldnt Set outselves as foregroundf");
            }
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWbd);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        internal static extern bool BringWindowToTop(IntPtr hWbd);
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWbd);

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void hideListBox(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { 
            Console.WriteLine("Timer Triggerd");
            try
            {
                Console.WriteLine(this.listBox);
            }catch(System.InvalidOperationException err)
            {
                Console.WriteLine(err);
            }
            //Console.WriteLine(listBox.Visibility);
            listBox.Visibility = Visibility.Hidden;
            Console.WriteLine("Tigger");
            });
        }

    }
    
}

/*
 * [3162] [UnityCrossThreadLogger]==> Event.MatchCreated {"id":-1,"payload":{"controllerFabricUri":"http://10.50.4.69:54284/match/v1/14833d33-b7f2-4efb-83b8-feb7013c95f4","matchEndpointHost":"client.arenamatch-a.east.magic-the-gathering-arena.com","matchEndpointPort":9505,"opponentScreenName":"ARGETLAM KARAZU","opponentIsWotc":false,"matchId":"14833d33-b7f2-4efb-83b8-feb7013c95f4","opponentRankingClass":"Silver","opponentRankingTier":4,"opponentMythicPercentile":0.0,"opponentMythicLeaderboardPlace":0,"eventId":"Play","opponentAvatarSelection":"Avatar_Basic_AjaniGoldmane","opponentCardBackSelection":"","opponentPetSelection":{"name":"ELD_BattlePass","variant":"Level2"},"avatarSelection":"Avatar_Basic_JayaBallard","cardbackSelection":"","petSelection":{"name":"","variant":""},"battlefield":"THB","opponentCommanderGrpIds":[],"commanderGrpIds":[],"matchType":"Queue"}}
[3170] [UnityCrossThreadLogger]4/26/2020 1:51:50 AM: Match to 9566DD393C638990: AuthenticateResponse
{ "transactionId": "03854de5-72e0-4a0e-a73e-8286d8e0e0e0", "requestId": 1, "authenticateResponse": { "clientId": "9566DD393C638990", "sessionId": "15ee0b83-ed16-49ff-9d84-8a895170d57c", "screenName": "p0wndizz7e#90729" } }
 * */

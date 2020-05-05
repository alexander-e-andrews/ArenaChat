using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace chatwindow
{
    class internetManager
    {
        static ClientWebSocket websocks;
        private const string connectionString = "ws://localhost:8080/chat?username={0}&opponent={1}";

        static internetManager()
        {
            //websocks = new ClientWebSocket();
        }

        public static async Task<bool> connectAsync(string username, string opponent)
        {
            string thisConnectionString = String.Format(connectionString,username, opponent);
 
            thisConnectionString = thisConnectionString.Replace("#", "%23");
            Uri uuri = new Uri(thisConnectionString);
            websocks = new ClientWebSocket();

            try
            {
                //UriBuilder connstring = new UriBuilder("ws", "localhost", 8080, "ws");

                await websocks.ConnectAsync(uuri, CancellationToken.None);
                Console.WriteLine("Finished Connection");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shit");
                Console.WriteLine($"ERROR - {ex.Message}");
            }
            //SShould do a single mesage to recieve here, but I am too lazy to code it

            return true;
        }
         
        public static async Task Send(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            ArraySegment<Byte> myArrSegAll = new ArraySegment<Byte>(bytes);
            await websocks.SendAsync(myArrSegAll, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task Receive(ListBox listBox, CancellationToken ct)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                //if websocks.
                WebSocketReceiveResult result = null;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        if (ct.IsCancellationRequested)
                        {
                            return;
                        }
                        try
                        {
                            result = await websocks.ReceiveAsync(buffer, ct);
                        }catch(Exception e)
                        {
                            return;
                        }
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        disconnect();
                        break;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var message = await reader.ReadToEndAsync();
                        listBox.Items.Add(message);
                        if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
                        {
                            Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                            scrollViewer.ScrollToBottom();

                        }
                    }
                }
            } while (true);
        }

        public static void disconnect()
        {
            try
            {
                websocks.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconected", CancellationToken.None);
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

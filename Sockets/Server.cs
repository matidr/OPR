using Domain;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Sockets
{
    public class Server
    {
        private static Socket serverSocket;
        private static bool serverIsOn = false;
        private static Context myContext;
        private static Operations operations;

        static void Main(string[] args)
        {
            
            StartServer();
            while (serverIsOn)
            {
                var client = serverSocket.Accept();
                Thread myThread = new Thread(() => HandleClient(client));
                myThread.Start();
            }
        }
        private static void StartServer()
        {
            myContext = new Context();
            operations = new Operations(myContext);
            // EndPoint(IP, Port)
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

            // Socket del servidor donde voy a escuchar conexiones
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Stream y TCP van de la mano para transmitir datos por TCP

            // Ahora tengo que asociar el endpoint y el socket
            serverSocket.Bind(serverIpEndPoint);
            serverSocket.Listen(100);
            serverIsOn = true;
            Console.WriteLine("Server is on");
        }
        private static void HandleClient(Socket clientSocket)
        {
            Thread myThread = new Thread(() => HandleBackgroundActivity(clientSocket));
            myThread.Start();

            Console.WriteLine("Start waiting for clients");
            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            while (serverIsOn)
            {
              
            }
            clientSocket.Close();
        }

        private static void HandleBackgroundActivity(Socket clientSocket)
        {
            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            while (serverIsOn)
            {
                var data = classLibrary.receiveData(clientSocket);
                string[] arrayData = data.Split(ClassLibrary.PROTOCOL_SEPARATOR.ToCharArray());
                string command = arrayData[0];
                string text = arrayData[1];

                switch (command)
                {
                    case ClassLibrary.LOGIN:
                        operations.login(clientSocket, classLibrary, text);
                        break;

                    case ClassLibrary.MENU_OPTION:
                        string[] menuOptionInfo = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                        string menuOption = menuOptionInfo[0];
                        string username = menuOptionInfo[1];
                        User theUser = myContext.ExistingUsers.Find(x => x.Username.Equals(username));
                        operations.MainMenu(clientSocket, classLibrary, theUser, menuOption);
                        break;

                    case ClassLibrary.CASE_4:
                        string[] case4Info = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                        string fromUsername = case4Info[0];
                        string toUsername = case4Info[1];
                        string message = case4Info[2];

                        operations.Case4(clientSocket, classLibrary, fromUsername, toUsername, message);
                        break;

                    case ClassLibrary.CLEAR_UNREAD_MESSAGES:
                        operations.ClearUnreadMessages(clientSocket, classLibrary, text);
                        break;
                }
            }
            clientSocket.Close();
        }

        
    }
}

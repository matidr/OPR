using Domain;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
using Log;



namespace Sockets
{
    public class Server
    {
        private static Socket serverSocket;
        private static bool serverIsOn = false;
        private static Context myContext;
        private static MessageLog mySystemLog;
        private static ClassLibrary classLibrary;

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
            myContext = Context.Instance;
            mySystemLog = new MessageLog();
            classLibrary = new ClassLibrary();
            // EndPoint(IP, Port)
            string serverIp = ConfigurationManager.AppSettings["ServerIpAdress"];
            int serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // Socket del servidor donde voy a escuchar conexiones
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Stream y TCP van de la mano para transmitir datos por TCP

            // Ahora tengo que asociar el endpoint y el socket
            serverSocket.Bind(serverIpEndPoint);
            serverSocket.Listen(100);
            serverIsOn = true;
            Console.WriteLine("Server is on");
            var operations = new ServerOperations(serverSocket, classLibrary, mySystemLog);
            Thread myThread = new Thread(() => operations.ServerMenu());
            myThread.Start();
        }
        private static void HandleClient(Socket clientSocket)
        {
            var operations = new ServerOperations(clientSocket, classLibrary, mySystemLog);
            Thread myThread = new Thread(() => HandleBackgroundActivity(clientSocket));

            myThread.Start();

            while (serverIsOn)
            {

            }
            clientSocket.Close();
        }

        private static void HandleBackgroundActivity(Socket clientSocket)
        {
            var operations = new ServerOperations(clientSocket, classLibrary, mySystemLog);
            while (serverIsOn)
            {
                var data = classLibrary.receiveData(clientSocket);
                if (!data.Equals(ClassLibrary.EMPTY_STRING))
                {
                    string[] arrayData = data.Split(ClassLibrary.PROTOCOL_SEPARATOR.ToCharArray());
                    string command = "";
                    string text = "";
                    if (arrayData.Length == 1)
                    {
                        command = ClassLibrary.MEDIA;
                        text = arrayData[0];
                    }
                    else
                    {
                        command = arrayData[0];
                        text = arrayData[1];
                    }
                    switch (command)
                    {
                        case ClassLibrary.LOGIN:
                            operations.login(text);
                            break;
                        case ClassLibrary.MENU_OPTION:
                            string[] menuOptionInfo = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                            string menuOption = menuOptionInfo[0];
                            string username = menuOptionInfo[1];
                            User theUser = myContext.ExistingUsers.Find(x => x.Username.Equals(username));
                            operations.MainMenu(theUser, menuOption);
                            break;
                        case ClassLibrary.SECONDARY_MENU:
                            string[] info = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                            string loggedInUsername = info[0];
                            string friendRequestUsername = info[1];
                            string accept = info[2];
                            User loggedInUser = myContext.ExistingUsers.Find(x => x.Username.Equals(loggedInUsername));
                            User userToAccept = myContext.ExistingUsers.Find(x => x.Username.Equals(friendRequestUsername));
                            operations.SecondaryMenu(loggedInUser, userToAccept, accept);
                            break;
                        case ClassLibrary.CASE_3:
                            string[] information = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                            string loggedUser = information[0];
                            string friendToAdd = information[1];
                            User uLoggedUser = myContext.ExistingUsers.Find(x => x.Username.Equals(loggedUser));
                            User uUserToAccept = myContext.ExistingUsers.Find(x => x.Username.Equals(friendToAdd));
                            operations.SendFriendRequest(uLoggedUser, uUserToAccept);
                            break;
                        case ClassLibrary.CASE_4:
                            string[] case4Info = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                            string fromUsername = case4Info[0];
                            string toUsername = case4Info[1];
                            string message = case4Info[2];
                            operations.Case4(fromUsername, toUsername, message);
                            break;
                        case ClassLibrary.CLEAR_UNREAD_MESSAGES:
                            operations.ClearUnreadMessages(text);
                            break;
                        case ClassLibrary.MEDIA:
                            classLibrary.ReadMedia(clientSocket, text);
                            operations.SaveFile(text);
                            break;

                        case ClassLibrary.DOWNLOAD_MEDIA:
                            string[] downloadMediaInfo = text.Split(ClassLibrary.LIST_SEPARATOR.ToCharArray());
                            string usernameToSend = downloadMediaInfo[0];
                            string filename = downloadMediaInfo[1];
                            operations.
                    }
                }
            }
            clientSocket.Close();
        }


    }
}

using Domain;
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
            Console.WriteLine("Start waiting for clients");
            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            while (serverIsOn)
            {
                var userID = classLibrary.receiveData(clientSocket);
                if (myContext.UserExist(userID))
                {
                    userID = operations.Login(clientSocket, classLibrary, userID);
                } else
                {
                    operations.Register(clientSocket, classLibrary, userID);
                }
                operations.MainMenu(clientSocket, classLibrary, new User(userID));
            }
            clientSocket.Close();
        }

        
    }
}

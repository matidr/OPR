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
        private static List<User> onlineUsers;
        private static Context myContext; 

        public static List<User> OnlineUsers { get => onlineUsers; set => onlineUsers = value; }

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
            OnlineUsers = new List<User>();
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
                classLibrary.sendData(clientSocket, "Conected");

                login();
                //validar que no haya uno igual ya conectado y responder
                login(clientSocket, classLibrary);
                
                if (myContext.userExist(userID))
                {
                    if(!myContext.correctPassword(userID, password))
                        Console.WriteLine("Contraseña incorrecta");

                }
                Console.WriteLine("El cliente envio:" + userID);

            }
            clientSocket.Close();
        }

        private static void login(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            string username = requestUsername(clientSocket, classLibrary);
            var password = classLibrary.receiveData(clientSocket);
            requestPassword(clientSocket, classLibrary, userID, password);
        }


        private static void requestPassword(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            var userID = classLibrary.receiveData(clientSocket);

            if (!myContext.userAlreadyConnected(userID))
            {
                classLibrary.sendData(clientSocket, "Ok, Ingrese su contraseña");
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Usuario ya conectado, pruebe con otro usuario");
                requestUsername(clientSocket, classLibrary);
            }

        }

        private static string requestUsername(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            var userID = classLibrary.receiveData(clientSocket);

            if (!myContext.userAlreadyConnected(userID))
            {
                classLibrary.sendData(clientSocket, "Ok, Ingrese su contraseña");
                return userID; 
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Usuario ya conectado, pruebe con otro usuario");
                requestUsername(clientSocket, classLibrary);
            }

        }

       



        //private static void acceptConnections() { 
        //    // Cuando llegan pedidos en paralelo, permite hasta 100 clientes al mismo tiempo(cuantos puedo guardar en la cola de futuras conexiones).
        //    serverSocket.Listen(100);

        //    Console.WriteLine("Start waiting for clients");
        //    // Espera a que se conecte un cliente, una vez que el cliente se conecta pasa a la siguiente linea
        //    serverSocket.Accept();
        //    Console.WriteLine("Client connected");
        //    Console.ReadLine();
        //}


    }
}

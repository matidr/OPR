using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Protocol;
using Domain;
using Sockets;
using System.Threading;
using System.Configuration;

namespace Cliente
{
    public class Client
    {
        private const string NULL = "NULL";
        private static Context myContext;
        private static ClassLibrary classLibrary;
        private static ClientOperations operations;
        private static bool clientIsConnected = false;
        private static Socket clientSocket;

        static void Main(string[] args)
        {
            //ALERT: cuando mandemos integer, no conviertamos en string y mandemos en string, mandemos c/ tipo de dato con el tipo que realmente es. Hay librerias q convierten c/tipo
            ConnectToServer();
        }

        private static void ConnectToServer()
        {
            myContext = new Context();
            classLibrary = new ClassLibrary();
            // endpoint del servidor al que me voy a conectar
            string serverIp = ConfigurationManager.AppSettings["ServerIpAdress"];
            int serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // endpoint del cliente (ip para la computadora local 127.0.0.1)
            string clientIp = ConfigurationManager.AppSettings["ClientIpAdress"];
            int clientPort = Convert.ToInt32(ConfigurationManager.AppSettings["ClientPort"]);
            var clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIp), clientPort);

            // socket del cliente
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Stream y TCP van de la mano para transmitir datos por TCP

            // Ahora tengo que asociar el endPoint y el socket
            try
            {
                clientSocket.Bind(clientEndPoint);

                Console.WriteLine("Connecting to server...");
                // Me conecto al endPoint del servidor
                clientSocket.Connect(serverIpEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Revisa el IP o puerto de conexión");
            }
            clientIsConnected = true;
            operations = new ClientOperations(myContext, clientSocket, classLibrary);
            Thread myThread = new Thread(() => ReceiveData(clientSocket));
            myThread.Start();


            operations.requestLogin();
            while (!ClassLibrary.LOGIN_FLAG)
            {

            }
            operations.MainMenu();

            while (clientIsConnected)
            {

            }
            clientSocket.Close();
        }

        private static void ReceiveData(Socket serverSocket)
        {
            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            while (clientIsConnected)
            {
                var data = classLibrary.receiveData(serverSocket);
                if (!data.Equals(ClassLibrary.EMPTY_STRING))
                {
                    string[] arrayData = data.Split(ClassLibrary.PROTOCOL_SEPARATOR.ToArray());
                    string command = arrayData[0];
                    string text = arrayData[1];

                    switch (command)
                    {
                        case ClassLibrary.LOGIN:
                            operations.validateLogin(text);
                            break;

                        case ClassLibrary.CASE_1:
                            operations.Case1(text);
                            break;

                        case ClassLibrary.CASE_2:
                            operations.Case2(text);
                            break;

                        case ClassLibrary.CASE_3:
                            operations.Case3(text);
                            break;

                        case ClassLibrary.CASE_4:
                            operations.Case4(text);
                            break;

                        case ClassLibrary.NEW_MESSAGE:
                            operations.NewMessage(text);
                            break;

                        case ClassLibrary.CASE_5:
                            operations.Case5(text);
                            break;

                        case ClassLibrary.SECONDARY_MENU:
                            if (text.Contains(ClassLibrary.PROTOCOL_OK_RESPONSE))
                            {
                                ClassLibrary.CASE2A_FLAG = true;
                            }
                            else if (text.Contains(NULL))
                            {
                                operations.EmptyFriendRequestList();
                                ClassLibrary.CASE2A_FLAG = true;
                                ClassLibrary.CASE5_FLAG = true; 
                            }

                            break;

                        case ClassLibrary.DISCONNECT:
                            clientIsConnected = false;
                            break;
                    }
                }
            }
        }
    }
}

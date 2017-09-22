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

namespace Cliente
{
    public class Client
    {
        private static Context myContext;
        private static Operations operations;
        private static bool clientIsConnected = false;
        static void Main(string[] args)
        {
            //ALERT: cuando mandemos integer, no conviertamos en string y mandemos en string, mandemos c/ tipo de dato con el tipo que realmente es. Hay librerias q convierten c/tipo
            
            ConnectToServer();

        }

        private static void ConnectToServer()
        {

            myContext = new Context();
            operations = new Operations(myContext);

            // endpoint del servidor al que me voy a conectar
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

            // endpoint del cliente (ip para la computadora local 127.0.0.1)
            var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

            // socket del cliente
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Stream y TCP van de la mano para transmitir datos por TCP

            // Ahora tengo que asociar el endPoint y el socket
            clientSocket.Bind(clientEndPoint);

            Console.WriteLine("Connecting to server...");
            // Me conecto al endPoint del servidor
            clientSocket.Connect(serverIpEndPoint);
            clientIsConnected = true;

            Thread myThread = new Thread(() => ReceiveData(clientSocket));
            myThread.Start();

            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            operations.requestLogin(clientSocket, classLibrary);
            while (!ClassLibrary.LOGIN_FLAG)
            {

            }
            operations.MainMenu(clientSocket, classLibrary);
            while (clientIsConnected) { 
           
            }
            clientSocket.Close();
        }

        private static void ReceiveData(Socket serverSocket)
        {
            Protocol.ClassLibrary classLibrary = new Protocol.ClassLibrary();
            while (clientIsConnected)
            {
                var data = classLibrary.receiveData(serverSocket);
                string[] arrayData = data.Split(ClassLibrary.PROTOCOL_SEPARATOR.ToArray());
                string command = arrayData[0];
                string text = arrayData[1];

                switch (command)
                {
                    case ClassLibrary.LOGIN:
                        operations.validateLogin(serverSocket, classLibrary, text);
                        break;

                    case ClassLibrary.CASE_1:
                        operations.Case1(text);
                        break;

                    case ClassLibrary.DISCONNECT:
                        clientIsConnected = false;
                        break;
                }
            }
        }
    }
}

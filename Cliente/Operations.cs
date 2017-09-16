using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Domain;

namespace Sockets
{
    public class Operations
    {
        private Context myContext;


        public Operations(Context context)
        {
            myContext = context;
        }
        public void Login(Socket clientSocket, Protocol.ClassLibrary classLibrary, string username)
        {
            classLibrary.sendData(clientSocket, username);
            username = requestUsername(clientSocket, classLibrary, username);
            Console.WriteLine("Ingrese su contrasena: ");
            var password = Console.ReadLine();
            classLibrary.sendData(clientSocket, password);
            requestPassword(clientSocket, classLibrary, username, password);
        }

        public void MainMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            Console.WriteLine("Menu Principal");
            Console.WriteLine("------------------------------");
            Console.WriteLine("1. Ver contactos conectados");
            Console.WriteLine("2. Ver solicitudes de amistad");
            Console.WriteLine("3. Enviar solicitud de amistad");
            Console.WriteLine("4. Enviar mensaje a un amigo");
            Console.WriteLine("5. Ver mensajes nuevos");
            Console.WriteLine("6. Logout");
            Console.WriteLine("------------------------------");
            Console.WriteLine("Elija una opcion y aprete enter: ");
            string menuOption = Console.ReadLine();
            classLibrary.sendData(clientSocket, menuOption);
        }


        private void requestPassword(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID, string password)
        {
            string response = classLibrary.receiveData(clientSocket);
            if (response.Contains("OK"))
            {
                Console.WriteLine("Bienvenido" + "\n");
                Console.ReadLine();
            }
            else if(response.Contains("ERROR"))
            {
                Console.WriteLine("Contrasena incorrecta. Ingresela nuevamente: ");
                password = Console.ReadLine();
                classLibrary.sendData(clientSocket, password);
                requestPassword(clientSocket, classLibrary, userID, password);
            }

        }

        private string requestUsername(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            string response = classLibrary.receiveData(clientSocket);
            if (response.Contains("OK"))
            {
                Console.WriteLine("Ingrese su contrasena: ");
                return userID;
            }
            else if (response.Contains("ERROR"))
            {
                Console.WriteLine("Ha habido un error con su username. Ingreselo nuevamente: ");
                userID = Console.ReadLine();
                classLibrary.sendData(clientSocket, userID);
                return requestUsername(clientSocket, classLibrary, userID);
            }
            return "";
        }
    }
}

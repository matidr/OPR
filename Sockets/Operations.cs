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
        public void Login(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            string username = requestUsername(clientSocket, classLibrary, userID);
            var password = classLibrary.receiveData(clientSocket);
            requestPassword(clientSocket, classLibrary, username, password);
        }

        public void Register(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            classLibrary.sendData(clientSocket, "OK. Ingrese su contrasena");
            string password = classLibrary.receiveData(clientSocket);
            User user = new User(userID, password);
            myContext.AddNewUser(user);
            myContext.ConnectUser(user);
            classLibrary.sendData(clientSocket, "OK. Bienvenido");
        }

        public void MainMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            var menuOption = classLibrary.receiveData(clientSocket);
            switch (menuOption)
            {
                case "1":

                    break;
                case "2":

                    break;
                case "3":

                    break;
                case "4":

                    break;
                case "5":

                    break;
                case "6":

                    break;
                default:
                    classLibrary.sendData(clientSocket, "ERROR. Opcion incorrecta");
                    MainMenu(clientSocket, classLibrary);
                    break;
            }
        }


        private void requestPassword(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID, string password)
        {

            if (!myContext.CorrectPassword(userID, password))
            {
                myContext.ConnectUser(new User(userID, password));
                classLibrary.sendData(clientSocket, "OK. Bienvenido");
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Contrasena incorrecta");
                password = classLibrary.receiveData(clientSocket);
                requestPassword(clientSocket, classLibrary, userID, password);
            }

        }

        private string requestUsername(Socket clientSocket, Protocol.ClassLibrary classLibrary, string userID)
        {
            if (!myContext.UserAlreadyConnected(userID))
            {
                classLibrary.sendData(clientSocket, "Ok, Ingrese su contraseña");
                return userID;
            }
            else
            {
                classLibrary.sendData(clientSocket, "ERROR: Usuario ya conectado, pruebe con otro usuario");
                userID = classLibrary.receiveData(clientSocket);
                return requestUsername(clientSocket, classLibrary, userID);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Domain;
using Protocol;

namespace Sockets
{
    public class Operations
    {
        private Context myContext;
        private List<User> connectedFriends;
        private User currentUser;
        

        public Operations(Context context)
        {
            myContext = context;
            connectedFriends = new List<User>();
        }

        public void MainMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            Console.WriteLine("");
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
            classLibrary.sendData(clientSocket, ClassLibrary.MENU_OPTION + ClassLibrary.PROTOCOL_SEPARATOR + menuOption + ClassLibrary.LIST_SEPARATOR + currentUser.Username);
            switch (menuOption)
            {
                case "1":
                    int i = 1;
                    Console.WriteLine("Your connected friends are: ");
                    if(connectedFriends.Count!=0)
                    {
                        foreach (User u in connectedFriends)
                        {
                            Console.WriteLine(i + ") " + u.Username);
                            i++;
                        }
                        Console.WriteLine("------------------------------" + "\n");
                    } else
                    {
                        Console.WriteLine("No tienes amigos conectados en este momento");
                    }
                    
                    MainMenu(clientSocket, classLibrary);
                    break;

                case "2":
                    string requests = classLibrary.receiveData(clientSocket);
                    while (!requests.Equals("FINISH"))
                    {
                        Console.WriteLine(requests);
                        requests = classLibrary.receiveData(clientSocket);
                    }
                    Console.WriteLine("------------------------------" + "\n");
                    MainMenu(clientSocket, classLibrary);
                    break;

                case "3":
                    break;

                case "4":
                    break;

                case "5":
                    string messages = classLibrary.receiveData(clientSocket);
                    while (!messages.Equals("FINISH"))
                    {
                        Console.WriteLine(messages);
                        requests = classLibrary.receiveData(clientSocket);
                    }
                    Console.WriteLine("------------------------------" + "\n");
                    MainMenu(clientSocket, classLibrary);
                    break;

                default:
                    Console.WriteLine("Por favor seleccione una opcion del menú" + "\n");
                    MainMenu(clientSocket, classLibrary);
                    break;
            }

        }

        public void validateLogin(Socket clientSocket, Protocol.ClassLibrary classLibrary, string response)
        {
            if (response.Contains("OK"))
            {
                Console.WriteLine("BIENVENIDO " + currentUser.Username);
            }
            else if (response.Contains("ERROR"))
            {
                Console.WriteLine(response);
                Console.WriteLine("Vuelva a ingresar los datos: ");
                requestLogin(clientSocket, classLibrary);
            }
        }

        public void requestLogin(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            currentUser = new User(username);
            Console.WriteLine("Enter password");
            var password = Console.ReadLine();
            string loginInfo = username + ClassLibrary.LIST_SEPARATOR + password;
            classLibrary.sendData(clientSocket, ClassLibrary.LOGIN + ClassLibrary.PROTOCOL_SEPARATOR + loginInfo);
        }

        public User getCurrentUser()
        {
            return currentUser;
        }

        public bool isUserLoggedIn()
        {
            return currentUser != null;
        }


        public void Case1(string text)
        {
            lock (connectedFriends)
            {
                string[] conFriendsArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                for (int i=0; i<conFriendsArray.Length - 1; i++)
                {
                    User user = new User(conFriendsArray[i]);
                    if (!connectedFriends.Contains(user)) { 
                    connectedFriends.Add(user);
                    }
                }
            }
        }
    }
}

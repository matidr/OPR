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
    public class ClientOperations
    {
        private Context myContext;
        private List<User> connectedFriends;
        private List<User> friendRequest;
        private List<Message> newMessages;
        private User currentUser;


        public ClientOperations(Context context)
        {
            myContext = context;
            connectedFriends = new List<User>();
            friendRequest = new List<User>();
            newMessages = new List<Message>();
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
                    while (!ClassLibrary.CASE1_FLAG) { }
                    int i = 1;
                    Console.WriteLine("Tus amigos conectados son: ");
                    if (connectedFriends.Count != 0)
                    {
                        foreach (User u in connectedFriends)
                        {
                            Console.WriteLine(i + ") " + u.Username);
                            i++;
                        }
                        Console.WriteLine("------------------------------" + "\n");
                    }
                    else
                    {
                        Console.WriteLine("No tienes amigos conectados en este momento");
                    }
                    ClassLibrary.CASE1_FLAG = false;
                    Console.WriteLine("------------------------------" + "\n");
                    MainMenu(clientSocket, classLibrary);
                    break;

                case "2":
                    while (!ClassLibrary.CASE2_FLAG) { }
                    ClassLibrary.CASE2_FLAG = false;
                    if (friendRequest.Count > 0)
                    {
                        int j = 1;
                        Console.WriteLine("Tus solicitudes de amistad pendientes son: ");
                        foreach (User u in friendRequest)
                        {
                            Console.WriteLine(j + ") " + u.Username);
                            j++;
                        }
                        Console.WriteLine("------------------------------" + "\n");
                        FriendRequestSubMenu(clientSocket, classLibrary);
                    }
                    else
                    {
                        Console.WriteLine("No tienes solicitudes de amistad pendientes");
                    }
                    Console.WriteLine("------------------------------" + "\n");
                    while (!ClassLibrary.CASE2A_FLAG) { }
                    MainMenu(clientSocket, classLibrary);
                    break;

                case "3":
                    break;

                case "4":
                    Console.WriteLine("Ingrese el nombre de usuario al que quiere mandarle msj: ");
                    string toUsername = Console.ReadLine();
                    Console.WriteLine("Ingrese el mensaje a enviar: ");
                    string message = Console.ReadLine();
                    classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + currentUser.Username + ClassLibrary.LIST_SEPARATOR + toUsername + ClassLibrary.LIST_SEPARATOR + message);
                    break;

                case "5":
                    while (!ClassLibrary.CASE5_FLAG) { }
                    int k = 1;
                    Console.WriteLine("Mensajes sin leer: ");
                    if (newMessages.Count != 0)
                    {
                        foreach (Message m in newMessages)
                        {
                            Console.WriteLine(k + ") " + m.Display);
                            k++;
                        }
                        Console.WriteLine("------------------------------" + "\n");
                    }
                    else
                    {
                        Console.WriteLine("No tienes mensajes sin leer");
                    }
                    Console.WriteLine("------------------------------" + "\n");
                    ClassLibrary.CASE5_FLAG = false;
                    MainMenu(clientSocket, classLibrary);
                    break;

                case "6":

                    break;

                default:
                    Console.WriteLine("Por favor seleccione una opcion del menú" + "\n");
                    MainMenu(clientSocket, classLibrary);
                    break;
            }

        }

        public void FriendRequestSubMenu(Socket clientSocket, Protocol.ClassLibrary classLibrary)
        {
            Console.WriteLine("Digite el nombre de usuario de la solicitud de amistad que desea gestionar");
            Console.WriteLine("o la opción 2 para volver al menú principal");
            Console.WriteLine("------------------------------");
            string friendRequestUsername = Console.ReadLine();
            if (friendRequest.Contains(new User(friendRequestUsername)))
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine("Para aceptar digite 1 y para rechazar 0 y para volver al menú principal 2");
                Console.WriteLine("------------------------------");
                string accept = Console.ReadLine();
                switch (accept)
                {
                    case "1": case "0":
                        string returnData = "";
                        returnData = currentUser.Username + ClassLibrary.LIST_SEPARATOR + friendRequestUsername + ClassLibrary.LIST_SEPARATOR + accept;
                        classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
                        break;
                    case "2":
                        break;
                    default:
                        Console.WriteLine("Opción invalida, por favor intente nuevamente");
                        Console.WriteLine("------------------------------");
                        FriendRequestSubMenu(clientSocket, classLibrary);
                        break;
                }
            }
            else
            {
                if (!friendRequestUsername.Equals("2"))
                {
                    Console.WriteLine("Usuario inválido, por favor intente nuevamente");
                    Console.WriteLine("------------------------------");
                    FriendRequestSubMenu(clientSocket, classLibrary);
                }
                else
                {
                    ClassLibrary.CASE2A_FLAG = true;
                }

            }
        }

        public void validateLogin(Socket clientSocket, Protocol.ClassLibrary classLibrary, string response)
        {
            if (response.Contains("OK"))
            {
                Console.WriteLine("BIENVENIDO " + currentUser.Username);
                ClassLibrary.LOGIN_FLAG = true;
            }
            else if (response.Contains("ERROR"))
            {
                Console.WriteLine(response);
                Console.WriteLine("Vuelva a ingresar los datos: ");
                requestLogin(clientSocket, classLibrary);
            }
            else if (response.Contains("MSJS"))
            {
                Console.WriteLine("Bienvenido. Tiene los siguientes mensajes sin leer: ");
                string[] unreadMessagesArray = response.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                for (int i = 1; i < unreadMessagesArray.Length - 1; i++)
                {
                    Console.WriteLine(unreadMessagesArray[i]);
                }
                classLibrary.sendData(clientSocket, ClassLibrary.CLEAR_UNREAD_MESSAGES + ClassLibrary.PROTOCOL_SEPARATOR + currentUser.Username);
                ClassLibrary.LOGIN_FLAG = true;
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
                for (int i = 0; i < conFriendsArray.Length - 1; i++)
                {
                    User user = new User(conFriendsArray[i]);
                    if (!connectedFriends.Contains(user))
                    {
                        connectedFriends.Add(user);
                    }
                }
                ClassLibrary.CASE1_FLAG = true;
            }
        }

        public void Case2(string text)
        {
            if (!text.Equals(""))
            {
                lock (friendRequest)
                {
                    friendRequest.Clear();
                    string[] conFriendsArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                    for (int i = 0; i < conFriendsArray.Length - 1; i++)
                    {
                        User user = new User(conFriendsArray[i]);
                        if (!friendRequest.Contains(user))
                        {
                            friendRequest.Add(user);
                        }
                    }
                }
                ClassLibrary.CASE2_FLAG = true;
            }
        }

        public void EmptyList()
        {
            friendRequest.Clear();
            ClassLibrary.CASE2_FLAG = true;
        }

        public void Case4(string text)
        {
            if (text.Contains("OK"))
            {
                Console.WriteLine("Mensaje enviado");
            }
        }

        public void NewMessage(string text)
        {
            string[] conMessagesArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
            string fromUser = conMessagesArray[0];
            string message = conMessagesArray[1];
            Console.WriteLine("Nuevo mensaje de " + fromUser + ": ");
            Console.WriteLine(message);
        }

        public void Case5(string text)
        {
            lock (newMessages)
            {
                string[] conMessagesArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                for (int i = 0; i < conMessagesArray.Length - 1; i++)
                {
                    Message msg = new Message(conMessagesArray[i]);
                    if (!newMessages.Contains(msg))
                    {
                        newMessages.Add(msg);
                    }
                }
                ClassLibrary.CASE5_FLAG = true;
            }
        }
    }
}

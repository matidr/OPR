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
        private const string CASE_0 = "0";
        private const string CASE_1 = "1";
        private const string CASE_2 = "2";
        private const string CASE_3 = "3";
        private const string CASE_4 = "4";
        private const string CASE_5 = "5";
        private const string CASE_6 = "6";
        private const string EMPTY_STRING = "";

        private Context myContext;
        private List<User> connectedFriends;
        private List<User> friendRequest;
        private List<ChatMessage> newMessages;
        private User currentUser;
        private Socket clientSocket;
        private ClassLibrary classLibrary;


        public ClientOperations(Context context, Socket socket, ClassLibrary classLibrary)
        {
            myContext = context;
            clientSocket = socket;
            this.classLibrary = classLibrary;
            connectedFriends = new List<User>();
            friendRequest = new List<User>();
            newMessages = new List<ChatMessage>();
        }

        public void MainMenu()
        {
            Console.WriteLine(EMPTY_STRING);
            Console.WriteLine("Menu Principal");
            Console.WriteLine("------------------------------");
            Console.WriteLine("1. Ver contactos conectados");
            Console.WriteLine("2. Ver solicitudes de amistad");
            Console.WriteLine("3. Enviar solicitud de amistad");
            Console.WriteLine("4. Enviar mensaje a un amigo");
            Console.WriteLine("5. Enviar archivo a un amigo");
            Console.WriteLine("6. Logout");
            Console.WriteLine("------------------------------");
            Console.WriteLine("Elija una opcion y aprete enter: ");
            string menuOption = Console.ReadLine();
            classLibrary.sendData(clientSocket, ClassLibrary.MENU_OPTION + ClassLibrary.PROTOCOL_SEPARATOR + menuOption + ClassLibrary.LIST_SEPARATOR + currentUser.Username);
            switch (menuOption)
            {
                case CASE_1:
                    while (!ClassLibrary.CASE1_FLAG) { }
                    ClassLibrary.CASE1_FLAG = false;
                    if (connectedFriends.Count > 0)
                    {
                        int i = 1;
                        Console.WriteLine("Tus amigos conectados son: ");
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
                    Console.WriteLine("------------------------------" + "\n");
                    MainMenu();
                    break;

                case CASE_2:
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
                        FriendRequestSubMenu();
                        while (!ClassLibrary.CASE2A_FLAG) { }
                        MainMenu();
                    }
                    else
                    {
                        Console.WriteLine("No tienes solicitudes de amistad pendientes");
                        MainMenu();
                    }
                    Console.WriteLine("------------------------------" + "\n");

                    break;

                case CASE_3:
                    ClassLibrary.CASE3_FLAG = false;
                    Console.WriteLine("Digite el nombre de usuario que desea agregar");
                    string friendRequestUsername = Console.ReadLine();
                    if (friendRequest.Contains(new User(friendRequestUsername)))
                    {
                        Console.WriteLine("Ya has enviado anteriormente esa solicitud de amistad");
                    }
                    else
                    {
                        string returnData = EMPTY_STRING;
                        returnData = currentUser.Username + ClassLibrary.LIST_SEPARATOR + friendRequestUsername;
                        classLibrary.sendData(clientSocket, ClassLibrary.CASE_3 + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
                    }

                    Console.WriteLine("------------------------------" + "\n");
                    while (!ClassLibrary.CASE3_FLAG) { }
                    MainMenu();
                    break;

                case CASE_4:
                    Console.WriteLine("Ingrese el nombre de usuario al que quiere mandarle msj: ");
                    string toUsername = Console.ReadLine();
                    Console.WriteLine("Ingrese el mensaje a enviar: ");
                    string message = Console.ReadLine();
                    classLibrary.sendData(clientSocket, ClassLibrary.CASE_4 + ClassLibrary.PROTOCOL_SEPARATOR + currentUser.Username + ClassLibrary.LIST_SEPARATOR + toUsername + ClassLibrary.LIST_SEPARATOR + message);
                    MainMenu();
                    break;

                case CASE_5:
                    Console.WriteLine("Ingrese el nombre del archivo a enviar: ");
                    string fileName = Console.ReadLine();
                    classLibrary.SendMedia(clientSocket, ClassLibrary.MEDIA + ClassLibrary.PROTOCOL_SEPARATOR + fileName);
                    MainMenu();
                    break;
                case CASE_6:

                    break;
                case "":

                    break; 

                default:
                    Console.WriteLine("Por favor seleccione una opcion del menú" + "\n");
                    MainMenu();
                    break;
            }

        }

        public void FriendRequestSubMenu()
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
                    case CASE_1:
                    case CASE_0:
                        string returnData = EMPTY_STRING;
                        returnData = currentUser.Username + ClassLibrary.LIST_SEPARATOR + friendRequestUsername + ClassLibrary.LIST_SEPARATOR + accept;
                        classLibrary.sendData(clientSocket, ClassLibrary.SECONDARY_MENU + ClassLibrary.PROTOCOL_SEPARATOR + returnData);
                        break;
                    case CASE_2:
                        ClassLibrary.CASE2_FLAG = true;
                        break;
                    default:
                        Console.WriteLine("Opción invalida, por favor intente nuevamente");
                        Console.WriteLine("------------------------------");
                        FriendRequestSubMenu();
                        break;
                }
            }
            else
            {
                if (!friendRequestUsername.Equals(CASE_2))
                {
                    Console.WriteLine("Usuario inválido, por favor intente nuevamente");
                    Console.WriteLine("------------------------------");
                    FriendRequestSubMenu();
                }
                else
                {
                    ClassLibrary.CASE2A_FLAG = true;
                }

            }
        }

        public void validateLogin(string response)
        {
            if (response.Contains(ClassLibrary.PROTOCOL_OK_RESPONSE))
            {
                Console.WriteLine("BIENVENIDO " + currentUser.Username);
                ClassLibrary.LOGIN_FLAG = true;
            }
            else if (response.Contains(ClassLibrary.PROTOCOL_ERROR_RESPONSE))
            {
                Console.WriteLine(response);
                Console.WriteLine("Vuelva a ingresar los datos: ");
                requestLogin();
            }
            else if (response.Contains(ClassLibrary.PROTOCOL_OK_MSJS_RESPONSE))
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

        public void requestLogin()
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
            if (!text.Equals(EMPTY_STRING))
            {
                lock (connectedFriends)
                {
                    connectedFriends.Clear();
                    string[] conFriendsArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                    for (int i = 0; i < conFriendsArray.Length - 1; i++)
                    {
                        User user = new User(conFriendsArray[i]);
                        if (!connectedFriends.Contains(user))
                        {
                            connectedFriends.Add(user);
                        }
                    }
                }
            }
            ClassLibrary.CASE1_FLAG = true;
        }

        public void Case2(string text)
        {
            if (!text.Equals(EMPTY_STRING))
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

        public void Case3(string text)
        {
            if (text.Contains(ClassLibrary.PROTOCOL_OK_RESPONSE))
            {
                Console.WriteLine("Usuario agregado");
            } else if(text.Contains(ClassLibrary.PROTOCOL_ERROR_RESPONSE))
            {
                Console.WriteLine("No se pudo enviar la solicitud porque el usuario a enviar no existe");
            }
            ClassLibrary.CASE3_FLAG = true;
        }

        public void EmptyFriendRequestList()
        {
            friendRequest.Clear();
            ClassLibrary.CASE2_FLAG = true;
        }

        public void Case4(string text)
        {
            if (text.Contains(ClassLibrary.PROTOCOL_OK_RESPONSE))
            {
                Console.WriteLine("Mensaje enviado");
            }
            else if (text.Contains(ClassLibrary.PROTOCOL_ERROR_RESPONSE))
            {
                Console.WriteLine("Error: debe agregar esta persona como amiga para poder enviarle mensajes");
            }
        }

        public void NewMessage(string text)
        {
            string[] conMessagesArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
            string fromUser = conMessagesArray[0];
            string message = conMessagesArray[1];
            Console.WriteLine("------------------------------" + "\n");
            Console.WriteLine("Nuevo mensaje de " + fromUser + ": ");
            Console.WriteLine(message);
            Console.WriteLine("------------------------------" + "\n");
        }

        public void Case5(string text)
        {
            lock (newMessages)
            {
                string[] conMessagesArray = text.Split(ClassLibrary.LIST_SEPARATOR.ToArray());
                for (int i = 0; i < conMessagesArray.Length - 1; i++)
                {
                    ChatMessage msg = new ChatMessage(conMessagesArray[i]);
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

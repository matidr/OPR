using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFUserClient
{
    class WCFClientOperations
    {
        private const string CASE_1 = "1";
        private const string CASE_2 = "2";
        private const string CASE_3 = "3";
        private const string CASE_4 = "4";
        private UserReference.UserClient userClient;

        public WCFClientOperations()
        {
            userClient = new UserReference.UserClient();
        }
        public void MainMenu()
        {
            Console.WriteLine("1) Alta de usuario al sistema");
            Console.WriteLine("2) Baja de usuario al sistema");
            Console.WriteLine("3) Modificacion de usuario del sistema");
            Console.WriteLine("4) Consulta de todos los usuarios del sistema");
            Console.WriteLine("Ingrese una opcion:");
            string option = Console.ReadLine();
            switch (option)
            {
                case CASE_1:
                    while (!SaveUser())
                    {
                        SaveUser();
                    }
                    MainMenu();
                    break;

                case CASE_2:
                    while (!DeleteUser())
                    {
                        DeleteUser();
                    }
                    MainMenu();
                    break;

                case CASE_3:
                    while (!ModifyUser())
                    {
                        ModifyUser();
                    }
                    MainMenu();
                    break;

                case CASE_4:
                    GetUsers();
                    MainMenu();
                    break;

                default:
                    Console.WriteLine("Opcion incorrecta. Intentelo de nuevo.");
                    MainMenu();
                    break;
            }
        }

        private bool SaveUser()
        {
            Console.WriteLine("Ingrese nombre de usuario");
            string username = Console.ReadLine();
            Console.WriteLine("Ingrese la contrasena");
            string password = Console.ReadLine();

            UserReference.User user = new UserReference.User();
            user.Username = username;
            user.Password = password;
            bool isSuccesful = userClient.SaveUser(user);
            if (isSuccesful)
            {
                Console.WriteLine("Usuario ingresado");
            }
            else
            {
                Console.WriteLine("Ha ocurrido un error. Intentelo nuevamente");
            }
            return isSuccesful;
        }

        private bool ModifyUser()
        {
            Console.WriteLine("Ingrese el nombre de usuario que desea modificar");
            string username = Console.ReadLine();
            Console.WriteLine("Ingrese la nueva contrasena");
            string password = Console.ReadLine();

            UserReference.User user = new UserReference.User();
            user.Username = username;
            user.Password = password;
            bool isSuccesful = userClient.ModifyUser(user);
            if (isSuccesful)
            {
                Console.WriteLine("Usuario modificado");
            }
            else
            {
                Console.WriteLine("El usuario ingresado no existe. Intentelo nuevamente");
            }
            return isSuccesful;
        }

        private bool DeleteUser()
        {
            Console.WriteLine("Ingrese el nombre de usuario que desea eliminar");
            string username = Console.ReadLine();

            UserReference.User user = new UserReference.User();
            user.Username = username;

            bool isSuccesful = userClient.DeleteUser(user);
            if (isSuccesful)
            {
                Console.WriteLine("Usuario eliminado");
            }
            else
            {
                Console.WriteLine("El usuario ingresado no existe. Intentelo nuevamente");
            }
            return isSuccesful;
        }

        private void GetUsers()
        {
            UserReference.User[] listUsers = userClient.GetUsers();
            if (listUsers.Count() > 0)
            {
                foreach (UserReference.User user in listUsers)
                {
                    Console.WriteLine(user.Username);
                }
            }
            else
            {
                Console.WriteLine("No hay usuarios en el sistema");
            }
        }
    }
}

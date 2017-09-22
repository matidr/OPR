using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Protocol
{
    public class ClassLibrary
    {
        private static int FIXED_SIZE = 4;

        //CONSTS
        public const string PROTOCOL_SEPARATOR = "&";
        public const string LIST_SEPARATOR = ";";
        public const string END_OF_MESSAGE = "FINISH";
        public const string CASE_1 = "CASE 1 ";
        public const string LOGIN = "LOGIN";
        public const string MENU_OPTION = "MENUOPTION";
        public const string DISCONNECT = "DISCONNECT";

        //FLAGS
        public static bool LOGIN_FLAG = false;
        public static bool CASE1_FLAG = false; 

        public int getFixedSize()
        {
            return FIXED_SIZE;
        }

        public void sendData(Socket clientSocket, String textToSend)
        {
            var data = System.Text.Encoding.ASCII.GetBytes(textToSend);
            var length = data.Length;
            var dataLength = BitConverter.GetBytes(length);

            //aca mando el largo
            int sent = 0;
            while (sent < FIXED_SIZE)
            {
                sent += clientSocket.Send(dataLength, sent, FIXED_SIZE - sent, SocketFlags.None);
            }
            //aca manda la data
            sent = 0;
            while (sent < data.Length)
            {
                sent += clientSocket.Send(data, sent, data.Length - sent, SocketFlags.None);
            }
        }

        public string receiveData(Socket clientSocket)
        {
            var dataReceived = new byte[FIXED_SIZE];
            int received = 0;
            while (received < FIXED_SIZE)
            {
                received += clientSocket.Receive(dataReceived, received, FIXED_SIZE - received, SocketFlags.None);
            }

            var dataReceived2 = new byte[BitConverter.ToInt32(dataReceived, 0)];

            received = 0;
            while (received < dataReceived2.Length)
            {
                received += clientSocket.Receive(dataReceived2, received, dataReceived2.Length - received, SocketFlags.None);
            }
            return System.Text.Encoding.ASCII.GetString(dataReceived2);
        }
    }
}

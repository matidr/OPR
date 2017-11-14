using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace Protocol
{
    public class ClassLibrary
    {
        private static int FIXED_SIZE = 4;

        //CONSTS
        public const string PROTOCOL_SEPARATOR = "&";
        public const string PROTOCOL_OK_RESPONSE = "OK";
        public const string PROTOCOL_ERROR_RESPONSE = "ERROR";
        public const string PROTOCOL_OK_MSJS_RESPONSE = "MSJS";
        public const string LIST_SEPARATOR = ";";
        public const string END_OF_MESSAGE = "FINISH";
        public const string CASE_1 = "CASE 1 ";
        public const string CASE_2 = "CASE 2 ";
        public const string CASE_2A = "CASE 2A ";
        public const string CASE_3 = "CASE 3 ";
        public const string CASE_4 = "CASE 4 ";
        public const string CASE_5 = "CASE 5 ";
        public const string NEW_MESSAGE = "New Message";
        public const string CLEAR_UNREAD_MESSAGES = "ClearUnreadMessages";
        public const string LOGIN = "LOGIN";
        public const string MENU_OPTION = "MENUOPTION";
        public const string DISCONNECT = "DISCONNECT";
        public const string SECONDARY_MENU = "SECONDARY MENU";
        public const string EMPTY_STRING = "";
        public const string MEDIA = "MEDIA";
        public const string DOWNLOAD_MEDIA = "DOWNLOAD_MEDIA";
        public const string REQUEST_MEDIA = "REQUEST_MEDIA";

        //FLAGS
        public static bool LOGIN_FLAG = false;
        public static bool CASE1_FLAG = false;
        public static bool CASE2_FLAG = false;
        public static bool CASE2A_FLAG = false;
        public static bool CASE3_FLAG = false;
        public static bool CASE5_FLAG = false;
        public static bool REQUEST_DOWNLOAD_FLAG = false;

        public int getFixedSize()
        {
            return FIXED_SIZE;
        }

        public void sendData(Socket clientSocket, String textToSend)
        {
            try
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
            catch (SocketException e)
            {
                Console.WriteLine("Error de conexion");
            }
        }

        public string receiveData(Socket clientSocket)
        {
            try
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
            catch (SocketException e)
            {
                Console.WriteLine("Error de conexion");
                return EMPTY_STRING;
            }
        }

        public void SendMedia(Socket clientSocket, string name)
        {
            sendData(clientSocket, ClassLibrary.MEDIA + ClassLibrary.PROTOCOL_SEPARATOR + name);

            string path = "C:\\ejemplo\\" + name;
            var fileInfo = new FileInfo(path);
            int fileLength = (int)fileInfo.Length;
            // ENVIO EL FILE LENGTH
            sendData(clientSocket, "" + fileLength);

            int numberOfBlocks = fileLength / 1024;
            int rest = fileLength % 1024;
            byte[] byteArray = new byte[1024];
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            int sent = 0;
            var length = byteArray.Length;
            var dataLength = BitConverter.GetBytes(length);
            for (int i = 0; i < numberOfBlocks; i++)
            {
                sent = 0;
                fileStream.Seek(1024 * i, SeekOrigin.Begin);
                int read2 = 0;
                while (read2 < 1024)
                {
                    read2 += fileStream.Read(byteArray, 0, 1024);
                }
                length = byteArray.Length;
                dataLength = BitConverter.GetBytes(length);

                //aca mando el largo
                sent = 0;
                while (sent < FIXED_SIZE)
                {
                    sent += clientSocket.Send(dataLength, sent, FIXED_SIZE - sent, SocketFlags.None);
                }
                //aca manda la data
                sent = 0;
                while (sent < byteArray.Length)
                {
                    sent += clientSocket.Send(byteArray, sent, byteArray.Length - sent, SocketFlags.None);
                }
            }

            sent = 0;
            fileStream.Seek(fileLength - rest, SeekOrigin.Begin);
            int read = 0;
            while (read < rest)
            {
                read += fileStream.Read(byteArray, 0, rest);
            }
            length = byteArray.Length;
            dataLength = BitConverter.GetBytes(length);

            //aca mando el largo
            sent = 0;
            while (sent < FIXED_SIZE)
            {
                sent += clientSocket.Send(dataLength, sent, FIXED_SIZE - sent, SocketFlags.None);
            }
            //aca manda la data
            sent = 0;
            while (sent < byteArray.Length)
            {
                sent += clientSocket.Send(byteArray, sent, byteArray.Length - sent, SocketFlags.None);
            }
            fileStream.Close();
        }

        public void ReadMedia(Socket clientSocket, string name)
        {

            string path = "C:\\ejemplo\\" + name;
            int fileLength = Int32.Parse(receiveData(clientSocket));

            int numberOfBlocks = fileLength / 1024;
            int rest = fileLength % 1024;
            byte[] byteArray = new byte[1024];
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            int received = 0;
            var length = byteArray.Length;
            var dataLength = BitConverter.GetBytes(length);
            for (int i = 0; i < numberOfBlocks; i++)
            {
                received = 0;
                fileStream.Seek(1024 * i, SeekOrigin.Begin);
                length = byteArray.Length;
                dataLength = BitConverter.GetBytes(length);

                //aca mando el largo
                received = 0;
                while (received < FIXED_SIZE)
                {
                    received += clientSocket.Receive(dataLength, received, FIXED_SIZE - received, SocketFlags.None);
                }
                //aca manda la data
                received = 0;
                while (received < byteArray.Length)
                {
                    received += clientSocket.Receive(byteArray, received, byteArray.Length - received, SocketFlags.None);
                }

                fileStream.Write(byteArray, 0, 1024);
            }

            received = 0;
            fileStream.Seek(fileLength - rest, SeekOrigin.Begin);
            length = byteArray.Length;
            dataLength = BitConverter.GetBytes(length);

            //aca mando el largo
            received = 0;
            while (received < FIXED_SIZE)
            {
                received += clientSocket.Receive(dataLength, received, FIXED_SIZE - received, SocketFlags.None);
            }
            //aca manda la data
            received = 0;
            while (received < byteArray.Length)
            {
                received += clientSocket.Receive(byteArray, received, byteArray.Length - received, SocketFlags.None);
            }

            fileStream.Write(byteArray, 0, rest);

            fileStream.Close();
        }


    }
}

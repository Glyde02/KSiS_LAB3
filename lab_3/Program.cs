using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace lab_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 1 - server, 2 - client");

            int type = Int32.Parse(Console.ReadLine());

            if (type == 1)
            {
                Server server = new Server();
                server.InitConnection();

            }
            else if (type == 2)
            {
                Client client = new Client();
                client.InitConnection();
            }
            else
            {
                Console.WriteLine("Error!");
            }

        }
    }


    abstract class TCP_Connection
    {
        abstract public void SendMessage(string outputMessage);
        abstract public void GetMessages();
    }

    class Client : TCP_Connection
    {
        static string ipAdress = "127.0.0.1";
        static int port = 79;
        public Socket socket;



        override public void GetMessages()
        {

                StringBuilder inputMessage = new StringBuilder();
                int bytesRead = 0;
                byte[] inputData = new byte[1000];

                bytesRead = socket.Receive(inputData);
                string text = Encoding.UTF8.GetString(inputData, 0, bytesRead);
                inputMessage.Append(text);

            Console.WriteLine("\nAnswer:");
                Console.WriteLine(inputMessage.ToString());
            Console.WriteLine();


        }

        override public void SendMessage(string outputMessage)
        {

            byte[] outputData = Encoding.UTF8.GetBytes(outputMessage);
            socket.Send(outputData);
        }

        public void InitConnection()
        {
            Console.WriteLine("Input ip adress");
            ipAdress = Console.ReadLine();
            Console.WriteLine("\n");

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);

            Console.WriteLine("Connection open\n");
            Console.WriteLine("Input request");
            string request = Console.ReadLine();

            SendMessage(request);

            GetMessages();
            CloseConnection();
            Console.ReadLine();
            //Thread clientThread = new Thread(new ThreadStart(GetMessages));
            //this.clientThread = new Thread(new ThreadStart(GetMessages));
            //this.clientThread.Start();
        }

        public void CloseConnection()
        {
            socket.Close();
            Console.WriteLine("Close connection");
        }
    }

    class Server
    {
        static string ipAdress = "127.0.0.1";
        static int port = 79;
        private Socket socket;
        public Socket clientSocket;

        

        public void GetMessages(Socket clientSocket)
        {

                StringBuilder inputMessage = new StringBuilder();
                int bytesRead = 0;
                byte[] inputData = new byte[1000];


                bytesRead = clientSocket.Receive(inputData);
                string text = Encoding.UTF8.GetString(inputData, 0, bytesRead);
                inputMessage.Append(text);


            Console.WriteLine("Request:");
            Console.WriteLine(inputMessage.ToString());
            Console.WriteLine();
            
            
            string answer = GetAnswer(inputMessage.ToString());
            //Console.WriteLine(answer);
            SendMessage(answer);
            //CloseThread();

            
        }

        private string GetAnswer(string input)
        {
            string names = "";

            SelectQuery query = new SelectQuery("Win32_UserAccount");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);


            if (input == "finger")
            {
                foreach (ManagementObject wmiObj in searcher.Get())
                {
                    //names += "AccountType: " + (string)wmiObj.Properties["AccountType"].Value.ToString() + "\n";
                    names += "Caption: " + (string)wmiObj.Properties["Caption"].Value + "\n";
                    //names += "Disabled: " + (string)wmiObj.Properties["Disabled"].Value.ToString() + "\n";
                    names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n";
                    names += "Full Name: " + (string)wmiObj.Properties["FullName"].Value + "\n";
                    //names += "InstallDate: " + (string)wmiObj.Properties["InstallDate"].Value + "\n";
                    //names += "LocalAccount: " + (string)wmiObj.Properties["LocalAccount"].Value.ToString() + "\n";
                    names += "Name: " + (string)wmiObj.Properties["Name"].Value + "\n";
                    //names += "PasswordChangeable: " + (string)wmiObj.Properties["PasswordChangeable"].Value.ToString() + "\n";
                    //names += "PasswordExpires: " + (string)wmiObj.Properties["PasswordExpires"].Value.ToString() + "\n";
                    //names += "PasswordRequired: " + (string)wmiObj.Properties["PasswordRequired"].Value.ToString() + "\n";
                    //names += "SID: " + (string)wmiObj.Properties["SID"].Value + "\n";
                    //names += "SIDType: " + (string)wmiObj.Properties["SIDType"].Value.ToString() + "\n";
                    //names += "Status: " + (string)wmiObj.Properties["Status"].Value + "\n";
                    //names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n\n";
                    names += "\n";
                }
            }
            else
            {
                foreach (ManagementObject wmiObj in searcher.Get())
                {
                    if (input == "finger " + (string)wmiObj.Properties["Name"].Value)
                    {
                        names += "Caption: " + (string)wmiObj.Properties["Caption"].Value + "\n";
                        names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n";
                        names += "Full Name: " + (string)wmiObj.Properties["FullName"].Value + "\n";
                        names += "User: " + (string)wmiObj.Properties["Name"].Value + "\n";
                        names += "\n";
                        //names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n\n";
                    }

                    if (input == "finger " + (string)wmiObj.Properties["FullName"].Value)
                    {
                        names += "Caption: " + (string)wmiObj.Properties["Caption"].Value + "\n";
                        names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n";
                        names += "Full Name: " + (string)wmiObj.Properties["FullName"].Value + "\n";
                        names += "User: " + (string)wmiObj.Properties["Name"].Value + "\n";
                        names += "\n";
                        //names += "Domain: " + (string)wmiObj.Properties["Domain"].Value + "\n\n";
                    }


                }
            }
            



            return names;
        }

        public void SendMessage(string outputMessage)
        {


                byte[] outputData = Encoding.UTF8.GetBytes(outputMessage);
                clientSocket.Send(outputData);

        }


        public void CloseConnection()
        {
            socket.Close();
        }

        public void InitConnection()
        {
            String host = System.Net.Dns.GetHostName();
            // Получение ip-адреса.
            //ipAdress = Dns.GetHostByName(host).AddressList[0].ToString();

            bool notFind = true;
            int i = 0;
            while (notFind && i < Dns.GetHostEntry(host).AddressList.Length)
            {
                if (Dns.GetHostEntry(host).AddressList[i].AddressFamily.ToString() == "InterNetwork")
                {
                    ipAdress = Dns.GetHostEntry(host).AddressList[i].ToString();
                    notFind = false;
                }
                else
                    i++;
            }

            //ipAdress = Dns.GetHostEntry(host).AddressList[4].ToString();
            Console.WriteLine(ipAdress);
            Console.WriteLine("\n");

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ipPoint);
            this.socket.Listen(3);

            lstn();
            //Task.Run(lstn);

            //this.serverThread = new Thread(new ThreadStart(GetMessages));
            //this.serverThread.Start();


        }

        public void lstn()
        {

            while (true)
            {
                Socket clientSock = this.socket.Accept();
                clientSocket = clientSock;
                Console.WriteLine("Accept");

                GetMessages(clientSocket);
                //SendMessage("Hi");


                //Task.Run(() => GetMessages(clientSocket));


            }
        }



    }


}

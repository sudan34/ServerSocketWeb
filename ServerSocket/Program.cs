using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int Port = 7022;

    static void Main(string[] args)
    {
        StartServer();
    }

    static void StartServer()
    {
        try
        {
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and port.
            listener.Bind(new IPEndPoint(IPAddress.Any, Port));

            // Start listening for incoming connections.
            listener.Listen(10); // Maximum 10 pending connections

            Console.WriteLine($"Server is listening on port {Port}...");

            while (true)
            {
                // Accept an incoming connection.
                Socket clientSocket = listener.Accept();

                // Start a new thread to handle the client communication.
                System.Threading.Thread clientThread = new System.Threading.Thread(() =>
                {
                    HandleClient(clientSocket);
                });
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = clientSocket.Receive(buffer)) > 0)
            {
                string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received from client: {clientMessage}");

                // Process the client message and prepare a response.
                string responseMessage = $"Server Response: {clientMessage}";
                Console.WriteLine($"Sending to client: {responseMessage}");

                // Send the response back to the client.
                clientSocket.Send(Encoding.ASCII.GetBytes(responseMessage));
            }

            // Close the client socket when done.
            clientSocket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const string ServerIp = "127.0.0.1"; // Change to the server's IP address
    private const int ServerPort = 7022; // Change to the server's port

    static async Task Main(string[] args)
    {
        await StartClientAsync();
    }

    static async Task StartClientAsync()
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));

            Console.WriteLine("Connected to the server.");

            while (true)
            {
                Console.Write("Enter a message to send to the server (or type 'exit' to quit): ");
                string messageToSend = Console.ReadLine();

                if (messageToSend.ToLower() == "exit")
                {
                    break; // Exit the loop and close the client
                }

                Console.WriteLine($"Sending to server: {messageToSend}");
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageToSend);
                await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);

                // Receive and display the server's response.
                var responseBuffer = new byte[1024];
                int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), SocketFlags.None);
                if (bytesRead > 0)
                {
                    string serverResponse = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
                    Console.WriteLine($"Received from server: {serverResponse}");
                }
                else
                {
                    Console.WriteLine("Server closed the connection.");
                    break;
                }
            }
            // Close the client socket when done.
            clientSocket.Close();
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

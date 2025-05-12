using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RealTimeChatAsync
{
    public class ChatMessage
    {
        public int Id { get; set; }                
        public string Sender { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public DateTime SentTime { get; set; } = DateTime.Now;
    }

    public class ChatRepository
    {
        private readonly string _connectionString; 

        public ChatRepository(string connectionString)  
        {
            _connectionString = connectionString; 
        }

        public async Task SaveMessageAsync(ChatMessage message) 
        { 
            string query = @" 
                INSERT INTO ChatMessages (Sender, Receiver, MessageText, SentTime)
                VALUES (@Sender, @Receiver, @MessageText, @SentTime);"; 
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Sender", message.Sender);
                    cmd.Parameters.AddWithValue("@Receiver", message.Receiver);
                    cmd.Parameters.AddWithValue("@MessageText", message.MessageText);
                    cmd.Parameters.AddWithValue("@SentTime", message.SentTime);

                    await cmd.ExecuteNonQueryAsync(); 
                }
            }
        }

        public async Task<List<ChatMessage>> GetRecentMessagesAsync(string user)
        {
            var messages = new List<ChatMessage>();

            string query = @"
                SELECT Id, Sender, Receiver, MessageText, SentTime
                FROM ChatMessages
                WHERE Receiver = @User
                ORDER BY SentTime DESC;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@User", user);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            messages.Add(new ChatMessage
                            {
                                Id = reader.GetInt32(0),
                                Sender = reader.GetString(1),
                                Receiver = reader.GetString(2),
                                MessageText = reader.GetString(3),
                                SentTime = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return messages;
        }

        public async Task DeleteMessageByIdAsync(int messageId)
        {
            string query = "DELETE FROM ChatMessages WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", messageId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=HP\\SQLEXPRESS;Database=QUES_10_05;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
            var repo = new ChatRepository(connectionString);

            while (true)
            {
                Console.WriteLine("\n--- Real-Time Chat Menu ---");
                Console.WriteLine("1. Send Message");
                Console.WriteLine("2. Show Messages");
                Console.WriteLine("3. Delete Message by ID");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option (1-4): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Sender Name: ");
                        string sender = Console.ReadLine() ?? "Unknown";

                        Console.Write("Enter Receiver Name: ");
                        string receiver = Console.ReadLine() ?? "Unknown";

                        Console.Write("Enter Message: ");
                        string messageText = Console.ReadLine() ?? "";

                        var message = new ChatMessage
                        {
                            Sender = sender,
                            Receiver = receiver,
                            MessageText = messageText,
                            SentTime = DateTime.Now
                        };

                        await repo.SaveMessageAsync(message);
                        Console.WriteLine("\nMessage sent successfully!");
                        break;

                    case "2":
                        Console.Write("Enter Receiver Name to view messages: ");
                        string user = Console.ReadLine() ?? "";

                        var messages = await repo.GetRecentMessagesAsync(user);
                        Console.WriteLine($"\n Recent messages for {user}:");

                        if (messages.Count == 0)
                            Console.WriteLine("No messages found.");
                        else
                            foreach (var msg in messages)
                                Console.WriteLine($"ID: {msg.Id} | From: {msg.Sender} | At: {msg.SentTime} | Message: {msg.MessageText}");
                        break;

                    case "3":
                        Console.Write("Enter Message ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int messageId))
                        {
                            await repo.DeleteMessageByIdAsync(messageId);
                            Console.WriteLine(" Message deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine(" Invalid ID entered.");
                        }
                        break;

                    case "4":
                        Console.WriteLine(" Exiting application...");
                        return;

                    default:
                        Console.WriteLine(" Invalid choice. Please enter a number from 1 to 4.");
                        break;
                }
            }
        }
    }
}

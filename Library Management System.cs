using System;
using System.Data;
using System.Data.SqlClient;

namespace LibraryManagementSystem
{
    class Program
    {
        static string connectionString = "Server=HP\\SQLEXPRESS;Database=EmployeeDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Library Management System ---");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. View Books");
                Console.WriteLine("3. Update Book");
                Console.WriteLine("4. Delete Book");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddBook(); break;
                    case "2": ViewBooks(); break;
                    case "3": UpdateBook(); break;
                    case "4": DeleteBook(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        static void AddBook()
        {
            Console.Write("Enter Book ID: ");
            string bookId = Console.ReadLine();

            Console.Write("Enter Title: ");
            string title = Console.ReadLine();

            Console.Write("Enter Author: ");
            string author = Console.ReadLine();

            Console.Write("Enter Published Year: ");
            int year = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("AddBook", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Author", author);
                cmd.Parameters.AddWithValue("@PublishedYear", year);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine(" Book added successfully.");
            }
        }

        static void ViewBooks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetBooks", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("\n--- Book List ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["BookId"]}, Title: {reader["Title"]}, Author: {reader["Author"]}, Year: {reader["PublishedYear"]}");
                }
                reader.Close();
            }
        }

        static void UpdateBook()
        {
            Console.Write("Enter Book ID to update: ");
            string id = Console.ReadLine();

            Console.Write("Enter New Title: ");
            string title = Console.ReadLine();

            Console.Write("Enter New Author: ");
            string author = Console.ReadLine();

            Console.Write("Enter New Published Year: ");
            int year = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateBook", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookId", id);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Author", author);
                cmd.Parameters.AddWithValue("@PublishedYear", year);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? " Book updated successfully." : "⚠ Book not found.");
            }
        }

        static void DeleteBook()
        {
            Console.Write("Enter Book ID to delete: ");
            string id = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteBook", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BookId", id);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Book deleted successfully." : " Book not found.");
            }
        }
    }
}

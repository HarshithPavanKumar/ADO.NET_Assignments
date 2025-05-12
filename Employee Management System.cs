using System;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    class Program
    {
        static string connectionString = "Server=HP\\SQLEXPRESS;Database=EmployeeDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Employee Management System ---");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. View Employees");
                Console.WriteLine("3. Update Employee");
                Console.WriteLine("4. Delete Employee");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        ViewEmployees();
                        break;
                    case "3":
                        UpdateEmployee();
                        break;
                    case "4":
                        DeleteEmployee();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void AddEmployee()
        {
            Console.Write("Enter EmployeeId: ");
            int employeeId = int.Parse(Console.ReadLine());
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter Job Title: ");
            string jobTitle = Console.ReadLine();
            Console.Write("Enter Salary: ");
            decimal salary = decimal.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Employees (EmployeeId, FirstName, LastName, JobTitle, Salary) VALUES (@EmployeeId, @FirstName, @LastName, @JobTitle, @Salary)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                cmd.Parameters.AddWithValue("@Salary", salary);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Employee added successfully." : "Error adding employee.");
            }
        }

        static void ViewEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("\n--- Employee List ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["EmployeeId"]}, Name: {reader["FirstName"]} {reader["LastName"]}, Title: {reader["JobTitle"]}, Salary: {reader["Salary"]}");
                }
                reader.Close();
            }
        }

        static void UpdateEmployee()
        {
            Console.Write("Enter Employee ID to update: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter New First Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter New Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter New Job Title: ");
            string jobTitle = Console.ReadLine();
            Console.Write("Enter New Salary: ");
            decimal salary = decimal.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, JobTitle = @JobTitle, Salary = @Salary WHERE EmployeeId = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Employee updated successfully." : "Employee not found.");
            }
        }

        static void DeleteEmployee()
        {
            Console.Write("Enter Employee ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Employees WHERE EmployeeId = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Employee deleted successfully." : "Employee not found.");
            }
        }
    }
}

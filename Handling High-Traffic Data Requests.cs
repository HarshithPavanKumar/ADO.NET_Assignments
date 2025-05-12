using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EmployeeManagementAsync
{
    
    public class Employee
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = string.Empty; 
        public DateTime JoiningDate { get; set; }
    }

    
    public class EmployeeRepository 
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString) 
        {
            _connectionString = connectionString;
        }

        
        public async Task<List<Employee>> GetRecentEmployeesAsync() 
        { 
            var employees = new List<Employee>(); 

            string query = @"
                SELECT Id, FullName, JoiningDate
                FROM Employees
                WHERE JoiningDate >= DATEADD(MONTH, -6, GETDATE());";

            using (SqlConnection conn = new SqlConnection(_connectionString)) 
            {
                await conn.OpenAsync(); // non-blocking open

                using (SqlCommand cmd = new SqlCommand(query, conn)) 
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) 
                {
                    while (await reader.ReadAsync()) 
                    { 
                        var employee = new Employee
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0), 
                            FullName = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1),
                            JoiningDate = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2)
                        };

                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }

        
        public List<Employee> GetRecentEmployeesSync() 
        {
            var employees = new List<Employee>();

            string query = @"
                SELECT Id, FullName, JoiningDate
                FROM Employees
                WHERE JoiningDate >= DATEADD(MONTH, -6, GETDATE());";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open(); // blocks until connected

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader()) // blocking
                {
                    while (reader.Read())
                    {
                        var employee = new Employee
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1),
                            JoiningDate = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2)
                        };

                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=HP\\SQLEXPRESS;Database=QUES_10_05;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
            var repository = new EmployeeRepository(connectionString);

            while (true)
            {
                Console.WriteLine("\nSelect data fetch mode:");
                Console.WriteLine("1. Synchronous");
                Console.WriteLine("2. Asynchronous");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice : ");
                string? option = Console.ReadLine(); 

                if (option == "3")
                {
                    Console.WriteLine("Exiting application...");
                    break;
                }

                try
                {
                    List<Employee> employees;
                    var stopwatch = Stopwatch.StartNew(); // Start measuring time

                    if (option == "1")
                    {
                        Console.WriteLine("\n[Using Synchronous Method — Blocking Execution]\n");
                        employees = repository.GetRecentEmployeesSync(); // blocking call
                    }
                    else if (option == "2")
                    {
                        Console.WriteLine("\n[Using Asynchronous Method — Non-Blocking Execution]\n");
                        employees = await repository.GetRecentEmployeesAsync(); // non-blocking call
                    }
                    else
                    {
                        Console.WriteLine(" Invalid choice. Please enter 1, 2, or 3.");
                        continue;
                    }

                    stopwatch.Stop(); // End timing

                    if (employees.Count > 0)
                    {
                        foreach (var emp in employees)
                        {
                            Console.WriteLine($"ID: {emp.Id}, Name: {emp.FullName}, Joined: {emp.JoiningDate.ToShortDateString()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No recent employees found.");
                    }

                    Console.WriteLine($"\n Time taken: {stopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while fetching employee data:");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

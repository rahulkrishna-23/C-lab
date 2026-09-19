using System;
using System.Data;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString =
            "Server=localhost;Database=studentdb;User ID=root;Password=sqllab26.;";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string createTable = @"
                CREATE TABLE IF NOT EXISTS Students
                (
                    Id INT PRIMARY KEY AUTO_INCREMENT,
                    Name VARCHAR(100) NOT NULL,
                    Department VARCHAR(100) NOT NULL,
                    Mark INT
                )";

            using (MySqlCommand command = new MySqlCommand(createTable, connection))
            {
                command.ExecuteNonQuery();
            }

            string insertData = @"
                INSERT INTO Students (Name, Department, Mark)
                SELECT 'Arun', 'IT', 85
                WHERE NOT EXISTS
                (SELECT 1 FROM Students WHERE Name = 'Arun');

                INSERT INTO Students (Name, Department, Mark)
                SELECT 'Priya', 'CSE', 90
                WHERE NOT EXISTS
                (SELECT 1 FROM Students WHERE Name = 'Priya');

                INSERT INTO Students (Name, Department, Mark)
                SELECT 'Kumar', 'ECE', 78
                WHERE NOT EXISTS
                (SELECT 1 FROM Students WHERE Name = 'Kumar');
            ";

            using (MySqlCommand command = new MySqlCommand(insertData, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        DataSet dataSet = new DataSet();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Id, Name, Department, Mark FROM Students";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable("Students");

                    table.Columns.Add("Id", typeof(int));
                    table.Columns.Add("Name", typeof(string));
                    table.Columns.Add("Department", typeof(string));
                    table.Columns.Add("Mark", typeof(int));

                    while (reader.Read())
                    {
                        DataRow row = table.NewRow();

                        row["Id"] = reader.GetInt32(0);
                        row["Name"] = reader.GetString(1);
                        row["Department"] = reader.GetString(2);
                        row["Mark"] = reader.GetInt32(3);

                        table.Rows.Add(row);
                    }

                    dataSet.Tables.Add(table);
                }
            }
        }

        Console.WriteLine("Database connection closed.");
        Console.WriteLine("Data is now available in the DataSet.");
        Console.WriteLine();

        DataTable students = dataSet.Tables["Students"];

        Console.WriteLine("STUDENT DETAILS");
        Console.WriteLine("----------------");

        foreach (DataRow row in students.Rows)
        {
            Console.WriteLine(
                "ID: " + row["Id"] +
                " | Name: " + row["Name"] +
                " | Department: " + row["Department"] +
                " | Mark: " + row["Mark"]
            );
        }

        Console.WriteLine();
        Console.WriteLine("Students with marks greater than 80:");
        Console.WriteLine("------------------------------------");

        foreach (DataRow row in students.Rows)
        {
            int mark = Convert.ToInt32(row["Mark"]);

            if (mark > 80)
            {
                Console.WriteLine(
                    row["Name"] + " - " +
                    row["Department"] + " - " +
                    row["Mark"]
                );
            }
        }

        Console.WriteLine();
        Console.WriteLine("Program completed successfully.");
    }
}
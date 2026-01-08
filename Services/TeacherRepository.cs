using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DB.Data
{
    public static class TeacherRepository
    {
        public static void Add(string name, string? email, string? mobile = null, string? gender = null, string? employmentDate = null)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO teachers (Name, Email, Mobile, Gender, EmploymentDate) VALUES ($name, $email, $mobile, $gender, $employment);";
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$email", email ?? "");
            cmd.Parameters.AddWithValue("$mobile", mobile ?? string.Empty);
            cmd.Parameters.AddWithValue("$gender", gender ?? string.Empty);
            cmd.Parameters.AddWithValue("$employment", employmentDate ?? string.Empty);
            cmd.ExecuteNonQuery();
        }

        public static (int Id, string Name, string Email, string Mobile, string Gender, string EmploymentDate)? GetById(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email, Mobile, Gender, EmploymentDate FROM teachers WHERE Id = $id LIMIT 1;";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read()) return (r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? string.Empty : r.GetString(2), r.IsDBNull(3) ? string.Empty : r.GetString(3), r.IsDBNull(4) ? string.Empty : r.GetString(4), r.IsDBNull(5) ? string.Empty : r.GetString(5));
            return null;
        }

        public static bool Update(int id, string name, string? email, string? mobile = null, string? gender = null, string? employmentDate = null)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE teachers SET Name = $name, Email = $email, Mobile = $mobile, Gender = $gender, EmploymentDate = $employmentDate WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$email", email ?? string.Empty);
            cmd.Parameters.AddWithValue("$mobile", mobile ?? string.Empty);
            cmd.Parameters.AddWithValue("$gender", gender ?? string.Empty);
            cmd.Parameters.AddWithValue("$employment", employmentDate ?? string.Empty);
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        public static List<(int Id, string Name, string Email, string Mobile, string Gender, string EmploymentDate)> GetAll()
        {
            var list = new List<(int, string, string, string, string, string)>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email, Mobile, Gender, EmploymentDate FROM teachers;";
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? string.Empty : r.GetString(2), r.IsDBNull(3) ? string.Empty : r.GetString(3), r.IsDBNull(4) ? string.Empty : r.GetString(4), r.IsDBNull(5) ? string.Empty : r.GetString(5)));
            return list;
        }

        public static bool Delete(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM teachers WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

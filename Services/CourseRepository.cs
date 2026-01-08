using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DB.Data
{
    public static class CourseRepository
    {
        public static int Add(string courseName, string? description, int? teacherId)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            // If a teacher id was supplied but doesn't exist, treat as null to avoid FK error
            if (teacherId.HasValue && TeacherRepository.GetById(teacherId.Value) == null)
            {
                teacherId = null;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO subjects (Name, Description, TeacherId) VALUES ($n, $d, $t); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$n", courseName);
            cmd.Parameters.AddWithValue("$d", description ?? string.Empty);
            cmd.Parameters.AddWithValue("$t", teacherId ?? (object)System.DBNull.Value);
            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public static List<(int Id, string Name, string Description, int? TeacherId)> GetAll()
        {
            var list = new List<(int, string, string, int?)>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Description, TeacherId FROM subjects;";
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? string.Empty : r.GetString(2), r.IsDBNull(3) ? (int?)null : r.GetInt32(3)));
            return list;
        }

        public static (int Id, string Name, string Description, int? TeacherId)? GetById(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Description, TeacherId FROM subjects WHERE Id = $id LIMIT 1;";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read()) return (r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? string.Empty : r.GetString(2), r.IsDBNull(3) ? (int?)null : r.GetInt32(3));
            return null;
        }

        public static bool Update(int id, string courseName, string? description, int? teacherId)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            // Ensure teacher exists before assigning FK
            if (teacherId.HasValue && TeacherRepository.GetById(teacherId.Value) == null)
            {
                teacherId = null;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE subjects SET Name = $n, Description = $d, TeacherId = $t WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$n", courseName);
            cmd.Parameters.AddWithValue("$d", description ?? string.Empty);
            cmd.Parameters.AddWithValue("$t", teacherId ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Delete(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM subjects WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

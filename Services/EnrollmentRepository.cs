using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DB.Data
{
    public static class EnrollmentRepository
    {
        public static void Add(int studentSerialNo, int subjectId, double? grade = null, int maxGrade = 100)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO student_subjects (StudentSerialNo, SubjectId, Grade, MaxGrade) VALUES ($s, $subjectId, $g, $m);";
            cmd.Parameters.AddWithValue("$s", studentSerialNo);
            cmd.Parameters.AddWithValue("$subjectId", subjectId);
            cmd.Parameters.AddWithValue("$g", grade ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$m", maxGrade);
            cmd.ExecuteNonQuery();
        }

        public static List<(int Id, int SubjectId, double? Grade, int MaxGrade)> LoadByStudent(int studentSerialNo)
        {
            var list = new List<(int Id, int SubjectId, double? Grade, int MaxGrade)>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, SubjectId, Grade, MaxGrade FROM student_subjects WHERE StudentSerialNo = $s;";
            cmd.Parameters.AddWithValue("$s", studentSerialNo);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((Id: r.GetInt32(0), SubjectId: r.GetInt32(1), Grade: r.IsDBNull(2) ? (double?)null : r.GetDouble(2), MaxGrade: r.GetInt32(3)));
            return list;
        }

        public static void DeleteAllForStudent(int studentSerialNo)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM student_subjects WHERE StudentSerialNo = $s;";
            cmd.Parameters.AddWithValue("$s", studentSerialNo);
            cmd.ExecuteNonQuery();
        }

        public static bool Delete(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM student_subjects WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Update(int id, int subjectId, double? grade, int maxGrade)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE student_subjects SET SubjectId = $sub, Grade = $g, MaxGrade = $m WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$sub", subjectId);
            cmd.Parameters.AddWithValue("$g", grade ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$m", maxGrade);
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DB.Data
{
    public static class AssignmentRepository
    {
        public static int Add(int studentSerialNo, int? courseId, string name, string? type, string? description, int maxGrade, double? grade, string? dateAssigned, string? dateDue, string? dateSubmitted, int? teacherId)
        {
            // Validate student
            var student = StudentRepository.LoadById(studentSerialNo);
            if (student == null) throw new InvalidOperationException("Student not found.");

            // Validate course and teacher; if invalid, set to null to avoid FK errors
            if (courseId.HasValue && CourseRepository.GetById(courseId.Value) == null) courseId = null;
            if (teacherId.HasValue && TeacherRepository.GetById(teacherId.Value) == null) teacherId = null;

            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO assignments (StudentSerialNo, CourseId, Name, Type, Description, MaxGrade, Grade, DateAssigned, DateDue, DateSubmitted, TeacherId) VALUES ($s,$c,$n,$t,$d,$m,$g,$da,$dd,$ds,$ti); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$s", studentSerialNo);
            cmd.Parameters.AddWithValue("$c", courseId ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$n", name);
            cmd.Parameters.AddWithValue("$t", type ?? string.Empty);
            cmd.Parameters.AddWithValue("$d", description ?? string.Empty);
            cmd.Parameters.AddWithValue("$m", maxGrade);
            cmd.Parameters.AddWithValue("$g", grade ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$da", string.IsNullOrWhiteSpace(dateAssigned) ? (object)System.DBNull.Value : dateAssigned);
            cmd.Parameters.AddWithValue("$dd", string.IsNullOrWhiteSpace(dateDue) ? (object)System.DBNull.Value : dateDue);
            cmd.Parameters.AddWithValue("$ds", string.IsNullOrWhiteSpace(dateSubmitted) ? (object)System.DBNull.Value : dateSubmitted);
            cmd.Parameters.AddWithValue("$ti", teacherId ?? (object)System.DBNull.Value);
            var id = (long)cmd.ExecuteScalar();
            return (int)id;
        }

        public static (int Id, int? CourseId, string Name, string? Type, string? Description, int MaxGrade, double? Grade, string? DateAssigned, string? DateDue, string? DateSubmitted, int? TeacherId)? GetById(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, CourseId, Name, Type, Description, MaxGrade, Grade, DateAssigned, DateDue, DateSubmitted, TeacherId FROM assignments WHERE Id = $id LIMIT 1;";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read()) return (r.GetInt32(0), r.IsDBNull(1) ? (int?)null : r.GetInt32(1), r.GetString(2), r.IsDBNull(3) ? null : r.GetString(3), r.IsDBNull(4) ? null : r.GetString(4), r.IsDBNull(5) ? 100 : r.GetInt32(5), r.IsDBNull(6) ? (double?)null : r.GetDouble(6), r.IsDBNull(7) ? null : r.GetString(7), r.IsDBNull(8) ? null : r.GetString(8), r.IsDBNull(9) ? null : r.GetString(9), r.IsDBNull(10) ? (int?)null : r.GetInt32(10));
            return null;
        }

        public static List<(int Id, int? CourseId, string Name, string? Type, double? Grade, int MaxGrade)> GetByStudent(int studentSerialNo)
        {
            var list = new List<(int, int?, string, string?, double?, int)>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, CourseId, Name, Type, Grade, MaxGrade FROM assignments WHERE StudentSerialNo = $s ORDER BY Id;";
            cmd.Parameters.AddWithValue("$s", studentSerialNo);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((r.GetInt32(0), r.IsDBNull(1) ? (int?)null : r.GetInt32(1), r.GetString(2), r.IsDBNull(3) ? null : r.GetString(3), r.IsDBNull(4) ? (double?)null : r.GetDouble(4), r.IsDBNull(5) ? 100 : r.GetInt32(5)));
            return list;
        }

        public static List<(int Id, int StudentSerialNo, int? CourseId, string Name, string? Type, double? Grade, int MaxGrade)> GetAll()
        {
            var list = new List<(int, int, int?, string, string?, double?, int)>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, StudentSerialNo, CourseId, Name, Type, Grade, MaxGrade FROM assignments ORDER BY Id;";
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add((r.GetInt32(0), r.GetInt32(1), r.IsDBNull(2) ? (int?)null : r.GetInt32(2), r.GetString(3), r.IsDBNull(4) ? null : r.GetString(4), r.IsDBNull(5) ? (double?)null : r.GetDouble(5), r.IsDBNull(6) ? 100 : r.GetInt32(6)));
            return list;
        }

        public static bool Update(int id, int? courseId, string name, string? type, string? description, int maxGrade, double? grade, string? dateAssigned, string? dateDue, string? dateSubmitted, int? teacherId)
        {
            // Validate course/teacher
            if (courseId.HasValue && CourseRepository.GetById(courseId.Value) == null) courseId = null;
            if (teacherId.HasValue && TeacherRepository.GetById(teacherId.Value) == null) teacherId = null;
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE assignments SET CourseId=$c, Name=$n, Type=$t, Description=$d, MaxGrade=$m, Grade=$g, DateAssigned=$da, DateDue=$dd, DateSubmitted=$ds, TeacherId=$ti WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$c", courseId ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$n", name);
            cmd.Parameters.AddWithValue("$t", type ?? string.Empty);
            cmd.Parameters.AddWithValue("$d", description ?? string.Empty);
            cmd.Parameters.AddWithValue("$m", maxGrade);
            cmd.Parameters.AddWithValue("$g", grade ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$da", string.IsNullOrWhiteSpace(dateAssigned) ? (object)System.DBNull.Value : dateAssigned);
            cmd.Parameters.AddWithValue("$dd", string.IsNullOrWhiteSpace(dateDue) ? (object)System.DBNull.Value : dateDue);
            cmd.Parameters.AddWithValue("$ds", string.IsNullOrWhiteSpace(dateSubmitted) ? (object)System.DBNull.Value : dateSubmitted);
            cmd.Parameters.AddWithValue("$ti", teacherId ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Delete(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM assignments WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

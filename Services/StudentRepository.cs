using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DB.Data
{
    public static class StudentRepository
    {
        public static List<StudentClass> LoadAll()
        {
            var students = new List<StudentClass>();
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM students;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new StudentClass
                {
                    SerialNo = reader.GetInt32(0),
                    StudentName = reader.GetString(1),
                    Gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Religion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    BirthDay = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    BirthMonth = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    BirthYear = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    Address = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    NationalId = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    Phone = reader.FieldCount > 9 && !reader.IsDBNull(9) ? reader.GetString(9) : null,
                    Email = reader.FieldCount > 10 && !reader.IsDBNull(10) ? reader.GetString(10) : null
                });
            }
            return students;
        }

        public static StudentClass? LoadById(int id)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM students WHERE SerialNo = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new StudentClass
                {
                    SerialNo = reader.GetInt32(0),
                    StudentName = reader.GetString(1),
                    Gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Religion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    BirthDay = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    BirthMonth = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    BirthYear = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    Address = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    NationalId = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    Phone = reader.FieldCount > 9 && !reader.IsDBNull(9) ? reader.GetString(9) : null,
                    Email = reader.FieldCount > 10 && !reader.IsDBNull(10) ? reader.GetString(10) : null
                };
            }
            return null;
        }

        public static void Add(StudentClass student)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO students 
                (StudentName, Gender, Religion, BirthDay, BirthMonth, BirthYear, Address, NationalId, Phone, Email)
                VALUES ($name, $gender, $religion, $day, $month, $year, $address, $nid, $phone, $email);";

            cmd.Parameters.AddWithValue("$name", student.StudentName);
            cmd.Parameters.AddWithValue("$gender", student.Gender);
            cmd.Parameters.AddWithValue("$religion", student.Religion);
            cmd.Parameters.AddWithValue("$day", student.BirthDay ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$month", student.BirthMonth ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$year", student.BirthYear ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$address", student.Address);
            cmd.Parameters.AddWithValue("$nid", student.NationalId);
            cmd.Parameters.AddWithValue("$phone", student.Phone ?? string.Empty);
            cmd.Parameters.AddWithValue("$email", student.Email ?? string.Empty);

            cmd.ExecuteNonQuery();
        }

        public static void Update(StudentClass student)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE students SET 
                StudentName = $name,
                Gender = $gender,
                Religion = $religion,
                BirthDay = $day,
                BirthMonth = $month,
                BirthYear = $year,
                Address = $address,
                NationalId = $nid,
                Phone = $phone,
                Email = $email
                WHERE SerialNo = $id;";

            cmd.Parameters.AddWithValue("$id", student.SerialNo);
            cmd.Parameters.AddWithValue("$name", student.StudentName);
            cmd.Parameters.AddWithValue("$gender", student.Gender);
            cmd.Parameters.AddWithValue("$religion", student.Religion);
            cmd.Parameters.AddWithValue("$day", student.BirthDay ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$month", student.BirthMonth ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$year", student.BirthYear ?? (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("$address", student.Address);
            cmd.Parameters.AddWithValue("$nid", student.NationalId);
            cmd.Parameters.AddWithValue("$phone", student.Phone ?? string.Empty);
            cmd.Parameters.AddWithValue("$email", student.Email ?? string.Empty);

            cmd.ExecuteNonQuery();
        }

        public static bool DeleteByName(string name)
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM students WHERE StudentName = $name;";
            cmd.Parameters.AddWithValue("$name", name);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static void DeleteAll()
        {
            using var connection = new SqliteConnection(DbPaths.GetConnectionString());
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM students;";
            cmd.ExecuteNonQuery();

            transaction.Commit();
        }
    }
}

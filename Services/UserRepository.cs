using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Models;

namespace Services
{
    public static class UserRepository
    {
        public static void Add(User user)
        {
            using var connection = new SqliteConnection(DB.Data.DbPaths.GetConnectionString());
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO users (Username, PasswordHash, Role) VALUES ($u, $p, $r);";
            cmd.Parameters.AddWithValue("$u", user.Username);
            cmd.Parameters.AddWithValue("$p", user.PasswordHash);
            cmd.Parameters.AddWithValue("$r", user.Role);
            cmd.ExecuteNonQuery();
        }

        public static User? GetByUsername(string username)
        {
            using var connection = new SqliteConnection(DB.Data.DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM users WHERE Username = $u LIMIT 1;";
            cmd.Parameters.AddWithValue("$u", username);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3)
                };
            }
            return null;
        }

        public static List<User> GetAll()
        {
            var list = new List<User>();
            using var connection = new SqliteConnection(DB.Data.DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM users;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3)
                });
            }
            return list;
        }

        public static bool DeleteByUsername(string username)
        {
            using var connection = new SqliteConnection(DB.Data.DbPaths.GetConnectionString());
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM users WHERE Username = $u;";
            cmd.Parameters.AddWithValue("$u", username);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

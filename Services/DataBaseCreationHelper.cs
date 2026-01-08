using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading;

namespace DB.Data
{
    public static class DataBaseCreationHelper
    {
        public static void CreateDB(string databaseName = "studentsCLI" ,string studentsTableName = "students", string subjectsTableName = "subjects",string assignmentTableName ="assignments", string? fullPath = null)
        {
            var connectionString = fullPath != null
                ? $"Data Source={fullPath}"
                : DbPaths.GetConnectionString(databaseName);

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Enable foreign keys enforcement in SQLite
            var fkCmd = connection.CreateCommand();
            fkCmd.CommandText = "PRAGMA foreign_keys = ON;";
            fkCmd.ExecuteNonQuery();

            // Students table
            var createStudents = connection.CreateCommand();
            createStudents.CommandText =
            #region Create Students Table
                @$"CREATE TABLE IF NOT EXISTS {studentsTableName} (
                    SerialNo INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentName TEXT NOT NULL,
                    Gender TEXT NOT NULL,
                    Religion TEXT,
                    BirthDay INTEGER,
                    BirthMonth INTEGER,
                    BirthYear INTEGER,
                    Address TEXT,
                    NationalId TEXT,
                    Phone TEXT,
                    Email TEXT
                );";
                #endregion

            createStudents.ExecuteNonQuery();

            // Indexes
            var idxCmd = connection.CreateCommand();
            idxCmd.CommandText = $"CREATE INDEX IF NOT EXISTS idx_{studentsTableName}_StudentName ON {studentsTableName}(StudentName);";
            idxCmd.ExecuteNonQuery();

            // Courses table (represents subjects/courses)
            // Teachers table - create before subjects so foreign key references are valid at creation time
            var teachersCmd = connection.CreateCommand();
            teachersCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS teachers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT,
                    Mobile TEXT,
                    Gender TEXT,
                    EmploymentDate TEXT
                );";
            teachersCmd.ExecuteNonQuery();

            // Courses table (represents subjects/courses)
            var coursesCmd = connection.CreateCommand();
            coursesCmd.CommandText =
            #region create Subjects Table
                 $@"CREATE TABLE IF NOT EXISTS {subjectsTableName} (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    TeacherId INTEGER,
                    FOREIGN KEY(TeacherId) REFERENCES teachers(Id) ON DELETE SET NULL
                );";
                #endregion
            coursesCmd.ExecuteNonQuery();

            // Student-Subjects linking table (student enrollments / grades)
            var studentSubjectsCmd = connection.CreateCommand();
            studentSubjectsCmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS student_subjects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentSerialNo INTEGER NOT NULL,
                    SubjectId INTEGER NOT NULL,
                    Grade REAL,
                    MaxGrade INTEGER DEFAULT 100,
                    FOREIGN KEY(StudentSerialNo) REFERENCES {studentsTableName}(SerialNo) ON DELETE CASCADE,
                    FOREIGN KEY(SubjectId) REFERENCES {subjectsTableName}(Id) ON DELETE CASCADE
                );";
            studentSubjectsCmd.ExecuteNonQuery();

            // Assignment table
            var assiCmd = connection.CreateCommand();
            assiCmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {assignmentTableName} (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentSerialNo INTEGER NOT NULL,
                    CourseId INTEGER,
                    Name TEXT NOT NULL,
                    Type TEXT,
                    Description TEXT,
                    MaxGrade INTEGER NOT NULL DEFAULT 100,
                    Grade REAL,
                    DateAssigned TEXT,
                    DateDue TEXT,
                    DateSubmitted TEXT,
                    TeacherId INTEGER,
                    FOREIGN KEY(StudentSerialNo) REFERENCES {studentsTableName}(SerialNo) ON DELETE CASCADE,
                    FOREIGN KEY(CourseId) REFERENCES {subjectsTableName}(Id) ON DELETE SET NULL,
                    FOREIGN KEY(TeacherId) REFERENCES teachers(Id) ON DELETE SET NULL
                );";
            assiCmd.ExecuteNonQuery();

            // Indexes for assignments for faster lookups
            var assignIdxStudent = connection.CreateCommand();
            assignIdxStudent.CommandText = $"CREATE INDEX IF NOT EXISTS idx_{assignmentTableName}_student ON {assignmentTableName}(StudentSerialNo);";
            assignIdxStudent.ExecuteNonQuery();

            var assignIdxCourse = connection.CreateCommand();
            assignIdxCourse.CommandText = $"CREATE INDEX IF NOT EXISTS idx_{assignmentTableName}_course ON {assignmentTableName}(CourseId);";
            assignIdxCourse.ExecuteNonQuery();

            // Users table
            var usersCmd = connection.CreateCommand();
            usersCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL
                );";
            usersCmd.ExecuteNonQuery();

            var usersIdx = connection.CreateCommand();
            usersIdx.CommandText = "CREATE INDEX IF NOT EXISTS idx_users_username ON users(Username);";
            usersIdx.ExecuteNonQuery();

            // Index for student_subjects
            var subjIdx = connection.CreateCommand();
            subjIdx.CommandText = $"CREATE INDEX IF NOT EXISTS idx_student_subjects_student ON student_subjects(StudentSerialNo);";
            subjIdx.ExecuteNonQuery();

            // Config table for app settings
            var configCmd = connection.CreateCommand();
            configCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS config (
                    Key TEXT PRIMARY KEY,
                    Value TEXT
                );";
            configCmd.ExecuteNonQuery();
        }
    }

    public static class DataBaseDeletionHelper
    {
        public static void DeleteDB(string databaseName = "studentsCLI")
        {
            // Resolve the actual DB path
            var dbPath = DbPaths.GetDatabasePath(databaseName);

            // Clear pooled connections for Microsoft.Data.Sqlite
            try
            {
                SqliteConnection.ClearAllPools();
            }
            catch
            {
                // If clearing pools fails for any reason, continue and attempt delete anyway.
            }

            // Give managed finalizers a chance to release any lingering handles
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Attempt delete with a small retry loop to mitigate transient locks
            if (File.Exists(dbPath))
            {
                const int maxAttempts = 3;
                const int delayMs = 100;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        File.Delete(dbPath);
                        break;
                    }
                    catch (IOException) when (attempt < maxAttempts)
                    {
                        Thread.Sleep(delayMs);
                        // retry
                    }
                }
            }

            // Do not recreate the database here. Let the application decide whether to
            // create a new database (user choice). This avoids silently recreating the
            // DB after a delete operation.
        }
    }

    public static class DataBaseBackupHelper
    {
        public static void BackupDatabase(string destinationPath, string databaseName = "studentsCLI")
        {
            var src = DbPaths.GetDatabasePath(databaseName);
            if (!File.Exists(src))
                throw new FileNotFoundException("Database file not found.", src);

            // Ensure destination folder exists
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            File.Copy(src, destinationPath, overwrite: true);
        }
    }

    // C#
    public static class DbPaths
    {
        private static string? _activeDatabasePath;

        // Path to the encrypted database on disk
        public static string GetDatabasePath(string name = "studentsCLI")
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appData, "StudentGrader", "CSharpCLI");
            Directory.CreateDirectory(appFolder);
            return Path.Combine(appFolder, $"{name}.db");
        }

        // When a decrypted temporary DB is available, set this to point to it so
        // callers use the decrypted DB for connections.
        public static string? ActiveDatabasePath
        {
            get => _activeDatabasePath;
            set => _activeDatabasePath = value;
        }

        public static string GetConnectionString(string name = "studentsCLI") =>
            $"Data Source={_activeDatabasePath ?? GetDatabasePath(name)}";
    }
}

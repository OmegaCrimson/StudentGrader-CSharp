using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Models;

namespace Services
{
    public static class StudentStorageService
    {
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static readonly string AppFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StudentGrader"
        );

        private static readonly string FilePath = Path.Combine(AppFolder, "students.json");

        public static void Save(List<Student> students)
        {
            try
            {
                Directory.CreateDirectory(AppFolder); // ensures folder exists
                string json = JsonSerializer.Serialize(students, options);
                File.WriteAllText(FilePath, json);
                LoggerService.LogSuccess($"Saved {students.Count} student(s) to AppData.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Save", ex);
            }
        }

        public static List<Student> Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    LoggerService.LogWarning("No saved data found in AppData.");
                    return new List<Student>();
                }

                string json = File.ReadAllText(FilePath);
                var students = JsonSerializer.Deserialize<List<Student>>(json, options) ?? new();
                LoggerService.LogSuccess($"Loaded {students.Count} student(s) from AppData.");
                return students;
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Load", ex);
                return new List<Student>();
            }
        }
    }
}

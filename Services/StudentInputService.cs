using System;
using Models;

namespace Services
{
    public static class StudentInputService
    {
        public static Student CreateStudent()
        {
            Console.Write("Name: ");
            string name = Console.ReadLine()?.Trim();

            Console.Write("Age: ");
            int age = int.TryParse(Console.ReadLine(), out int parsedAge) ? parsedAge : 0;

            var student = new Student { Name = name, Age = age };

            Console.WriteLine("Add subjects (leave name empty to finish):");
            while (true)
            {
                Console.Write("Subject name: ");
                string subjectName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(subjectName)) break;

                Console.Write("Score: ");
                int grade = int.TryParse(Console.ReadLine(), out int s) ? s : 0;

                Console.Write("Max Score: ");
                int maxGrade = int.TryParse(Console.ReadLine(), out int m) ? m : 100;

                Console.Write("Teacher: ");
                string teacher = Console.ReadLine()?.Trim();

                student.AddSubject(new Subject(subjectName, grade, maxGrade, teacher));
            }

            return student;
        }
    }
}

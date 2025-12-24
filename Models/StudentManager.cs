using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class StudentManager
    {
        private readonly List<Student> students = new();

        public void AddStudent(Student student)
        {
            students.Add(student);
        }

        public bool RemoveStudent(string name)
        {
            return students.RemoveAll(s => s.Name == name.Trim()) > 0;
        }

        public Student? FindByName(string name)
        {
            return students.FirstOrDefault(s => s.Name == name.Trim());
        }

        public List<Student> GetAllStudents()
        {
            return students;
        }

        public double OverallAverage()
        {
            var allGrades = students.SelectMany(s => s.Subjects.Select(sub => sub.Grade));
            return allGrades.Any() ? allGrades.Average() : 0;
        }

        public void ClearStudents()
        {
            students.Clear();
        }

        public int Count => students.Count;
    }
}
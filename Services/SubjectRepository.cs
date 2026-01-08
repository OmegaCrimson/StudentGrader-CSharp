using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DB.Data
{
    public static class SubjectRepository
    {
        // Course-level operations
        public static int AddCourse(string name, string? description, int? teacherId) => CourseRepository.Add(name, description, teacherId);

        public static List<(int Id, string Name, string Description, int? TeacherId)> GetAllCourses() => CourseRepository.GetAll();

        public static bool DeleteCourse(int id) => CourseRepository.Delete(id);

        // Enrollment-level operations
        public static void AddForStudent(int studentSerialNo, int subjectId, double? grade = null, int maxGrade = 100) => EnrollmentRepository.Add(studentSerialNo, subjectId, grade, maxGrade);

        public static List<(int Id, int SubjectId, double? Grade, int MaxGrade)> LoadByStudent(int studentSerialNo) => EnrollmentRepository.LoadByStudent(studentSerialNo);

        public static void DeleteAllForStudent(int studentSerialNo) => EnrollmentRepository.DeleteAllForStudent(studentSerialNo);

        public static bool DeleteEnrollment(int id) => EnrollmentRepository.Delete(id);
    }
}

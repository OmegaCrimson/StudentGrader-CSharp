using System;
using DB.Data;

namespace Services
{
    public static class StudentPrinterService
    {
        public static void PrintStudent(StudentClass student)
        {
            Console.WriteLine("────────────────────────────────────────────");
            Console.WriteLine($"ID: {student.SerialNo}");
            Console.WriteLine($"Name: {student.StudentName}");
            Console.WriteLine($"Gender: {student.Gender}");
            Console.WriteLine($"Religion: {student.Religion}");
            Console.WriteLine($"Birth Date: {(student.BirthDay.HasValue ? student.BirthDay.Value.ToString() : "-")}/{(student.BirthMonth.HasValue ? student.BirthMonth.Value.ToString() : "-")}/{(student.BirthYear.HasValue ? student.BirthYear.Value.ToString() : "-")}");
            Console.WriteLine($"Address: {student.Address}");
            Console.WriteLine($"National ID: {student.NationalId}");
            Console.WriteLine($"Phone: {student.Phone ?? "-"}");
            Console.WriteLine($"Email: {student.Email ?? "-"}");

            // Load enrollments for this student
            var enrolls = EnrollmentRepository.LoadByStudent(student.SerialNo);
            Console.WriteLine($"Subjects ({enrolls.Count}):");

            if (enrolls.Count == 0)
            {
                Console.WriteLine("   (No subjects)");
            }
            else
            {
                double totalScore = 0;
                double totalMax = 0;

                foreach (var en in enrolls)
                {
                    var course = CourseRepository.GetById(en.SubjectId);
                    var courseName = course.HasValue ? course.Value.Name : $"(id:{en.SubjectId})";
                    string teacherName = "";
                    if (course.HasValue && course.Value.TeacherId.HasValue)
                    {
                        var t = TeacherRepository.GetById(course.Value.TeacherId.Value);
                        if (t.HasValue) teacherName = t.Value.Name;
                    }
                    Console.WriteLine($"   - {courseName} | Score: {(en.Grade.HasValue ? en.Grade.Value.ToString() : "-")}/{en.MaxGrade} | Teacher: {teacherName}");
                    if (en.Grade.HasValue) totalScore += en.Grade.Value;
                    totalMax += en.MaxGrade;
                }

                double average = totalMax > 0 ? (totalScore / totalMax) * 100 : 0;
                Console.WriteLine($"Average Score: {average:0.##}%");
            }

            Console.WriteLine("────────────────────────────────────────────\n");
        }
    }
}

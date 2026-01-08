using System.Collections.Generic;
using System.IO;
using DB.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

public static class PdfExportHelper
{
    public static void ExportStudents(IEnumerable<StudentClass> students, string filePath, bool includeSubjects = false)
    {
        // Ensure directory exists
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var doc = new Document(PageSize.A4, 25, 25, 30, 30);
        PdfWriter.GetInstance(doc, fs);
        doc.Open();

        // Title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
        doc.Add(new Paragraph("Student Report", titleFont));
        doc.Add(new Paragraph("\n"));

        // Build table
        var baseCols = new[] { "ID", "Name", "Gender", "Religion", "Birth", "Address", "National ID" };
        var subjectCols = new[] { "Subject", "Grade", "Max", "Teacher" };
        int colCount = includeSubjects ? baseCols.Length + subjectCols.Length : baseCols.Length;

        var table = new PdfPTable(colCount) { WidthPercentage = 100 };

        // Header row
        foreach (var h in baseCols) table.AddCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
        if (includeSubjects)
            foreach (var h in subjectCols) table.AddCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));

        // Data rows
        foreach (var s in students)
        {
            var enrolls = includeSubjects ? EnrollmentRepository.LoadByStudent(s.SerialNo) : new System.Collections.Generic.List<(int Id, int SubjectId, double? Grade, int MaxGrade)>();

            if (enrolls.Count == 0)
            {
                AddStudentRow(table, s, null, includeSubjects);
            }
            else
            {
                foreach (var en in enrolls)
                {
                    Models.Subject? subjModel = null;
                    if (includeSubjects)
                    {
                        var course = CourseRepository.GetById(en.SubjectId);
                        var teacherName = string.Empty;
                        if (course.HasValue && course.Value.TeacherId.HasValue)
                        {
                            var t = TeacherRepository.GetById(course.Value.TeacherId.Value);
                            if (t.HasValue) teacherName = t.Value.Name;
                        }
                        subjModel = new Models.Subject(course.HasValue ? course.Value.Name : $"(id:{en.SubjectId})", en.Grade ?? 0, en.MaxGrade, teacherName);
                    }
                    AddStudentRow(table, s, subjModel, includeSubjects);
                }
            }
        }

        doc.Add(table);
        doc.Close();
    }

    private static void AddStudentRow(PdfPTable table, StudentClass s, Models.Subject? subj, bool includeSubjects)
    {
        table.AddCell(s.SerialNo.ToString());
        table.AddCell(s.StudentName);
        table.AddCell(s.Gender);
        table.AddCell(s.Religion);
        table.AddCell($"{s.BirthDay}/{s.BirthMonth}/{s.BirthYear}");
        table.AddCell(s.Address);
        table.AddCell(s.NationalId);

        if (includeSubjects)
        {
            if (subj != null)
            {
                table.AddCell(subj.Name);
                table.AddCell(subj.Grade.ToString());
                table.AddCell(subj.MaxGrade.ToString());
                table.AddCell(subj.Teacher);
            }
            else
            {
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
            }
        }
    }
}

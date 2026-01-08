using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using DB.Data;

public static class ExcelExportHelper
{
    // Export students only (basic)
    public static void ExportStudents(IEnumerable<StudentClass> students, string filePath)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Students");

        string[] headers = { "SerialNo", "StudentName", "Gender", "Religion", "BirthDay", "BirthMonth", "BirthYear", "Address", "NationalId", "Phone", "Email" };
        for (int c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];

        int row = 2;
        foreach (var s in students)
        {
            ws.Cell(row, 1).Value = s.SerialNo;
            ws.Cell(row, 2).Value = s.StudentName;
            ws.Cell(row, 3).Value = s.Gender;
            ws.Cell(row, 4).Value = s.Religion;
            ws.Cell(row, 5).Value = s.BirthDay;
            ws.Cell(row, 6).Value = s.BirthMonth;
            ws.Cell(row, 7).Value = s.BirthYear;
            ws.Cell(row, 8).Value = s.Address;
            ws.Cell(row, 9).Value = s.NationalId;
            ws.Cell(row, 10).Value = s.Phone;
            ws.Cell(row, 11).Value = s.Email;
            row++;
        }

        ws.Columns().AdjustToContents();
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        wb.SaveAs(filePath);
    }

    // Export students only (formatted)
    public static void ExportStudentsFormatted(IEnumerable<StudentClass> students, string filePath)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Students");

        string[] headers = { "م", "اسـم الطالب", "النوع", "الديانه", "يوم تاريخ الميلاد", "شهر تاريخ الميلاد", "سنة تاريخ الميلاد", "العنوان", "الرقم القومى", "الهاتف", "البريد" };
        for (int c = 0; c < headers.Length; c++)
        {
            var cell = ws.Cell(1, c + 1);
            cell.Value = headers[c];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E86AB");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        int row = 2;
        foreach (var s in students)
        {
            ws.Cell(row, 1).Value = s.SerialNo;
            ws.Cell(row, 2).Value = s.StudentName;
            ws.Cell(row, 3).Value = s.Gender;
            ws.Cell(row, 4).Value = s.Religion;
            ws.Cell(row, 5).Value = s.BirthDay;
            ws.Cell(row, 6).Value = s.BirthMonth;
            ws.Cell(row, 7).Value = s.BirthYear;
            ws.Cell(row, 8).Value = s.Address;
            ws.Cell(row, 9).Value = s.NationalId;
            ws.Cell(row, 10).Value = s.Phone;
            ws.Cell(row, 11).Value = s.Email;
            row++;
        }

        ws.Range(1, 1, row - 1, headers.Length).SetAutoFilter();
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        wb.SaveAs(filePath);
    }

    // Export students with subjects (flattened)
    public static void ExportStudentsWithSubjects(IEnumerable<StudentClass> students, string filePath)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("StudentsWithSubjects");

        string[] headers = { "SerialNo", "StudentName", "Gender", "Religion", "BirthDay", "BirthMonth", "BirthYear", "Address", "NationalId", "Phone", "Email", "SubjectName", "Grade", "MaxGrade", "Teacher" };
        for (int c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];

        int row = 2;
            foreach (var s in students)
            {
                var enrolls = EnrollmentRepository.LoadByStudent(s.SerialNo);
                if (enrolls.Count == 0)
                {
                    WriteStudentRow(ws, row, s);
                    row++;
                }
                else
                {
                    foreach (var en in enrolls)
                    {
                        WriteStudentRow(ws, row, s);
                        var course = CourseRepository.GetById(en.SubjectId);
                        ws.Cell(row, 10).Value = course.HasValue ? course.Value.Name : $"(id:{en.SubjectId})";
                        ws.Cell(row, 11).Value = en.Grade;
                        ws.Cell(row, 12).Value = en.MaxGrade;
                        var teacherName = "";
                        if (course.HasValue && course.Value.TeacherId.HasValue)
                        {
                            var t = TeacherRepository.GetById(course.Value.TeacherId.Value);
                            if (t.HasValue) teacherName = t.Value.Name;
                        }
                        ws.Cell(row, 13).Value = teacherName;
                        row++;
                    }
                }
            }

        ws.Columns().AdjustToContents();
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        wb.SaveAs(filePath);
    }

    private static void WriteStudentRow(IXLWorksheet ws, int row, StudentClass s)
    {
        ws.Cell(row, 1).Value = s.SerialNo;
        ws.Cell(row, 2).Value = s.StudentName;
        ws.Cell(row, 3).Value = s.Gender;
        ws.Cell(row, 4).Value = s.Religion;
        ws.Cell(row, 5).Value = s.BirthDay;
        ws.Cell(row, 6).Value = s.BirthMonth;
        ws.Cell(row, 7).Value = s.BirthYear;
        ws.Cell(row, 8).Value = s.Address;
        ws.Cell(row, 9).Value = s.NationalId;
    }
}

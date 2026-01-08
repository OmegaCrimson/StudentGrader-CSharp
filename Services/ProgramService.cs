using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DB.Data;
using UI;

namespace Services
{
    public class ProgramService
    {
        // kept legacy constants but prefer localized messages via Localization.Get

        public static void Start()
        {

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            // ensure console uses UTF-8 so Arabic text prints correctly
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch { }
            // High-level flow: ensure DB exists, ensure users, login, run main loop
            var dbPath = DB.Data.DbPaths.GetDatabasePath();
            if (!File.Exists(dbPath))
            {
                Console.WriteLine(string.Format(Localization.Get("NoticeDatabaseNotFound"), dbPath));
                Console.Write(Localization.Get("PromptCreateDatabase"));
                var resp = Console.ReadLine()?.Trim().ToLower();
                if (resp == "y" || resp == "yes")
                {
                    DataBaseCreationHelper.CreateDB(fullPath: dbPath);
                    Console.WriteLine(string.Format(Localization.Get("InfoDatabaseCreated"), dbPath));
                }
                else
                {
                    LoggerService.LogInfo(Localization.Get("InfoExitingProgram"));
                    return;
                }
            }

            // Use the actual DB file directly (no encryption / temp decrypted DB)
            DB.Data.DbPaths.ActiveDatabasePath = dbPath;

            EnsureInitialAdminUser();

            var currentUser = LoginUser();
            if (currentUser == null) return;

            var options = BuildOptions(currentUser);

            RunMainLoop(options, currentUser);

            // Cleanup on exit (no encryption step)
            LoggerService.LogSuccess("Exiting...");
        }

        // NOTE: Admin-password / DB encryption flow removed.
        // The following method is retained but intentionally disabled to preserve
        // historical behavior for reference. If you need to re-enable an admin
        // password flow in the future, move logic from here into a dedicated
        // authentication manager.
        private static bool EnsureAdminPasswordExists()
        {
            // Encryption removed - always return true
            return true;
        }

        // DB decryption no longer used. Kept as a commented reference.
        /*
        private static string? PromptAndDecryptDatabase()
        {
            // previous implementation decrypted an encrypted DB into a temporary
            // file using an admin password. Encryption has been removed, so this
            // method is no longer used.
            return null;
        }
        */

        private static void EnsureInitialAdminUser()
        {
            if (UserRepository.GetAll().Count > 0) return;
            Console.WriteLine(Localization.Get("NoticeNoUsersFound"));
            // Hardcode username to 'Admin'
            var adminUser = "Admin";
            Console.WriteLine(string.Format(Localization.Get("NoticeCreatingAdminAccount"), adminUser));
            Console.Write(Localization.Get("PromptCreateAdminPassword"));
            var adminUserPass = Console.ReadLine() ?? "";
            var user = new Models.User
            {
                Username = adminUser,
                PasswordHash = Services.AuthService.HashPasswordForUser(adminUserPass),
                Role = "admin"
            };
            UserRepository.Add(user);
            LoggerService.LogSuccess("Admin user created.");
        }

        private static Models.User? LoginUser()
        {
            Models.User? currentUser = null;
            while (currentUser == null)
            {
                Console.Write("Username (leave empty for Admin): ");
                var u = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(u)) u = "Admin";
                Console.Write("Password: ");
                var p = Console.ReadLine() ?? "";
                var foundUser = UserRepository.GetByUsername(u);
                if (foundUser != null && Services.AuthService.VerifyPasswordForUser(p, foundUser.PasswordHash))
                {
                    currentUser = foundUser;
                    LoggerService.LogSuccess($"Logged in as {currentUser.Username} ({currentUser.Role}).");
                }
                else
                {
                    LoggerService.LogWarning("Invalid username or password. Try again.");
                }
            }
            return currentUser;
        }

        private static List<string> BuildOptions(Models.User currentUser)
        {
            var options = new List<string>
            {
                "Add Student",
                "View Student",
                "View All Students",
                "Edit Student",
                "Manage Student (Subjects)",
                "Delete Student",
                "Delete All Students",
                "Manage Courses",
                "Manage Teachers",
                "Manage Assignments",
                "Export",
                "Switch User",
                "Logout",
            };

            bool isAdmin = string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);
            if (isAdmin) options.Add("Admin Tools");

            options.Add("Exit Program");
            return options;
        }

        private static void RunMainLoop(List<string> options, Models.User currentUser)
        {
            bool running = true;
            bool isAdmin = string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);

            while (running)
            {
                // Apply header and localized title
                UIHelper.BodyWithHeader(Localization.Get("MainHeader"), options);
                Console.Write("Input: ");
                string? input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out var choice) || choice < 1 || choice > options.Count)
                {
                    LoggerService.LogWarning($"Invalid input. Try 1–{options.Count}.");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }

                var selected = options[choice - 1];
                switch (selected)
                {
                    case "Add Student":
                        var added = StudentInputService.CreateStudent();
                        StudentRepository.Add(added);
                        LoggerService.LogSuccess("Student added.");
                        break;

                    case "Manage Courses":
                        if (!isAdmin) { LoggerService.LogWarning("Not authorized."); break; }
                        bool managingCourses = true;
                        while (managingCourses)
                        {
                            Console.Clear();
                            Console.WriteLine("Courses:");
                            var courses = CourseRepository.GetAll();
                            foreach (var c in courses) Console.WriteLine($" - {c.Id}: {c.Name} (TeacherId: {c.TeacherId})");
                            Console.WriteLine("\nOptions: 1=Add Course, 2=Edit Course, 3=Delete Course, 4=Back");
                            Console.Write("Choice: ");
                            var ch = Console.ReadLine()?.Trim();
                            switch (ch)
                            {
                                case "1":
                                    Console.Write("Course name: "); var cn = Console.ReadLine()?.Trim() ?? ""; Console.Write("Description: "); var cd = Console.ReadLine()?.Trim() ?? ""; Console.Write("TeacherId (or empty): "); var tid = int.TryParse(Console.ReadLine(), out int tval) ? tval : (int?)null; CourseRepository.Add(cn, cd, tid); LoggerService.LogSuccess("Course added.");
                                    break;
                                case "2":
                                    Console.Write("Course id to edit: "); if (int.TryParse(Console.ReadLine(), out int editId)) {
                                        var existing = CourseRepository.GetById(editId);
                                        if (!existing.HasValue) { LoggerService.LogWarning("Course not found."); break; }
                                        Console.Write($"Name ({existing.Value.Name}): "); var newName = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newName)) newName = existing.Value.Name;
                                        Console.Write($"Description ({existing.Value.Description}): "); var newDesc = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newDesc)) newDesc = existing.Value.Description;
                                        Console.Write($"TeacherId ({existing.Value.TeacherId?.ToString() ?? "none"}): "); var newTid = int.TryParse(Console.ReadLine(), out int nt) ? (int?)nt : existing.Value.TeacherId;
                                        if (CourseRepository.Update(editId, newName, newDesc, newTid)) LoggerService.LogSuccess("Course updated."); else LoggerService.LogWarning("Update failed.");
                                    } else { LoggerService.LogWarning("Invalid id."); }
                                    break;
                                case "3": Console.Write("Course id to delete: "); if (int.TryParse(Console.ReadLine(), out int di)) { if (CourseRepository.Delete(di)) LoggerService.LogInfo("Course deleted."); else LoggerService.LogWarning("Course not found or delete failed."); } break;
                                case "4": managingCourses = false; break;
                                default: LoggerService.LogWarning("Invalid choice."); break;
                            }
                        }
                        break;

                    case "Manage Teachers":
                        if (!isAdmin) { LoggerService.LogWarning("Not authorized."); break; }
                        bool managingTeachers = true;
                        while (managingTeachers)
                        {
                            Console.Clear(); Console.WriteLine("Teachers:"); var teachers = TeacherRepository.GetAll(); foreach (var t in teachers) Console.WriteLine($" - {t.Id}: {t.Name} ({t.Email})"); Console.WriteLine("\nOptions: 1=Add Teacher, 2=Delete Teacher, 3=Back"); Console.Write("Choice: "); var tch = Console.ReadLine()?.Trim(); switch (tch)
                            {
                                case "1": Console.Write("Name: "); var tn = Console.ReadLine()?.Trim() ?? ""; Console.Write("Email: "); var te = Console.ReadLine()?.Trim() ?? ""; TeacherRepository.Add(tn, te); LoggerService.LogSuccess("Teacher added."); break;
                                case "2": Console.Write("Teacher id to delete: "); if (int.TryParse(Console.ReadLine(), out int tdid)) { if (TeacherRepository.Delete(tdid)) LoggerService.LogInfo("Teacher deleted."); else LoggerService.LogWarning("Delete failed."); } break;
                                case "3": managingTeachers = false; break;
                                default: LoggerService.LogWarning("Invalid choice."); break;
                            }
                        }
                        break;

                    case "Manage Assignments":
                        if (!isAdmin) { LoggerService.LogWarning("Not authorized."); break; }
                        bool managingAssign = true;
                        while (managingAssign)
                        {
                            Console.Clear();
                            Console.WriteLine("Assignments Manager:\n1=By Student, 2=All Assignments, 3=Back");
                            Console.Write("Choice: ");
                            var mgrChoice = Console.ReadLine()?.Trim();
                            switch (mgrChoice)
                            {
                                case "1":
                                    Console.Write("Enter student ID (or empty to back): ");
                                    var sidStr = Console.ReadLine()?.Trim();
                                    if (string.IsNullOrEmpty(sidStr)) break;
                                    if (!int.TryParse(sidStr, out int stId)) { LoggerService.LogWarning("Invalid ID."); break; }
                                    var assigns = AssignmentRepository.GetByStudent(stId);
                                    Console.WriteLine($"Assignments for student {stId}:");
                                    foreach (var a in assigns)
                                    {
                                        Console.WriteLine($" - {a.Id}: {a.Name} | Course:{a.CourseId?.ToString() ?? "-"} | Grade:{a.Grade?.ToString() ?? "-"}");
                                    }
                                    Console.WriteLine("Options: 1=Add, 2=Edit, 3=Delete, 4=Back"); Console.Write("Choice: "); var ach = Console.ReadLine()?.Trim();
                                    switch (ach)
                                    {
                                        case "1":
                                            Console.Write("Name: "); var an = Console.ReadLine()?.Trim() ?? "";
                                            Console.Write("CourseId (or empty): "); var ac = int.TryParse(Console.ReadLine(), out int acv) ? acv : (int?)null;
                                            Console.Write("Type (or empty): "); var at = Console.ReadLine()?.Trim();
                                            Console.Write("Description (or empty): "); var ad = Console.ReadLine()?.Trim();
                                            Console.Write("MaxGrade (or empty=100): "); var am = int.TryParse(Console.ReadLine(), out int amv) ? amv : 100;
                                            Console.Write("Grade (or empty): "); var ag = double.TryParse(Console.ReadLine(), out double agv) ? (double?)agv : null;
                                            Console.Write("DateAssigned (ISO or empty): "); var daa = Console.ReadLine()?.Trim();
                                            Console.Write("DateDue (ISO or empty): "); var dad = Console.ReadLine()?.Trim();
                                            Console.Write("TeacherId (or empty): "); var ati = int.TryParse(Console.ReadLine(), out int tiv) ? (int?)tiv : null;
                                            AssignmentRepository.Add(stId, ac, an, at, ad, am, ag, daa, dad, null, ati);
                                            LoggerService.LogSuccess("Assignment added.");
                                            break;
                                        case "2":
                                            Console.Write("Assignment id to edit: ");
                                            if (!int.TryParse(Console.ReadLine(), out int editAssignId)) { LoggerService.LogWarning("Invalid id."); break; }
                                            var assign = AssignmentRepository.GetById(editAssignId);
                                            if (assign == null) { LoggerService.LogWarning("Assignment not found."); break; }
                                            // Edit fields
                                            Console.Write($"Name ({assign.Value.Name}): "); var newName = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newName)) newName = assign.Value.Name;
                                            Console.Write($"CourseId ({assign.Value.CourseId?.ToString() ?? "none"} or new name): "); var courseInput = Console.ReadLine()?.Trim() ?? "";
                                            int? newCourseId = assign.Value.CourseId;
                                            if (!string.IsNullOrWhiteSpace(courseInput))
                                            {
                                                if (int.TryParse(courseInput, out int parsedCourse)) newCourseId = parsedCourse;
                                                else
                                                {
                                                    Console.Write("Description for new course (or empty): "); var newCourseDesc = Console.ReadLine()?.Trim(); Console.Write("TeacherId for new course (or empty): "); var newCourseTid = int.TryParse(Console.ReadLine(), out int nct) ? (int?)nct : null;
                                                    newCourseId = CourseRepository.Add(courseInput, newCourseDesc, newCourseTid);
                                                }
                                            }
                                            Console.Write($"Type ({assign.Value.Type ?? ""}): "); var newType = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newType)) newType = assign.Value.Type;
                                            Console.Write($"Description ({assign.Value.Description ?? ""}): "); var newDesc = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newDesc)) newDesc = assign.Value.Description;
                                            Console.Write($"MaxGrade ({assign.Value.MaxGrade}): "); var newMaxStr = Console.ReadLine()?.Trim(); int newMax = string.IsNullOrWhiteSpace(newMaxStr) ? assign.Value.MaxGrade : (int.TryParse(newMaxStr, out int nmv) ? nmv : assign.Value.MaxGrade);
                                            Console.Write($"Grade ({assign.Value.Grade?.ToString() ?? ""}): "); var newGradeStr = Console.ReadLine()?.Trim(); double? newGrade = string.IsNullOrWhiteSpace(newGradeStr) ? assign.Value.Grade : (double.TryParse(newGradeStr, out double ngv) ? (double?)ngv : assign.Value.Grade);
                                            Console.Write($"DateAssigned ({assign.Value.DateAssigned ?? ""}): "); var newDa = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newDa)) newDa = assign.Value.DateAssigned;
                                            Console.Write($"DateDue ({assign.Value.DateDue ?? ""}): "); var newDd = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newDd)) newDd = assign.Value.DateDue;
                                            Console.Write($"DateSubmitted ({assign.Value.DateSubmitted ?? ""}): "); var newDs = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(newDs)) newDs = assign.Value.DateSubmitted;
                                            Console.Write($"TeacherId ({assign.Value.TeacherId?.ToString() ?? "none"}): "); var newTiStr = Console.ReadLine()?.Trim(); int? newTi = string.IsNullOrWhiteSpace(newTiStr) ? assign.Value.TeacherId : (int.TryParse(newTiStr, out int ntid) ? (int?)ntid : assign.Value.TeacherId);
                                            if (AssignmentRepository.Update(editAssignId, newCourseId, newName, newType, newDesc, newMax, newGrade, newDa, newDd, newDs, newTi)) LoggerService.LogSuccess("Assignment updated."); else LoggerService.LogWarning("Update failed.");
                                            break;
                                        case "3": Console.Write("Assignment id to delete: "); if (int.TryParse(Console.ReadLine(), out int aid2)) { if (AssignmentRepository.Delete(aid2)) LoggerService.LogInfo("Assignment deleted."); else LoggerService.LogWarning("Delete failed."); } break;
                                        default: break;
                                    }
                                    break;
                                case "2":
                                    // List all assignments across all students
                                    var allAssignments = AssignmentRepository.GetAll();
                                    if (allAssignments == null || allAssignments.Count == 0)
                                    {
                                        LoggerService.LogInfo(Localization.Get("NoAssignments"));
                                        Console.WriteLine(Localization.Get("PressAnyKeyToContinue"));
                                        Console.ReadKey(true);
                                        break;
                                    }
                                    Console.WriteLine("All Assignments:");
                                    foreach (var a in allAssignments)
                                    {
                                        // a: (Id, StudentSerialNo, CourseId, Name, Type, Grade, MaxGrade)
                                        string studentName = "(unknown)";
                                        try
                                        {
                                            var s = StudentRepository.LoadById(a.StudentSerialNo);
                                            if (s != null) studentName = s.StudentName;
                                            else studentName = $"(id:{a.StudentSerialNo})";
                                        }
                                        catch { studentName = $"(id:{a.StudentSerialNo})"; }

                                        string courseName = a.CourseId.HasValue ? $"(id:{a.CourseId.Value})" : "-";
                                        if (a.CourseId.HasValue)
                                        {
                                            var c = CourseRepository.GetById(a.CourseId.Value);
                                            if (c.HasValue) courseName = c.Value.Name;
                                        }

                                        Console.WriteLine($" - {a.Id}: {a.Name} | Student: {studentName} | Course: {courseName} | Grade: {(a.Grade.HasValue ? a.Grade.Value.ToString() : "-")}/{a.MaxGrade}");
                                    }
                                    Console.WriteLine();
                                    Console.WriteLine(Localization.Get("PressAnyKeyToContinue"));
                                    Console.ReadKey(true);
                                    break;
                                case "3": managingAssign = false; break;
                                default: LoggerService.LogWarning("Invalid choice."); break;
                            }
                        }
                        break;

                    case "View Student":
                        Console.Write("Enter student name: ");
                        var sname = Console.ReadLine()?.Trim();
                        var single = StudentRepository.LoadAll().FirstOrDefault(s => s.StudentName.Equals(sname, StringComparison.OrdinalIgnoreCase));
                        if (single != null) StudentPrinterService.PrintStudent(single); else LoggerService.LogWarning("Student not found.");
                        break;

                    case "View All Students":
                        var list = StudentRepository.LoadAll();
                        if (list.Count == 0) LoggerService.LogInfo("No students to display."); else foreach (var s in list) StudentPrinterService.PrintStudent(s);
                        break;

                    case "Edit Student":
                        Console.Write("Enter student ID to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int eid))
                        {
                            var st = StudentRepository.LoadById(eid);
                            if (st == null) { LoggerService.LogWarning("Student not found."); break; }
                            Console.WriteLine("Editing student. Leave field empty to keep current value.");
                            Console.Write($"Name ({st.StudentName}): "); var nn = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(nn)) st.StudentName = nn;
                            Console.Write($"Gender ({st.Gender}): "); var ng = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(ng)) st.Gender = ng;
                            Console.Write($"Religion ({st.Religion}): "); var nr = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(nr)) st.Religion = nr;
                            Console.Write($"Birth Day ({st.BirthDay}): "); if (int.TryParse(Console.ReadLine(), out int bd)) st.BirthDay = bd;
                            Console.Write($"Birth Month ({st.BirthMonth}): "); if (int.TryParse(Console.ReadLine(), out int bm)) st.BirthMonth = bm;
                            Console.Write($"Birth Year ({st.BirthYear}): "); if (int.TryParse(Console.ReadLine(), out int by)) st.BirthYear = by;
                            Console.Write($"Address ({st.Address}): "); var na = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(na)) st.Address = na;
                            Console.Write($"National ID ({st.NationalId}): "); var nid = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(nid)) st.NationalId = nid;
                            StudentRepository.Update(st);
                            LoggerService.LogSuccess("Student updated.");
                        }
                        else { LoggerService.LogWarning("Invalid ID."); }
                        break;

                    case "Manage Student (Subjects)":
                        Console.Write("Enter student ID to manage subjects: ");
                        if (int.TryParse(Console.ReadLine(), out int msid))
                        {
                            var student = StudentRepository.LoadById(msid);
                            if (student == null) { LoggerService.LogWarning("Student not found."); break; }
                            bool managing = true;
                            while (managing)
                            {
                                Console.Clear();
                                StudentPrinterService.PrintStudent(student);
                                Console.WriteLine("Subjects:");
                                var enrolls = EnrollmentRepository.LoadByStudent(student.SerialNo);
                                if (enrolls.Count == 0)
                                    Console.WriteLine("   (No subjects)");
                                else
                                {
                                    int idx = 1;
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
                                        Console.WriteLine($"   {idx}. {courseName}: { (en.Grade.HasValue ? en.Grade.Value.ToString() : "-") }/{en.MaxGrade} (Teacher: {teacherName}) [enrollmentId:{en.Id}]");
                                        idx++;
                                    }
                                }
                                Console.WriteLine("\nOptions: 1=Add Subject, 2=Edit Enrollment, 3=Delete Enrollment, 4=Clear Subjects, 5=Back");
                                Console.Write("Choice: ");
                                var subChoice = Console.ReadLine()?.Trim();
                                switch (subChoice)
                                {
                                    case "1":
                                        // Present available courses to link
                                        var courses = CourseRepository.GetAll();
                                        Console.WriteLine("Available Courses:");
                                        foreach (var c in courses) Console.WriteLine($" {c.Id}: {c.Name}");
                                        Console.Write("Course ID to add (or new name): "); var cidOrName = Console.ReadLine()?.Trim() ?? "";
                                        int subjectId;
                                        if (int.TryParse(cidOrName, out int parsedId))
                                        {
                                            subjectId = parsedId;
                                        }
                                        else
                                        {
                                            Console.Write("Description: "); var desc = Console.ReadLine()?.Trim(); Console.Write("TeacherId (or empty): "); var tid = int.TryParse(Console.ReadLine(), out int tval) ? (int?)tval : null;
                                            subjectId = CourseRepository.Add(cidOrName, desc, tid);
                                        }
                                        Console.Write("Score: "); double grade = double.TryParse(Console.ReadLine(), out double g) ? g : 0; Console.Write("Max Score: "); int max = int.TryParse(Console.ReadLine(), out int m) ? m : 100;
                                        // If chosen subject id does not exist, create new course or treat as null
                                        var chosenCourse = CourseRepository.GetById(subjectId);
                                        if (subjectId <= 0 || (!chosenCourse.HasValue && subjectId != 0))
                                        {
                                            LoggerService.LogWarning("Selected course not found. Enrollment aborted.");
                                            break;
                                        }
                                        EnrollmentRepository.Add(student.SerialNo, subjectId, grade, max);
                                        LoggerService.LogSuccess("Subject (course) added to student.");
                                        break;
                                    case "2":
                                        Console.Write("Enter enrollment Id to edit: ");
                                        if (!int.TryParse(Console.ReadLine(), out int enrollmentId)) { LoggerService.LogWarning("Invalid id."); break; }
                                        var enToEdit = enrolls.Find(e => e.Id == enrollmentId);
                                        if (enToEdit.Id == 0) { LoggerService.LogWarning("Enrollment not found."); break; }
                                        Console.Write($"Current Score ({enToEdit.Grade?.ToString() ?? "-"}): "); var newScoreStr = Console.ReadLine()?.Trim(); double? newScore = string.IsNullOrWhiteSpace(newScoreStr) ? enToEdit.Grade : (double.TryParse(newScoreStr, out double ns) ? (double?)ns : enToEdit.Grade);
                                        Console.Write($"Current Max ({enToEdit.MaxGrade}): "); var newMaxStr = Console.ReadLine()?.Trim(); int newMax = string.IsNullOrWhiteSpace(newMaxStr) ? enToEdit.MaxGrade : (int.TryParse(newMaxStr, out int nm) ? nm : enToEdit.MaxGrade);
                                        if (EnrollmentRepository.Update(enrollmentId, enToEdit.SubjectId, newScore, newMax)) LoggerService.LogSuccess("Enrollment updated."); else LoggerService.LogWarning("Update failed.");
                                        break;
                                    case "3":
                                        Console.Write("Enter enrollment Id to delete: ");
                                        if (!int.TryParse(Console.ReadLine(), out int enrollmentDeleteId)) { LoggerService.LogWarning("Invalid id."); break; }
                                        if (EnrollmentRepository.Delete(enrollmentDeleteId)) LoggerService.LogInfo("Enrollment deleted."); else LoggerService.LogWarning("Delete failed.");
                                        break;
                                    case "4": SubjectRepository.DeleteAllForStudent(student.SerialNo); LoggerService.LogInfo("All subjects cleared."); break;
                                    case "5": managing = false; break;
                                    default: LoggerService.LogWarning("Invalid choice."); break;
                                }
                            }
                        }
                        else { LoggerService.LogWarning("Invalid ID."); }
                        break;

                    case "Delete Student":
                        Console.Write("Enter student name to delete: "); var td = Console.ReadLine()?.Trim(); var deleted = StudentRepository.DeleteByName(td ?? "none"); LoggerService.LogInfo(deleted ? "Student deleted." : "Student not found.");
                        break;

                    case "Delete All Students":
                        Console.Write("Are you sure you want to delete all students? (y/n): "); if (Console.ReadLine()?.Trim().ToLower() == "y") { StudentRepository.DeleteAll(); LoggerService.LogInfo("All students deleted."); }
                        break;

                    case "Export":
                        bool inExport = true;
                        while (inExport)
                        {
                            Console.Clear();
                            Console.WriteLine(Localization.Get("ExportOptionsTitle"));
                            Console.Write("Choice: ");
                            var ex = Console.ReadLine()?.Trim();
                            switch (ex)
                            {
                                case "1":
                                    var allStudents = StudentRepository.LoadAll(); ExcelExportHelper.ExportStudentsFormatted(allStudents, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students.xlsx")); LoggerService.LogSuccess(Localization.Get("UserAdded"));
                                    break;
                                case "2":
                                    var allWithSubjects = StudentRepository.LoadAll(); ExcelExportHelper.ExportStudentsWithSubjects(allWithSubjects, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "StudentsWithSubjects.xlsx")); LoggerService.LogSuccess(Localization.Get("UserAdded"));
                                    break;
                                case "3":
                                    var allStudentsPdf = StudentRepository.LoadAll(); PdfExportHelper.ExportStudents(allStudentsPdf, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students.pdf"), includeSubjects: false); LoggerService.LogSuccess(Localization.Get("UserAdded"));
                                    break;
                                case "4":
                                    var allStudentsWithSubjectsPdf = StudentRepository.LoadAll(); PdfExportHelper.ExportStudents(allStudentsWithSubjectsPdf, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "StudentsWithSubjects.pdf"), includeSubjects: true); LoggerService.LogSuccess(Localization.Get("UserAdded"));
                                    break;
                                case "5":
                                    inExport = false;
                                    break;
                                default:
                                    LoggerService.LogWarning(Localization.Get("InvalidInput"));
                                    break;
                            }
                            if (inExport)
                            {
                                Console.WriteLine(Localization.Get("PressAnyKeyToContinue"));
                                Console.ReadKey(true);
                            }
                        }
                        break;

                    case "Admin Tools":
                        if (!isAdmin) { LoggerService.LogWarning("Not authorized."); break; }
                        bool inAdmin = true;
                        while (inAdmin)
                        {
                            Console.Clear();
                            Console.WriteLine(Localization.Get("AdminToolsTitle"));
                            Console.Write(Localization.Get("ChoicePrompt"));
                            var a = Console.ReadLine()?.Trim();
                            switch (a)
                            {
                                case "1":
                                    Console.Write(Localization.Get("BackupPrompt"));
                                    var destPath = Console.ReadLine()?.Trim();
                                    try
                                    {
                                        if (string.IsNullOrWhiteSpace(destPath))
                                        {
                                            // default backup location in user's Documents
                                            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                            var backupDir = System.IO.Path.Combine(docs, "StudentGraderBackups");
                                            Directory.CreateDirectory(backupDir);
                                            var dest = System.IO.Path.Combine(backupDir, $"students_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                                            DB.Data.DataBaseBackupHelper.BackupDatabase(dest);
                                            LoggerService.LogSuccess(string.Format(Localization.Get("BackupSaved"), dest));
                                        }
                                        else
                                        {
                                            DB.Data.DataBaseBackupHelper.BackupDatabase(destPath);
                                            LoggerService.LogSuccess(string.Format(Localization.Get("BackupSaved"), destPath));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerService.LogWarning(string.Format(Localization.Get("BackupFailed"), ex.Message));
                                    }
                                    break;
                                case "2":
                                    Console.Write(Localization.Get("PromptCreateDatabase"));
                                    Console.Write("Are you sure you want to delete DB? (y/n): ");
                                    if (Console.ReadLine()?.Trim().ToLower() == "y")
                                    {
                                        DataBaseDeletionHelper.DeleteDB();
                                        LoggerService.LogInfo(Localization.Get("InfoDatabaseDeleted"));
                                        try { if (File.Exists(DB.Data.DbPaths.ActiveDatabasePath ?? "")) File.Delete(DB.Data.DbPaths.ActiveDatabasePath); } catch { }
                                        DB.Data.DbPaths.ActiveDatabasePath = null;

                                        // After deletion prompt to create new DB or exit
                                        var dbPath = DB.Data.DbPaths.GetDatabasePath();
                                        Console.Write(Localization.Get("PromptCreateDatabaseAfterDeletion"));
                                        var createResp = Console.ReadLine()?.Trim().ToLower();
                                        if (createResp == "y" || createResp == "yes")
                                        {
                                            DataBaseCreationHelper.CreateDB(fullPath: dbPath);
                                            Console.WriteLine(string.Format(Localization.Get("InfoDatabaseCreated"), dbPath));
                                            DB.Data.DbPaths.ActiveDatabasePath = dbPath;
                                            // Ensure admin user and force relogin
                                            EnsureInitialAdminUser();
                                            var newCurrent = LoginUser();
                                            if (newCurrent == null) return;
                                            currentUser = newCurrent;
                                            isAdmin = string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);
                                            options = BuildOptions(currentUser);
                                            inAdmin = false; // leave admin tools
                                        }
                                        else
                                        {
                                            LoggerService.LogInfo(Localization.Get("InfoExitingProgram"));
                                            return;
                                        }
                                    }
                                    break;
                                case "3":
                                    Console.Write("Are you sure you want to reset DB? (y/n): "); if (Console.ReadLine()?.Trim().ToLower() == "y") { DataBaseDeletionHelper.DeleteDB(); LoggerService.LogInfo("DB deleted."); try { if (File.Exists(DB.Data.DbPaths.ActiveDatabasePath ?? "")) File.Delete(DB.Data.DbPaths.ActiveDatabasePath); } catch { } var dbNew = DB.Data.DbPaths.GetDatabasePath(); DataBaseCreationHelper.CreateDB(fullPath: dbNew); DB.Data.DbPaths.ActiveDatabasePath = dbNew; LoggerService.LogSuccess("DB reset."); }
                                    break;

                    // "Forgot Password" functionality removed because DB encryption
                    // logic has been removed. If you need to reset authentication
                    // state, implement a deliberate admin-only flow in AuthService.

                                case "4":
                                    bool managingUsersLoop = true;
                                    while (managingUsersLoop)
                                    {
                                        Console.Clear(); Console.WriteLine(Localization.Get("UsersTitle")); var users = UserRepository.GetAll(); foreach (var us in users) Console.WriteLine($" - {us.Username} ({us.Role})"); Console.WriteLine("\n" + Localization.Get("UserOptions")); Console.Write(Localization.Get("EnterUsername")); var ch = Console.ReadLine()?.Trim(); switch (ch)
                                        {
                                            case "1":
                                                Console.Write(Localization.Get("EnterUsername")); var nu = Console.ReadLine()?.Trim() ?? "";
                                                if (string.Equals(nu, "Admin", StringComparison.OrdinalIgnoreCase)) { LoggerService.LogWarning(Localization.Get("CannotCreateAdminUser")); break; }
                                                Console.Write(Localization.Get("EnterPassword")); var npw = Console.ReadLine() ?? ""; Console.Write(Localization.Get("EnterRole")); var role = Console.ReadLine()?.Trim() ?? "user"; var addedUser = new Models.User { Username = nu, PasswordHash = Services.AuthService.HashPasswordForUser(npw), Role = role };
                                                try { UserRepository.Add(addedUser); LoggerService.LogSuccess(Localization.Get("UserAdded")); } catch (Exception ex) { LoggerService.LogWarning(string.Format(Localization.Get("BackupFailed"), ex.Message)); }
                                                break;
                                            case "2":
                                                Console.Write(Localization.Get("EnterUsername")); var du = Console.ReadLine()?.Trim() ?? "";
                                                if (string.IsNullOrWhiteSpace(du)) { LoggerService.LogWarning(Localization.Get("InvalidInput")); break; }
                                                if (string.Equals(du, "Admin", StringComparison.OrdinalIgnoreCase)) { LoggerService.LogWarning(Localization.Get("CannotDeleteAdminUser")); break; }
                                                if (UserRepository.DeleteByUsername(du)) LoggerService.LogInfo(Localization.Get("UserDeleted")); else LoggerService.LogWarning(Localization.Get("InvalidInput"));
                                                break;
                                            case "3":
                                                managingUsersLoop = false; break;
                                            default: LoggerService.LogWarning("Invalid choice."); break;
                                        }
                                    }
                                    break;

                                case "5":
                                    // Change language
                                    Console.WriteLine(Localization.Get("LanguageMenuTitle"));
                                    var ln = Console.ReadLine()?.Trim();
                                    if (ln == "1")
                                    {
                                        Localization.SetLanguage(Localization.Language.En);
                                        Console.WriteLine(string.Format(Localization.Get("LanguageChanged"), "English"));
                                    }
                                    else if (ln == "2")
                                    {
                                        Localization.SetLanguage(Localization.Language.Ar);
                                        Console.WriteLine(string.Format(Localization.Get("LanguageChanged"), "Arabic"));
                                    }
                                    else
                                    {
                                        LoggerService.LogWarning(Localization.Get("InvalidInput"));
                                    }
                                    break;
                                case "6":
                                    // Change theme
                                    Console.WriteLine(Localization.Get("ThemeMenuTitle"));
                                    var th = Console.ReadLine()?.Trim();
                                    if (th == "1") { ThemeManager.Apply(ThemeManager.Theme.Default); Console.WriteLine(string.Format(Localization.Get("ThemeChanged"), "Default")); }
                                    else if (th == "2") { ThemeManager.Apply(ThemeManager.Theme.Dark); Console.WriteLine(string.Format(Localization.Get("ThemeChanged"), "Dark")); }
                                    else if (th == "3") { ThemeManager.Apply(ThemeManager.Theme.HighContrast); Console.WriteLine(string.Format(Localization.Get("ThemeChanged"), "HighContrast")); }
                                    else { LoggerService.LogWarning(Localization.Get("InvalidInput")); }
                                    break;
                                case "7":
                                    inAdmin = false;
                                    break;
                                default:
                                    LoggerService.LogWarning("Invalid choice.");
                                    break;
                            }
                            if (inAdmin)
                            {
                                Console.WriteLine("\nPress any key to continue...");
                                Console.ReadKey(true);
                            }
                        }
                        break;

                    // Admin password / change logic removed.

                    case "Switch User":
                        // Prompt for new login and swap current user
                        var newUser = LoginUser();
                        if (newUser == null) { LoggerService.LogWarning("Failed to switch user."); break; }
                        currentUser = newUser;
                        isAdmin = string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);
                        options = BuildOptions(currentUser);
                        LoggerService.LogSuccess($"Switched to {currentUser.Username} ({currentUser.Role}).");
                        break;

                    case "Logout":
                        LoggerService.LogInfo("Logging out...");
                        var reUser = LoginUser();
                        if (reUser == null) { running = false; break; }
                        currentUser = reUser;
                        isAdmin = string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);
                        options = BuildOptions(currentUser);
                        break;

                    case "Exit Program":
                        running = false;
                        break;

                    default:
                        LoggerService.LogWarning($"Unhandled action: {selected}");
                        break;
                }

                // Pause after action
                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                }
            }
        }

        // Encryption cleanup removed. Kept as reference.
        /*
        private static void EncryptAndCleanupOnExit(string adminPassword)
        {
            try
            {
                var active = DB.Data.DbPaths.ActiveDatabasePath;
                if (!string.IsNullOrEmpty(active) && File.Exists(active))
                {
                    Services.AuthService.EncryptFileTo(active, DB.Data.DbPaths.GetDatabasePath(), adminPassword);
                    File.Delete(active);
                    DB.Data.DbPaths.ActiveDatabasePath = null;
                }
            }
            catch { }
            LoggerService.LogSuccess("Exiting...");
        }
        */
    }
}

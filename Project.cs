/*
 * C# Foundational — Student Grading App
 * مشروع تقييم الطلاب — أساسيات C#
 *
 * Author / المؤلف: Mohamed Gonem / محمد غنيم
 * Version / الإصدار: 1.4
 * Copyright © 2025 Mohamed Gonem
 *
 * License / الرخصة: MIT License
 * You may use, modify, and distribute this software freely,
 * but please give credit to the original author: Mohamed Gonem / محمد غنيم
 *
 * Description:
 * This is a console-based student grading system supporting English and Arabic digits.
 * It allows multi-subject grade entry, GPA and average calculation,
 * letter grades, exporting reports, and simple menu-driven navigation.
 *
 * وصف البرنامج:
 * نظام تقييم الطلاب على شكل برنامج كونسول يدعم الأرقام الإنجليزية والعربية.
 * يتيح إدخال درجات لعدة مواد، حساب المعدل التراكمي (GPA) والمتوسط،
 * عرض التقديرات الحرفية، تصدير التقارير، وقائمة تنقل بسيطة.
 */


using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project
{

    public static class Keys
    {
        public const char Yes = 'y';
        public const char YesUpper = 'Y';
        public const char YesAr = 'ن';
        public const char No = 'n';
        public const char NoUpper = 'N';
        public const char NoAr = 'ل';

        public const char Digit0 = '0';
        public const char Digit1 = '1';
        public const char Digit2 = '2';
        public const char Digit3 = '3';
        public const char Digit4 = '4';
        public const char Digit5 = '5';
        public const char Digit6 = '6';
        public const char Digit7 = '7';
        public const char Digit8 = '8';
        public const char Digit9 = '9';

        public const char Digit0Ar = '٠';
        public const char Digit1Ar = '١';
        public const char Digit2Ar = '٢';
        public const char Digit3Ar = '٣';
        public const char Digit4Ar = '٤';
        public const char Digit5Ar = '٥';
        public const char Digit6Ar = '٦';
        public const char Digit7Ar = '٧';
        public const char Digit8Ar = '٨';
        public const char Digit9Ar = '٩';

        public static bool IsYes(char c)
        {
            return c == Yes || c == YesUpper || c == YesAr;
        }

        public static bool IsNo(char c)
        {
            return c == No || c == NoUpper || c == NoAr;
        }
    }

    public static class Converter
    {
        public static string NormalizeDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input
                .Replace(Keys.Digit0Ar, Keys.Digit0)
                .Replace(Keys.Digit1Ar, Keys.Digit1)
                .Replace(Keys.Digit2Ar, Keys.Digit2)
                .Replace(Keys.Digit3Ar, Keys.Digit3)
                .Replace(Keys.Digit4Ar, Keys.Digit4)
                .Replace(Keys.Digit5Ar, Keys.Digit5)
                .Replace(Keys.Digit6Ar, Keys.Digit6)
                .Replace(Keys.Digit7Ar, Keys.Digit7)
                .Replace(Keys.Digit8Ar, Keys.Digit8)
                .Replace(Keys.Digit9Ar, Keys.Digit9);
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double GradeOne { get; set; }
        public double GradeTwo { get; set; }
        public double GradeThree { get; set; }
        public double GradeFour { get; set; }
        public double GradeFive { get; set; }

        public double Average
        {
            get
            {
                return (GradeOne + GradeTwo + GradeThree + GradeFour + GradeFive) / 5.0;
            }
        }

        public double GPA
        {
            get
            {
                double avg = Average;
                if (avg >= 90) return 4.0;
                if (avg >= 85) return 3.7;
                if (avg >= 80) return 3.3;
                if (avg >= 75) return 3.0;
                if (avg >= 70) return 2.7;
                if (avg >= 65) return 2.3;
                if (avg >= 60) return 2.0;
                if (avg >= 50) return 1.0;
                return 0.0;
            }
        }

        public string LetterGrade
        {
            get
            {
                double avg = Average;
                if (avg >= 95) return "A+";
                if (avg >= 90) return "A";
                if (avg >= 85) return "A-";
                if (avg >= 80) return "B+";
                if (avg >= 75) return "B";
                if (avg >= 70) return "B-";
                if (avg >= 65) return "C+";
                if (avg >= 60) return "C";
                if (avg >= 50) return "D";
                return "F";
            }
        }
    }

    public static class Localization
    {
        public enum Language { En, Ar }
        public static Language CurrentLanguage = Language.En;

        public static string MenuStart => CurrentLanguage == Language.En ? "Start Program" : "بدء البرنامج";
        public static string MenuReadMe => CurrentLanguage == Language.En ? "About/Read me" : "حول البرنامج / اقرأني";
        public static string MenuChangeLanguage => CurrentLanguage == Language.En ? "Change Language" : "تغيير اللغة";
        public static string MenuExit => CurrentLanguage == Language.En ? "Exit" : "الخروج";

        public static string InvalidInput => CurrentLanguage == Language.En ? "Invalid Input" : "إدخال غير صحيح";
        public static string InvalidChoice => CurrentLanguage == Language.En ? "Invalid Choice!!" : "اختيار غير صحيح";

        public static string ConfirmExit => CurrentLanguage == Language.En ? "Confirm Exit Program" : "تأكيد الخروج من البرنامج";
        public static string PressYesNo => CurrentLanguage == Language.En ? "Press (Y)es or (N)o" : "اضغط (ن)ـعم أو (ل)ـا";

        public static string AddRecord => CurrentLanguage == Language.En ? "Add student record" : "إضافة سجل طالب";
        public static string EditRecord => CurrentLanguage == Language.En ? "Edit student record" : "تعديل سجل طالب";
        public static string DeleteRecord => CurrentLanguage == Language.En ? "Delete student record" : "حذف سجل طالب";
        public static string ViewRecord => CurrentLanguage == Language.En ? "View one record report" : "عرض تقرير طالب واحد";
        public static string ViewExportAll => CurrentLanguage == Language.En ? "View/Export All records report" : "عرض/تصدير تقرير جميع السجلات";
        public static string ResetData => CurrentLanguage == Language.En ? "Reset Data" : "إعادة ضبط البيانات";
        public static string ResetDataPrebuilt => CurrentLanguage == Language.En ? "Reset Data (Use Pre-Built)" : "إعادة ضبط البيانات (استخدام سجلات جاهزة)";
        public static string Settings => CurrentLanguage == Language.En ? "Settings" : "الإعدادات";
        public static string BackToMenu => CurrentLanguage == Language.En ? "Back to programs menu" : "العودة إلى القائمة الرئيسية";

        public static string EnterStudentName => CurrentLanguage == Language.En ? "Enter Student Name:" : "أدخل اسم الطالب:";
        public static string RecordAdded => CurrentLanguage == Language.En ? "Record Added!" : "تم إضافة السجل!";
        public static string RecordUpdated => CurrentLanguage == Language.En ? "Record Updated!" : "تم تحديث السجل!";
        public static string RecordDeleted => CurrentLanguage == Language.En ? "Record Deleted" : "تم حذف السجل";
        public static string NoRecordsFound => CurrentLanguage == Language.En ? "No records found!!" : "لا توجد سجلات للعرض!!";
        public static string InvalidID => CurrentLanguage == Language.En ? "Invalid ID" : "معرّف غير صالح";
        public static string RecordNotFound => CurrentLanguage == Language.En ? "Record not found" : "السجل غير موجود";
        public static string EnterID => CurrentLanguage == Language.En ? "Enter ID:" : "أدخل المعرّف:";
        public static string EnterGrade(string subject) => CurrentLanguage == Language.En ? $"Enter Student [{subject}] Grade:" : $"أدخل درجة الطالب [{subject}]:";
        public static string InvalidValue => CurrentLanguage == Language.En ? "Invalid Value!! 0 - 100" : "قيمة غير صالحة!! 0 - 100";
        public static string InvalidNumberInput => CurrentLanguage == Language.En ? "Invalid Input!! Only Numbers" : "إدخال غير صالح!! الأرقام فقط";
        public static string PressYToViewNow => CurrentLanguage == Language.En ? "Press (Y) to view now or press any key to continue!" : "اضغط (ن) للعرض الآن أو أي مفتاح للمتابعة!";
        public static string SettingsNotImplemented => CurrentLanguage == Language.En ? "Settings not implemented yet" : "الإعدادات لم تُنفذ بعد";
        public static string FileSaved(string path) => CurrentLanguage == Language.En ? $"File saved as [{path}]" : $"تم حفظ الملف باسم [{path}]";
        public static string PressAnyKey => CurrentLanguage == Language.En ? "Press Any Key To Continue." : "اضغط أي مفتاح للإستمرار.";

        public static string[] ReadMeSections
        {
            get
            {
                if (CurrentLanguage == Language.En)
                {
                    return new string[]
                    {
                        "# C# Foundational — Student Grading App\n\nA console-based student grading system.",
                        "## Features\n- Multi-subject grade entry\n- GPA and average calculation\n- Accepts English (0–9) and Arabic (٠–٩) digits\n- Export reports\n- Simple menu-driven navigation",
                        "## How to Use\nRun the app and select menu items by number. Enter grades when prompted."
                    };
                }
                else
                {
                    return new string[]
                    {
                        "# مشروع تقييم الطلاب — أساسيات C#\n\nنظام تقييم الطلاب على شكل برنامج كونسول.",
                        "## الميزات\n- إدخال الدرجات لعدة مواد\n- حساب المتوسط وGPA\n- يدعم الأرقام الإنجليزية والعربية\n- تصدير التقارير\n- قائمة تنقل بسيطة",
                        "## كيفية الاستخدام\nشغّل البرنامج وحدد عناصر القائمة بالأرقام. أدخل الدرجات عند الطلب."
                    };
                }
            }
        }
    }

    public static class UI
    {
        public static int SafeWidth
        {
            get
            {
                try
                {
                    int w = Console.WindowWidth;
                    if (w < 20) w = 20;
                    if (w > 120) w = 120;
                    return w;
                }
                catch
                {
                    return 80;
                }
            }
        }

        private static bool IsAllowedColor(ConsoleColor color, ConsoleColor[] allowed)
        {
            foreach (var c in allowed)
                if (c == color) return true;
            return false;
        }

        public static void Divider(char c = '─', ConsoleColor color = ConsoleColor.White)
        {
            var allowedColors = new[] { ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.DarkGray, ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green };
            if (!IsAllowedColor(color, allowedColors))
                color = ConsoleColor.White;

            Console.ForegroundColor = color;
            Console.WriteLine(new string(c, Math.Min(SafeWidth, 60)));
            Console.ResetColor();
        }

        public static void FontColor(string text, ConsoleColor color)
        {
            var allowedColors = new[] { ConsoleColor.White, ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow };
            if (!IsAllowedColor(color, allowedColors))
                color = ConsoleColor.White;

            if (text.Length < 20)
                text = text.PadRight(20);

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Header(string text, ConsoleColor borderColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.Red)
        {
            if (!IsAllowedColor(borderColor, new[] { ConsoleColor.White, ConsoleColor.Red }))
                borderColor = ConsoleColor.White;
            if (!IsAllowedColor(textColor, new[] { ConsoleColor.White, ConsoleColor.Red }))
                textColor = ConsoleColor.Red;

            Divider('─', borderColor);
            FontColor(text, textColor);
            Divider('─', borderColor);
        }

        public static void Menu(List<string> items)
        {
            Divider('─', ConsoleColor.DarkGray);
            for (int i = 0; i < items.Count; i++)
            {
                string index = $"[{i + 1}]".PadRight(6);
                string text = items[i];
                if (text.Length < 16) text = text.PadRight(16);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(index);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(text);
            }
            Divider('─', ConsoleColor.DarkGray);
        }

        public static void Boxed(string text, ConsoleColor color = ConsoleColor.White, char borderChar = '─')
        {
            if (!IsAllowedColor(color, new[] { ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow }))
                color = ConsoleColor.White;

            int width = Math.Max(text.Length, 20);
            string border = new string(borderChar, width);

            Console.ForegroundColor = color;
            Console.WriteLine(border);
            Console.WriteLine(text.PadRight(width));
            Console.WriteLine(border);
            Console.ResetColor();
        }
    }

    public static class App
    {
        public static List<string> SubjectsList = new List<string> { "C#", "C", "C++", ".NET", "Python" };
        private static readonly string DataPath = "students.json";
        private static List<Student> students = new List<Student>();

        public static void Start()
        {
            bool runningProg = true;
            char userInput;
            Console.Clear();
            LoadData();

            while (runningProg)
            {
                UI.Header("Average Grading");
                UI.Menu(new List<string>{
                    Localization.AddRecord,
                    Localization.EditRecord,
                    Localization.DeleteRecord,
                    Localization.ViewRecord,
                    Localization.ViewExportAll,
                    Localization.ResetData,
                    Localization.ResetDataPrebuilt,
                    Localization.Settings,
                    Localization.BackToMenu});

                UI.Divider();
                userInput = Console.ReadKey(true).KeyChar;
                UI.Divider();

                switch (userInput)
                {
                    case Keys.Digit1:
                    case Keys.Digit1Ar:
                        AddRcd();
                        break;

                    case Keys.Digit2:
                    case Keys.Digit2Ar:
                        EditRcd();
                        break;

                    case Keys.Digit3:
                    case Keys.Digit3Ar:
                        DelRcd();
                        break;

                    case Keys.Digit4:
                    case Keys.Digit4Ar:
                        ShwRcd();
                        break;

                    case Keys.Digit5:
                    case Keys.Digit5Ar:
                        AllRcds("Student_Report.txt");
                        break;

                    case Keys.Digit6:
                    case Keys.Digit6Ar:
                        Rst();
                        break;

                    case Keys.Digit7:
                    case Keys.Digit7Ar:
                        RstPb();
                        break;

                    case Keys.Digit8:
                    case Keys.Digit8Ar:
                        Stts();
                        break;

                    case Keys.Digit9:
                    case Keys.Digit9Ar:
                        if (Exit() == 1) runningProg = false;
                        break;

                    default:
                        UI.Boxed(Localization.InvalidInput);
                        break;
                }
            }
        }

        private static void AddRcd()
        {
            int id = GetNextId();
            UI.Boxed(("Student ID: " + id));
            UI.Boxed(Localization.EnterStudentName);
            string name = Console.ReadLine() ?? "";

            List<double> gradeOut = new List<double>();
            for (int i = 0; i < SubjectsList.Count; i++)
            {
                gradeOut.Add(GradeReader(i));
            }

            students.Add(new Student
            {
                Id = id,
                Name = name,
                GradeOne = gradeOut[0],
                GradeTwo = gradeOut[1],
                GradeThree = gradeOut[2],
                GradeFour = gradeOut[3],
                GradeFive = gradeOut[4]
            });

            SaveData();
            UI.Boxed(Localization.RecordAdded);
            UI.Boxed(Localization.PressYToViewNow, ConsoleColor.Yellow);
            char userInput = Console.ReadKey(true).KeyChar;
            if (Keys.IsYes(userInput))
            {
                ShwRcd(id);
            }
            else
            {
                Wt();
            }
        }

        private static void EditRcd()
        {
            UI.Boxed(Localization.EnterID);
            string raw = Console.ReadLine() ?? "";
            raw = Converter.NormalizeDigits(raw);
            if (!int.TryParse(raw, out int id))
            {
                UI.Boxed(Localization.InvalidID);
                Wt();
                return;
            }

            var st = students.FirstOrDefault(s => s.Id == id);
            if (st == null)
            {
                UI.Boxed(Localization.RecordNotFound);
                Wt();
                return;
            }

            UI.Boxed("Enter new name:");
            st.Name = Console.ReadLine() ?? "";

            List<double> grades = new List<double>();
            for (int i = 0; i < SubjectsList.Count; i++)
            {
                grades.Add(GradeReader(i));
            }

            st.GradeOne = grades[0];
            st.GradeTwo = grades[1];
            st.GradeThree = grades[2];
            st.GradeFour = grades[3];
            st.GradeFive = grades[4];

            SaveData();
            UI.Boxed(Localization.RecordUpdated);
            UI.Boxed(Localization.PressYToViewNow, ConsoleColor.Yellow);
            char userInput = Console.ReadKey(true).KeyChar;
            if (Keys.IsYes(userInput))
            {
                ShwRcd(id);
            }
            else
            {
                Wt();
            }
        }

        private static void DelRcd(int id = 0)
        {
            if (id == 0)
            {
                UI.Boxed(Localization.EnterID);
                string raw = Converter.NormalizeDigits(Console.ReadLine() ?? "");
                if (!int.TryParse(raw, out id))
                {
                    UI.Boxed(Localization.InvalidID);
                    Wt();
                    return;
                }
            }

            var st = students.FirstOrDefault(s => s.Id == id);
            if (st == null)
            {
                UI.Boxed(Localization.RecordNotFound);
                Wt();
                return;
            }

            students.Remove(st);
            SaveData();

            UI.Boxed(Localization.RecordDeleted);
            Wt();
        }

        private static void ShwRcd(int id = 0)
        {
            if (students.Count == 0)
            {
                UI.Boxed(Localization.NoRecordsFound);
                Console.ReadLine();
                return;
            }
            if (id == 0)
            {
                UI.Boxed(Localization.EnterID);
                string raw = Console.ReadLine() ?? "";
                raw = Converter.NormalizeDigits(raw);
                if (!int.TryParse(raw, out id))
                {
                    UI.Boxed(Localization.InvalidID);
                    Wt();
                    return;
                }
            }

            var s = students.FirstOrDefault(st => st.Id == id);
            if (s == null)
            {
                UI.Boxed(Localization.RecordNotFound);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine(new string('─', 30));
                Console.WriteLine($"ID: {s.Id} | Name: {s.Name}");
                Console.WriteLine();
                Console.WriteLine($"{SubjectsList[0]}: {s.GradeOne}");
                Console.WriteLine($"{SubjectsList[1]}: {s.GradeTwo}");
                Console.WriteLine($"{S
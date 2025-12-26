# 🎓 Student Grader (C#) — CLI App  
# برنامج تقييم الطلاب (C#) — تطبيق كونسول  

[![Build Status](https://github.com/OmegaCrimson/StudentGrader-CSharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/OmegaCrimson/StudentGrader-CSharp/actions/workflows/dotnet.yml)  
![GitHub release](https://img.shields.io/github/v/release/OmegaCrimson/StudentGrader-CSharp)  
![License](https://img.shields.io/github/license/OmegaCrimson/StudentGrader-CSharp)  
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)  
![Downloads](https://img.shields.io/github/downloads/OmegaCrimson/StudentGrader-CSharp/total)

**Author / المؤلف:** Mohamed Gonem / محمد غنيم  
**Version / الإصدار:** 2.0  
**License / الرخصة:** MIT License  
**Language Support / دعم اللغة:** English + Arabic digits  

A modular, console-based student grading system built in C#.  
Supports multi-subject entry, GPA calculation, persistent storage, and clean CLI navigation.

نظام تقييم طلاب مبني بلغة C# يعمل عبر الكونسول.  
يدعم إدخال درجات متعددة، حساب المعدل التراكمي، حفظ البيانات، والتنقل عبر قائمة بسيطة.

---

## ✨ Features / الميزات

- ✅ Add, view, and delete student records / إضافة وعرض وحذف بيانات الطلاب  
- ✅ Multi-subject support per student / دعم عدة مواد لكل طالب  
- ✅ GPA and percentage calculation / حساب المعدل التراكمي والنسبة المئوية  
- ✅ Input validation with Arabic/English digit support / التحقق من صحة الإدخال ودعم الأرقام العربية والإنجليزية  
- ✅ Persistent storage in AppData / حفظ البيانات تلقائيًا في مجلد AppData  
- ✅ Clean CLI UI with modular services / واجهة كونسول منظمة باستخدام خدمات منفصلة  
- ✅ Logs actions and errors / تسجيل الأحداث والأخطاء  
- ✅ Modular architecture for maintainability / هيكلية مرنة وسهلة التوسعة  

---

## 🚀 Download & Run / التحميل والتشغيل

### 🔹 Option 1: Download Prebuilt Executable

1. Go to the [Releases](https://github.com/OmegaCrimson/StudentGrader-CSharp/releases) page  
2. Download the latest `.zip` or `.exe` file for your OS  
3. Extract and run:
   - `StudentGrader.exe` (Windows)
   - or `dotnet StudentGrader.dll` (cross-platform)

### 🔹 Option 2: Build from Source

1. Clone the repo:
   ```bash
   git clone https://github.com/OmegaCrimson/StudentGrader-CSharp.git
   cd student-grader
   ```

2. Build and run:
   ```bash
   dotnet build
   dotnet run
   ```

---

## 🧪 Sample Usage / مثال على الاستخدام

```plaintext
Student Grader
──────────────
1. Add Student
2. View Student
3. View All Students
4. Delete Student
5. Delete All Students
6. Exit Program

Input: 1
Name: Ali
Age: 20
Subject name: Math
Score: 90
Max Score: 100
Teacher: Mr. Ahmed
...
```

---

## 🗂️ Project Structure / هيكل المشروع

```
student-grader/
├── Models/           # Student and Subject classes
├── Services/         # CRUD logic, input, printing, storage, logging
├── UI/               # UIHelper for headers and menus
├── Validations/      # Input validation logic
├── Exceptions/       # Custom validation exceptions
├── Program.cs        # Entry point
├── StudentGrader.csproj
├── .gitignore
└── README.md
```

---

## 🛠️ Technologies Used / التقنيات المستخدمة

- **C# 8.0+**
- **.NET SDK 8.0**
- Console I/O
- JSON serialization
- Modular architecture
- GitHub Actions (CI/CD)

---

## 📦 Build & Release Automation

This project uses **GitHub Actions** to automatically:

- Build the app on every push to `main`
- Run tests (if added)
- Publish release artifacts (executables) to the [Releases](https://github.com/OmegaCrimson/StudentGrader-CSharp/releases) page

You can download the latest version without building manually.

---

## 📄 License / الرخصة

This project is licensed under the **MIT License**.  
You may use, modify, and distribute it freely — just credit the author: **Mohamed Gonem / محمد غنيم**

هذا المشروع مرخّص تحت رخصة **MIT**.  
يمكنك استخدامه وتعديله وتوزيعه بحرية — فقط اذكر المؤلف: **Mohamed Gonem / محمد غنيم**

---

## 🙌 Acknowledgments / شكر وتقدير

- Built with care, clarity, and curiosity  
- Inspired by real-world grading systems and CLI design patterns  
- Special thanks to the open-source community for tools and ideas

---

**Built to be useful. Designed to be clear.  
تم بناؤه ليكون مفيدًا، وصُمم ليكون واضحًا.**

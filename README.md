# Student Grader (C#) — CLI App  
برنامج تقييم الطلاب (C#) — تطبيق كونسول

A modular, console-based student grading system built in C#.  
نظام تقييم طلاب يعمل عبر الكونسول، يدعم إدخال درجات متعددة، حساب المعدل، وحفظ البيانات.

---

## 📦 Project Overview

[![Build](https://github.com/OmegaCrimson/StudentGrader-CSharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/OmegaCrimson/StudentGrader-CSharp/actions/workflows/dotnet.yml)
![Release](https://img.shields.io/github/v/release/OmegaCrimson/StudentGrader-CSharp)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Cross--platform-green)
![License](https://img.shields.io/github/license/OmegaCrimson/StudentGrader-CSharp)
![Downloads](https://img.shields.io/github/downloads/OmegaCrimson/StudentGrader-CSharp/total)
![Last Commit](https://img.shields.io/github/last-commit/OmegaCrimson/StudentGrader-CSharp)
![Commits per Month](https://img.shields.io/github/commit-activity/m/OmegaCrimson/StudentGrader-CSharp)
![Issues](https://img.shields.io/github/issues/OmegaCrimson/StudentGrader-CSharp)
![PRs](https://img.shields.io/github/issues-pr/OmegaCrimson/StudentGrader-CSharp)
![Contributors](https://img.shields.io/github/contributors/OmegaCrimson/StudentGrader-CSharp)
![Code Size](https://img.shields.io/github/languages/code-size/OmegaCrimson/StudentGrader-CSharp)
![Top Language](https://img.shields.io/github/languages/top/OmegaCrimson/StudentGrader-CSharp)
![Lines of Code](https://img.shields.io/tokei/lines/github/OmegaCrimson/StudentGrader-CSharp)
![Maintenance](https://img.shields.io/maintenance/yes/2025)

**Author:** Mohamed Gonem / محمد غنيم  
**Version:** 2.0  
**License:** MIT  
**Languages:** English + Arabic digits

---

## ✨ Features

- Add, view, and delete student records  
- Multi-subject support per student  
- GPA and percentage calculation  
- Input validation (Arabic & English digits)  
- Auto-saving to AppData  
- Clean CLI interface with modular services  
- Action/error logging  
- Extensible architecture

---

## 🚀 Getting Started

### Option 1: Download Executable

1. Visit the [Releases](https://github.com/OmegaCrimson/StudentGrader-CSharp/releases) page  
2. Download the latest `.zip` or `.exe`  
3. Run:
   - `StudentGrader.exe` (Windows)
   - or `dotnet StudentGrader.dll` (cross-platform)

### Option 2: Build from Source

```bash
git clone https://github.com/OmegaCrimson/StudentGrader-CSharp.git
cd StudentGrader-CSharp
dotnet build
dotnet run
```

---

## 🧪 Sample CLI Output

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
```

---

## 🗂️ Project Structure

```
StudentGrader-CSharp/
├── Models/           # Student and Subject classes
├── Services/         # CRUD, input, storage, logging
├── UI/               # Menu and display helpers
├── Validations/      # Input validation logic
├── Exceptions/       # Custom exceptions
├── Program.cs        # Entry point
└── StudentGrader.csproj
```

---

## 🛠️ Tech Stack

- C# 8.0+
- .NET SDK 8.0
- Console I/O
- JSON serialization
- GitHub Actions (CI/CD)

---

## 🔁 CI/CD Automation

This project uses GitHub Actions to:

- Build on every push to `main`
- Run tests (if added)
- Publish `.exe` and `.zip` files to [Releases](https://github.com/OmegaCrimson/StudentGrader-CSharp/releases)

---

## 📄 License

Licensed under the [MIT License](LICENSE).  
Use, modify, and distribute freely — just credit the author: **Mohamed Gonem / محمد غنيم**

---

## 🙌 Acknowledgments

- Built with care, clarity, and curiosity  
- Inspired by real-world grading systems and CLI design patterns  
- Thanks to the open-source community for tools and ideas

---

**Built to be useful. Designed to be clear.  
تم بناؤه ليكون مفيدًا، وصُمم ليكون واضحًا.**
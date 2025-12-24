using Exceptions;
using Validations;

namespace Models
{

    public class Subject
    {
        private string name;
        private int maxGrade;
        private double grade;
        private string teacher;

        public string Name
        {
            get => name;
            set
            {
                string trimmedName = value.Trim();
                StringValidator.ValidateRequired(trimmedName, "Subject name");
                StringValidator.ValidateLength(trimmedName, 3, 30, "Subject name");
                name = trimmedName;
            }
        }

        public int MaxGrade
        {
            get => maxGrade;
            set
            {
                RangeValidator.ValidateInt(value, 0, 1000, "Subject maximum grade");
                maxGrade = value;
            }
        }
        public double Grade
        {
            get => grade;
            set
            {
                RangeValidator.ValidateDouble(value, 0.0, (double)maxGrade, "Subject grade");
                grade = value;
            }
        }
        public string Teacher
        {
            get => teacher;
            set
            {
                string trimmedName = value.Trim();
                StringValidator.ValidateRequired(trimmedName, "Teacher name");
                StringValidator.ValidateLength(trimmedName, 3, 30, "Teacher name");
                teacher = trimmedName;
            }
        }
        public Subject(string name, double grade, int maxGrade, string teacher)
        {
            Name = name;
            MaxGrade = maxGrade;
            Grade = grade;
            Teacher = teacher;
        }
    }
}
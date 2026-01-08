using System;
using System.Collections.Generic;
using System.Text;
using DB.Data;
using Validations;

namespace Services
{
    public static class StudentInputService
    {
        public static StudentClass CreateStudent()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("Name: ");
            string name;
            while (true) 
            { 
                try
                { 
                    name = Console.ReadLine()?.Trim() ?? "";
                    StringValidator.ValidateLength(name, 3, 30, "Name");
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Name: ");
                    continue;
                }
                break;
            }
            string _name = name;

            Console.Write("Gender (or empty): ");
            var _gender = Console.ReadLine()?.Trim();

            Console.Write("Religion (or empty): ");
            var _religion = Console.ReadLine()?.Trim();

            // Fix for CS1525 and CS8602 in Birth Day input section
            Console.Write("Birth Day (1–31): ");
            int day;
            string? dayInput;
            while (true)
            {
                try
                {
                    try
                    {
                        dayInput = Console.ReadLine();
                        StringValidator.ValidateRequired(dayInput ?? "", "Birth Day");
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Write("Birth Day (1–31): ");
                        continue;
                    }
                day = int.TryParse(dayInput, out int parsedDay) ? parsedDay : 1;
                RangeValidator.ValidateInt(day, 1, 31, "Birth Day");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Birth Day (1–31): ");
                    continue;
                }
                break;
            }
            int _day = day;

            Console.Write("Birth Month (1–12): ");
            int month;
            string? monthInput;
            while (true)
            {
                try
                {
                    try
                    {
                        monthInput = Console.ReadLine();
                        StringValidator.ValidateRequired(monthInput ?? "", "Birth Month");
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Write("Birth Month (1–12): ");
                        continue;
                    }
                    month = int.TryParse(monthInput, out int parsedDay) ? parsedDay : 1;
                    RangeValidator.ValidateInt(month, 1, 12, "Birth Day");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Birth Month (1–12): ");
                    continue;
                }
                break;
            }
            int _month = month;

            Console.Write("Birth Year: ");
            int year;
            string? yearInput;
            while (true)
            {
                try
                {
                    try
                    {
                        yearInput = Console.ReadLine();
                        StringValidator.ValidateRequired(yearInput ?? "", "Birth Year");
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Write("Birth Year (1990–2020): ");
                        continue;
                    }
                    year = int.TryParse(yearInput, out int parsedDay) ? parsedDay : 1;
                    RangeValidator.ValidateInt(year, 1990, 2020, "Birth Year");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Birth Year (1990–2020): ");
                    continue;
                }
                break;
            }
            int _year = year;

            Console.Write("Address: ");
            string address;
            while (true)
            {
                try
                {
                    address = Console.ReadLine()?.Trim() ?? "";
                    StringValidator.ValidateLength(name, 3, 50, "Address");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Address: ");
                    continue;
                }
                break;
            }
            string _address = address;

            Console.Write("National ID: ");
            string nid;
            while (true)
            {
                try
                {
                    nid = Console.ReadLine()?.Trim() ?? "";
                    StringValidator.ValidateLength(nid, 6, 20, "National ID");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("National ID: ");
                    continue;
                }
                break;
            }
            string _nid = nid;

            // Optional phone and email
            Console.Write("Phone (or empty): ");
            var phone = Console.ReadLine()?.Trim();
            Console.Write("Email (or empty): ");
            var email = Console.ReadLine()?.Trim();

            return new StudentClass
            {
                StudentName = _name,
                Gender = _gender,
                Religion = _religion,
                BirthDay = _day,
                BirthMonth = _month,
                BirthYear = _year,
                Address = _address,
                NationalId = _nid
                ,Phone = string.IsNullOrWhiteSpace(phone) ? null : phone
                ,Email = string.IsNullOrWhiteSpace(email) ? null : email
            };
        }
    }
}

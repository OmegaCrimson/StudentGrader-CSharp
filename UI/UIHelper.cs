using System;
using System.Linq;
using System.Collections.Generic;
using Exceptions;
using Validations;
using Services;

namespace UI
{
    public static class UIHelper
    {
        public static void Line(int length, int style)
        {
            try
            {
                RangeValidator.ValidateInt(length, 1, 30, "line length");
                RangeValidator.ValidateInt(style, 1, 5, "line style");

                char lineChar = GetLineChar(style);
                string line = string.Empty;

                for (int i = 0; i < length; i++)
                {
                    line += lineChar;
                }

                line += '\n';
                Console.Write(line);
            }
            catch (IntInvalidValueException e)
            {
                LoggerService.LogError("Line", e);
            }
        }

        public static void Header(string message, int style = 2, int length = 30, bool clear = true)
        {
            try
            {
                if (clear)
                    Console.Clear();
                Line(length, style);
                Console.WriteLine(message);
                Line(length, style);

            }
            catch (IntInvalidValueException e)
            {
                LoggerService.LogError("Line", e);
            }
        }

        public static void Body(List<string> lines, bool numbered = true, int indent = 2)
        {
            try
            {
                if (lines == null)
                    throw new ArgumentNullException(nameof(lines), "Body content cannot be null.");

                RangeValidator.ValidateInt(indent, 0, 10, "Indentation");

                if (lines.Count == 0)
                {
                    Console.WriteLine("(No content)");
                    return;
                }

                string indentSpace = new string(' ', indent);
                for (int i = 0; i < lines.Count; i++)
                {
                    string prefix = numbered ? $"{i + 1}. " : "- ";
                    Console.WriteLine($"{indentSpace}{prefix}{lines[i]}");
                }
            }
            catch (IntInvalidValueException e)
            {
                LoggerService.LogError("Error", e);
            }
            catch (Exception e)
            {
                LoggerService.LogError("Error", e);
            }
        }

        public static void BodyWithHeader(string message, List<string> lines, int style = 1, int length = 30, bool clear = true, bool numbered = true, int indent = 2)
        {
            try
            {
                Header(message, style, length,false);
                Body(lines, numbered, indent);
                Line(length, style);
            }
            catch (Exception e)
            {
                LoggerService.LogError("Error", e);
            }
        }

        private static char GetLineChar(int style)
        {
            return style switch
            {
                1 => '\u2500', 
                2 => '\u2501',
                3 => '\u2550',
                4 => '\u2015',
                5 => '\u2013',
                _ => '-'
            };
        }
    }
}

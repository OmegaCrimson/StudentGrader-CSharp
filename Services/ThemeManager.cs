using System;

namespace Services
{
    public static class ThemeManager
    {
        public enum Theme
        {
            Default,
            Dark,
            HighContrast
        }

        public static Theme CurrentTheme { get; private set; } = Theme.Default;

        public static void Apply(Theme t)
        {
            CurrentTheme = t;
            switch (t)
            {
                case Theme.Dark:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case Theme.HighContrast:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Clear();
        }
    }
}

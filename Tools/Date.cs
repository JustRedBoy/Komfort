﻿using System;

namespace Tools
{
    /// <summary>
    /// Class for working with date
    /// </summary>
    public static class Date
    {
        /// <summary>
        /// Get a string with the name of the previous month and year
        /// </summary>
        public static string GetPrevDate()
        {
            return $"{GetNamePrevMonth()} {DateTime.Now.Year}";
        }

        /// <summary>
        /// Get a string with the name of the current month
        /// </summary>
        public static string GetNameCurMonth()
        {
            return GetNameMonth(DateTime.Now.Month);
        }

        /// <summary>
        /// Get a string with the name of the previous month
        /// </summary>
        public static string GetNamePrevMonth()
        {
            return GetNameMonth(DateTime.Now.AddMonths(-1).Month);
        }

        /// <summary>
        /// Get a number of the month
        /// </summary>
        public static int GetNumMonth(string month) => month switch
        {
            "январь" => 1,
            "февраль" => 2,
            "март" => 3,
            "апрель" => 4,
            "май" => 5,
            "июнь" => 6,
            "июль" => 7,
            "август" => 8,
            "сентябрь" => 9,
            "октябрь" => 10,
            "ноябрь" => 11,
            "декабрь" => 12,
            _ => throw new ArgumentException("Недопустимый месяц")
        };

        /// <summary>
        /// Get a string with the name of the month
        /// </summary>
        public static string GetNameMonth(int month) => month switch
        {
            1 => "январь",
            2 => "февраль",
            3 => "март",
            4 => "апрель",
            5 => "май",
            6 => "июнь",
            7 => "июль",
            8 => "август",
            9 => "сентябрь",
            10 => "октябрь",
            11 => "ноябрь",
            12 => "декабрь",
            _ => throw new ArgumentException("Недопустимый месяц")
        };

        /// <summary>
        /// Get a string with the number of the month (2 digits)
        /// </summary>
        public static string GetShortNumMonth(int numMonth) =>
            numMonth < 10 ? "0" + numMonth : numMonth.ToString();
    }
}

using System;
using ClinicApp.Enums;

namespace ClinicApp.Utils;

// Задача 3. Логіка форматування не належить жодному конкретному об'єкту,
// тому вона винесена в static class: у нього немає екземплярів,
// тільки статичні методи (створити new ClinicFormatter() неможливо).
public static class ClinicFormatter
{
    // BloodType.APositive -> "A+"
    public static string FormatBloodType(BloodType bt)
    {
        switch (bt)
        {
            case BloodType.APositive: return "A+";
            case BloodType.ANegative: return "A-";
            case BloodType.BPositive: return "B+";
            case BloodType.BNegative: return "B-";
            case BloodType.ABPositive: return "AB+";
            case BloodType.ABNegative: return "AB-";
            case BloodType.OPositive: return "O+";
            case BloodType.ONegative: return "O-";
            default: return "невідомо";
        }
    }

    // Speciality.Cardiologist -> "Кардіологія"
    public static string FormatSpeciality(Speciality s)
    {
        switch (s)
        {
            case Speciality.Therapist: return "Терапія";
            case Speciality.Cardiologist: return "Кардіологія";
            case Speciality.Surgeon: return "Хірургія";
            case Speciality.Pediatrician: return "Педіатрія";
            case Speciality.Neurologist: return "Неврологія";
            case Speciality.Dermatologist: return "Дерматологія";
            case Speciality.Ophthalmologist: return "Офтальмологія";
            case Speciality.Dentist: return "Стоматологія";
            default: return s.ToString();
        }
    }

    // 41 -> "41 рік", 33 -> "33 роки", 16 -> "16 років".
    // Правила відмінювання: дивимось на останню цифру,
    // але числа 11-14 — виняток (завжди "років").
    public static string FormatAge(int age)
    {
        int lastTwo = age % 100;
        int last = age % 10;

        if (lastTwo >= 11 && lastTwo <= 14) return age + " років";
        if (last == 1) return age + " рік";
        if (last >= 2 && last <= 4) return age + " роки";
        return age + " років";
    }

    // "0501234567" -> "(050) 123-4567"
    public static string FormatPhone(string phone)
    {
        if (phone == null) return "";

        // лишаємо тільки цифри
        string digits = "";
        for (int i = 0; i < phone.Length; i++)
        {
            if (char.IsDigit(phone[i]))
            {
                digits += phone[i];
            }
        }

        if (digits.Length != 10) return phone;   // нестандартний номер — лишаємо як є

        return "(" + digits.Substring(0, 3) + ") " + digits.Substring(3, 3) + "-" + digits.Substring(6, 4);
    }
}

using System;

namespace ClinicApp.Models;

// Задача 2. Терміновий прийом: дорожчий за базову ціну і має причину терміновості.
public class UrgentAppointment : Appointment
{
    // Лише для читання: задається в конструкторі й більше не змінюється
    public string UrgencyNote { get; }

    public UrgentAppointment(int patientId, int doctorId, DateTime scheduledAt,
                             string urgencyNote = "", int durationMinutes = 30)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
        UrgencyNote = urgencyNote == null ? "" : urgencyNote.Trim();
    }

    // На 50% дорожче за базову ціну.
    // base.GetCost() повертає ціну батька, і ми лише домножуємо її,
    // тому формула базового тарифу лишається в одному місці.
    public override decimal GetCost()
    {
        return base.GetCost() * 1.5m;
    }

    // sealed override: метод перевизначено востаннє — підкласи цього класу
    // вже не зможуть змінити опис термінового прийому
    public sealed override string GetDescription()
    {
        string result = "Терміновий";
        if (UrgencyNote.Length > 0)
        {
            result += $" ({UrgencyNote})";
        }
        return result;
    }

    // new, а не override: у батька метод не віртуальний, тому його не можна
    // перевизначити — його можна лише приховати. Наслідок видно в Задачі 3:
    // через змінну типу Appointment викликається варіант батька.
    public new int GetPriority()
    {
        return 1;
    }
}

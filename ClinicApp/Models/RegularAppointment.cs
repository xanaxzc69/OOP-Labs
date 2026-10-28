using System;

namespace ClinicApp.Models;

// Задача 1. Перший підклас — звичайний прийом.
// Ціну він не змінює, тому GetCost() не перевизначається:
// успадкована базова реалізація вже дає потрібний результат.
public class RegularAppointment : Appointment
{
    // Конструктор нічого свого не робить — лише передає параметри батькові
    public RegularAppointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
    }

    public override string GetDescription()
    {
        return "Звичайний прийом";
    }
}

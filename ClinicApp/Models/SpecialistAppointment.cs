using System;

namespace ClinicApp.Models;

// Задача 2. Консультація спеціаліста.
// sealed class — клас закритий для успадкування: від нього не можна
// створити жодного підкласу (компілятор не дозволить).
public sealed class SpecialistAppointment : Appointment
{
    // Лише для читання: тема консультації задається в конструкторі
    public string ConsultationTopic { get; }

    public SpecialistAppointment(int patientId, int doctorId, DateTime scheduledAt,
                                 string consultationTopic = "", int durationMinutes = 30)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
        ConsultationTopic = consultationTopic == null ? "" : consultationTopic.Trim();
    }

    // На 30% дорожче за базову ціну
    public override decimal GetCost()
    {
        return base.GetCost() * 1.3m;
    }

    public override string GetDescription()
    {
        string result = "Консультація спеціаліста";
        if (ConsultationTopic.Length > 0)
        {
            result += $" ({ConsultationTopic})";
        }
        return result;
    }
}

using System;

namespace ClinicApp;

// Задача 5. Запис пацієнта до лікаря.
// Статус — кінцевий автомат:
//   "Scheduled" -> "Cancelled"  або  "Scheduled" -> "Completed"  (лише один раз)
// Клас зберігає тільки PatientId і DoctorId, а не посилання на об'єкти —
// тому ToString() показує лише числа (це вирішується в AppointmentManager).
public class Appointment
{
    private static int _nextId = 1;

    public int Id { get; }
    public int PatientId { get; }
    public int DoctorId { get; }
    public DateTime ScheduledAt { get; }
    public int DurationMinutes { get; }

    // Ззовні — тільки читання; змінюються лише через Cancel()/Complete()
    public string Status { get; private set; }
    public string Notes { get; private set; } = "";

    // durationMinutes — необов'язковий параметр, за замовчуванням 30 хв
    public Appointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        Id = _nextId++;
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        Status = "Scheduled";
    }

    // ---------- Обчислювані властивості ----------

    public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);

    public bool IsUpcoming => ScheduledAt > DateTime.Now && Status == "Scheduled";

    // ---------- Переходи стану ----------

    public bool Cancel(string reason = "")
    {
        if (Status != "Scheduled")
        {
            return false;
        }
        Status = "Cancelled";
        if (reason.Length > 0)
        {
            Notes = reason;
        }
        return true;
    }

    public bool Complete()
    {
        if (Status != "Scheduled")
        {
            return false;
        }
        Status = "Completed";
        return true;
    }

    public override string ToString()
    {
        string result = $"[{Id}] Пацієнт #{PatientId} → Лікар #{DoctorId} | " +
                        $"{ScheduledAt.ToString("dd.MM.yyyy HH:mm")}–{EndsAt.ToString("HH:mm")} | {Status}";
        if (Notes.Length > 0)
        {
            result += " | " + Notes;
        }
        return result;
    }
}

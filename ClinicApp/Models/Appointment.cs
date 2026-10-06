using System;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Запис пацієнта до лікаря.
// Статус — кінцевий автомат:
//   Scheduled -> Cancelled  або  Scheduled -> Completed  (лише один раз).
public class Appointment
{
    private static int _nextId = 1;

    // Задача 2: приватне поле + явна властивість через нього.
    // Задача 4: перевірку виконує ClinicValidator
    private int _durationMinutes;

    public int Id { get; }
    public int PatientId { get; }
    public int DoctorId { get; }
    public DateTime ScheduledAt { get; }

    public int DurationMinutes
    {
        get { return _durationMinutes; }
        set
        {
            ClinicValidator.ValidatePositive(value, "Тривалість прийому");
            _durationMinutes = value;
        }
    }

    // Ззовні — тільки читання; змінюються лише через Cancel()/Complete()
    public AppointmentStatus Status { get; private set; }
    public string Notes { get; private set; } = "";

    // durationMinutes — необов'язковий параметр, за замовчуванням 30 хв
    public Appointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        Id = _nextId++;
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;   // присвоєння через властивість
        Status = AppointmentStatus.Scheduled;
    }

    // ---------- Обчислювані властивості ----------

    public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);

    public bool IsUpcoming => ScheduledAt > DateTime.Now && Status == AppointmentStatus.Scheduled;

    // ---------- Переходи стану ----------

    public bool Cancel(string reason = "")
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            return false;
        }
        Status = AppointmentStatus.Cancelled;
        if (reason.Length > 0)
        {
            Notes = reason;
        }
        return true;
    }

    public bool Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            return false;
        }
        Status = AppointmentStatus.Completed;
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

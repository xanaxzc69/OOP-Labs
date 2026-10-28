using System;
using ClinicApp.Enums;
using ClinicApp.Interfaces;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Запис пацієнта до лікаря.
// Статус — кінцевий автомат:
//   Scheduled -> Cancelled  або  Scheduled -> Completed  (лише один раз).
// Задача 2: один клас підписує одразу два контракти.
// Для коду, що працює через IPayable, це «щось, що можна оплатити»,
// для коду через ICancellable — «щось, що можна скасувати».
public class Appointment : IPayable, ICancellable
{
    private static int _nextId = 1;

    // Задача 2: приватне поле + явна властивість через нього.
    // Задача 4: перевірку виконує ClinicValidator
    private int _durationMinutes;

    // Задача 1: приватна ознака оплати — ззовні змінити її можна
    // лише через MarkPaid(), і лише за правилами клініки
    private bool _isPaid = false;


    // Тариф клініки: 10 грн за кожну хвилину прийому
    public const decimal PricePerMinute = 10m;

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

    // ---------- Реалізація контракту ICancellable ----------

    // Стан не дублюється: чи скасовано — випливає зі Status,
    // а причина — з приміток, які вже зберігає сам запис
    public bool IsCancelled => Status == AppointmentStatus.Cancelled;

    public string CancellationReason => IsCancelled ? Notes : "";

    // ---------- Задача 1. Віртуальні методи ----------

    // virtual означає «підклас має право замінити цю поведінку».
    // Базова формула: тривалість у хвилинах × тариф.
    public virtual decimal GetCost()
    {
        return DurationMinutes * PricePerMinute;
    }

    // Опис запису; кожен вид прийому описує себе по-своєму
    public virtual string GetDescription()
    {
        return "Прийом";
    }

    // Навмисно НЕ virtual: у Задачі 3 на цьому методі видно різницю
    // між override (вибір за реальним типом об'єкта) і new (вибір за типом змінної)
    public int GetPriority()
    {
        return 3;
    }

    // ---------- Реалізація контракту IPayable ----------

    // Властивість контракту спирається на віртуальний метод, тому
    // менеджер рахунків автоматично бачить ціну того підкласу, який реально створено
    public decimal Amount => GetCost();

    public bool IsPaid => _isPaid;

    // Оплатити можна лише не скасований і ще не оплачений запис
    public bool MarkPaid()
    {
        // Спираємось на властивість контракту, а не на перевірку Status
        if (IsCancelled) return false;
        if (_isPaid) return false;

        _isPaid = true;
        return true;
    }

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
        string payment = IsPaid ? "оплачено" : "не оплачено";
        string result = $"{GetDescription()} [{Id}] Пацієнт #{PatientId} → Лікар #{DoctorId} | " +
                        $"{ScheduledAt.ToString("dd.MM.yyyy HH:mm")}–{EndsAt.ToString("HH:mm")} | {Status} | {payment}";
        if (Notes.Length > 0)
        {
            result += " | " + Notes;
        }
        return result + $" | {GetCost()} грн";
    }
}

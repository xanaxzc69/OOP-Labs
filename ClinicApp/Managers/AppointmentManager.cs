using System;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Задача 6. Менеджер записів.
// Отримує посилання на PatientManager і DoctorManager через конструктор —
// це дозволяє валідувати ID при записі та відображати імена замість чисел.
public class AppointmentManager
{
    private const int MaxAppointments = 500;

    private Appointment[] _appointments = new Appointment[MaxAppointments];
    private int _count = 0;

    private PatientManager _patients;
    private DoctorManager _doctors;

    public int Count => _count;

    // Задача 3. Індексатор — доступ до запису за порядковим номером
    public Appointment this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _appointments[index];
        }
    }

    // Зберігаємо посилання на вже існуючі менеджери (нових не створюємо)
    public AppointmentManager(PatientManager patients, DoctorManager doctors)
    {
        _patients = patients;
        _doctors = doctors;
    }

    // ---------- Створення запису з валідацією ----------
    // Задача 3: звичайний прийом тепер створюється підкласом RegularAppointment
    public bool Book(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        if (!CanBook(patientId, doctorId)) return false;

        return Register(new RegularAppointment(patientId, doctorId, scheduledAt, durationMinutes));
    }

    // Терміновий прийом із причиною терміновості
    public bool BookUrgent(int patientId, int doctorId, DateTime scheduledAt,
                           string urgencyNote = "", int durationMinutes = 30)
    {
        if (!CanBook(patientId, doctorId)) return false;

        return Register(new UrgentAppointment(patientId, doctorId, scheduledAt, urgencyNote, durationMinutes));
    }

    // Консультація спеціаліста із темою консультації
    public bool BookSpecialist(int patientId, int doctorId, DateTime scheduledAt,
                               string consultationTopic = "", int durationMinutes = 30)
    {
        if (!CanBook(patientId, doctorId)) return false;

        return Register(new SpecialistAppointment(patientId, doctorId, scheduledAt, consultationTopic, durationMinutes));
    }

    // Перевірки однакові для всіх трьох методів, тому зібрані в одному місці
    private bool CanBook(int patientId, int doctorId)
    {
        if (_patients.FindById(patientId) == null)
        {
            Console.WriteLine($"Помилка: пацієнта з ID {patientId} не знайдено.");
            return false;
        }

        if (_doctors.FindById(doctorId) == null)
        {
            Console.WriteLine($"Помилка: лікаря з ID {doctorId} не знайдено.");
            return false;
        }

        if (_count == MaxAppointments)
        {
            Console.WriteLine($"Помилка: досягнуто ліміт у {MaxAppointments} записів.");
            return false;
        }

        return true;
    }

    // Метод приймає базовий тип, тому однаково кладе в масив будь-який вид прийому
    private bool Register(Appointment appointment)
    {
        _appointments[_count++] = appointment;

        Patient patient = _patients.FindById(appointment.PatientId);
        Doctor doctor = _doctors.FindById(appointment.DoctorId);
        Console.WriteLine($"Запис [{appointment.Id}] створено: {appointment.GetDescription()} | " +
                          $"{patient.FullName} → {doctor.FullName} о " +
                          $"{appointment.ScheduledAt.ToString("dd.MM.yyyy HH:mm")} | {appointment.GetCost()} грн");
        return true;
    }

    // ---------- Пошук ----------

    // Лінійний пошук у _appointments
    private Appointment FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].Id == id)
            {
                return _appointments[i];
            }
        }
        return null;
    }

    public Appointment[] GetAll()
    {
        Appointment[] result = new Appointment[_count];
        for (int i = 0; i < _count; i++)
        {
            result[i] = _appointments[i];
        }
        return result;
    }

    // Усі Get-методи — двопрохідний патерн: рахуємо, потім заповнюємо масив точного розміру
    public Appointment[] GetByPatient(int patientId)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].PatientId == patientId) matches++;
        }

        Appointment[] result = new Appointment[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].PatientId == patientId) result[index++] = _appointments[i];
        }
        return result;
    }

    public Appointment[] GetByDoctor(int doctorId)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].DoctorId == doctorId) matches++;
        }

        Appointment[] result = new Appointment[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].DoctorId == doctorId) result[index++] = _appointments[i];
        }
        return result;
    }

    public Appointment[] GetByDate(DateTime date)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date) matches++;
        }

        Appointment[] result = new Appointment[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date) result[index++] = _appointments[i];
        }
        return result;
    }

    // Задача 4. Перевантаження GetByDate: три числа замість DateTime.
    // Щоб не дублювати логіку, викликаємо вже існуючий варіант.
    public Appointment[] GetByDate(int year, int month, int day)
    {
        return GetByDate(new DateTime(year, month, day));
    }

    public Appointment[] GetUpcoming()
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming) matches++;
        }

        Appointment[] result = new Appointment[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming) result[index++] = _appointments[i];
        }
        return result;
    }

    // ---------- Задача 2. Споживач контракту ICancellable ----------

    // Метод нічого не знає про Appointment — лише про те, що кожен елемент
    // масиву можна спробувати скасувати. Завтра сюди можна передати
    // масив будь-яких інших об'єктів, які підписали цей контракт.
    public static int CancelAll(ICancellable[] items, string reason = "")
    {
        if (items == null) return 0;

        int cancelled = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            if (items[i].Cancel(reason)) cancelled++;
        }
        return cancelled;
    }

    // ---------- Зміна статусу (делегуємо об'єкту Appointment) ----------

    public bool Cancel(int id, string reason = "")
    {
        Appointment appointment = FindById(id);
        if (appointment == null)
        {
            return false;
        }
        return appointment.Cancel(reason);
    }

    public bool Complete(int id)
    {
        Appointment appointment = FindById(id);
        if (appointment == null)
        {
            return false;
        }
        return appointment.Complete();
    }

    // ---------- Вивід з іменами замість ID ----------

    public void DisplayAppointment(Appointment a)
    {
        Patient patient = _patients.FindById(a.PatientId);
        Doctor doctor = _doctors.FindById(a.DoctorId);

        string patientName = patient != null ? patient.FullName : $"Пацієнт #{a.PatientId}";
        string doctorName = doctor != null ? doctor.FullName : $"Лікар #{a.DoctorId}";

        string line = $"[{a.Id}] {patientName} → {doctorName} | " +
                      $"{a.ScheduledAt.ToString("dd.MM.yyyy HH:mm")}–{a.EndsAt.ToString("HH:mm")} | {a.Status}";
        if (a.Notes.Length > 0)
        {
            line += " | " + a.Notes;
        }
        Console.WriteLine(line);
    }

    public void DisplayList(Appointment[] list)
    {
        if (list.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }
        for (int i = 0; i < list.Length; i++)
        {
            DisplayAppointment(list[i]);
        }
    }
}

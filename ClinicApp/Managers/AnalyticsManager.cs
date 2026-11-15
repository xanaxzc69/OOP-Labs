using System;
using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Задача 4 (Lab10). Аналітичний модуль клініки.
//
// Джерела даних менеджер не створює, а отримує ззовні через конструктор:
// він лише читає вже наявні записи, лікарів і пацієнтів.
//
// Ключова ідея задачі — ЛІНИВЕ обчислення через yield return.
// Звичайний метод, що повертає DoctorStats[], мусив би спершу порахувати
// статистику ВСІХ лікарів, скласти масив і лише потім віддати його.
// Метод з yield return повертає IEnumerable<DoctorStats> і працює інакше:
// він рахує статистику одного лікаря, одразу віддає її в цикл foreach
// і зупиняється до наступного запиту. Якщо перебір перервати після
// десятого лікаря, статистика решти не обчислюватиметься взагалі.
public class AnalyticsManager
{
    private readonly AppointmentManager _appointments;
    private readonly DoctorManager _doctors;
    private readonly PatientManager _patients;

    public AnalyticsManager(AppointmentManager appointments, DoctorManager doctors, PatientManager patients)
    {
        _appointments = appointments;
        _doctors = doctors;
        _patients = patients;
    }

    // ---------- Статистика лікарів ----------

    // Повертається не готовий масив, а послідовність, яку можна перебирати.
    // Тіло методу не виконується в момент виклику: воно запускається лише
    // тоді, коли починається foreach, і щоразу зупиняється на yield return.
    public IEnumerable<DoctorStats> ComputeDoctorStats()
    {
        foreach (Doctor doctor in _doctors.GetAll())
        {
            Appointment[] doctorAppointments = _appointments.GetByDoctor(doctor.Id);

            int count = 0;
            decimal revenue = 0m;
            // Для лікаря без жодного прийому дати не існує, тому починаємо
            // з DateTime.MinValue — знімок потім покаже прочерк замість дати
            DateTime lastDate = DateTime.MinValue;

            foreach (Appointment appointment in doctorAppointments)
            {
                // Скасовані прийоми не відбулися: ані в навантаження,
                // ані у виручку вони не йдуть
                if (appointment.IsCancelled) continue;

                count++;
                revenue += appointment.GetCost();

                if (appointment.ScheduledAt > lastDate)
                {
                    lastDate = appointment.ScheduledAt;
                }
            }

            // Лікарі без прийомів теж потрапляють у звіт — із нулями.
            // Якби ми їх пропускали, у списку «хто скільки працює»
            // зникли б саме ті, хто не працює зовсім.
            yield return new DoctorStats(doctor.Id, doctor.FullName, count, revenue, lastDate);
        }
    }

    // ---------- Статистика пацієнтів ----------

    public IEnumerable<PatientStats> ComputePatientStats()
    {
        foreach (Patient patient in _patients.GetAll())
        {
            Appointment[] patientAppointments = _appointments.GetByPatient(patient.Id);

            int visits = 0;
            decimal spent = 0m;
            DateTime lastVisit = DateTime.MinValue;

            foreach (Appointment appointment in patientAppointments)
            {
                if (appointment.IsCancelled) continue;

                visits++;
                spent += appointment.GetCost();

                if (appointment.ScheduledAt > lastVisit)
                {
                    lastVisit = appointment.ScheduledAt;
                }
            }

            yield return new PatientStats(patient.Id, patient.FullName, visits, spent, lastVisit);
        }
    }

    // ---------- Демонстрація лінивості ----------

    // Метод бере лише перші skipAfter елементів послідовності й на цьому
    // перериває перебір. Решта об'єктів DoctorStats не створюється взагалі —
    // саме в цьому практична користь yield return на великих обсягах даних.
    public List<DoctorStats> TakeTopDoctors(int howMany)
    {
        List<DoctorStats> result = new List<DoctorStats>();
        if (howMany <= 0) return result;

        foreach (DoctorStats stats in ComputeDoctorStats())
        {
            result.Add(stats);
            if (result.Count == howMany) break;   // перебір зупиняється тут
        }
        return result;
    }
}

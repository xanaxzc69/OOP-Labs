using System;
using ClinicApp.Managers;
using ClinicApp.Models;
using ClinicApp.Utils;

namespace ClinicApp;

// Задача 7. Клініка — оркестратор, що об'єднує три менеджери під одним дахом
public class Clinic
{
    public string Name { get; set; }

    public PatientManager Patients { get; }
    public DoctorManager Doctors { get; }
    public AppointmentManager Appointments { get; }

    // Задача 4: клініка отримує ще один менеджер — медичних записів
    public MedicalRecordManager MedicalRecords { get; }

    // Задача 4: менеджер рахунків; працює поверх менеджера записів
    public BillingManager Billing { get; }

    public Clinic(string name)
    {
        Name = name;
        // Порядок важливий: AppointmentManager отримує вже створені менеджери
        Patients = new PatientManager();
        Doctors = new DoctorManager();
        Appointments = new AppointmentManager(Patients, Doctors);
        MedicalRecords = new MedicalRecordManager();
        Billing = new BillingManager(Appointments);
    }

    // Розклад на конкретну дату
    public void DisplaySchedule(DateTime date)
    {
        Console.WriteLine($"=== Розклад на {date.ToString("dd.MM.yyyy")} ===");
        Appointments.DisplayList(Appointments.GetByDate(date));
    }

    // Загальний звіт клініки
    public void GenerateReport()
    {
        Appointment[] upcoming = Appointments.GetUpcoming();   // отримуємо один раз

        Console.WriteLine("==========================================");
        Console.WriteLine($"  ЗВІТ КЛІНІКИ: {Name}");
        Console.WriteLine("==========================================");
        Console.WriteLine($"Пацієнтів: {Patients.Count}");
        Console.WriteLine($"Лікарів: {Doctors.Count}");
        Console.WriteLine($"Майбутніх записів: {upcoming.Length}");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Навантаження лікарів:");

        Doctor[] allDoctors = Doctors.GetAll();
        for (int i = 0; i < allDoctors.Length; i++)
        {
            int count = 0;
            for (int j = 0; j < upcoming.Length; j++)
            {
                if (upcoming[j].DoctorId == allDoctors[i].Id)
                {
                    count++;
                }
            }
            Console.WriteLine($"  {allDoctors[i].FullName} ({ClinicFormatter.FormatSpeciality(allDoctors[i].Speciality)}): {count} записів");
        }
        Console.WriteLine("==========================================");
    }
}
